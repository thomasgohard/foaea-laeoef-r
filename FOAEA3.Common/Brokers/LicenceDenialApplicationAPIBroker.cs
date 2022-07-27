using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialApplicationAPIBroker : ILicenceDenialApplicationAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/licencedenials/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/licencedenials/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
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

        public List<LicenceDenialToApplData> GetLicenceDenialToApplData(string fedSource)
        {
            string apiCall = $"api/v1/licenceDenials/LicenceDenialToApplication?federalSource={fedSource}";
            return ApiHelper.GetDataAsync<List<LicenceDenialToApplData>>(apiCall).Result;
        }

        public LicenceDenialApplicationData ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var appData = new LicenceDenialApplicationData
            {
                Appl_EnfSrv_Cd = appl_EnfSrv_Cd,
                Appl_CtrlCd = appl_CtrlCd,
                Subm_SubmCd = "MSGBRO"
            };
            string key = ApplKey.MakeKey(appl_EnfSrv_Cd, appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenials/{key}/ProcessLicenceDenialResponse";
            return ApiHelper.PutDataAsync<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData).Result;
        }

        public List<LicenceDenialOutgoingProvincialData> GetOutgoingProvincialLicenceDenialData(int maxRecords, string activeState,
                                                                                                string recipientCode)
        {
            string baseCall = "api/v1/OutgoingProvincialLicenceDenialResults";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}&isXML=true";
            return ApiHelper.GetDataAsync<List<LicenceDenialOutgoingProvincialData>>(apiCall).Result;
        }


    }
}
