using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialEventAPIBroker : ILicenceDenialEventAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialEventAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<List<ApplicationEventData>> GetRequestedLICINEventsAsync(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            string apiCall = $"api/v1/licenceDenialEvents/RequestedLICIN?enforcementServiceCode={enfSrvCd}&" +
                             $"appl_EnfSrv_Cd={appl_EnfSrv_Cd}&appl_CtrlCd={appl_CtrlCd}";
            return await ApiHelper.GetDataAsync<List<ApplicationEventData>>(apiCall);
        }
        
        public async Task<List<ApplicationEventDetailData>> GetRequestedLICINEventDetailsAsync(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            string apiCall = $"api/v1/licenceDenialEventDetails/RequestedLICIN?enforcementServiceCode={enfSrvCd}&" +
                             $"appl_EnfSrv_Cd={appl_EnfSrv_Cd}&appl_CtrlCd={appl_CtrlCd}";
            return await ApiHelper.GetDataAsync<List<ApplicationEventDetailData>>(apiCall);
        }

    }
}
