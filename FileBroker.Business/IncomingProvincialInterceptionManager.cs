using DBHelper;
using FileBroker.Common.Helpers;
using FileBroker.Model;
using FOAEA3.Resources;
using Newtonsoft.Json;

namespace FileBroker.Business
{
    public class IncomingProvincialInterceptionManager
    {
        private string FileName { get; }
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }
        private IFileBrokerConfigurationHelper Config { get; }
        private Dictionary<string, string> Translations { get; }
        private bool IsFrench { get; }
        private string EnfSrv_Cd { get; }

        private IncomingProvincialHelper IncomingFileHelper { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public IncomingProvincialInterceptionManager(RepositoryList db,
                                                     APIBrokerList foaeaApis,
                                                     string fileName,
                                                     IFileBrokerConfigurationHelper config)
        {
            FileName = fileName;
            APIs = foaeaApis;
            DB = db;
            Config = config;

            string provinceCode = fileName[0..2].ToUpper();
            IsFrench = Config.ProvinceConfig.FrenchAuditProvinceCodes?.Contains(provinceCode) ?? false;

            Translations = LoadTranslations();

            EnfSrv_Cd = provinceCode + "01"; // e.g. ON01

            string provCode = FileName[..2].ToUpper();
            IncomingFileHelper = new IncomingProvincialHelper(config, provCode);

            FoaeaAccess = new FoaeaSystemAccess(foaeaApis, Config.FoaeaLogin);
        }

        private Dictionary<string, string> LoadTranslations()
        {
            var translations = new Dictionary<string, string>();

            if (IsFrench)
            {
                var Translations = DB.TranslationTable.GetTranslations().Result;
                foreach (var translation in Translations)
                    translations.Add(translation.EnglishText, translation.FrenchText);

                APIs.InterceptionApplications.ApiHelper.CurrentLanguage = LanguageHelper.FRENCH_LANGUAGE;
                LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);
            }

            return translations;
        }

        private string Translate(string englishText)
        {
            if (IsFrench && Translations.ContainsKey(englishText))
            {
                return Translations[englishText];
            }
            else
                return englishText;
        }

