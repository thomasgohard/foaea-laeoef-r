using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Model.Base;

namespace TestData.TestDB
{
    public class InMemoryApplicationEvent : IApplicationEventRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void CloseNETPTraceEvents()
        {
            throw new NotImplementedException();
        }

        public void DeleteBFEvent(string subm_SubmCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationEventData> GetActiveEventBFs()
        {
            throw new NotImplementedException();
        }

        public List<ApplicationEventData> GetRequestedTRCINTracingEvents(string appl_EnfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationEventData> GetEventBF(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState)
        {
            throw new NotImplementedException();
        }

        public string GetLastError()
        {
            throw new NotImplementedException();
        }

        public DataList<ApplicationEventData> GetRequestedSINEventDataForFile(string enfSrv_Cd, string fileName)
        {
            throw new NotImplementedException();
        }

        public int GetTraceEventCount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate, EventCode eventReasonCode, int eventId)
        {
            throw new NotImplementedException();
        }

        public bool SaveEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED, string activeState = "")
        {
            throw new NotImplementedException();
        }

        public bool SaveEventDetail(ApplicationEventDetailData eventDetailData)
        {
            throw new NotImplementedException();
        }

        public bool SaveEvents(List<ApplicationEventData> events, ApplicationState applicationState = ApplicationState.UNDEFINED, string activeState = "")
        {
            throw new NotImplementedException();
        }

        public List<ApplicationEventData> GetApplicationEvents(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            throw new NotImplementedException();
        }

        public List<SinInboundToApplData> GetLatestSinEventDataSummary()
        {
            throw new NotImplementedException();
        }
    }
}
