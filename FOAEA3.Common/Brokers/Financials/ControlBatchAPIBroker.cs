using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Financials
{
    public class ControlBatchAPIBroker : IControlBatchAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ControlBatchAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task CloseControlBatch(string batchId)
        {
            string apiCall = $"api/v1/ControlBatches/close?batchId={batchId}";
            await ApiHelper.SendCommand(apiCall, token: Token);
        }

        public async Task<DateTime> GetLastUiBatchLoaded()
        {
            string apiCall = $"api/v1/ControlBatches/LastUiBatchLoaded";
            return await ApiHelper.GetData<DateTime>(apiCall, token: Token);
        }

        public async Task<List<BatchSimpleData>> GetReadyDivertFundsBatches(string enfSrv, string enfSrvLoc)
        {
            string apiCall = $"api/v1/ControlBatches/readyDivertFunds?enfSrv={enfSrv}&enfSrvLoc={enfSrvLoc}";
            return await ApiHelper.GetData<List<BatchSimpleData>>(apiCall, token: Token);
        }

        public async Task<ControlBatchData> CreateControlBatch(ControlBatchData controlBatchData)
        {
            string apiCall = $"api/v1/ControlBatches";
            return await ApiHelper.PostData<ControlBatchData, ControlBatchData>(apiCall, controlBatchData, token: Token);
        }

        public async Task<ControlBatchData> GetControlBatch(string batchId)
        {
            string apiCall = $"api/v1/ControlBatches/{batchId}";
            return await ApiHelper.GetData<ControlBatchData>(apiCall, token: Token);
        }

        public async Task MarkBatchAsLoaded(ControlBatchData controlBatchData)
        {
            string batchId = controlBatchData.Batch_Id;
            string apiCall = $"api/v1/ControlBatches/{batchId}/markAsLoaded";
            await ApiHelper.PutData<ControlBatchData, ControlBatchData>(apiCall, controlBatchData, token: Token);
        }
    }
}