        public async Task<MessageDataList> ExtractAndProcessRequestsInFile(string sourceInterceptionData, List<UnknownTag> unknownTags,
                                                                           bool includeInfoInMessages = false)
        {
            var result = new MessageDataList();

            var fileAuditManager = new FileAuditManager(DB.FileAudit, Config, DB.MailService);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(FileName);
            var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

            // check that it is the expected cycle
            if (!FileHelper.IsExpectedCycle(fileTableData, FileName, out int expectedCycle, out int actualCycle))
            {
                if (actualCycle != -1)
                    result.AddSystemError($"Error for file {FileName}: expecting cycle {expectedCycle} but trying to load cycle {actualCycle}.");
                else
                    result.AddSystemError($"Error for file {FileName}: invalid file cycle?");

                return result;
            }

            await DB.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

            bool isValid = true;

            // convert data from json into object
            var interceptionFileData = ExtractInterceptionDataFromJson(sourceInterceptionData, out string error);
            var interceptionFile = interceptionFileData.NewDataSet;

            if (!string.IsNullOrEmpty(error))
            {
                isValid = false;
                result.AddSystemError(error);
            }
            else
            {
                ValidateHeader(interceptionFile.INTAPPIN01, ref result, ref isValid);
                ValidateFooter(interceptionFile, ref result, ref isValid);

                if (isValid)
                {
                    int errorCount = 0;
                    int warningCount = 0;
                    int successCount = 0;

                    await FoaeaAccess.SystemLogin();

                    try
                    {
                        foreach (var data in interceptionFile.INTAPPIN10)
                        {
                            bool isValidRequest = true;

                            var fileAuditData = new FileAuditData
                            {
                                Appl_EnfSrv_Cd = data.dat_Appl_EnfSrvCd,
                                Appl_CtrlCd = data.dat_Appl_CtrlCd,
                                Appl_Source_RfrNr = data.dat_Appl_Source_RfrNr,
                                InboundFilename = FileName + ".XML"
                            };

                            var requestError = new MessageDataList();

                            ValidateActionCode(data, ref requestError, ref isValidRequest);

                            if (isValidRequest)
                            {
                                var interceptionData = interceptionFile.INTAPPIN11.Find(t => t.dat_Appl_CtrlCd == data.dat_Appl_CtrlCd);
                                var financialData = interceptionFile.INTAPPIN12.Find(t => t.dat_Appl_CtrlCd == data.dat_Appl_CtrlCd);
                                var sourceSpecificData = interceptionFile.INTAPPIN13.Where(t => t.dat_Appl_CtrlCd == data.dat_Appl_CtrlCd).ToList();

                                int thisErrorCount = 0;
                                bool isValidData = true;
                                InterceptionApplicationData thisApplication;
                                (thisApplication, thisErrorCount, isValidData) = await GetAndValidateAppDataFromRequest(
                                                                                            data, interceptionData,
                                                                                            financialData, sourceSpecificData,
                                                                                            fileAuditData);
                                var interceptionMessage = new MessageData<InterceptionApplicationData>
                                {
                                    Application = thisApplication,
                                    MaintenanceAction = data.Maintenance_ActionCd,
                                    MaintenanceLifeState = data.dat_Appl_LiSt_Cd,
                                    NewRecipientSubmitter = data.dat_New_Owner_RcptSubmCd,
                                    NewIssuingSubmitter = data.dat_New_Owner_SubmCd,
                                    NewUpdateSubmitter = data.dat_Update_SubmCd ?? data.dat_Subm_SubmCd
                                };
                                errorCount += thisErrorCount;

                                if (isValidData)
                                {
                                    var inboundHistoryTable = DB.LoadInboundAuditTable;
                                    var appl = interceptionMessage.Application;
                                    var fileName = FileName + ".XML";

                                    if (!await inboundHistoryTable.RowExists(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr,
                                                                             fileName))
                                    {
                                        await inboundHistoryTable.AddRow(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName);

                                        var messages = await SendDataToFoaea(interceptionMessage);

                                        ProcessMessages(messages, fileAuditData, includeInfoInMessages, result,
                                                        ref errorCount, ref warningCount, ref successCount);

                                        await inboundHistoryTable.MarkRowAsCompleted(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName);
                                    }
                                }

                            }
                            else
                            {
                                fileAuditData.ApplicationMessage = requestError[0].Description;
                                errorCount++;
                            }

                            await DB.FileAudit.InsertFileAuditData(fileAuditData);

                        }

                        int totalFilesCount = await fileAuditManager.GenerateAuditFile(FileName + ".XML", unknownTags, errorCount, warningCount, successCount);
                        await fileAuditManager.SendStandardAuditEmail(FileName + ".XML", Config.AuditConfig.AuditRecipients,
                                                                           errorCount, warningCount, successCount, unknownTags.Count,
                                                                           totalFilesCount);

                        if (Config.ProvinceConfig.AutoAcceptEnfSrvCodes.Contains(EnfSrv_Cd))
                            await AutoAcceptVariations(EnfSrv_Cd);

                    }
                    finally
                    {
                        await FoaeaAccess.SystemLogout();
                    }

                }

            }

            if (!isValid)
            {
                result.AddSystemError($"One of more error(s) occured in file ({FileName}.XML)");

                await fileAuditManager.SendSystemErrorAuditEmail(FileName, Config.AuditConfig.AuditRecipients, result);
            }

            await DB.FileAudit.MarkFileAuditCompletedForFile(FileName);
            await DB.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, false);
            await DB.FileTable.SetNextCycleForFileType(fileTableData);

