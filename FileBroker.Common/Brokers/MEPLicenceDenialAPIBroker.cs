using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FileBroker.Common.Brokers
{
    public class MEPLicenceDenialAPIBroker : IMEPLicenceDenialAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public MEPLicenceDenialAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/LicenceDenialFiles/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/LicenceDenialFiles/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }
    }
}
