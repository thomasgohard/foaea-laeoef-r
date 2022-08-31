using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryTracingApplicationAPIBroker : ITracingApplicationAPIBroker
    {
        public int Count { get; set; }
        public MessageDataList LastMessages { get; set; }

        public async Task<TracingApplicationData> CloseTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<TracingApplicationData> CreateTracingApplicationAsync(TracingApplicationData tracingData)
        {
            return await Task.Run(() => { return tracingData; });
        }

        public async Task<TracingApplicationData> FullyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<TracingApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<List<TracingOutgoingFederalData>> GetOutgoingFederalTracingRequestsAsync(int maxRecords, string activeState, int lifeState, string enfServiceCode)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<List<TracingOutgoingProvincialData>> GetOutgoingProvincialTracingDataAsync(int maxRecords, string activeState, string recipientCode)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrvCd, string fileCycle)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<List<TraceToApplData>> GetTraceToApplDataAsync()
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<TracingApplicationData> PartiallyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<TracingApplicationData> TransferTracingApplicationAsync(TracingApplicationData tracingApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task<TracingApplicationData> UpdateTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }
    }
}
