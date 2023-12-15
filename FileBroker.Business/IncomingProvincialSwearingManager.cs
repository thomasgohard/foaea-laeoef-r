using DBHelper;
using FileBroker.Model.Enums;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace FileBroker.Business;

public class IncomingProvincialSwearingManager : IncomingProvincialManagerBase
{
    public IncomingProvincialSwearingManager(RepositoryList db, APIBrokerList foaeaApis, string fileName,
                                             IFileBrokerConfigurationHelper config) : base(db, foaeaApis, fileName, config)
    {
    }

    public async Task<MessageDataList> ExtractAndProcessRequestsInFile(string sourceSwearingData, List<UnknownTag> unknownTags, bool includeInfoInMessages = false)
    {
        var result = new MessageDataList();

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(FileName);
        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

        var swearingFileData = ExtractTracingDataFromJson(sourceSwearingData, out string error);
        var swearingFile = swearingFileData.NewDataSet;

        bool isValid = true;

        var fileAuditManager = new FileAuditManager(DB.FileAudit, Config, DB.MailService);

        if (!string.IsNullOrEmpty(error))
        {
            isValid = false;
            result.AddSystemError(error);
        }
        else
        {
            ValidateHeader(swearingFile.Header, result, ref isValid);
            ValidateFooter(swearingFile, result, ref isValid);

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
                        foreach (var swearingDetails in swearingFile.SwearingDetail)
                        {
                            var appl = await APIs.TracingApplications.GetApplication(swearingDetails.EnfSrv_Cd, swearingDetails.Appl_CtrlCd);

                            var fileAuditData = new FileAuditData
                            {
                                Appl_EnfSrv_Cd = swearingDetails.EnfSrv_Cd,
                                Appl_CtrlCd = swearingDetails.Appl_CtrlCd.Trim(),
                                Appl_Source_RfrNr = appl?.Appl_Source_RfrNr ?? "No Reference Number",
                                InboundFilename = FileName + ".XML"
                            };

                            if ((appl == null) || (string.IsNullOrEmpty(appl.Appl_CtrlCd)))
                            {
                                errorCount++;

                                fileAuditData.ApplicationMessage = $"Application {swearingDetails.EnfSrv_Cd.Trim()} {swearingDetails.Appl_CtrlCd.Trim()} does not exist. Swearing held for 90 days.";

                                await StoreSwearingAffidavitInfo(swearingDetails);
                            }
                            else if (appl.AppLiSt_Cd != ApplicationState.PENDING_ACCEPTANCE_SWEARING_6)
                            {
                                if ((appl.AppLiSt_Cd > ApplicationState.APPLICATION_REJECTED_9) &&
                                    (appl.AppLiSt_Cd != ApplicationState.MANUALLY_TERMINATED_14))
                                {
                                    errorCount++;
                                    fileAuditData.ApplicationMessage = $"Application {swearingDetails.EnfSrv_Cd.Trim()} {swearingDetails.Appl_CtrlCd.Trim()} has already been sworn.";
                                }
                                else if (appl.AppLiSt_Cd == ApplicationState.APPLICATION_REJECTED_9)
                                {
                                    errorCount++;
                                    fileAuditData.ApplicationMessage = $"Application {swearingDetails.EnfSrv_Cd.Trim()} {swearingDetails.Appl_CtrlCd.Trim()} has been rejected.";
                                }
                                else if (appl.AppLiSt_Cd == ApplicationState.MANUALLY_TERMINATED_14)
                                {
                                    errorCount++;
                                    fileAuditData.ApplicationMessage = $"Application {swearingDetails.EnfSrv_Cd.Trim()} {swearingDetails.Appl_CtrlCd.Trim()} has been cancelled.";
                                }
                                else
                                {
                                    successCount++;
                                    fileAuditData.ApplicationMessage = "Success";

                                    await StoreSwearingAffidavitInfo(swearingDetails);
                                }
                            }
                            else
                            {
                                var requestError = new MessageDataList();

                                var loadInboundAuditTable = DB.LoadInboundAuditTable;
                                var fileName = FileName + ".XML";

                                if (!await loadInboundAuditTable.RowExists(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName))
                                {
                                    await loadInboundAuditTable.AddRow(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName);

                                    var foaeaMessages = await SendRequestToFoaea(appl, swearingDetails);

                                    ProcessMessages(foaeaMessages, fileAuditData, includeInfoInMessages, result,
                                                    ref errorCount, ref warningCount, ref successCount);

                                    await loadInboundAuditTable.MarkRowAsCompleted(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, appl.Appl_Source_RfrNr, fileName);
                                }
                            }

                            await DB.FileAudit.InsertFileAuditData(fileAuditData);
                        }

                        AuditFileFormat outputFormat = await GetAuditOutputFormat(fileTableData.PrcId);
                        int totalFilesCount = await fileAuditManager.GenerateProvincialAuditFile(FileName + ".XML", unknownTags,
                                                                                                 errorCount, warningCount, successCount,
                                                                                                 outputFormat);

                        await fileAuditManager.SendStandardAuditEmail(FileName + ".XML", Config.AuditConfig.AuditRecipients,
                                                                      errorCount, warningCount, successCount, unknownTags.Count,
                                                                      totalFilesCount, outputFormat);
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

    private async Task StoreSwearingAffidavitInfo(MEPSwearing_RecType61 swearingDetails)
    {
        var swornDate = swearingDetails.Affdvt_Sworn_Dte.ConvertToDateTimeIgnoringTimeZone();
        if (swornDate.HasValue)
        {
            var affidavitData = new AffidavitData
            {
                EnfSrv_Cd = swearingDetails.EnfSrv_Cd,
                Appl_CtrlCd = swearingDetails.Appl_CtrlCd,
                Affdvt_Sworn_Dte = swornDate.Value,
                Subm_Affdvt_SubmCd = swearingDetails.Subm_Affdvt_SubmCd,
                Affdvt_FileRecv_Dte = DateTime.Now,
                AppCtgy_Cd = "T01",
                OriginalFileName = FileName + ".XML"
            };

            await APIs.TracingApplications.InsertAffidavit(affidavitData);
        }
    }

    private async Task<AuditFileFormat> GetAuditOutputFormat(int prcId)
    {
        var fileTableFlagData = await DB.FileTable.GetAuditFileFormatForProcessId(prcId);

        if ((fileTableFlagData is not null) && (int.TryParse(fileTableFlagData.IncludeAudit, out int formatFlag)))
            return (AuditFileFormat)formatFlag;
        else
            return AuditFileFormat.TextFormat;
    }

    private static void ProcessMessages(MessageDataList foaeaMessages, FileAuditData fileAuditData, bool includeInfoInMessages,
                                        MessageDataList result, ref int errorCount, ref int warningCount, ref int successCount)
    {
        if (foaeaMessages.ContainsMessagesOfType(MessageType.Error))
        {
            var errors = foaeaMessages.FindAll(m => m.Severity == MessageType.Error);

            fileAuditData.ApplicationMessage = errors[0].Description;
            errorCount++;
        }
        else if (foaeaMessages.ContainsMessagesOfType(MessageType.Warning))
        {
            var warnings = foaeaMessages.FindAll(m => m.Severity == MessageType.Warning);

            fileAuditData.ApplicationMessage = warnings[0].Description;
            warningCount++;
        }
        else
        {
            if (includeInfoInMessages)
            {
                var infos = foaeaMessages.FindAll(m => m.Severity == MessageType.Information);

                result.AddRange(infos);
            }

            fileAuditData.ApplicationMessage = "Success";
            successCount++;
        }
    }

    public async Task<MessageDataList> SendRequestToFoaea(TracingApplicationData appl, MEPSwearing_RecType61 swearingDetails)
    {
        appl.Appl_RecvAffdvt_Dte = swearingDetails.Affdvt_Sworn_Dte.ConvertToDateTimeIgnoringTimeZone();
        appl.Subm_Affdvt_SubmCd = swearingDetails.Subm_Affdvt_SubmCd;

        var tracing = await APIs.TracingApplications.CertifyTracingApplication(appl);

        return tracing.Messages;
    }

    private void ValidateHeader(MEPSwearing_RecType01 headerData, MessageDataList result, ref bool isValid)
    {
        int cycle = FileHelper.ExtractCycleFromFilename(FileName);
        if (int.Parse(headerData.Cycle) != cycle)
        {
            isValid = false;
            result.AddSystemError($"Cycle in file [{headerData.Cycle}] does not match cycle of file [{cycle}]");
        }
    }

    private static void ValidateFooter(MEPSwearing_SwearingDataSet tracingFile, MessageDataList result, ref bool isValid)
    {
        if (int.Parse(tracingFile.Trailer.CountOfDetailRecords) != tracingFile.SwearingDetail.Count)
        {
            isValid = false;
            result.AddSystemError("Invalid CountOfDetailRecords in section 99");
        }
    }

    private static MEPSwearingFileData ExtractTracingDataFromJson(string sourceTracingData, out string error)
    {
        error = string.Empty;

        MEPSwearingFileData result;
        try
        {
            result = JsonConvert.DeserializeObject<MEPSwearingFileData>(sourceTracingData);
        }
        catch
        {
            try
            {
                var tempResult = JsonConvert.DeserializeObject<MEPSwearingFileDataSingle>(sourceTracingData);

                result = new MEPSwearingFileData();

                if (tempResult != null)
                {
                    result.NewDataSet.Header = tempResult.NewDataSet.Header;
                    result.NewDataSet.SwearingDetail = new List<MEPSwearing_RecType61>();
                    if (tempResult.NewDataSet.SwearingDetail.RecType != null)
                    {
                        result.NewDataSet.SwearingDetail.Add(tempResult.NewDataSet.SwearingDetail);
                    };
                    result.NewDataSet.Trailer = tempResult.NewDataSet.Trailer;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                result = new MEPSwearingFileData();
            }
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
            Appl_CtrlCd = baseData.dat_Appl_CtrlCd.Trim(),
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

        if ((tracingFinData is not null) && (tracingFinData.Value.dat_Financial_Details.Tax_Data is not null))
        {
            foreach (var detail in tracingFinData.Value.dat_Financial_Details.Tax_Data)
            {
                if (short.TryParse(detail.Tax_Year, out var taxYear))
                    tracingApplication.YearsAndTaxForms.Add(taxYear, detail.Tax_Form);

                // TODO: report bad data if tryparse failed
            }
        }

        return tracingApplication;
    }


}
