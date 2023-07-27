using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FileBroker.Common.Brokers
{
    public class MEPTracingAPIBroker : IMEPTracingAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public MEPTracingAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/TracingFiles/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/TracingFiles/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }
    }
}
