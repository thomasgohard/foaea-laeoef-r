using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryTracingApplicationAPIBroker : ITracingApplicationAPIBroker
    {
        public int Count { get; set; }
        public MessageDataList LastMessages { get; set; }

        public IAPIBrokerHelper ApiHelper => throw new System.NotImplementedException();

        public string Token { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Task<TracingApplicationData> CancelTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> CreateTracingApplicationAsync(TracingApplicationData tracingData)
        {
            return Task.FromResult(tracingData);
        }

        public Task<TracingApplicationData> FullyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TracingOutgoingFederalData>> GetOutgoingFederalTracingRequestsAsync(int maxRecords, string activeState, int lifeState, string enfServiceCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingOutgoingProvincialData> GetOutgoingProvincialTracingDataAsync(int maxRecords, string activeState, string recipientCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrvCd, string fileCycle)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TraceToApplData>> GetTraceToApplDataAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> PartiallyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> TransferTracingApplicationAsync(TracingApplicationData tracingApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> UpdateTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }
    }
}
