using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialApplicationAPIBroker : ILicenceDenialApplicationAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public LicenceDenialApplicationAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/licencedenials/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/licencedenials/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<LicenceDenialApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/licencedenials/{key}";
            return await ApiHelper.GetData<LicenceDenialApplicationData>(apiCall, token: Token);
        }

        public async Task<List<LicenceDenialOutgoingFederalData>> GetOutgoingFederalLicenceDenialRequests(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalLicenceDenialRequests";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return await ApiHelper.GetData<List<LicenceDenialOutgoingFederalData>>(apiCall, token: Token);
        }

        public async Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplData(string fedSource)
        {
            string apiCall = $"api/v1/licenceDenials/LicenceDenialToApplication?federalSource={fedSource}";
            return await ApiHelper.GetData<List<LicenceDenialToApplData>>(apiCall, token: Token);
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
            string apiCall = $"api/v1/licenceDenials/{key}/ProcessLicenceDenialResponse";
            return await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData, token: Token);
        }

        public async Task<List<LicenceDenialOutgoingProvincialData>> GetOutgoingProvincialLicenceDenialData(int maxRecords, string activeState,
                                                                                                string recipientCode)
        {
            string baseCall = "api/v1/OutgoingProvincialLicenceDenialResults";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}&isXML=true";
            return await ApiHelper.GetData<List<LicenceDenialOutgoingProvincialData>>(apiCall, token: Token);
        }

        public async Task<LicenceDenialApplicationData> CreateLicenceDenialApplication(LicenceDenialApplicationData appData)
        {
            string apiCall = $"api/v1/licenceDenials";
            return await ApiHelper.PostData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData, token: Token);
        }

        public async Task<LicenceDenialApplicationData> UpdateLicenceDenialApplication(LicenceDenialApplicationData appData)
        {
            string key = ApplKey.MakeKey(appData.Appl_EnfSrv_Cd, appData.Appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenials/{key}";
            var data = await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall,
                                                                                               appData, token: Token);
            return data;
        }

        public async Task<LicenceDenialApplicationData> TransferLicenceDenialApplication(LicenceDenialApplicationData appData,
                                                                string newRecipientSubmitter, string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(appData.Appl_EnfSrv_Cd, appData.Appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenial/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                           $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = await ApiHelper.PutData<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall,
                                                                                              appData, token: Token);
            return data;
        }
    }
}
