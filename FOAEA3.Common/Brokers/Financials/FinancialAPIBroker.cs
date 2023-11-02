using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Financials
{
    public class FinancialAPIBroker : IFinancialAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public FinancialAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<CR_PADReventData>> GetActiveCR_PADRevents(string enfSrv)
        {
            string apiCall = $"api/v1/PADRevents/active?enfSrv={enfSrv}";
            return await ApiHelper.GetData<List<CR_PADReventData>>(apiCall, token: Token);
        }

        public async Task CloseCR_PADRevents(string batchId, string enfSrv)
        {
            string apiCall = $"api/v1/PADRevents/close?batchId={batchId}&enfSrv={enfSrv}";
            await ApiHelper.SendCommand(apiCall, token: Token);
        }

        public async Task<List<BlockFundData>> GetBlockFunds(string enfSrv)
        {
            string apiCall = $"api/v1/BlockFunds?enfSrv={enfSrv}";
            return await ApiHelper.GetData<List<BlockFundData>>(apiCall, token: Token);
        }

        public async Task<List<DivertFundData>> GetDivertFunds(string enfSrv, string batchId)
        {
            string apiCall = $"api/v1/DivertFunds?enfSrv={enfSrv}&batchId={batchId}";
            return await ApiHelper.GetData<List<DivertFundData>>(apiCall, token: Token);
        }

        public async Task<List<IFMSdata>> GetIFMSDataFromFoaea(string batchId)
        {
            string apiCall = $"api/v1/IFMS?batchId={batchId}";
            return await ApiHelper.GetData<List<IFMSdata>>(apiCall, token: Token);
        }       
    }
}
