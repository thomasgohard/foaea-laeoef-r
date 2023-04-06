using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryTracing : ITracingRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task CreateESDCEventTraceDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateTracingDataAsync(TracingApplicationData data)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavitAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TracingOutgoingFederalData>> GetFederalOutgoingDataAsync(int maxRecords, string activeState, ApplicationState lifeState, string enfServiceCode)
        {
            throw new NotImplementedException();
        }

        public Task<TracingOutgoingProvincialData> GetProvincialOutgoingDataAsync(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public Task<List<TraceToApplData>> GetTraceToApplDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TracingApplicationData> GetTracingDataAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TracingDataExistsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTracingDataAsync(TracingApplicationData data)
        {
            throw new NotImplementedException();
        }
    }
}
