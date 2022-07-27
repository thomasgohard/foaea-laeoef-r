using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class BackendProcessesAPIBroker : IBackendProcessesAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public BackendProcessesAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/applicationsAmountOwed/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }
    }
}
