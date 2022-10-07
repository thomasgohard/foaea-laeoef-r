using DBHelper;
using FOAEA3.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FileBroker.Business
{
    public class IncomingProvincialInterceptionManager
    {
        private string FileName { get; }
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }
        private ProvincialAuditFileConfig AuditConfiguration { get; }
        private Dictionary<string, string> Translations { get; }
        private bool IsFrench { get; }
        private string EnfSrv_Cd { get; }

        private string FOAEA_userName { get; }
        private string FOAEA_userPassword { get; }
        private string FOAEA_submitter { get; }

        public IncomingProvincialInterceptionManager(string fileName,
                                                     APIBrokerList apis,
                                                     RepositoryList repositories,
                                                     ProvincialAuditFileConfig auditConfig,
                                                     IConfiguration config)
        {
            FileName = fileName;
            APIs = apis;
            DB = repositories;
            AuditConfiguration = auditConfig;
            Translations = new Dictionary<string, string>();

            FOAEA_userName = config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues();
            FOAEA_userPassword = config["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues();
            FOAEA_submitter = config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues();

            // load translations

            string provinceCode = fileName[0..2].ToUpper();
            IsFrench = auditConfig.FrenchAuditProvinceCodes?.Contains(provinceCode) ?? false;

            if (IsFrench)
            {
                var translations = repositories.TranslationTable.GetTranslationsAsync().Result;
                foreach (var translation in translations)
                    Translations.Add(translation.EnglishText, translation.FrenchText);
                APIs.InterceptionApplications.ApiHelper.CurrentLanguage = LanguageHelper.FRENCH_LANGUAGE;
                LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);
            }

            EnfSrv_Cd = provinceCode + "01"; // e.g. ON01

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

        public async Task<MessageDataList> ExtractAndProcessRequestsInFileAsync(string sourceInterceptionData, List<UnknownTag> unknownTags, bool includeInfoInMessages = false)
        {
            var result = new MessageDataList();

            var fileAuditManager = new FileAuditManager(DB.FileAudit, AuditConfiguration, DB.MailService);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(FileName);
            var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

            // check that it is the expected cycle
            if (!FileHelper.IsExpectedCycle(fileTableData, FileName, out int expectedCycle, out int actualCycle))
            {
                if (actualCycle != -1)
                    result.AddSystemError($"Error for file {FileName}: expecting cycle {expectedCycle} but trying to load cycle {actualCycle}.");
                else
                    result.AddSystemError($"Error for file {FileName}: invalid file cycle?");

                return result;
            }

            await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, true);

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
                ValidateHeader(interceptionFile, ref result, ref isValid);
                ValidateFooter(interceptionFile, ref result, ref isValid);

                if (isValid)
                {
                    int errorCount = 0;
                    int warningCount = 0;
                    int successCount = 0;

                    var loginData = new FoaeaLoginData
                    {
                        UserName = FOAEA_userName,
                        Password = FOAEA_userPassword,
                        Submitter = FOAEA_submitter
                    };

                    var claims = await APIs.Accounts.LoginAsync(loginData);

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
                                (thisApplication, thisErrorCount, isValidData) = await GetAndValidateAppDataFromRequestAsync(
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

                                await AddRequestToAuditAsync(interceptionMessage);

                                if (isValidData)
                                {
                                    var loadInboundDB = DB.LoadInboundAuditTable;
                                    var appl = interceptionMessage.Application;
                                    var fileName = FileName + ".XML";

                                    if (!await loadInboundDB.AlreadyExistsAsync(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr,
                                                                                fileName))
                                    {

                                        await loadInboundDB.AddNewAsync(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName);

                                        var messages = await SendDataToFoaeaAsync(interceptionMessage);

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

                                        await loadInboundDB.MarkAsCompletedAsync(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName);

                                    }
                                }

                            }
                            else
                            {
                                fileAuditData.ApplicationMessage = requestError[0].Description;
                                errorCount++;
                            }

                            await DB.FileAudit.InsertFileAuditDataAsync(fileAuditData);

                        }

                        await fileAuditManager.GenerateAuditFileAsync(FileName, unknownTags, errorCount, warningCount, successCount);
                        await fileAuditManager.SendStandardAuditEmailAsync(FileName, AuditConfiguration.AuditRecipients,
                                                                           errorCount, warningCount, successCount, unknownTags.Count);

                        if (AuditConfiguration.AutoAcceptEnfSrvCodes.Contains(EnfSrv_Cd))
                            await AutoAcceptVariationsAsync(EnfSrv_Cd);

                    }
                    finally
                    {
                        // TODO: fix token
                        await APIs.Accounts.LogoutAsync(loginData);
                    }

                }

            }

            if (!isValid)
            {
                result.AddSystemError($"One of more error(s) occured in file ({FileName}.XML)");

                await fileAuditManager.SendSystemErrorAuditEmailAsync(FileName, AuditConfiguration.AuditRecipients, result);
            }

            await DB.FileAudit.MarkFileAuditCompletedForFileAsync(FileName);
            await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, false);
            await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData);

            return result;
        }

        private async Task AddRequestToAuditAsync(MessageData<InterceptionApplicationData> interceptionMessage)
        {
            var requestLogData = new RequestLogData
            {
                MaintenanceAction = interceptionMessage.MaintenanceAction,
                MaintenanceLifeState = interceptionMessage.MaintenanceLifeState,
                Appl_EnfSrv_Cd = interceptionMessage.Application.Appl_EnfSrv_Cd,
                Appl_CtrlCd = interceptionMessage.Application.Appl_CtrlCd,
                LoadedDateTime = DateTime.Now
            };

            _ = await DB.RequestLogTable.AddAsync(requestLogData);
        }

        public async Task AutoAcceptVariationsAsync(string enfService)
        {
            var prodAudit = APIs.ProductionAudits;

            string processName = $"Process Auto Accept Variation {enfService}";
            await prodAudit.InsertAsync(processName, "Divert Funds Started", "O");

            APIs.InterceptionApplications.ApiHelper.CurrentSubmitter = "FO2SSS";
            // TODO: fix token
            var applAutomation = await APIs.InterceptionApplications.GetApplicationsForVariationAutoAcceptAsync(enfService);

            // TODO: fix token
            foreach (var appl in applAutomation)
                await APIs.InterceptionApplications.AcceptVariationAsync(appl);

            await prodAudit.InsertAsync(processName, "Ended", "O");
        }

        public async Task<MessageDataList> SendDataToFoaeaAsync(MessageData<InterceptionApplicationData> interceptionMessageData)
        {
            InterceptionApplicationData interception;
            var existingMessages = interceptionMessageData.Application.Messages;

            APIs.InterceptionApplications.ApiHelper.CurrentSubmitter = interceptionMessageData.NewUpdateSubmitter;

            if (interceptionMessageData.MaintenanceAction == "A")
            {
                // TODO: fix token
                interception = await APIs.InterceptionApplications.CreateInterceptionApplicationAsync(interceptionMessageData.Application);
            }
            else // if (interceptionMessageData.MaintenanceAction == "C")
            {
                switch (interceptionMessageData.MaintenanceLifeState)
                {
                    case "00": // change
                    case "0":
                        // TODO: fix token
                        interception = await APIs.InterceptionApplications.UpdateInterceptionApplicationAsync(interceptionMessageData.Application);
                        break;

                    case "14": // cancellation
                        // TODO: fix token
                        interception = await APIs.InterceptionApplications.CancelInterceptionApplicationAsync(interceptionMessageData.Application);
                        break;

                    case "17": // variation
                        // TODO: fix token
                        interception = await APIs.InterceptionApplications.VaryInterceptionApplicationAsync(interceptionMessageData.Application);
                        break;

                    case "29": // transfer
                        // TODO: fix token
                        interception = await APIs.InterceptionApplications.TransferInterceptionApplicationAsync(interceptionMessageData.Application,
                                                                                                     interceptionMessageData.NewRecipientSubmitter,
                                                                                                     interceptionMessageData.NewIssuingSubmitter);
                        break;

                    case "35": // suspend
                        // TODO: fix token
                        interception = await APIs.InterceptionApplications.SuspendInterceptionApplicationAsync(interceptionMessageData.Application);
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

        private void ValidateHeader(MEPInterception_InterceptionDataSet interceptionFile, ref MessageDataList result, ref bool isValid)
        {
            int cycle = FileHelper.GetCycleFromFilename(FileName);
            if (int.Parse(interceptionFile.INTAPPIN01.Cycle) != cycle)
            {
                isValid = false;
                result.AddSystemError($"Cycle in file [{interceptionFile.INTAPPIN01.Cycle}] does not match cycle of file [{cycle}]");
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

        public async Task<(InterceptionApplicationData, int errorCount, bool isValidData)> GetAndValidateAppDataFromRequestAsync(MEPInterception_RecType10 baseData,
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

            isValidData = await IsValidApplAsync(interceptionApplication, fileAuditData);
            if ((interceptionApplication is not null) && isValidData && !isCancelOrSuspend)
            {
                ExtractDefaultFinancialData(isVariation, financialData, ref isValidData, now, interceptionApplication);

                isValidData = await IsValidFinancialInformationAsync(interceptionApplication, fileAuditData);
                if (isValidData)
                {
                    foreach (var sourceSpecific in sourceSpecificData)
                    {
                        bool isSourceSpecificDataValid;
                        HoldbackConditionData newSourceSpecificData;
                        (newSourceSpecificData, isSourceSpecificDataValid) = await ExtractAndValidateSourceSpecificDataAsync(sourceSpecific, fileAuditData,
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

        private async Task<bool> IsValidApplAsync(InterceptionApplicationData interceptionApplication, FileAuditData fileAuditData)
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

            // TODO: fix token
            var validatedApplication = await APIs.Applications.ValidateCoreValuesAsync(interceptionApplication);
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

        private async Task<bool> IsValidFinancialInformationAsync(InterceptionApplicationData interceptionApplication, FileAuditData fileAuditData)
        {
            // TODO: fix token
            var validatedApplication = await APIs.InterceptionApplications.ValidateFinancialCoreValuesAsync(interceptionApplication);
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
                Appl_Dbtr_Parent_SurNme = baseData.dat_Appl_Dbtr_Parent_SurNme_Birth,
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

        private async Task<(HoldbackConditionData, bool)> ExtractAndValidateSourceSpecificDataAsync(MEPInterception_RecType13 sourceSpecific,
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
                await DB.ErrorTrackingTable.MessageBrokerErrorAsync("Source Specific holdback amount in both fixed and per transaction",
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
