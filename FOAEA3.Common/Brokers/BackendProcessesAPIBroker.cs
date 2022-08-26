using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class BackendProcessesAPIBroker : IBackendProcessesAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public BackendProcessesAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }
    }
}
