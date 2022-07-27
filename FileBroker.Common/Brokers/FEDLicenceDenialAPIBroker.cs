using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FileBroker.Common.Brokers
{
    public class FEDLicenceDenialAPIBroker : IFEDLicenceDenialAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public FEDLicenceDenialAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/FederalLicenceDenialFiles/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/FederalLicenceDenialFiles/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }
    }
}