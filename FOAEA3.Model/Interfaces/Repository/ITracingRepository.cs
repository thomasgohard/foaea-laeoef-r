using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ITracingRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<TracingApplicationData> GetTracingData(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateTracingData(TracingApplicationData data);
        Task UpdateTracingData(TracingApplicationData data);
        Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle);
        Task<bool> TracingDataExists(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavit();
        Task<List<TraceToApplData>> GetTraceToApplData();
        Task<List<TracingOutgoingFederalData>> GetFederalOutgoingData(int maxRecords, string activeState,
                                                                      ApplicationState lifeState, string enfServiceCode);
        Task<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords, string activeState,
                                                                      string recipientCode, bool isXML = true);
        Task CreateESDCEventTraceData();
    }

}
