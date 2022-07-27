using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationAPIBroker : IApplicationAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public string GetVersion()
        {
            string apiCall = $"api/v1/applications/Version";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public string GetConnection()
        {
            string apiCall = $"api/v1/applications/DB";
            return ApiHelper.GetStringAsync(apiCall, maxAttempts: 1).Result;
        }

        public ApplicationData GetApplication(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string apiCall = $"api/v1/applications/{key}";
            return ApiHelper.GetDataAsync<ApplicationData>(apiCall).Result;
        }

        public ApplicationData SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string baseCall = "api/v1/Applications";
            string apiCall = $"{baseCall}/{key}/SinConfirmation";
            return ApiHelper.PutDataAsync<ApplicationData, SINConfirmationData>(apiCall, confirmationData).Result;
        }

        List<StatsOutgoingProvincialData> IApplicationAPIBroker.GetOutgoingProvincialStatusData(int maxRecords, 
                                                                                    string activeState, string recipientCode)
        {
            string baseCall = "api/v1/Applications";
            string apiCall = $"{baseCall}/stats?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}";
            return ApiHelper.GetDataAsync<List<StatsOutgoingProvincialData>>(apiCall).Result;
        }

        public ApplicationData ValidateCoreValues(ApplicationData application)
        {
            string key = ApplKey.MakeKey(application.Appl_EnfSrv_Cd, application.Appl_CtrlCd);
            string apiCall = $"api/v1/Applications/{key}/ValidateCoreValues";
            return ApiHelper.PutDataAsync<ApplicationData, ApplicationData>(apiCall, application).Result;
        }
    }
}
