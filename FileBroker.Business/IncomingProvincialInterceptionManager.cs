using Newtonsoft.Json;

namespace FileBroker.Business
{
    public class IncomingProvincialInterceptionManager
    {
        private string FileName { get; }
        private APIBrokerList APIs { get; }
        private RepositoryList Repositories { get; }
        private ProvincialAuditFileConfig AuditConfiguration { get; }

        public IncomingProvincialInterceptionManager(string fileName,
                                                     APIBrokerList apis,
                                                     RepositoryList repositories,
                                                     ProvincialAuditFileConfig auditConfig)
        {
            FileName = fileName;
            APIs = apis;
            Repositories = repositories;
            AuditConfiguration = auditConfig;
        }

        public MessageDataList ExtractAndProcessRequestsInFile(string sourceInterceptionData, bool includeInfoInMessages = false)
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
                                Application = GetInterceptionApplicationDataFromRequest(data, interceptionData,
                                                                                        financialData, sourceSpecificData),
                                MaintenanceAction = data.Maintenance_ActionCd,
                                MaintenanceLifeState = data.dat_Appl_LiSt_Cd,
                                NewRecipientSubmitter = data.dat_New_Owner_RcptSubmCd,
                                NewIssuingSubmitter = data.dat_New_Owner_SubmCd,
                                NewUpdateSubmitter = data.dat_Update_SubmCd
                            };

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

                                fileAuditData.ApplicationMessage = "Success";
                                successCount++;
                            }

                        }
                        else
                        {
                            fileAuditData.ApplicationMessage = requestError[0].Description;
                            errorCount++;
                        }

                        Repositories.FileAudit.InsertFileAuditData(fileAuditData);

                    }

                    fileAuditManager.GenerateAuditFile(FileName, errorCount, warningCount, successCount);
                    fileAuditManager.SendStandardAuditEmail(FileName, AuditConfiguration.AuditRecipients, errorCount, warningCount, successCount);
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

        private static InterceptionApplicationData GetInterceptionApplicationDataFromRequest(MEPInterception_RecType10 baseData,
                                                                                             MEPInterception_RecType11 interceptionData,
                                                                                             MEPInterception_RecType12 financialData,
                                                                                             List<MEPInterception_RecType13> sourceSpecificData)
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
                Appl_Rcptfrm_Dte = baseData.dat_Appl_Rcptfrm_dte.Date,
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

            DateTime now = DateTime.Now;

            interceptionApplication.IntFinH.Appl_EnfSrv_Cd = interceptionApplication.Appl_EnfSrv_Cd;
            interceptionApplication.IntFinH.Appl_CtrlCd = interceptionApplication.Appl_CtrlCd;
            interceptionApplication.IntFinH.IntFinH_Dte = now;
            interceptionApplication.IntFinH.IntFinH_LmpSum_Money = financialData.dat_IntFinH_LmpSum_Money.Convert<decimal>();
            interceptionApplication.IntFinH.IntFinH_PerPym_Money = financialData.dat_IntFinH_Perpym_Money.Convert<decimal?>();
            interceptionApplication.IntFinH.PymPr_Cd = financialData.dat_PymPr_Cd;
            interceptionApplication.IntFinH.IntFinH_NextRecalcDate_Cd = financialData.dat_IntFinH_NextRecalc_Dte.Convert<int?>();
            interceptionApplication.IntFinH.HldbCtg_Cd = financialData.dat_HldbCtg_Cd;
            interceptionApplication.IntFinH.IntFinH_DefHldbPrcnt = financialData.dat_IntFinH_DfHldbPrcnt.Convert<int?>();
            interceptionApplication.IntFinH.IntFinH_DefHldbAmn_Money = financialData.dat_IntFinH_DefHldbAmn_Money.Convert<decimal?>();
            interceptionApplication.IntFinH.IntFinH_DefHldbAmn_Period = financialData.dat_IntFinH_DefHldbAmn_Period;
            if (baseData.dat_Appl_LiSt_Cd == "17")
                interceptionApplication.IntFinH.IntFinH_VarIss_Dte = financialData.dat_IntFinH_VarIss_Dte ?? DateTime.Now;

            interceptionApplication.IntFinH.IntFinH_VarIss_Dte = financialData.dat_IntFinH_VarIss_Dte;

            interceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind = financialData.dat_IntFinH_CmlPrPym_Ind.Convert<byte?>();
            if (interceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.HasValue)
            {
                // XML Code indicating if periodic payments are cumulative: 1=no, 2=yes.
                // In the database, though, 0=no and 1=yes, so we must substract 1 from received value, if any
                interceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind--;
            }

            foreach (var sourceSpecific in sourceSpecificData)
            {
                interceptionApplication.HldbCnd.Add(new HoldbackConditionData
                {
                    Appl_CtrlCd = interceptionApplication.Appl_CtrlCd,
                    Appl_EnfSrv_Cd = interceptionApplication.Appl_EnfSrv_Cd,
                    IntFinH_Dte = now,
                    EnfSrv_Cd = sourceSpecific.dat_EnfSrv_Cd,
                    HldbCnd_MxmPerChq_Money = sourceSpecific.dat_HldbCnd_MxmPerChq_Money.Convert<decimal?>(),
                    HldbCnd_SrcHldbAmn_Money = sourceSpecific.dat_HldbCnd_Hldb_Amn_Money.Convert<decimal?>(),
                    HldbCnd_SrcHldbPrcnt = sourceSpecific.dat_HldbCnd_SrcHldbPrcnt.Convert<int?>(),
                    HldbCtg_Cd = sourceSpecific.dat_HldbCtg_Cd
                });
            }

            return interceptionApplication;
        }

    }
}
