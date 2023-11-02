using FOAEA3.Model;
using FOAEA3.Model.Enums;
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

        public Task<TracingApplicationData> CancelTracingApplication(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> CertifyTracingApplication(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> CreateTracingApplication(TracingApplicationData tracingData)
        {
            return Task.FromResult(tracingData);
        }

        public Task<TracingApplicationData> FullyServiceApplication(TracingApplicationData tracingApplication, FederalSource fedSource)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TracingOutgoingFederalData>> GetOutgoingFederalTracingRequests(int maxRecords, string activeState, int lifeState, string enfServiceCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingOutgoingProvincialData> GetOutgoingProvincialTracingData(int maxRecords, string activeState, string recipientCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityData(string enfSrvCd, string fileCycle)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TraceToApplData>> GetTraceToApplData()
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> InsertAffidavit(AffidavitData data)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> PartiallyServiceApplication(TracingApplicationData tracingApplication, FederalSource fedSource)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> TransferTracingApplication(TracingApplicationData tracingApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            throw new System.NotImplementedException();
        }

        public Task<TracingApplicationData> UpdateTracingApplication(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }
    }
}
