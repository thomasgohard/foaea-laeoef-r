using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryTracing : ITracingRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public async Task CreateTracingDataAsync(TracingApplicationData data)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavitAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<TracingOutgoingFederalData>> GetFederalOutgoingDataAsync(int maxRecords, string activeState, ApplicationState lifeState, string enfServiceCode)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<TracingOutgoingProvincialData>> GetProvincialOutgoingDataAsync(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrv_Cd, string cycle)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<TraceToApplData>> GetTraceToApplDataAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<TracingApplicationData> GetTracingDataAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> TracingDataExistsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateTracingDataAsync(TracingApplicationData data)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
