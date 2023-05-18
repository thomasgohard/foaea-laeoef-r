using DBHelper;
using FileBroker.Common.Helpers;
using Newtonsoft.Json;

namespace FileBroker.Business;

public class IncomingProvincialTracingManager
{
    private string FileName { get; }
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }
    private IFileBrokerConfigurationHelper Config { get; }
    private IncomingProvincialHelper IncomingFileHelper { get; }
    private FoaeaSystemAccess FoaeaAccess { get; }

    public IncomingProvincialTracingManager(RepositoryList db,
                                            APIBrokerList foaeaApis,
                                            string fileName,
                                            IFileBrokerConfigurationHelper config)
    {
        FileName = fileName;
        APIs = foaeaApis;
        DB = db;
        Config = config;

        string provCode = FileName[..2].ToUpper();
        IncomingFileHelper = new IncomingProvincialHelper(Config, provCode);

        FoaeaAccess = new FoaeaSystemAccess(foaeaApis, Config.FoaeaLogin);
    }

    public async Task<MessageDataList> ExtractAndProcessRequestsInFile(string sourceTracingData, List<UnknownTag> unknownTags, bool includeInfoInMessages = false)
    {
        var result = new MessageDataList();

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(FileName);
        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

        var tracingFileData = ExtractTracingDataFromJson(sourceTracingData, out string error);
        var tracingFile = tracingFileData.NewDataSet;

        bool isValid = true;

        var fileAuditManager = new FileAuditManager(DB.FileAudit, Config, DB.MailService);

        if (!string.IsNullOrEmpty(error))
        {
            isValid = false;
            result.AddSystemError(error);
        }
        else
        {
            ValidateHeader(tracingFile.TRCAPPIN01, ref result, ref isValid);
            ValidateFooter(tracingFile, ref result, ref isValid);

            if (isValid)
            {
                int errorCount = 0;
                int warningCount = 0;
                int successCount = 0;

                if (!await FoaeaAccess.SystemLogin())
                {
                    isValid = false;
                    result.AddSystemError("Failed to login to FOAEA!");
                }
                else
                {
                    try
                    {
                        foreach (var baseData in tracingFile.TRCAPPIN20)
                        {
                            bool isValidActionCode = true;

                            var fileAuditData = new FileAuditData
                            {
                                Appl_EnfSrv_Cd = baseData.dat_Appl_EnfSrvCd,
                                Appl_CtrlCd = baseData.dat_Appl_CtrlCd,
                                Appl_Source_RfrNr = baseData.dat_Appl_Source_RfrNr,
                                InboundFilename = FileName + ".XML"
                            };

                            var requestError = new MessageDataList();

                            ValidateActionCode(baseData, ref requestError, ref isValidActionCode);

                            if (isValidActionCode)
                            {
                                var traceData = tracingFile.TRCAPPIN21?.Find(t => t.dat_Appl_CtrlCd == baseData.dat_Appl_CtrlCd);
                                var traceFinData = tracingFile.TRCAPPIN22?.Find(t => t.dat_Appl_CtrlCd == baseData.dat_Appl_CtrlCd);

                                var tracingMessage = new MessageData<TracingApplicationData>
                                {
                                    Application = CombineMatchingTracingApplicationDataFromRequest(baseData, traceData, traceFinData),
                                    MaintenanceAction = baseData.Maintenance_ActionCd,
                                    MaintenanceLifeState = baseData.dat_Appl_LiSt_Cd,
                                    NewRecipientSubmitter = baseData.dat_New_Owner_RcptSubmCd,
                                    NewIssuingSubmitter = baseData.dat_New_Owner_SubmCd,
                                    NewUpdateSubmitter = baseData.dat_Update_SubmCd
                                };

                                var messages = await SendRequestToFoaea(tracingMessage);

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

                            await DB.FileAudit.InsertFileAuditData(fileAuditData);

                        }

                        int totalFilesCount = await fileAuditManager.GenerateAuditFile(FileName + ".XML", unknownTags, errorCount, warningCount, successCount);
                        await fileAuditManager.SendStandardAuditEmail(FileName + ".XML", Config.AuditConfig.AuditRecipients,
                                                                      errorCount, warningCount, successCount, unknownTags.Count,
                                                                      totalFilesCount);
                    }
                    finally
                    {
                        await FoaeaAccess.SystemLogout();
                    }
                }
            }

        }

        if (!isValid)
        {
            result.AddSystemError($"One of more error(s) occured trying to load file ({FileName}.XML)");

            await fileAuditManager.SendSystemErrorAuditEmail(FileName, Config.AuditConfig.AuditRecipients, result);
        }

        if (!result.ContainsMessagesOfType(MessageType.Error))
        {
            await DB.FileAudit.MarkFileAuditCompletedForFile(FileName);
            await DB.FileTable.SetNextCycleForFileType(fileTableData);
        }

        return result;
    }

    public async Task<MessageDataList> SendRequestToFoaea(MessageData<TracingApplicationData> tracingMessageData)
    {
        TracingApplicationData tracing;

        if (tracingMessageData.MaintenanceAction == "A")
        {
            tracing = await APIs.TracingApplications.CreateTracingApplicationAsync(tracingMessageData.Application);
        }
        else // if (tracingMessageData.MaintenanceAction == "C")
        {
            switch (tracingMessageData.MaintenanceLifeState)
            {
                case "00": // change
                case "0":
                    tracing = await APIs.TracingApplications.UpdateTracingApplicationAsync(tracingMessageData.Application);
                    break;

                case "14": // cancellation
                    tracingMessageData.Application.AppLiSt_Cd = ApplicationState.MANUALLY_TERMINATED_14;
                    tracing = await APIs.TracingApplications.CancelTracingApplicationAsync(tracingMessageData.Application);
                    break;

                case "29": // transfer
                    tracing = await APIs.TracingApplications.TransferTracingApplicationAsync(tracingMessageData.Application,
                                                                                  tracingMessageData.NewRecipientSubmitter,
                                                                                  tracingMessageData.NewIssuingSubmitter);
                    break;

                default:
                    tracing = tracingMessageData.Application;
                    tracing.Messages.AddError($"Unknown dat_Appl_LiSt_Cd ({tracingMessageData.MaintenanceLifeState})" +
                                              $" for Maintenance_ActionCd ({tracingMessageData.MaintenanceAction})");
                    break;
            }
        }

        return tracing.Messages;
    }

    private void ValidateHeader(MEPTracing_RecType01 headerData, ref MessageDataList result, ref bool isValid)
    {
        int cycle = FileHelper.ExtractCycleFromFilename(FileName);
        if (int.Parse(headerData.Cycle) != cycle)
        {
            isValid = false;
            result.AddSystemError($"Cycle in file [{headerData.Cycle}] does not match cycle of file [{cycle}]");
        }
        if (!IncomingFileHelper.IsValidTermsAccepted(headerData.TermsAccepted))
        {
            isValid = false;
            result.AddSystemError($"type 01 Terms Accepted invalid text: {headerData.TermsAccepted}");
        }
    }

    private static void ValidateFooter(MEPTracing_TracingDataSet tracingFile, ref MessageDataList result, ref bool isValid)
    {
        if (int.Parse(tracingFile.TRCAPPIN99.ResponseCnt) != tracingFile.TRCAPPIN20.Count)
        {
            isValid = false;
            result.AddSystemError("Invalid ResponseCnt in section 99");
        }
    }

    private static void ValidateActionCode(MEPTracing_RecType20 data, ref MessageDataList result, ref bool isValid)
    {
        bool validActionLifeState = true;
        string actionCode = data.Maintenance_ActionCd.Trim();
        string actionState = data.dat_Appl_LiSt_Cd.Trim();

        if ((actionCode == "A") && actionState.NotIn("00", "0"))
            validActionLifeState = false;
        else if ((actionCode == "C") && (actionState.NotIn("00", "0", "14", "29")))
            validActionLifeState = false;
        else if (actionCode.NotIn("A", "C"))
            validActionLifeState = false;

        if (!validActionLifeState)
        {
            isValid = false;
            result.AddSystemError($"Invalid MaintenanceAction [{actionCode}] and MaintenanceLifeState [{actionState}] combination.");
        }

    }

    private static MEPTracingFileData ExtractTracingDataFromJson(string sourceTracingData, out string error)
    {
        error = string.Empty;

        MEPTracingFileData result;
        try
        {
            result = JsonConvert.DeserializeObject<MEPTracingFileData>(sourceTracingData);
        }
        catch (Exception e)
        {
            error = e.Message;
            result = new MEPTracingFileData();
        }

        return result;
    }

    private static TracingApplicationData CombineMatchingTracingApplicationDataFromRequest(MEPTracing_RecType20 baseData,
                                                                                           MEPTracing_RecType21? tracingData,
                                                                                           MEPTracing_RecType22? tracingFinData)
    {
        var tracingApplication = new TracingApplicationData
        {
            Appl_EnfSrv_Cd = baseData.dat_Appl_EnfSrvCd,
            Appl_CtrlCd = baseData.dat_Appl_CtrlCd,
            Appl_Source_RfrNr = baseData.dat_Appl_Source_RfrNr,
            Subm_Recpt_SubmCd = baseData.dat_Subm_Rcpt_SubmCd,
            Subm_SubmCd = baseData.dat_Subm_SubmCd,
            Appl_Lgl_Dte = baseData.dat_Appl_Lgl_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? default,
            Appl_Dbtr_SurNme = baseData.dat_Appl_Dbtr_SurNme,
            Appl_Dbtr_FrstNme = baseData.dat_Appl_Dbtr_FrstNme,
            Appl_Dbtr_MddleNme = baseData.dat_Appl_Dbtr_MddleNme,
            Appl_Dbtr_Brth_Dte = baseData.dat_Appl_Dbtr_Brth_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date,
            Appl_Dbtr_Gendr_Cd = baseData.dat_Appl_Dbtr_Gendr_Cd.Trim() == "" ? "M" : baseData.dat_Appl_Dbtr_Gendr_Cd.Trim(),
            Appl_Dbtr_Entrd_SIN = baseData.dat_Appl_Dbtr_Entrd_SIN,
            Appl_Dbtr_Parent_SurNme_Birth = baseData.dat_Appl_Dbtr_Parent_SurNme_Birth,
            Appl_CommSubm_Text = baseData.dat_Appl_CommSubm_Text,
            Appl_Rcptfrm_Dte = baseData.dat_Appl_Rcptfrm_dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? default,
            AppCtgy_Cd = baseData.dat_Appl_AppCtgy_Cd,
            Appl_Group_Batch_Cd = baseData.dat_Appl_Group_Batch_Cd,
            Medium_Cd = baseData.dat_Appl_Medium_Cd,
            Appl_Affdvt_DocTypCd = baseData.dat_Appl_Affdvt_Doc_TypCd,
            AppReas_Cd = baseData.dat_Appl_Reas_Cd,
            Appl_Reactv_Dte = baseData.dat_Appl_Reactv_Dte.ConvertToDateTimeIgnoringTimeZone(),
            AppLiSt_Cd = (ApplicationState)int.Parse(baseData.dat_Appl_LiSt_Cd),
            Appl_SIN_Cnfrmd_Ind = 0,
            ActvSt_Cd = "A",

            // tracing data
            Appl_Crdtr_SurNme = tracingData?.dat_Appl_Crdtr_SurNme,
            Appl_Crdtr_FrstNme = tracingData?.dat_Appl_Crdtr_FrstNme,
            Appl_Crdtr_MddleNme = tracingData?.dat_Appl_Crdtr_MddleNme,
            Trace_Child_Text = tracingData?.dat_Trace_Child_Text,
            Trace_Breach_Text = tracingData?.dat_Trace_Breach_Text,
            Trace_ReasGround_Text = tracingData?.dat_Trace_ReasGround_Text,
            FamPro_Cd = tracingData?.dat_FamPro_Cd,
            Statute_Cd = tracingData?.dat_Statute_Cd,
            Trace_LstCyclCmp_Dte = DateTime.Now,
            Trace_LiSt_Cd = 0,
            InfoBank_Cd = tracingData?.dat_InfoBank_Cd,

            // new fields
            TraceInformation = tracingFinData?.dat_Tracing_Info.ConvertToShort() ?? 0,
            PhoneNumber = tracingFinData?.dat_Trace_Dbtr_PhoneNumber,
            EmailAddress = tracingFinData?.dat_Trace_Dbtr_EmailAddress,
            Declaration = tracingFinData?.dat_Trace_Declaration,
            IncludeSinInformation = tracingFinData?.dat_SIN_Information.ConvertToBool() ?? false,

            // financial data
            IncludeFinancialInformation = tracingFinData?.dat_Financial_Information.ConvertToBool() ?? false
        };

        return tracingApplication;
    }
}
