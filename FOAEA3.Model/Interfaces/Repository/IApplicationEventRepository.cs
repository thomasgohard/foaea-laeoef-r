using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationEventRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<ApplicationEventData> GetApplicationEvents(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null);
        List<ApplicationEventData> GetEventBF(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState);
        List<ApplicationEventData> GetActiveEventBFs();        
        bool SaveEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED,
                       string activeState = "");
        bool SaveEvents(List<ApplicationEventData> events, ApplicationState applicationState = ApplicationState.UNDEFINED,
                        string activeState = "");
        string GetLastError();
        
        // tracing specific event management

        void CloseNETPTraceEvents();
        int GetTraceEventCount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate,
                               EventCode eventReasonCode, int eventId);
        List<ApplicationEventData> GetRequestedTRCINTracingEvents(string enfSrv_Cd, string cycle);
        List<ApplicationEventData> GetRequestedLICINLicenceDenialEvents(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                        string appl_CtrlCd);
        void DeleteBFEvent(string subm_SubmCd, string appl_CtrlCd);
        DataList<ApplicationEventData> GetRequestedSINEventDataForFile(string enfSrv_Cd, string fileName);
        List<SinInboundToApplData> GetLatestSinEventDataSummary();
    }
}
