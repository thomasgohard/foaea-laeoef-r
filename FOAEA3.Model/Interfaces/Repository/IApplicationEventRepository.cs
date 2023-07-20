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

        Task<List<ApplicationEventData>> GetApplicationEvents(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null);
        Task<List<ApplicationEventData>> GetEventBF(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState);
        Task<List<ApplicationEventData>> GetActiveEventBFs();
        Task<bool> SaveEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED,
                       string activeState = "");
        Task<bool> SaveEvents(List<ApplicationEventData> events, ApplicationState applicationState = ApplicationState.UNDEFINED,
                        string activeState = "");
        string GetLastError();
        
        // tracing specific event management

        Task CloseNETPTraceEvents();
        Task<int> GetTraceEventCount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate,
                               EventCode eventReasonCode, int eventId);
        Task<List<ApplicationEventData>> GetRequestedTRCINTracingEvents(string enfSrv_Cd, string cycle);
        Task<List<ApplicationEventData>> GetRequestedLICINLicenceDenialEvents(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                        string appl_CtrlCd);

        Task DeleteBFEvent(string subm_SubmCd, string appl_CtrlCd);
        Task<DataList<ApplicationEventData>> GetRequestedSINEventDataForFile(string enfSrv_Cd, string fileName);
        Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary();
    }
}