            return result;
        }

        private static void ProcessMessages(MessageDataList messages, FileAuditData fileAuditData, bool includeInfoInMessages, 
                                            MessageDataList result, ref int errorCount, ref int warningCount, ref int successCount)
        {
            

            if (messages.ContainsMessagesOfType(MessageType.Error))
            {
                var errors = messages.FindAll(m => m.Severity == MessageType.Error);
                fileAuditData.ApplicationMessage = BuildDescriptionForMessage(errors.First());
                errorCount++;
            }
            else if (messages.ContainsMessagesOfType(MessageType.Warning))
            {
                var warnings = messages.FindAll(m => m.Severity == MessageType.Warning);
                fileAuditData.ApplicationMessage = BuildDescriptionForMessage(warnings.First());
                warningCount++;
            }
            else
            {
                if (includeInfoInMessages)
                {
                    var infos = messages.FindAll(m => m.Severity == MessageType.Information);
                    if (infos.Any())
                    {
                        fileAuditData.ApplicationMessage = BuildDescriptionForMessage(infos.First());
                        result.AddRange(infos);
                    }
                }

                if (string.IsNullOrEmpty(fileAuditData.ApplicationMessage))
                    fileAuditData.ApplicationMessage = LanguageResource.AUDIT_SUCCESS;
                successCount++;
            }
                       
        }

        public async Task AutoAcceptVariations(string enfService)
        {
            var prodAudit = APIs.ProductionAudits;

            string processName = $"Process Auto Accept Variation {enfService}";
            await prodAudit.Insert(processName, "Auto accept variation", "O");

            await APIs.InterceptionApplications.AutoAcceptVariations(enfService);

            await prodAudit.Insert(processName, "Ended", "O");
        }

        public async Task<MessageDataList> SendDataToFoaea(MessageData<InterceptionApplicationData> interceptionMessageData)
        {
            var interception = interceptionMessageData.Application;
            var existingMessages = interception.Messages;

            APIs.InterceptionApplications.ApiHelper.CurrentSubmitter = interceptionMessageData.NewUpdateSubmitter;

            if (interceptionMessageData.MaintenanceAction == "A")
            {
                interception = await APIs.InterceptionApplications.CreateInterceptionApplication(interception);
            }
            else // if (interceptionMessageData.MaintenanceAction == "C")
            {
                switch (interceptionMessageData.MaintenanceLifeState)
                {
                    case "00": // change
                    case "0":
                        interception = await APIs.InterceptionApplications.UpdateInterceptionApplication(interception);
                        break;

                    case "14": // cancellation
                        interception = await APIs.InterceptionApplications.CancelInterceptionApplication(interception);
                        break;

                    case "17": // variation
                        interception = await APIs.InterceptionApplications.VaryInterceptionApplication(interception);
                        break;

                    case "29": // transfer
                        interception = await APIs.InterceptionApplications.TransferInterceptionApplication(interception,
                                                                                                     interceptionMessageData.NewRecipientSubmitter,
                                                                                                     interceptionMessageData.NewIssuingSubmitter);
                        break;

                    case "35": // suspend
                        interception = await APIs.InterceptionApplications.SuspendInterceptionApplication(interception);
                        break;

                    default:
                        interception = interceptionMessageData.Application;
                        interception.Messages.AddError($"Unknown dat_Appl_LiSt_Cd ({interceptionMessageData.MaintenanceLifeState})" +
                                                       $" for Maintenance_ActionCd ({interceptionMessageData.MaintenanceAction})");
                        break;
                }
            }

            existingMessages.AddRange(interception.Messages);
            return existingMessages;
        }

        private static string BuildDescriptionForMessage(MessageData error)
        {
            var thisErrorDescription = string.Empty;
            if (error.Code != EventCode.UNDEFINED)
                thisErrorDescription += error.Code.ToString();
            if (!string.IsNullOrEmpty(error.Description.Trim()))
                thisErrorDescription += error.Description.Trim();

            return thisErrorDescription;
        }

        private void ValidateHeader(MEPInterception_RecType01 interceptionFile, ref MessageDataList result, ref bool isValid)
        {
            int cycle = FileHelper.ExtractCycleFromFilename(FileName);
            if (int.Parse(interceptionFile.Cycle) != cycle)
            {
                isValid = false;
                result.AddSystemError($"Cycle in file [{interceptionFile.Cycle}] does not match cycle of file [{cycle}]");
            }

            if (!IncomingFileHelper.IsValidTermsAccepted(interceptionFile.TermsAccepted))
            {
                isValid = false;
                result.AddSystemError($"type 01 Terms Accepted invalid text: {interceptionFile.TermsAccepted}");
            }

        }

        private static void ValidateFooter(MEPInterception_InterceptionDataSet interceptionFile, ref MessageDataList result, ref bool isValid)
        {
            if (int.Parse(interceptionFile.INTAPPIN99.ResponseCnt) != interceptionFile.INTAPPIN10.Count)
            {
                isValid = false;
                result.AddSystemError("Invalid ResponseCnt in section 99");
            }
        }

        private static void ValidateActionCode(MEPInterception_RecType10 data, ref MessageDataList result, ref bool isValid)
        {
            bool validActionLifeState = true;
            string actionCode = data.Maintenance_ActionCd.Trim();
            string actionState = data.dat_Appl_LiSt_Cd.Trim();

            if ((actionCode == "A") && actionState.NotIn("00", "0"))
                validActionLifeState = false;
            else if ((actionCode == "C") && (actionState.NotIn("00", "0", "14", "17", "29", "35")))
                validActionLifeState = false;
            else if (actionCode.NotIn("A", "C"))
                validActionLifeState = false;

            if (!validActionLifeState)
            {
                isValid = false;
                result.AddSystemError($"Invalid MaintenanceAction [{actionCode}] and MaintenanceLifeState [{actionState}] combination.");
            }

        }

        private static MEPInterceptionFileData ExtractInterceptionDataFromJson(string sourceInterceptionData, out string error)
        {
            error = string.Empty;

            MEPInterceptionFileData result;
            try
            {
                result = JsonConvert.DeserializeObject<MEPInterceptionFileData>(sourceInterceptionData);
            }
            catch
            {
                try
                {
                    var single = JsonConvert.DeserializeObject<MEPInterceptionFileDataSingle>(sourceInterceptionData);
                    if (single is null)
                        throw new NullReferenceException("json conversion failed for MEPInterceptionFileDataSingle");

                    result = new MEPInterceptionFileData();
                    result.NewDataSet.INTAPPIN01 = single.NewDataSet.INTAPPIN01;
                    result.NewDataSet.INTAPPIN10.Add(single.NewDataSet.INTAPPIN10);
                    result.NewDataSet.INTAPPIN11.Add(single.NewDataSet.INTAPPIN11);
                    result.NewDataSet.INTAPPIN12.Add(single.NewDataSet.INTAPPIN12);
                    result.NewDataSet.INTAPPIN99 = single.NewDataSet.INTAPPIN99;
                }
                catch (Exception ee)
                {
                    error = ee.Message;
                    result = new MEPInterceptionFileData();
                }
            }

            return result;
        }

        public async Task<(InterceptionApplicationData, int errorCount, bool isValidData)> GetAndValidateAppDataFromRequest(MEPInterception_RecType10 baseData,
                                                                              MEPInterception_RecType11 interceptionData,
                                                                              MEPInterception_RecType12 financialData,
                                                                              List<MEPInterception_RecType13> sourceSpecificData,
                                                                              FileAuditData fileAuditData)
        {
            DateTime now = DateTime.Now;
            bool isVariation = baseData.dat_Appl_LiSt_Cd == "17";
            bool isCancelOrSuspend = baseData.dat_Appl_LiSt_Cd.In("14", "35");
            bool isCreate = false;

            if (baseData.Maintenance_ActionCd == "A")
                isCreate = true;

            bool isValidData;
            int errorCount = 0;

            var interceptionApplication = ExtractInterceptionBaseData(baseData, interceptionData, now);

            isValidData = await IsValidAppl(interceptionApplication, fileAuditData);
            if ((interceptionApplication is not null) && isValidData && !isCancelOrSuspend)
            {
                ExtractDefaultFinancialData(isVariation, financialData, ref isValidData, now, interceptionApplication);

                isValidData = await IsValidFinancialInformation(interceptionApplication, fileAuditData);
                if (isValidData)
                {
                    foreach (var sourceSpecific in sourceSpecificData)
                    {
                        bool isSourceSpecificDataValid;
                        HoldbackConditionData newSourceSpecificData;
                        (newSourceSpecificData, isSourceSpecificDataValid) = await ExtractAndValidateSourceSpecificData(sourceSpecific, fileAuditData,
                                                                                                                             interceptionApplication);
                        if (!isSourceSpecificDataValid)
                            isValidData = false;
                        interceptionApplication.HldbCnd.Add(newSourceSpecificData);
                    }
                }
            }

            if (isCreate && (interceptionApplication != null) &&
                ((interceptionApplication.IntFinH is null) || (interceptionApplication.IntFinH.IntFinH_Dte == DateTime.MinValue)))
            {
                isValidData = false;
            }

            if (!isValidData)
                errorCount++;

            return (interceptionApplication, errorCount, isValidData);
        }

        private async Task<bool> IsValidAppl(InterceptionApplicationData interceptionApplication, FileAuditData fileAuditData)
        {
            var fieldErrors = new Dictionary<string, string>
            {
                {"Appl_Dbtr_Addr_PrvCd", "Invalid Debtor Address Province Code (<Appl_Dbtr_Addr_PrvCd>) value {0} for Country Code (<Appl_Dbtr_Addr_CtryCd>) value {1}"},
                {"Medium_Cd", "Invalid Medium Code (<dat_Appl_Medium_Cd>) value"},
                {"Appl_Dbtr_LngCd", "Invalid Debtor Language Code (<dat_Appl_Dbtr_LngCd>) value"},
                {"Appl_Dbtr_Gendr_Cd", "Invalid Debtor Gender Code(<dat_Appl_Dbtr_Gendr_Cd>) value"},
                {"Appl_EnfSrv_Cd", "Invalid Enforcement Service Code (<dat_Appl_EnfSrvCd>) value"},
                {"Appl_Affdvt_DocTypCd", "Invalid Supporting Document Type (<dat_Appl_Affdvt_Doc_TypCd>) value"},
                {"Appl_Dbtr_Addr_CtryCd", "Invalid Debtor Address Country Code (<dat_Appl_Dbtr_Addr_CtryCd>) value"},
                {"AppReas_Cd", "Invalid Reason Code (<dat_Appl_Reas_Cd>) value"},
                {"AppList_Cd", "Invalid Life State Code (<dat_Appl_LiSt_Cd>) value"},
                {"AppCtgy_Cd", "Invalid Application Category Code (<dat_Appl_AppCtgy_Cd>) value"}
            };

            var validatedApplication = await APIs.Applications.ValidateCoreValues(interceptionApplication);
            if (validatedApplication.Appl_Dbtr_Addr_PrvCd is not null)
                interceptionApplication.Appl_Dbtr_Addr_PrvCd = validatedApplication.Appl_Dbtr_Addr_PrvCd; // might have been updated via validation!

            var errors = validatedApplication.Messages.GetMessagesForType(MessageType.Error);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    if (fieldErrors.ContainsKey(error.Field))
                    {
                        if (error.Field == "Appl_Dbtr_Addr_PrvCd")
                            fileAuditData.ApplicationMessage = String.Format(Translate(fieldErrors[error.Field]),
                                                                             validatedApplication.Appl_Dbtr_Addr_PrvCd,
                                                                             validatedApplication.Appl_Dbtr_Addr_CtryCd);
                        else
                            fileAuditData.ApplicationMessage = Translate(fieldErrors[error.Field]);
                    }
                    else
                        fileAuditData.ApplicationMessage = error.Description;

                    break;
                }

                return false;
            }

            return true;

        }

        private async Task<bool> IsValidFinancialInformation(InterceptionApplicationData interceptionApplication, FileAuditData fileAuditData)
        {
            var validatedApplication = await APIs.InterceptionApplications.ValidateFinancialCoreValues(interceptionApplication);
            interceptionApplication.IntFinH = validatedApplication.IntFinH; // might have been updated via validation!
            interceptionApplication.Messages.AddRange(validatedApplication.Messages);

            var errors = validatedApplication.Messages.GetMessagesForType(MessageType.Error);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    fileAuditData.ApplicationMessage = Translate(error.Description);

                    break;
                }

                return false;
            }

            return true;

        }

        private static InterceptionApplicationData ExtractInterceptionBaseData(MEPInterception_RecType10 baseData, MEPInterception_RecType11 interceptionData, DateTime now)
        {
            var interceptionApplication = new InterceptionApplicationData
            {
                Appl_EnfSrv_Cd = baseData.dat_Appl_EnfSrvCd,
                Appl_CtrlCd = baseData.dat_Appl_CtrlCd,
                Appl_Source_RfrNr = baseData.dat_Appl_Source_RfrNr,
                Subm_Recpt_SubmCd = baseData.dat_Subm_Rcpt_SubmCd,
                Subm_SubmCd = baseData.dat_Subm_SubmCd,
                Appl_Lgl_Dte = baseData.dat_Appl_Lgl_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? DateTime.MinValue,
                Appl_Dbtr_SurNme = baseData.dat_Appl_Dbtr_SurNme,
                Appl_Dbtr_FrstNme = baseData.dat_Appl_Dbtr_FrstNme,
                Appl_Dbtr_MddleNme = baseData.dat_Appl_Dbtr_MddleNme,
                Appl_Dbtr_Brth_Dte = baseData.dat_Appl_Dbtr_Brth_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date,
                Appl_Dbtr_Gendr_Cd = baseData.dat_Appl_Dbtr_Gendr_Cd.Trim() == "" ? "M" : baseData.dat_Appl_Dbtr_Gendr_Cd.Trim(),
                Appl_Dbtr_Entrd_SIN = baseData.dat_Appl_Dbtr_Entrd_SIN,
                Appl_Dbtr_Parent_SurNme_Birth = baseData.dat_Appl_Dbtr_Parent_SurNme_Birth,
                Appl_CommSubm_Text = baseData.dat_Appl_CommSubm_Text,
                Appl_Rcptfrm_Dte = now, // as per spec, ignore the baseData.dat_Appl_Rcptfrm_dte.Date value
                AppCtgy_Cd = baseData.dat_Appl_AppCtgy_Cd,
                Appl_Group_Batch_Cd = baseData.dat_Appl_Group_Batch_Cd,
                Medium_Cd = baseData.dat_Appl_Medium_Cd,
                Appl_Affdvt_DocTypCd = baseData.dat_Appl_Affdvt_Doc_TypCd,
                AppReas_Cd = baseData.dat_Appl_Reas_Cd,
                Appl_Reactv_Dte = baseData.dat_Appl_Reactv_Dte.ConvertToDateTimeIgnoringTimeZone(),

                AppLiSt_Cd = (ApplicationState)(baseData.dat_Appl_LiSt_Cd.Convert<int>()),
                Appl_SIN_Cnfrmd_Ind = 0,
                ActvSt_Cd = "A",

                Appl_Dbtr_LngCd = interceptionData.dat_Appl_Dbtr_LngCd,
                Appl_Dbtr_Addr_Ln = interceptionData.dat_Appl_Dbtr_Addr_Ln?.Trim(),
                Appl_Dbtr_Addr_Ln1 = interceptionData.dat_Appl_Dbtr_Addr_Ln1?.Trim(),
                Appl_Dbtr_Addr_CityNme = interceptionData.dat_Appl_Dbtr_Addr_CityNme?.Trim(),
                Appl_Dbtr_Addr_CtryCd = interceptionData.dat_Appl_Dbtr_Addr_CtryCd?.Trim(),
                Appl_Dbtr_Addr_PCd = interceptionData.dat_Appl_Dbtr_Addr_PCd?.Trim(),
                Appl_Dbtr_Addr_PrvCd = interceptionData.dat_Appl_Dbtr_Addr_PrvCd?.Trim(),
                Appl_Crdtr_SurNme = interceptionData.dat_Appl_Crdtr_SurNme?.Trim(),
                Appl_Crdtr_FrstNme = interceptionData.dat_Appl_Crdtr_FrstNme?.Trim(),
                Appl_Crdtr_MddleNme = interceptionData.dat_Appl_Crdtr_MddleNme?.Trim(),
                Appl_Crdtr_Brth_Dte = interceptionData.dat_Appl_Crdtr_Brth_Dte.ConvertToDateTimeIgnoringTimeZone()
            };
            return interceptionApplication;
        }

        private static bool ExtractDefaultFinancialData(bool isVariation, MEPInterception_RecType12 financialData,
                                                        ref bool isValidData, DateTime now, InterceptionApplicationData interceptionApplication)
        {
            isValidData = true;

            var intFinH = interceptionApplication.IntFinH;
            intFinH.Appl_EnfSrv_Cd = interceptionApplication.Appl_EnfSrv_Cd;
            intFinH.Appl_CtrlCd = interceptionApplication.Appl_CtrlCd;
            intFinH.IntFinH_Dte = now;
            intFinH.IntFinH_LmpSum_Money = financialData.dat_IntFinH_LmpSum_Money.Convert<decimal>();
            intFinH.IntFinH_PerPym_Money = financialData.dat_IntFinH_Perpym_Money.Convert<decimal?>();
            intFinH.PymPr_Cd = financialData.dat_PymPr_Cd;
            intFinH.IntFinH_NextRecalcDate_Cd = financialData.dat_IntFinH_NextRecalc_Dte.Convert<int?>();
            intFinH.HldbCtg_Cd = financialData.dat_HldbCtg_Cd;
            intFinH.IntFinH_DefHldbPrcnt = financialData.dat_IntFinH_DfHldbPrcnt.Convert<int?>();
            intFinH.IntFinH_DefHldbAmn_Money = financialData.dat_IntFinH_DefHldbAmn_Money.Convert<decimal?>();
            intFinH.IntFinH_DefHldbAmn_Period = financialData.dat_IntFinH_DefHldbAmn_Period;
            if (isVariation)
                intFinH.IntFinH_VarIss_Dte = financialData.dat_IntFinH_VarIss_Dte.ConvertToDateTimeIgnoringTimeZone() ?? now;

            intFinH.IntFinH_CmlPrPym_Ind = financialData.dat_IntFinH_CmlPrPym_Ind.Convert<byte?>();
            if (intFinH.IntFinH_CmlPrPym_Ind.HasValue)
            {
                // XML Code indicating if periodic payments are cumulative: 1=no, 2=yes.
                // In the database, though, 0=no and 1=yes, so we must substract 1 from received value, if any
                intFinH.IntFinH_CmlPrPym_Ind--;
            }

            intFinH.IntFinH_LmpSum_Money /= 100M;
            if (intFinH.IntFinH_PerPym_Money is not null)
                intFinH.IntFinH_PerPym_Money /= 100M;

            if (intFinH.IntFinH_DefHldbAmn_Money is not null)
                intFinH.IntFinH_DefHldbAmn_Money /= 100M;

            // TODO: validations should be moved to backend Interception manager/API
            // Call API to validate financial info

            return isValidData;
        }

        private async Task<(HoldbackConditionData, bool)> ExtractAndValidateSourceSpecificData(MEPInterception_RecType13 sourceSpecific,
                                                                                           FileAuditData fileAuditData,
                                                                                           InterceptionApplicationData interceptionApplication)
        {
            bool isValidData = true;
            var newHoldback = new HoldbackConditionData
            {
                Appl_CtrlCd = interceptionApplication.Appl_CtrlCd,
                Appl_EnfSrv_Cd = interceptionApplication.Appl_EnfSrv_Cd,
                IntFinH_Dte = interceptionApplication.IntFinH.IntFinH_Dte,
                EnfSrv_Cd = sourceSpecific.dat_EnfSrv_Cd,
                HldbCnd_MxmPerChq_Money = sourceSpecific.dat_HldbCnd_MxmPerChq_Money.Convert<decimal?>(),
                HldbCnd_SrcHldbAmn_Money = sourceSpecific.dat_HldbCnd_Hldb_Amn_Money.Convert<decimal?>(),
                HldbCnd_SrcHldbPrcnt = sourceSpecific.dat_HldbCnd_SrcHldbPrcnt.Convert<int?>(),
                HldbCtg_Cd = sourceSpecific.dat_HldbCtg_Cd
            };

            // fix money
            if (newHoldback.HldbCnd_MxmPerChq_Money is not null) newHoldback.HldbCnd_MxmPerChq_Money /= 100M;
            if (newHoldback.HldbCnd_SrcHldbAmn_Money is not null) newHoldback.HldbCnd_SrcHldbAmn_Money /= 100M;

            // other fixes
            if ((newHoldback.HldbCnd_SrcHldbPrcnt is 0) && (newHoldback.HldbCtg_Cd == "1"))
            {
                newHoldback.HldbCnd_SrcHldbPrcnt = null;
                newHoldback.HldbCtg_Cd = "0";
            }

            if (newHoldback.HldbCnd_SrcHldbPrcnt is null)
                newHoldback.HldbCnd_SrcHldbPrcnt = 0;
            else if (newHoldback.HldbCnd_SrcHldbPrcnt is < 0 or > 100)
            {
                fileAuditData.ApplicationMessage = Translate("Invalid percentage (<dat_HldbCnd_SrcHldbPrcnt>) was submitted with an amount < 0 or > 100");
                isValidData = false;
            }

            if ((newHoldback.HldbCnd_MxmPerChq_Money is not null) && (newHoldback.HldbCnd_SrcHldbAmn_Money is not null))
            {
                await DB.ErrorTrackingTable.MessageBrokerError("Source Specific holdback amount in both fixed and per transaction",
                                                                           $"{interceptionApplication.Appl_EnfSrv_Cd.Trim()} {interceptionApplication.Appl_CtrlCd.Trim()}" +
                                                                           $" Source: {newHoldback.EnfSrv_Cd}" +
                                                                           $" Per Transaction: ${newHoldback.HldbCnd_MxmPerChq_Money}" +
                                                                           $" Fixed Amount: ${newHoldback.HldbCnd_SrcHldbAmn_Money}",
                                                                           null, displayExceptionError: false);
                fileAuditData.ApplicationMessage = Translate("Fixed Amount and Amount Garnisheed per Transaction values cannot be entered for the same source department.") +
                                                   " " +
                                                   Translate($"Source Specific Holdback amount in both fixed (<dat_HldbCnd_Hldb_Amn_Money>) value {newHoldback.HldbCnd_SrcHldbAmn_Money} and per transaction (<dat_HldbCnd_MxmPerChq_Money>) value {newHoldback.HldbCnd_MxmPerChq_Money}");
                isValidData = false;
            }

            return (newHoldback, isValidData);
        }

    }
}
