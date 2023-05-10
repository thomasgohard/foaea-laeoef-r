namespace FileBroker.Business;

public partial class IncomingFederalTracingManager
{
    public enum EFederalSource
    {
        Unknown,
        CRA,
        EI,
        NETP
    }

    public async Task<List<string>> ProcessFlatFileAsync(string flatFileContent, string flatFileName)
    {
        var fileTableData = await GetFileTableDataAsync(flatFileName);
        var tracingFileData = new FedTracingFileBase();
        (string enfSrvCd, EFederalSource fedSource) = ConfigureTracingFileData(tracingFileData, fileTableData.Name);

        var errors = new List<string>();

        if (fedSource == EFederalSource.Unknown)
            errors.Add($"No match for [{fileTableData.Name}] or not support federal tracing source.");

        else if (!fileTableData.Active.HasValue || !fileTableData.Active.Value)
            errors.Add($"[{fileTableData.Name}] is not active.");

        if (errors.Any())
            return errors;

        await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, true);

        string fileCycle = Path.GetExtension(flatFileName)[1..];
        try
        {
            var fileLoader = new IncomingFederalTracingFileLoader(DB.FlatFileSpecs, fileTableData.PrcId);
            await fileLoader.FillTracingFileDataFromFlatFileAsync(tracingFileData, flatFileContent, errors);

            if (errors.Any())
                return errors;

            ValidateHeader(tracingFileData.TRCIN01, flatFileName, ref errors);
            ValidateFooter(tracingFileData.TRCIN99, tracingFileData.TRCIN02, ref errors);

            if (errors.Any())
                return errors;

            await FoaeaAccess.SystemLoginAsync();
            try
            {

                if ((tracingFileData.TRCIN02.Count == 0) && (fedSource == EFederalSource.NETP))
                    await CloseNETPTraceEventsAsync();
                else
                {
                    var tracingResponses = await ExtractTracingResponsesFromFileDataAsync(tracingFileData, enfSrvCd, fileCycle, errors);

                    if (errors.Any())
                        return errors;

                    if ((tracingResponses != null) && (tracingFileData.TRCIN02.Count > 0))
                        await SendTracingResponsesToFoaea(tracingFileData.TRCIN02, tracingResponses, fileTableData.PrcId,
                                                          enfSrvCd, fedSource, fileCycle, flatFileName, errors);
                }
            }
            finally
            {
                await FoaeaAccess.SystemLogoutAsync();
            }

        }
        catch (Exception e)
        {
            errors.Add("An error occurred: " + e.Message);
        }
        finally
        {
            await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, fileCycle.Length);
            await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, false);
        }

        return errors;

    }

    private static void ValidateHeader(FedTracing_RecType01 dataFromFile, string flatFileName, ref List<string> errors)
    {
        int cycle = FileHelper.ExtractCycleFromFilename(flatFileName);
        if (dataFromFile.Cycle != cycle)
        {
            errors.Add($"Cycle in file [{dataFromFile.Cycle}] does not match cycle of file [{cycle}]");
        }
    }

    private static void ValidateFooter(FedTracing_RecType99 dataFromFile, List<FedTracing_RecType02> dataSummary, ref List<string> errors)
    {
        if (dataFromFile.ResponseCnt != dataSummary.Count)
        {
            errors.Add("Invalid ResponseCnt in section 99");
        }
    }

    private static (string enfSrvCd, EFederalSource fedSource) ConfigureTracingFileData(
                                                                       FedTracingFileBase fedTracingData,
                                                                       string fileTableName)
    {
        string enfSrvCd = string.Empty;
        EFederalSource fedSource = EFederalSource.Unknown;

        switch (fileTableName)
        {
            case "RC3STSIT": // CRA Tracing
                enfSrvCd = "RC01";
                fedSource = EFederalSource.CRA;
                fedTracingData.AddResidentialRecTypes("03", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                                                      "16", "17", "18", "19", "20", "21");
                fedTracingData.AddEmployerRecTypes("04", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32",
                                                   "33", "34", "35", "36");
                break;

            case "HR3STSIT": // EI Tracing
                enfSrvCd = "HR01";
                fedSource = EFederalSource.EI;
                fedTracingData.AddResidentialRecTypes("03");
                fedTracingData.AddEmployerRecTypes("04");
                break;

            case "EI3STSIT":  // NETP Tracing
                enfSrvCd = "EI02";
                fedSource = EFederalSource.NETP;
                fedTracingData.AddEmployerRecTypes("80", "81");
                break;
        }

        return (enfSrvCd, fedSource);
    }

    public async Task SendTracingResponsesToFoaea(List<FedTracing_RecType02> fileTracingSummary, List<TraceResponseData> tracingResponses,
                                                  int processId, string enfSrvCd, EFederalSource fedSource, string fileCycle,
                                                  string flatFileName, List<string> errors)
    {
        try
        {
            string cutOffDaysValue = await DB.ProcessParameterTable.GetValueForParameterAsync(processId, "evnt_cutoff");
            int cutOffDays = int.Parse(cutOffDaysValue);

            var activeTraceEvents = await APIs.TracingEvents.GetRequestedTRCINEventsAsync(enfSrvCd, fileCycle);
            var activeTraceEventDetails = await APIs.TracingEvents.GetActiveTracingEventDetailsAsync(enfSrvCd, fileCycle);

            if (fedSource == EFederalSource.NETP)
            {
                foreach (var item in fileTracingSummary)
                {
                    await MarkTraceEventsAsProcessedAsync(item.dat_Appl_EnfSrvCd, item.dat_Appl_CtrlCd, flatFileName, newState: 2,
                                                          activeTraceEvents, activeTraceEventDetails);
                }
                await CloseOrInactivateTraceEventDetailsAsync(cutOffDays, activeTraceEventDetails);
                await SendTRACEDataToTrcRspAsync(tracingResponses);
                await CloseNETPTraceEventsAsync();
            }
            else
            {
                foreach (var item in fileTracingSummary)
                {
                    var appl = await APIs.TracingApplications.GetApplicationAsync(item.dat_Appl_EnfSrvCd, item.dat_Appl_CtrlCd);

                    await MarkTraceEventsAsProcessedAsync(item.dat_Appl_EnfSrvCd, item.dat_Appl_CtrlCd, flatFileName, (short)appl.AppLiSt_Cd,
                                                          activeTraceEvents, activeTraceEventDetails);
                }
                await CloseOrInactivateTraceEventDetailsAsync(cutOffDays, activeTraceEventDetails);
                await SendTRACEDataToTrcRspAsync(tracingResponses);
                await UpdateTracingApplicationsAsync(enfSrvCd, fileCycle);
                await CloseOrInactivateTraceEventDetailsAsync(cutOffDays, activeTraceEventDetails);
            }
        }
        catch (Exception e)
        {
            errors.Add("Error sending tracing responses to FOAEA: " + e.Message);
        }
    }

    private async Task<FileTableData> GetFileTableDataAsync(string flatFileName)
    {
        string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

        return await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
    }

    private async Task<List<TraceResponseData>> ExtractTracingResponsesFromFileDataAsync(FedTracingFileBase tracingFileData, string enfSrvCd, string fileCycle,
                                                                        List<string> errors)
    {
        var cycles = await APIs.TracingApplications.GetTraceCycleQuantityDataAsync(enfSrvCd, fileCycle);

        return IncomingFederalTracingResponse.GenerateFromFileData(tracingFileData, enfSrvCd, cycles, ref errors);
    }
    
    private async Task CloseNETPTraceEventsAsync()
    {
        await APIs.TracingEvents.CloseNETPTraceEventsAsync();
    }

    private async Task SendTRACEDataToTrcRspAsync(List<TraceResponseData> responseData)
    {
        await APIs.TracingResponses.InsertBulkDataAsync(responseData);
    }

}
