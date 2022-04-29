namespace FileBroker.Business;

public class IncomingFederalTracingManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList Repositories { get; }

    public enum EFederalSource
    {
        Unknown,
        CRA,
        EI,
        NETP
    }

    public IncomingFederalTracingManager(APIBrokerList apiBrokers, RepositoryList repositories)
    {
        APIs = apiBrokers;
        Repositories = repositories;
    }

    public List<string> ProcessFlatFile(string flatFileContent, string flatFileName)
    {
        var fileTableData = GetFileTableData(flatFileName);
        var tracingFileData = new FedTracingFileBase();
        (string enfSrvCd, EFederalSource fedSource) = ConfigureTracingFileData(tracingFileData, fileTableData.Name);

        var errors = new List<string>();

        if (fedSource == EFederalSource.Unknown)
            errors.Add($"No match for [{fileTableData.Name}] or not support federal tracing source.");

        else if (!fileTableData.Active.HasValue || !fileTableData.Active.Value)
            errors.Add($"[{fileTableData.Name}] is not active.");

        if (errors.Any())
            return errors;

        Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

        string fileCycle = Path.GetExtension(flatFileName)[1..];
        try
        {
            var fileLoader = new IncomingFederalTracingFileLoader(Repositories.FlatFileSpecs, fileTableData.PrcId);
            fileLoader.FillTracingFileDataFromFlatFile(tracingFileData, flatFileContent, ref errors);

            if (errors.Any())
                return errors;

            ValidateHeader(tracingFileData.TRCIN01, flatFileName, ref errors);
            ValidateFooter(tracingFileData.TRCIN99, tracingFileData.TRCIN02, ref errors);

            if (errors.Any())
                return errors;

            if ((tracingFileData.TRCIN02.Count == 0) && (fedSource == EFederalSource.NETP))
                CloseNETPTraceEvents();
            else
            {
                var tracingResponses = ExtractTracingResponsesFromFileData(tracingFileData, enfSrvCd, fileCycle, ref errors);

                if (errors.Any())
                    return errors;

                if ((tracingResponses != null) && (tracingFileData.TRCIN02.Count > 0))
                    SendTracingResponsesToFOAEA(tracingFileData.TRCIN02, tracingResponses, fileTableData.PrcId,
                                                enfSrvCd, fedSource, fileCycle,
                                                flatFileName, ref errors);
            }

        }
        catch (Exception e)
        {
            errors.Add("An error occurred: " + e.Message);
        }
        finally
        {
            Repositories.FileTable.SetNextCycleForFileType(fileTableData, fileCycle.Length);
            Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, false);
        }

        return errors;

    }

    private FileTableData GetFileTableData(string flatFileName)
    {
        string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

        return Repositories.FileTable.GetFileTableDataForFileName(fileNameNoCycle);
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

    private void CloseNETPTraceEvents()
    {
        APIs.TracingEvents.CloseNETPTraceEvents();
    }

    private List<TraceResponseData> ExtractTracingResponsesFromFileData(FedTracingFileBase tracingFileData, string enfSrvCd, string fileCycle,
                                                                        ref List<string> errors)
    {
        var cycles = APIs.TracingApplications.GetTraceCycleQuantityData(enfSrvCd, fileCycle);

        return IncomingFederalTracingResponse.GenerateFromFileData(tracingFileData, enfSrvCd, cycles, ref errors);
    }

    public void SendTracingResponsesToFOAEA(List<FedTracing_RecType02> fileTracingSummary, List<TraceResponseData> tracingResponses,
                                            int processId, string enfSrvCd, EFederalSource fedSource, string fileCycle,
                                            string flatFileName, ref List<string> errors)
    {
        try
        {
            string cutOffDaysValue = Repositories.ProcessParameterTable.GetValueForParameter(processId, "evnt_cutoff");
            int cutOffDays = int.Parse(cutOffDaysValue);

            var activeTraceEvents = APIs.TracingEvents.GetRequestedTRCINEvents(enfSrvCd, fileCycle);
            var activeTraceEventDetails = APIs.TracingEvents.GetActiveTracingEventDetails(enfSrvCd, fileCycle);

            if (fedSource == EFederalSource.NETP)
            {
                foreach (var item in fileTracingSummary)
                {
                    MarkTraceEventsAsProcessed(item.dat_Appl_EnfSrvCd, item.dat_Appl_CtrlCd, flatFileName, newState:2, 
                                               ref activeTraceEvents, ref activeTraceEventDetails);
                }
                CloseOrInactivateTraceEventDetails(cutOffDays, ref activeTraceEventDetails);
                SendTRACEDataToTrcRsp(tracingResponses);
                CloseNETPTraceEvents();
            }
            else
            {
                foreach (var item in fileTracingSummary)
                {
                    var appl = APIs.TracingApplications.GetApplication(item.dat_Appl_EnfSrvCd, item.dat_Appl_CtrlCd);

                    MarkTraceEventsAsProcessed(item.dat_Appl_EnfSrvCd, item.dat_Appl_CtrlCd, flatFileName, (short)appl.AppLiSt_Cd,
                                               ref activeTraceEvents, ref activeTraceEventDetails);
                }
                CloseOrInactivateTraceEventDetails(cutOffDays, ref activeTraceEventDetails);
                SendTRACEDataToTrcRsp(tracingResponses);
                UpdateTracingApplications(enfSrvCd, fileCycle);
                CloseOrInactivateTraceEventDetails(cutOffDays, ref activeTraceEventDetails);
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

    private void MarkTraceEventsAsProcessed(string applEnfSrvCd, string applCtrlCd, string flatFileName, short newState,
                                            ref List<ApplicationEventData> activeTraceEvents,
                                            ref List<ApplicationEventDetailData> activeTraceEventDetails)
    {
        var activeTraceEvent = activeTraceEvents
                                   .Where(m => m.Appl_EnfSrv_Cd == applEnfSrvCd && m.Appl_CtrlCd == applCtrlCd)
                                   .FirstOrDefault();

        if (activeTraceEvent != null)
        {
            activeTraceEvent.ActvSt_Cd = "P";
            activeTraceEvent.Event_Compl_Dte = DateTime.Now;

            APIs.ApplicationEvents.SaveEvent(activeTraceEvent);

            int eventId = activeTraceEvent.Event_Id;
            string eventReason = $"[FileNm:{flatFileName}[ErrDes:000000MSGBRO]" +
                                 $"[(EnfSrv:{applEnfSrvCd.Trim()})(CtrlCd:{applCtrlCd.Trim()})]";

            var activeTraceEventDetail = activeTraceEventDetails
                                            .Where(m => m.Event_Id.Value == eventId)
                                            .FirstOrDefault();

            if (activeTraceEventDetail != null)
            {
                activeTraceEventDetail.Event_Reas_Text = eventReason;
                activeTraceEventDetail.Event_Effctv_Dte = DateTime.Now;
                activeTraceEventDetail.AppLiSt_Cd = newState;
                activeTraceEventDetail.ActvSt_Cd = "C";
                activeTraceEventDetail.Event_Compl_Dte = DateTime.Now;

                APIs.ApplicationEvents.SaveEventDetail(activeTraceEventDetail);
            }
        }

    }

    private void CloseOrInactivateTraceEventDetails(int cutOffDate, ref List<ApplicationEventDetailData> activeTraceEventDetails)
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

            APIs.ApplicationEvents.SaveEventDetail(row);

        }

    }

    private void SendTRACEDataToTrcRsp(List<TraceResponseData> responseData)
    {
        APIs.TracingResponses.InsertBulkData(responseData);
    }

    private void UpdateTracingApplications(string enfSrvCd, string fileCycle)
    {
        var traceToApplData = APIs.TracingApplications.GetTraceToApplData();

        foreach (var row in traceToApplData)
        {
            ProcessTraceToApplData(row, enfSrvCd, fileCycle);
        }

    }

    private void ProcessTraceToApplData(TraceToApplData row, string enfSrvCd, string fileCycle)
    {
        var activeTracingEvents = APIs.TracingEvents.GetRequestedTRCINEvents(enfSrvCd, fileCycle);
        var activeTracingEventDetails = APIs.TracingEvents.GetActiveTracingEventDetails(enfSrvCd, fileCycle);

        string newEventState;

        if (row.Tot_Invalid > 0)
        {
            newEventState = "I";
        }
        else
        {
            var tracingApplication = APIs.TracingApplications.GetApplication(row.Appl_EnfSrv_Cd, row.Appl_CtrlCd);

            if (row.Tot_Childs == row.Tot_Closed)
            {
                APIs.TracingApplications.FullyServiceApplication(tracingApplication, enfSrvCd);
                newEventState = "C";
            }
            else
            {
                APIs.TracingApplications.PartiallyServiceApplication(tracingApplication, enfSrvCd);
                newEventState = "A";
            }
        }

        int eventId = row.Event_Id;
        var eventData = activeTracingEvents.Where(m => m.Event_Id == eventId).FirstOrDefault();
        if (eventData != null)
        {
            eventData.ActvSt_Cd = newEventState;
            APIs.ApplicationEvents.SaveEvent(eventData);
        }

        if (newEventState.In("A", "C"))
        {
            var eventDetailData = activeTracingEventDetails.Where(m => m.Event_Id == eventId).FirstOrDefault();
            if (eventDetailData != null)
            {
                eventDetailData.Event_Compl_Dte = DateTime.Now;
                APIs.ApplicationEvents.SaveEventDetail(eventDetailData);
            }
        }

    }

}
