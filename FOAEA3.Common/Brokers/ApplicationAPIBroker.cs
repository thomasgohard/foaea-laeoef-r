using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationAPIBroker : IApplicationAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ApplicationAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/applications/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/applications/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<ApplicationData> GetApplication(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string apiCall = $"api/v1/applications/{key}";
            return await ApiHelper.GetData<ApplicationData>(apiCall, token: Token);
        }

        public async Task<ApplicationData> SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd,
                                                                        SINConfirmationData confirmationData)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string baseCall = "api/v1/Applications";
            string apiCall = $"{baseCall}/{key}/SinConfirmation";
            return await ApiHelper.PutData<ApplicationData, SINConfirmationData>(apiCall,
                                                                            confirmationData, token: Token);
        }
        
        public async Task<List<ApplicationData>> GetApplicationsForSin(string confirmedSIN)
        {
            string apiCall = "api/v1/Applications/ConfirmedSIN";

            var confirmedSinData = new StringData { Data = confirmedSIN };

            return await ApiHelper.PutData<List<ApplicationData>, StringData>(apiCall, confirmedSinData, token: Token);
        }

        public async Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusData(int maxRecords,
                                                                                        string activeState,
                                                                                        string recipientCode)
        {
            string baseCall = "api/v1/Applications";
            string apiCall = $"{baseCall}/stats?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}";
            return await ApiHelper.GetData<List<StatsOutgoingProvincialData>>(apiCall, token: Token);
        }

        public async Task<ApplicationData> ValidateCoreValues(ApplicationData application)
        {
            string key = ApplKey.MakeKey(application.Appl_EnfSrv_Cd, application.Appl_CtrlCd);
            string apiCall = $"api/v1/Applications/{key}/ValidateCoreValues";
            return await ApiHelper.PutData<ApplicationData, ApplicationData>(apiCall, application,
                                                                                            token: Token);
        }
    }
}
