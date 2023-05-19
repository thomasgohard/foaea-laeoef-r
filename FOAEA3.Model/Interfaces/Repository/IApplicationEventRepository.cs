using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IApplicationEventRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<ApplicationEventData>> GetApplicationEventsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null);
        Task<List<ApplicationEventData>> GetEventBFAsync(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState);
        Task<List<ApplicationEventData>> GetActiveEventBFsAsync();
        Task<bool> SaveEventAsync(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED,
                       string activeState = "");
        Task<bool> SaveEventsAsync(List<ApplicationEventData> events, ApplicationState applicationState = ApplicationState.UNDEFINED,
                        string activeState = "");
        string GetLastError();
        
        // tracing specific event management

        Task CloseNETPTraceEventsAsync();
        Task<int> GetTraceEventCountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate,
                               EventCode eventReasonCode, int eventId);
        Task<List<ApplicationEventData>> GetRequestedTRCINTracingEventsAsync(string enfSrv_Cd, string cycle);
        Task<List<ApplicationEventData>> GetRequestedLICINLicenceDenialEventsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                        string appl_CtrlCd);

        Task DeleteBFEventAsync(string subm_SubmCd, string appl_CtrlCd);
        Task<DataList<ApplicationEventData>> GetRequestedSINEventDataForFileAsync(string enfSrv_Cd, string fileName);
        Task<List<SinInboundToApplData>> GetLatestSinEventDataSummaryAsync();
    }
}
