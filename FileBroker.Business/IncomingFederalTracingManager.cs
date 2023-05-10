using DBHelper;
using FileBroker.Common.Helpers;

namespace FileBroker.Business;

public partial class IncomingFederalTracingManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }

    private FoaeaSystemAccess FoaeaAccess { get; }

    public IncomingFederalTracingManager(APIBrokerList apis, RepositoryList repositories,
                                         IFileBrokerConfigurationHelper config)
    {
        APIs = apis;
        DB = repositories;

        FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
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
