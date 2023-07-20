using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationEvent : IApplicationEventRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task CloseNETPTraceEvents()
        {
            throw new NotImplementedException();
        }

        public Task DeleteBFEvent(string subm_SubmCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventData>> GetActiveEventBFs()
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventData>> GetApplicationEvents(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventData>> GetEventBF(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState)
        {
            throw new NotImplementedException();
        }

        public string GetLastError()
        {
            throw new NotImplementedException();
        }

        public Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary()
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventData>> GetRequestedLICINLicenceDenialEvents(string enfSrv_Cd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<ApplicationEventData>> GetRequestedSINEventDataForFile(string enfSrv_Cd, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventData>> GetRequestedTRCINTracingEvents(string enfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTraceEventCount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate, EventCode eventReasonCode, int eventId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED, string activeState = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEvents(List<ApplicationEventData> events, ApplicationState applicationState = ApplicationState.UNDEFINED, string activeState = "")
        {
            throw new NotImplementedException();
        }
    }
}
