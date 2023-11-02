using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialTerminationApplicationAPIBroker : ILicenceDenialTerminationApplicationAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public LicenceDenialTerminationApplicationAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<LicenceDenialApplicationData> ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var appData = new LicenceDenialApplicationData
            {
                Appl_EnfSrv_Cd = appl_EnfSrv_Cd,
                Appl_CtrlCd = appl_CtrlCd,
                Subm_SubmCd = LoginsAPIBroker.SYSTEM_SUBMITTER
            };
            string key = ApplKey.MakeKey(appl_EnfSrv_Cd, appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenials/{key}/ProcessLicenceDenialTerminationResponse";
            return await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData, token: Token);
        }

        public async Task<LicenceDenialApplicationData> CreateLicenceDenialTerminationApplication(LicenceDenialApplicationData appData,
                                                                                            string controlCodeForL01)
        {
            string apiCall = $"api/v1/licenceDenialTerminations?controlCodeForL01={controlCodeForL01}";
            return await ApiHelper.PostData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData, token: Token);
        }

        public async Task<LicenceDenialApplicationData> CancelLicenceDenialTerminationApplication(LicenceDenialApplicationData appData)
        {
            string key = ApplKey.MakeKey(appData.Appl_EnfSrv_Cd, appData.Appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenialTerminations/{key}/cancel";
            var data = await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall,
                                                                                               appData, token: Token);
            return data;
        }

        public async Task<LicenceDenialApplicationData> UpdateLicenceDenialTerminationApplication(LicenceDenialApplicationData appData)
        {
            string key = ApplKey.MakeKey(appData.Appl_EnfSrv_Cd, appData.Appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenialTerminations/{key}";
            var data = await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall,
                                                                                               appData, token: Token);
            return data;

        }

        public async Task<LicenceDenialApplicationData> TransferLicenceDenialTerminationApplication(LicenceDenialApplicationData appData,
                                                                string newRecipientSubmitter, string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(appData.Appl_EnfSrv_Cd, appData.Appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenialTerminations/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                           $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall,
                                                                                              appData, token: Token);
            return data;
        }
    }
}
