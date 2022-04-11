using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;

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

        public List<LicenceDenialOutgoingFederalData> GetOutgoingFederalLicenceDenialRequests(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalLicenceDenialRequests";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return ApiHelper.GetDataAsync<List<LicenceDenialOutgoingFederalData>>(apiCall).Result;
        }

    }
}
