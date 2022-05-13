using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FileBroker.Business
{
    public class IncomingProvincialInterceptionManager
    {
        private string FileName { get; }
        private APIBrokerList APIs { get; }
        private RepositoryList Repositories { get; }
        private ProvincialAuditFileConfig AuditConfiguration { get; }
        private Dictionary<string, string> Translations { get; }
        private bool IsFrench { get; }

        public IncomingProvincialInterceptionManager(string fileName,
                                                     APIBrokerList apis,
                                                     RepositoryList repositories,
                                                     ProvincialAuditFileConfig auditConfig)
        {
            FileName = fileName;
            APIs = apis;
            Repositories = repositories;
            AuditConfiguration = auditConfig;
            Translations = new Dictionary<string, string>();

            // load translations

            string provinceCode = fileName[0..2].ToUpper();
            IsFrench = auditConfig.FrenchAuditProvinceCodes.Contains(provinceCode);

            if (IsFrench)
            {
                var translations = repositories.TranslationDB.GetTranslations();
                foreach (var translation in translations)
                    Translations.Add(translation.EnglishText, translation.FrenchText);
            }

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

        public MessageDataList ExtractAndProcessRequestsInFile(string sourceInterceptionData, List<UnknownTag> unknownTags, bool includeInfoInMessages = false)
        {
            var result = new MessageDataList();

            var fileAuditManager = new FileAuditManager(Repositories.FileAudit, AuditConfiguration, Repositories.MailServiceDB);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(FileName);
            var fileTableData = Repositories.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

            Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

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

                            var interceptionMessage = new MessageData<InterceptionApplicationData>
                            {
                                Application = GetAndValidateAppDataFromRequest(data, interceptionData, financialData, sourceSpecificData,
                                                                               ref fileAuditData, ref errorCount, out bool isValidData),
                                MaintenanceAction = data.Maintenance_ActionCd,
                                MaintenanceLifeState = data.dat_Appl_LiSt_Cd,
                                NewRecipientSubmitter = data.dat_New_Owner_RcptSubmCd,
                                NewIssuingSubmitter = data.dat_New_Owner_SubmCd,
                                NewUpdateSubmitter = data.dat_Update_SubmCd
                            };

                            if (isValidData)
                            {
                                var messages = ProcessApplicationRequest(interceptionMessage);

                                if (messages.ContainsMessagesOfType(MessageType.Error))
                                {
                                    var errors = messages.FindAll(m => m.Severity == MessageType.Error);
                                    fileAuditData.ApplicationMessage = errors[0].Description;
                                    errorCount++;
                                }
                                else if (messages.ContainsMessagesOfType(MessageType.Warning))
                                {
                                    var warnings = messages.FindAll(m => m.Severity == MessageType.Warning);
                                    fileAuditData.ApplicationMessage = warnings[0].Description;
                                    warningCount++;
                                }
                                else
                                {
                                    if (includeInfoInMessages)
                                    {
                                        var infos = messages.FindAll(m => m.Severity == MessageType.Information);
                                        result.AddRange(infos);
                                    }

                                    if (string.IsNullOrEmpty(fileAuditData.ApplicationMessage))
                                        fileAuditData.ApplicationMessage = "Success";
                                    successCount++;
                                }
                            }

                        }
                        else
                        {
                            fileAuditData.ApplicationMessage = requestError[0].Description;
                            errorCount++;
                        }

                        Repositories.FileAudit.InsertFileAuditData(fileAuditData);

                    }

                    fileAuditManager.GenerateAuditFile(FileName, unknownTags, errorCount, warningCount, successCount);
                    fileAuditManager.SendStandardAuditEmail(FileName, AuditConfiguration.AuditRecipients, 
                                                            errorCount, warningCount, successCount, unknownTags.Count);
                }

            }

            if (!isValid)
            {
                result.AddSystemError($"One of more error(s) occured in file ({FileName}.XML)");

                fileAuditManager.SendSystemErrorAuditEmail(FileName, AuditConfiguration.AuditRecipients, result);
            }

            Repositories.FileAudit.MarkFileAuditCompletedForFile(FileName);
            Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, false);
            Repositories.FileTable.SetNextCycleForFileType(fileTableData);

            return result;
        }

        public MessageDataList ProcessApplicationRequest(MessageData<InterceptionApplicationData> interceptionMessageData)
        {
            InterceptionApplicationData interception;

            APIs.InterceptionApplications.ApiHelper.CurrentSubmitter = interceptionMessageData.Application.Subm_SubmCd;

            if (interceptionMessageData.MaintenanceAction == "A")
            {
                interception = APIs.InterceptionApplications.CreateInterceptionApplication(interceptionMessageData.Application);
            }
            else // if (interceptionMessageData.MaintenanceAction == "C")
            {
                switch (interceptionMessageData.MaintenanceLifeState)
                {
                    case "00": // change
                    case "0":
                        interception = APIs.InterceptionApplications.UpdateInterceptionApplication(interceptionMessageData.Application);
                        break;

                    case "14": // cancellation
                        interception = APIs.InterceptionApplications.UpdateInterceptionApplication(interceptionMessageData.Application);
                        break;

                    case "17": // variation
                        interception = APIs.InterceptionApplications.VaryInterceptionApplication(interceptionMessageData.Application);
                        break;

                    case "29": // transfer
                        interception = APIs.InterceptionApplications.TransferInterceptionApplication(interceptionMessageData.Application,
                                                                                      interceptionMessageData.NewRecipientSubmitter,
                                                                                      interceptionMessageData.NewIssuingSubmitter);
                        break;

                    default:
                        interception = interceptionMessageData.Application;
                        interception.Messages.AddError($"Unknown dat_Appl_LiSt_Cd ({interceptionMessageData.MaintenanceLifeState})" +
                                                       $" for Maintenance_ActionCd ({interceptionMessageData.MaintenanceAction})");
                        break;
                }
            }

            return interception.Messages;
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

            if ((data.Maintenance_ActionCd == "A") && data.dat_Appl_LiSt_Cd.NotIn("00", "0"))
                validActionLifeState = false;
            else if ((data.Maintenance_ActionCd == "C") && (data.dat_Appl_LiSt_Cd.NotIn("00", "0", "14", "29")))
                validActionLifeState = false;
            else if (data.Maintenance_ActionCd.NotIn("A", "C"))
                validActionLifeState = false;

            if (!validActionLifeState)
            {
                isValid = false;
                result.AddSystemError($"Invalid MaintenanceAction [{data.Maintenance_ActionCd}] and MaintenanceLifeState [{data.dat_Appl_LiSt_Cd}] combination.");
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
                    // result.NewDataSet.INTAPPIN13.Add(single.NewDataSet.INTAPPIN13);
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

        private InterceptionApplicationData GetAndValidateAppDataFromRequest(MEPInterception_RecType10 baseData,
                                                                             MEPInterception_RecType11 interceptionData,
                                                                             MEPInterception_RecType12 financialData,
                                                                             List<MEPInterception_RecType13> sourceSpecificData,
                                                                             ref FileAuditData fileAuditData,
                                                                             ref int errorCount,
                                                                             out bool isValidData)
        {
            DateTime now = DateTime.Now;
            bool isVariation = (baseData.dat_Appl_LiSt_Cd == "17");

            isValidData = true;

            var interceptionApplication = ExtractInterceptionBaseData(baseData, interceptionData, now);

            ExtractAndValidateDefaultFinancialInformation(isVariation, financialData, fileAuditData, ref isValidData,
                                                          now, interceptionApplication);

            foreach (var sourceSpecific in sourceSpecificData)
                interceptionApplication.HldbCnd.Add(ExtractAndValidateSourceSpecificFinancialInformation(sourceSpecific,
                                                                                                         fileAuditData,
                                                                                                         ref isValidData,
                                                                                                         now,
                                                                                                         interceptionApplication));

            if (!isValidData)
                errorCount++;

            return interceptionApplication;
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
                Appl_Lgl_Dte = baseData.dat_Appl_Lgl_Dte.Date,
                Appl_Dbtr_SurNme = baseData.dat_Appl_Dbtr_SurNme,
                Appl_Dbtr_FrstNme = baseData.dat_Appl_Dbtr_FrstNme,
                Appl_Dbtr_MddleNme = baseData.dat_Appl_Dbtr_MddleNme,
                Appl_Dbtr_Brth_Dte = baseData.dat_Appl_Dbtr_Brth_Dte.Date,
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
                Appl_Reactv_Dte = baseData.dat_Appl_Reactv_Dte,

                AppLiSt_Cd = (ApplicationState)(baseData.dat_Appl_LiSt_Cd.Convert<int>()),
                Appl_SIN_Cnfrmd_Ind = 0,
                ActvSt_Cd = "A",

                Appl_Dbtr_LngCd = interceptionData.dat_Appl_Dbtr_LngCd,
                Appl_Dbtr_Addr_Ln = interceptionData.dat_Appl_Dbtr_Addr_Ln,
                Appl_Dbtr_Addr_Ln1 = interceptionData.dat_Appl_Dbtr_Addr_Ln1,
                Appl_Dbtr_Addr_CityNme = interceptionData.dat_Appl_Dbtr_Addr_CityNme,
                Appl_Dbtr_Addr_CtryCd = interceptionData.dat_Appl_Dbtr_Addr_CtryCd,
                Appl_Dbtr_Addr_PCd = interceptionData.dat_Appl_Dbtr_Addr_PCd,
                Appl_Dbtr_Addr_PrvCd = interceptionData.dat_Appl_Dbtr_Addr_PrvCd,
                Appl_Crdtr_SurNme = interceptionData.dat_Appl_Crdtr_SurNme,
                Appl_Crdtr_FrstNme = interceptionData.dat_Appl_Crdtr_FrstNme,
                Appl_Crdtr_MddleNme = interceptionData.dat_Appl_Crdtr_MddleNme,
                Appl_Crdtr_Brth_Dte = interceptionData.dat_Appl_Crdtr_Brth_Dte
            };
            return interceptionApplication;
        }

        private bool ExtractAndValidateDefaultFinancialInformation(bool isVariation,
                                                                   MEPInterception_RecType12 financialData,
                                                                   FileAuditData fileAuditData,
                                                                   ref bool isValidData,
                                                                   DateTime now,
                                                                   InterceptionApplicationData interceptionApplication)
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
                intFinH.IntFinH_VarIss_Dte = financialData.dat_IntFinH_VarIss_Dte ?? DateTime.Now;

            intFinH.IntFinH_VarIss_Dte = financialData.dat_IntFinH_VarIss_Dte;

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

            // fix and validate various options
            if (intFinH.IntFinH_DefHldbAmn_Money is null)
            {
                intFinH.IntFinH_DefHldbAmn_Money = 0;
                intFinH.IntFinH_DefHldbAmn_Period = null;
            }
            else
            {
                if (intFinH.IntFinH_DefHldbAmn_Period is null)
                {
                    fileAuditData.ApplicationMessage = Translate("Default Holdback Amount (<IntFinH_DefHldbAmn_Money>) provided with no default holdback amount period code (<dat_IntFinH_DefHldbAmn_Period>)");
                    isValidData = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(intFinH.PymPr_Cd))
                    {
                        if (intFinH.IntFinH_DefHldbAmn_Period.ToUpper() != "C")
                        {
                            fileAuditData.ApplicationMessage = Translate("Invalid Default Holdback Amount Period Code (must be monthly) (<dat_IntFinH_DefHldbAmn_Period>)");
                            isValidData = false;
                        }
                        else
                        {
                            if (intFinH.IntFinH_DefHldbAmn_Period.ToUpper() == intFinH.PymPr_Cd.ToUpper())
                            {
                                fileAuditData.ApplicationMessage = Translate("Default Holdback Amount Period Code and Payment Period Code (both must be Monthly) (<dat_IntFinH_DefHldbAmn_Period> <dat_PymPr_Cd)");
                                isValidData = false;
                            }
                            else
                            {
                                if (intFinH.IntFinH_DefHldbAmn_Period.ToUpper() != "C")
                                {
                                    fileAuditData.ApplicationMessage = Translate("Invalid Default Holdback Amount Period Code (must be monthly) (<dat_IntFinH_DefHldbAmn_Period>)");
                                    isValidData = false;
                                }
                                if (intFinH.PymPr_Cd.ToUpper() != "C")
                                {
                                    fileAuditData.ApplicationMessage = Translate("Invalid Payment Period Code (must be Monthly or N/A when a Default Fixed Amount is chosen) (<dat_PymPr_Cd>)");
                                    isValidData = false;
                                }
                            }
                        }
                    }
                }
            }

            if (intFinH.IntFinH_DefHldbPrcnt is null)
                intFinH.IntFinH_DefHldbPrcnt = 0;
            else if (intFinH.IntFinH_DefHldbPrcnt is < 0 or > 100)
            {
                fileAuditData.ApplicationMessage = Translate("Invalid percentage (<IntFinH_DefHldbPrcnt>) was submitted with an amount < 0 or > 100");
                isValidData = false;
            }
            
            if (intFinH.IntFinH_DefHldbPrcnt is 0 && intFinH.IntFinH_DefHldbAmn_Money is 0)
            {
                intFinH.HldbTyp_Cd = null;
                intFinH.IntFinH_DefHldbPrcnt = null;
                intFinH.IntFinH_DefHldbAmn_Money = null;
                intFinH.IntFinH_DefHldbAmn_Period = null;
            }
            else if (intFinH.IntFinH_DefHldbPrcnt is > 0 && intFinH.IntFinH_DefHldbAmn_Money is 0)
            {
                intFinH.HldbTyp_Cd = "T";
                intFinH.IntFinH_DefHldbAmn_Money = null;
                intFinH.IntFinH_DefHldbAmn_Period = null;
            }
            else
            {
                intFinH.HldbTyp_Cd = "P";
                intFinH.IntFinH_DefHldbPrcnt = null;
            }

            if (intFinH.IntFinH_PerPym_Money is not null)
            {
                if (intFinH.IntFinH_PerPym_Money == 0)
                {
                    intFinH.IntFinH_PerPym_Money = null;
                    intFinH.PymPr_Cd = null;
                    intFinH.IntFinH_CmlPrPym_Ind = null;
                    fileAuditData.ApplicationMessage = Translate("Success. Periodic Payment Amount (<dat_IntFinH_Perpym_Money>) was not submitted. All data for Periodic Payment Amount (<dat_IntFinH_Perpym_Money>), Frequency Payment Code (<PymPr_Cd>) and Cumulative Payment Indicator (<dat_IntFinH_CmlPrPym_Ind>) has been removed");
                }
                else
                {
                    if (intFinH.PymPr_Cd is null)
                    {
                        fileAuditData.ApplicationMessage = Translate("Periodic Amount (<dat_IntFinH_Perpym_Money>) provided with no frequency payment code (<dat_PymPr_Cd>)");
                        isValidData = false;
                    }
                    else
                    {
                        var rEx = new Regex("^[A-G]$");
                        if (!rEx.IsMatch(intFinH.PymPr_Cd.ToUpper()))
                        {
                            fileAuditData.ApplicationMessage = Translate("Invalid Frequency Payment Code (<dat_PymPr_Cd>)");
                            isValidData = false;
                        }
                    }
                    if (intFinH.IntFinH_CmlPrPym_Ind is null)
                    {
                        fileAuditData.ApplicationMessage = Translate("Periodic Payment Amount (<dat_IntFinH_Perpym_Money>) was submitted with an amount > 0. Cumulative Payment Indicator (<dat_IntFinH_CmlPrPym_Ind>) does not exist or is invalid");
                        isValidData = false;
                    }
                }
            }
            else
            {
                if (intFinH.PymPr_Cd is not null)
                {
                    intFinH.IntFinH_PerPym_Money = null;
                    intFinH.PymPr_Cd = null;
                    intFinH.IntFinH_CmlPrPym_Ind = null;
                    fileAuditData.ApplicationMessage = Translate("Success. Periodic Payment Amount (<dat_IntFinH_Perpym_Money>) was not submitter. All data for Periodic Payment Amount (<dat_IntFinH_Perpym_Money>), Frequency Payment Code (<PymPr_Cd>) and Cumulative Payment Indicator (<dat_IntFinH_CmlPrPym_Ind>) has been removed");
                }
            }

            // CR 672 - The amount in the arrears field (I01) should not be able to be negative via FTP (FTP) 
            if (intFinH.IntFinH_LmpSum_Money < 0)
            {
                fileAuditData.ApplicationMessage = Translate("Lump Sum Amount (<dat_IntFinH_LmpSum_Money>) was submitted with an amount < 0");
                isValidData = false;
            }
            // CR 672 - dat_IntFinH_LmpSum_Money and dat_IntFinH_Perpym_Money both have no value
            if ((intFinH.IntFinH_LmpSum_Money == 0) && (intFinH.IntFinH_PerPym_Money is null))
            {
                fileAuditData.ApplicationMessage = Translate("Lump Sum Amount (<dat_IntFinH_LmpSum_Money>) and Periodic Payment Amount (<dat_IntFinH_Perpym_Money>) both sent without a value.");
                isValidData = false;
            }

            if (intFinH.IntFinH_NextRecalcDate_Cd is not null)
            {
                if ((intFinH.PymPr_Cd is null) || (intFinH.PymPr_Cd.ToUpper().NotIn("A", "B", "C")))
                {
                    fileAuditData.ApplicationMessage = Translate("Warning: Next Recalculation Date Code (<dat_IntFinH_NextRecalc_Dte>) can only be used if the payment period is monthly, weekly or bi-weekly. Value will be ignored.");
                    intFinH.IntFinH_NextRecalcDate_Cd = null;
                }
                else
                {
                    int nextRecalcCode = intFinH.IntFinH_NextRecalcDate_Cd.Value;

                    if ((intFinH.PymPr_Cd.ToUpper() == "A") && (nextRecalcCode is < 1 or > 7))
                    {
                        fileAuditData.ApplicationMessage = Translate("Warning: Invalid value for Next Recalculation Date Code (<dat_IntFinH_NextRecalc_Dte>). Must be between 1 and 7 for weekly the payment period. Value will be ignored.");
                        intFinH.IntFinH_NextRecalcDate_Cd = null;
                    }
                    else if ((intFinH.PymPr_Cd.ToUpper() == "B") && (nextRecalcCode is < 1 or > 14))
                    {
                        fileAuditData.ApplicationMessage = Translate("Warning: Invalid value for Next Recalculation Date Code (<dat_IntFinH_NextRecalc_Dte>). Must be between 1 and 14 for bi-weekly the payment period. Value will be ignored.");
                        intFinH.IntFinH_NextRecalcDate_Cd = null;
                    }
                    else if ((intFinH.PymPr_Cd.ToUpper() == "C") && (nextRecalcCode is < 1 or > 31))
                    {
                        fileAuditData.ApplicationMessage = Translate("Warning: Invalid value for Next Recalculation Date Code (<dat_IntFinH_NextRecalc_Dte>). Must be between 1 and 31 for monthly the payment period. Value will be ignored.");
                        intFinH.IntFinH_NextRecalcDate_Cd = null;
                    }
                }
            }

            return isValidData;
        }

        private HoldbackConditionData ExtractAndValidateSourceSpecificFinancialInformation(MEPInterception_RecType13 sourceSpecific,
                                                                                           FileAuditData fileAuditData,
                                                                                           ref bool isValidData,
                                                                                           DateTime now,
                                                                                           InterceptionApplicationData interceptionApplication)
        {
            var newHoldback = new HoldbackConditionData
            {
                Appl_CtrlCd = interceptionApplication.Appl_CtrlCd,
                Appl_EnfSrv_Cd = interceptionApplication.Appl_EnfSrv_Cd,
                IntFinH_Dte = now,
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
            if (newHoldback.HldbCnd_SrcHldbPrcnt is null)
                newHoldback.HldbCnd_SrcHldbPrcnt = 0;
            else if (newHoldback.HldbCnd_SrcHldbPrcnt is < 0 or > 100)
            {
                fileAuditData.ApplicationMessage = Translate("Invalid percentage (<dat_HldbCnd_SrcHldbPrcnt>) was submitted with an amount < 0 or > 100");
                isValidData = false;
            }

            if ((newHoldback.HldbCnd_MxmPerChq_Money is not null) && (newHoldback.HldbCnd_SrcHldbAmn_Money is not null))
            {
                Repositories.ErrorTrackingDB.MessageBrokerError("Source Specific holdback amount in both fixed and per transaction",
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

            return newHoldback;
        }

    }
}
