using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialTerminationApplicationAPIBroker : ILicenceDenialTerminationApplicationAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public LicenceDenialTerminationApplicationAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<LicenceDenialApplicationData> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var appData = new LicenceDenialApplicationData
            {
                Appl_EnfSrv_Cd = appl_EnfSrv_Cd,
                Appl_CtrlCd = appl_CtrlCd,
                Subm_SubmCd = "MSGBRO"
            };
            string key = ApplKey.MakeKey(appl_EnfSrv_Cd, appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenials/{key}/ProcessLicenceDenialTerminationResponse";
            return await ApiHelper.PutDataAsync<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData, token: Token);
        }
    }
}
