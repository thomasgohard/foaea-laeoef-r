using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Financials
{
    public class FinancialEventAPIBroker : IFinancialEventAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public FinancialEventAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv)
        {
            string apiCall = $"api/v1/FinancialEvents/PADRevents/active?enfSrv={enfSrv}";
            return await ApiHelper.GetDataAsync<List<CR_PADReventData>>(apiCall, token: Token);
        }

        public async Task<List<IFMSdata>> GetIFMSasync(string batchId)
        {
            string apiCall = $"api/v1/FinancialEvents/IFMS?enfSrv={batchId}";
            return await ApiHelper.GetDataAsync<List<IFMSdata>>(apiCall, token: Token);
        }
    }
}
