using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationAPIBroker : IApplicationAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/applications/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/applications/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<ApplicationData> GetApplicationAsync(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string apiCall = $"api/v1/applications/{key}";
            return await ApiHelper.GetDataAsync<ApplicationData>(apiCall);
        }

        public async Task<ApplicationData> SinConfirmationAsync(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string baseCall = "api/v1/Applications";
            string apiCall = $"{baseCall}/{key}/SinConfirmation";
            return await ApiHelper.PutDataAsync<ApplicationData, SINConfirmationData>(apiCall, confirmationData);
        }

        public async Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusDataAsync(int maxRecords,
                                                                                    string activeState, string recipientCode)
        {
            string baseCall = "api/v1/Applications";
            string apiCall = $"{baseCall}/stats?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}";
            return await ApiHelper.GetDataAsync<List<StatsOutgoingProvincialData>>(apiCall);
        }

        public async Task<ApplicationData> ValidateCoreValuesAsync(ApplicationData application)
        {
            string key = ApplKey.MakeKey(application.Appl_EnfSrv_Cd, application.Appl_CtrlCd);
            string apiCall = $"api/v1/Applications/{key}/ValidateCoreValues";
            return await ApiHelper.PutDataAsync<ApplicationData, ApplicationData>(apiCall, application);
        }
    }
}
