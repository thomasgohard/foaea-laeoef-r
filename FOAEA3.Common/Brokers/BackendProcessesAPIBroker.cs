using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class BackendProcessesAPIBroker : IBackendProcessesAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public BackendProcessesAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }
    }
}
