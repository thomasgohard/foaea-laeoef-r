using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class BackendProcessesAPIBroker : IBackendProcessesAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public BackendProcessesAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }
    }
}
