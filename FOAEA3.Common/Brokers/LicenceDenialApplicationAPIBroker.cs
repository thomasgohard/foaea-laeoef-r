using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using FOAEA3.Resources.Helpers;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialApplicationAPIBroker : ILicenceDenialApplicationAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public LicenceDenialApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/licencedenials/{key}";
            return ApiHelper.GetDataAsync<LicenceDenialApplicationData>(apiCall).Result;
        }
    }
}
