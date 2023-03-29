using DBHelper;
using FileBroker.Common.Helpers;
using Newtonsoft.Json;

namespace FileBroker.Business;

public class IncomingFederalTracingManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }

    private FoaeaSystemAccess FoaeaAccess { get; }

    public enum EFederalSource
    {
        Unknown,
        CRA,
        EI,
        NETP
    }

    public IncomingFederalTracingManager(APIBrokerList apis, RepositoryList repositories,
                                         IFileBrokerConfigurationHelper config)
    {
        APIs = apis;
        DB = repositories;

        FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
    }

    public async Task<MessageDataList> ProcessXmlFileAsync(string sourceTracingData, string flatFileName)
    {
        var result = new MessageDataList();

        short cycle = (short)FileHelper.GetCycleFromFilename(flatFileName);
        var fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);
        var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

        //await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, true);

        // bool isValid = true;

        var tracingFileData = ExtractTracingFinancialDataFromJson(sourceTracingData, out string error);
        var tracingFile = tracingFileData.CRATraceIn;

        bool isValid = true;

        if (!string.IsNullOrEmpty(error))
        {
            //isValid = false;
            result.AddSystemError(error);
        }
        else
        {
            ValidateXmlHeader(tracingFile.Header, flatFileName, ref result, ref isValid);
            ValidateXmlFooter(tracingFile, ref result, ref isValid);

            if (isValid)
            {
                await FoaeaAccess.SystemLoginAsync();
                try
                {
                    foreach (var response in tracingFile.TraceResponse)
                    {
                        var traceFinancialResultData = ConvertToFoaea(response, cycle);
                        await SendTraceFinancialResultToFoaea(traceFinancialResultData);
                    }
                }
                finally
                {
                    await FoaeaAccess.SystemLogoutAsync();
                }
            }
        }

        return result;
    }

    private async Task SendTraceFinancialResultToFoaea(TraceFinancialResponseData traceFinancialResultData)
    {
        await APIs.TracingResponses.AddTraceFinancialResponseData(traceFinancialResultData);
    }

    private static TraceFinancialResponseData ConvertToFoaea(FedTracingFinancial_TraceResponse traceResponse, short cycle)
    {
        var result = new TraceFinancialResponseData
        {
            Appl_EnfSrv_Cd = traceResponse.Appl_EnfSrvCd,
            Appl_CtrlCd = traceResponse.Appl_CtrlCd,
            EnfSrv_Cd = "RC01",
            TrcRsp_Rcpt_Dte = DateTime.Now,
            TrcRsp_SeqNr = 0,
            TrcRsp_Trace_CyclNr = cycle,
            ActvSt_Cd = "A",
            Sin = traceResponse.SIN,
            SinXref = traceResponse.SIN_XRef,
            TrcSt_Cd = traceResponse.ResponseCode
        };

        if (traceResponse.Tax_Response.Tax_Data is not null)
        {
            result.TraceFinancialDetails = new List<TraceFinancialResponseDetailData>();

            foreach (var taxData in traceResponse.Tax_Response.Tax_Data)
            {
                var details = new TraceFinancialResponseDetailData
                {
                    TaxForm = taxData.Form
                };
                if (short.TryParse(taxData.Year, out short fiscalYear))
                    details.FiscalYear = fiscalYear;

                if (taxData.Field is not null)
                {
                    details.TraceDetailValues = new List<TraceFinancialResponseDetailValueData>();

                    foreach (var valueData in taxData.Field)
                    {
                        var detailValue = new TraceFinancialResponseDetailValueData
                        {
                            FieldName = valueData.Name,
                            FieldValue = valueData.Value
                        };
                        details.TraceDetailValues.Add(detailValue);
                    }
                }

                result.TraceFinancialDetails.Add(details);
            }
        }

        return result;
    }

    private static void ValidateXmlHeader(FedTracingFinancial_Header header, string flatFileName, ref MessageDataList result, ref bool isValid)
    {
        int cycle = FileHelper.GetCycleFromFilename(flatFileName);
        if (header.Cycle != cycle)
        {
            result.AddError($"Cycle in file [{header.Cycle}] does not match cycle of file [{cycle}]");
            isValid = false;
        }
    }

    private static void ValidateXmlFooter(FedTracingFinancial_CRATraceIn tracingFile, ref MessageDataList result, ref bool isValid)
    {
        if (tracingFile.TraceResponse.Count != tracingFile.Footer.ResponseCount)
        {
            result.AddError("Invalid ResponseCount in footer section");
            isValid = false;
        }
    }

    private static FedTracingFinancialFileBase ExtractTracingFinancialDataFromJson(string sourceTracingData, out string error)
    {
        error = string.Empty;

        FedTracingFinancialFileBase result;
        try
        {
            result = JsonConvert.DeserializeObject<FedTracingFinancialFileBase>(sourceTracingData);
        }
        catch (Exception e)
        {
            error = e.Message;
            result = new FedTracingFinancialFileBase();
        }

        return result;
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
                        await SendTracingResponsesToFOAEAAsync(tracingFileData.TRCIN02, tracingResponses, fileTableData.PrcId,
                                                               enfSrvCd, fedSource, fileCycle,
                                                               flatFileName, errors);
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

    private async Task<FileTableData> GetFileTableDataAsync(string flatFileName)
    {
        string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

        return await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
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

    private async Task CloseNETPTraceEventsAsync()
    {
        await APIs.TracingEvents.CloseNETPTraceEventsAsync();
    }

    private async Task<List<TraceResponseData>> ExtractTracingResponsesFromFileDataAsync(FedTracingFileBase tracingFileData, string enfSrvCd, string fileCycle,
                                                                        List<string> errors)
    {
        var cycles = await APIs.TracingApplications.GetTraceCycleQuantityDataAsync(enfSrvCd, fileCycle);

        return IncomingFederalTracingResponse.GenerateFromFileData(tracingFileData, enfSrvCd, cycles, ref errors);
    }

    public async Task SendTracingResponsesToFOAEAAsync(List<FedTracing_RecType02> fileTracingSummary, List<TraceResponseData> tracingResponses,
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

    private static void ValidateHeader(FedTracing_RecType01 dataFromFile, string flatFileName, ref List<string> errors)
    {
        int cycle = FileHelper.GetCycleFromFilename(flatFileName);
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

    private async Task MarkTraceEventsAsProcessedAsync(string applEnfSrvCd, string applCtrlCd, string flatFileName, short newState,
                                                  List<ApplicationEventData> activeTraceEvents,
                                                  List<ApplicationEventDetailData> activeTraceEventDetails)
    {
        var activeTraceEvent = activeTraceEvents
                                   .Where(m => m.Appl_EnfSrv_Cd == applEnfSrvCd && m.Appl_CtrlCd == applCtrlCd)
                                   .FirstOrDefault();

        if (activeTraceEvent != null)
        {
            activeTraceEvent.ActvSt_Cd = "P";
            activeTraceEvent.Event_Compl_Dte = DateTime.Now;

            await APIs.ApplicationEvents.SaveEventAsync(activeTraceEvent);

            int eventId = activeTraceEvent.Event_Id;
            string eventReason = $"[FileNm:{flatFileName}[ErrDes:000000MSGBRO]" +
                                 $"[(EnfSrv:{applEnfSrvCd.Trim()})(CtrlCd:{applCtrlCd.Trim()})]";

            var activeTraceEventDetail = activeTraceEventDetails
                                            .Where(m => m.Event_Id.HasValue && m.Event_Id.Value == eventId)
                                            .FirstOrDefault();

            if (activeTraceEventDetail != null)
            {
                activeTraceEventDetail.Event_Reas_Text = eventReason;
                activeTraceEventDetail.Event_Effctv_Dte = DateTime.Now;
                activeTraceEventDetail.AppLiSt_Cd = newState;
                activeTraceEventDetail.ActvSt_Cd = "C";
                activeTraceEventDetail.Event_Compl_Dte = DateTime.Now;

                await APIs.ApplicationEvents.SaveEventDetailAsync(activeTraceEventDetail);
            }
        }

    }

    private async Task CloseOrInactivateTraceEventDetailsAsync(int cutOffDate, List<ApplicationEventDetailData> activeTraceEventDetails)
    {

        foreach (var row in activeTraceEventDetails)
        {
            if (row.Event_TimeStamp.AddDays(cutOffDate) < DateTime.Now)
            {
                row.Event_Reas_Cd = EventCode.C50054_DETAIL_EVENT_HAS_EXCEEDED_TO_ALLOWABLE_TIME_TO_REMAIN_ACTIVE;
                row.ActvSt_Cd = "I";
                row.AppLiSt_Cd = 1;
            }
            else
            {
                row.ActvSt_Cd = "C";
                row.Event_Compl_Dte = DateTime.Now;
            }

            await APIs.ApplicationEvents.SaveEventDetailAsync(row);

        }

    }

    private async Task SendTRACEDataToTrcRspAsync(List<TraceResponseData> responseData)
    {
        await APIs.TracingResponses.InsertBulkDataAsync(responseData);
    }

    private async Task UpdateTracingApplicationsAsync(string enfSrvCd, string fileCycle)
    {
        var traceToApplData = await APIs.TracingApplications.GetTraceToApplDataAsync();

        foreach (var row in traceToApplData)
        {
            await ProcessTraceToApplDataAsync(row, enfSrvCd, fileCycle);
        }

    }

    private async Task ProcessTraceToApplDataAsync(TraceToApplData row, string enfSrvCd, string fileCycle)
    {
        var activeTracingEvents = await APIs.TracingEvents.GetRequestedTRCINEventsAsync(enfSrvCd, fileCycle);
        var activeTracingEventDetails = await APIs.TracingEvents.GetActiveTracingEventDetailsAsync(enfSrvCd, fileCycle);

        string newEventState;

        if (row.Tot_Invalid > 0)
        {
            newEventState = "I";
        }
        else
        {
            var tracingApplication = await APIs.TracingApplications.GetApplicationAsync(row.Appl_EnfSrv_Cd, row.Appl_CtrlCd);

            if (row.Tot_Childs == row.Tot_Closed)
            {
                await APIs.TracingApplications.FullyServiceApplicationAsync(tracingApplication, enfSrvCd);
                newEventState = "C";
            }
            else
            {
                await APIs.TracingApplications.PartiallyServiceApplicationAsync(tracingApplication, enfSrvCd);
                newEventState = "A";
            }
        }

        int eventId = row.Event_Id;
        var eventData = activeTracingEvents.Where(m => m.Event_Id == eventId).FirstOrDefault();
        if (eventData != null)
        {
            eventData.ActvSt_Cd = newEventState;
            await APIs.ApplicationEvents.SaveEventAsync(eventData);
        }

        if (newEventState.In("A", "C"))
        {
            var eventDetailData = activeTracingEventDetails.Where(m => m.Event_Id == eventId).FirstOrDefault();
            if (eventDetailData != null)
            {
                eventDetailData.Event_Compl_Dte = DateTime.Now;
                await APIs.ApplicationEvents.SaveEventDetailAsync(eventDetailData);
            }
        }

    }

}
