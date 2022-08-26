using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Threading.Tasks;

namespace FileBroker.Common.Brokers
{
    public class MEPInterceptionAPIBroker : IMEPInterceptionAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public MEPInterceptionAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/InterceptionFiles/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/InterceptionFiles/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<string> GetLatestProvincialFileAsync(string partnerId)
        {
            string apiCall = $"api/v1/InterceptionFiles?partnerId={partnerId}";
            return await ApiHelper.GetStringAsync(apiCall);
        }

    }
}
