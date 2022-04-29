using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ITracingRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }
        TracingApplicationData GetTracingData(string appl_EnfSrv_Cd, string appl_CtrlCd);
        void CreateTracingData(TracingApplicationData data);
        void UpdateTracingData(TracingApplicationData data);
        List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle);
        bool TracingDataExists(string appl_EnfSrv_Cd, string appl_CtrlCd);
        DataList<TracingApplicationData> GetApplicationsWaitingForAffidavit();
        List<TraceToApplData> GetTraceToApplData();
        List<TracingOutgoingFederalData> GetFederalOutgoingData(int maxRecords,
                                                                string activeState,
                                                                ApplicationState lifeState,
                                                                string enfServiceCode);
        List<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords,
                                                                      string activeState,
                                                                      string recipientCode,
                                                                      bool isXML = true);
    }

}
