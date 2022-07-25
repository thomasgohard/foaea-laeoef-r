using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FileBroker.Common.Brokers
{
    public class MEPInterceptionAPIBroker : IMEPInterceptionAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public MEPInterceptionAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/InterceptionFiles/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/InterceptionFiles/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetLatestProvincialFile(string partnerId)
        {
            string apiCall = $"api/v1/InterceptionFiles?partnerId={partnerId}";
            return ApiHelper.GetStringAsync(apiCall).Result;
        }

    }
}
