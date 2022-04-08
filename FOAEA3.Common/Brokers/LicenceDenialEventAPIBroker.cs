using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialEventAPIBroker : ILicenceDenialEventAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialEventAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public List<ApplicationEventData> GetRequestedLICINEvents(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            string apiCall = $"api/v1/licenceDenialEvents/RequestedLICIN?enforcementServiceCode={enfSrvCd}&" +
                             $"appl_EnfSrv_Cd={appl_EnfSrv_Cd}&appl_CtrlCd={appl_CtrlCd}";
            return ApiHelper.GetDataAsync<List<ApplicationEventData>>(apiCall).Result;
        }
    }
}
