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

        public Task CreateESDCEventTraceData()
        {
            throw new NotImplementedException();
        }

        public Task CreateTracingData(TracingApplicationData data)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavit()
        {
            throw new NotImplementedException();
        }

        public Task<List<TracingOutgoingFederalData>> GetFederalOutgoingData(int maxRecords, string activeState, ApplicationState lifeState, string enfServiceCode)
        {
            throw new NotImplementedException();
        }

        public Task<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public Task<List<TraceToApplData>> GetTraceToApplData()
        {
            throw new NotImplementedException();
        }

        public Task<TracingApplicationData> GetTracingData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TracingDataExists(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTracingData(TracingApplicationData data)
        {
            throw new NotImplementedException();
        }
    }
}
