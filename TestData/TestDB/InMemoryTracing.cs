using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace TestData.TestDB
{
    public class InMemoryTracing : ITracingRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void CreateTracingData(TracingApplicationData data)
        {
            throw new NotImplementedException();
        }

        public DataList<TracingApplicationData> GetApplicationsWaitingForAffidavit()
        {
            throw new NotImplementedException();
        }

        public List<TracingOutgoingFederalData> GetFederalOutgoingData(int maxRecords, string activeState, ApplicationState lifeState, string enfServiceCode)
        {
            throw new NotImplementedException();
        }

        public List<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            throw new NotImplementedException();
        }

        public List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public List<TraceToApplData> GetTraceToApplData()
        {
            throw new NotImplementedException();
        }

        public TracingApplicationData GetTracingData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public bool TracingDataExists(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public void UpdateTracingData(TracingApplicationData data)
        {
            throw new NotImplementedException();
        }
    }
}
