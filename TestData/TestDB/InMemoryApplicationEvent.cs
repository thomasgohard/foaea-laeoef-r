using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationEvent : IApplicationEventRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public async Task CloseNETPTraceEventsAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task DeleteBFEventAsync(string subm_SubmCd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventData>> GetActiveEventBFsAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventData>> GetApplicationEventsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventData>> GetEventBFAsync(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public string GetLastError()
        {
            throw new NotImplementedException();
        }

        public async Task<List<SinInboundToApplData>> GetLatestSinEventDataSummaryAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventData>> GetRequestedLICINLicenceDenialEventsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<DataList<ApplicationEventData>> GetRequestedSINEventDataForFileAsync(string enfSrv_Cd, string fileName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventData>> GetRequestedTRCINTracingEventsAsync(string enfSrv_Cd, string cycle)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<int> GetTraceEventCountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate, EventCode eventReasonCode, int eventId)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> SaveEventAsync(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED, string activeState = "")
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> SaveEventsAsync(List<ApplicationEventData> events, ApplicationState applicationState = ApplicationState.UNDEFINED, string activeState = "")
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
