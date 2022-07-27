using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FileBroker.Common.Brokers
{
    public class MEPTracingAPIBroker : IMEPTracingAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public MEPTracingAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/TracingFiles/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/TracingFiles/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }
    }
}
