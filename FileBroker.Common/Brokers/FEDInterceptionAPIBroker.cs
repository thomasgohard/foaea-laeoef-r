using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Threading.Tasks;

namespace FileBroker.Common.Brokers
{
    public class FEDInterceptionAPIBroker : IFEDInterceptionAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public FEDInterceptionAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/FederalInterceptionFiles/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/FederalInterceptionFiles/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }
    }
}