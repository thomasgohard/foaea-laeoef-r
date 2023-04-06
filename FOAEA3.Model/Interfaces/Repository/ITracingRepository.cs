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

        Task<TracingApplicationData> GetTracingDataAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateTracingDataAsync(TracingApplicationData data);
        Task UpdateTracingDataAsync(TracingApplicationData data);
        Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrv_Cd, string cycle);
        Task<bool> TracingDataExistsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavitAsync();
        Task<List<TraceToApplData>> GetTraceToApplDataAsync();
        Task<List<TracingOutgoingFederalData>> GetFederalOutgoingDataAsync(int maxRecords,
                                                                string activeState,
                                                                ApplicationState lifeState,
                                                                string enfServiceCode);
        Task<TracingOutgoingProvincialData> GetProvincialOutgoingDataAsync(int maxRecords,
                                                                      string activeState,
                                                                      string recipientCode,
                                                                      bool isXML = true);
        Task CreateESDCEventTraceDataAsync();
    }

}
