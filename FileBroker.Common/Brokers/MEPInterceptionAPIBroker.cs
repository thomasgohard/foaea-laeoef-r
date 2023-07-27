using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FileBroker.Common.Brokers
{
    public class MEPInterceptionAPIBroker : IMEPInterceptionAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public MEPInterceptionAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/InterceptionFiles/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/InterceptionFiles/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetLatestProvincialFile(string partnerId)
        {
            string apiCall = $"api/v1/InterceptionFiles?partnerId={partnerId}";
            return await ApiHelper.GetString(apiCall, token: Token);
        }

    }
}
