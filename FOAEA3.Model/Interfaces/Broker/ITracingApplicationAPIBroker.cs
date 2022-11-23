using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ITracingApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<TracingApplicationData> CloseTracingApplicationAsync(TracingApplicationData tracingApplication);
        Task<TracingApplicationData> CreateTracingApplicationAsync(TracingApplicationData tracingData);
        Task<TracingApplicationData> FullyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd);
        Task<TracingApplicationData> PartiallyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd);
        Task<TracingApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrvCd, string fileCycle);
        Task<List<TraceToApplData>> GetTraceToApplDataAsync();
        Task<TracingApplicationData> UpdateTracingApplicationAsync(TracingApplicationData tracingApplication);
        Task<TracingApplicationData> TransferTracingApplicationAsync(TracingApplicationData tracingApplication,
                                                                 string newRecipientSubmitter,
                                                                 string newIssuingSubmitter);
        Task<List<TracingOutgoingFederalData>> GetOutgoingFederalTracingRequestsAsync(int maxRecords, string activeState,
                                                                           int lifeState, string enfServiceCode);
        Task<List<TracingOutgoingProvincialData>> GetOutgoingProvincialTracingDataAsync(int maxRecords, string activeState,
                                                                             string recipientCode);
    }
}
