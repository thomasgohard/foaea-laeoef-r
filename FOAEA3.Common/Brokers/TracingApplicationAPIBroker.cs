using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{

    public class TracingApplicationAPIBroker : ITracingApplicationAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public TracingApplicationAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/tracings/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/tracings/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<TracingApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}";
            return await ApiHelper.GetData<TracingApplicationData>(apiCall, token: Token);
        }

        public async Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityData(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/traceCycles?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return await ApiHelper.GetData<List<TraceCycleQuantityData>>(apiCall, token: Token);
        }

        public async Task<List<TraceToApplData>> GetTraceToApplData()
        {
            string apiCall = $"api/v1/tracings/TraceToApplication";
            return await ApiHelper.GetData<List<TraceToApplData>>(apiCall, token: Token);
        }

        public async Task<TracingApplicationData> FullyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}?command=FullyServiceApplication&EnforcementServiceCode={enfSrvCd}";
            return await ApiHelper.PutData<TracingApplicationData, TracingApplicationData>(apiCall, tracingApplication, token: Token);
        }

        public async Task<TracingApplicationData> PartiallyServiceApplication(TracingApplicationData tracingApplication, FederalSource fedSource)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}?command=PartiallyServiceApplication&fedSource={fedSource}";
            return await ApiHelper.PutData<TracingApplicationData, TracingApplicationData>(apiCall, tracingApplication, token: Token);
        }

        public async Task<TracingApplicationData> CreateTracingApplication(TracingApplicationData tracingApplication)
        {
            var data = await ApiHelper.PostData<TracingApplicationData, TracingApplicationData>("api/v1/tracings",
                                                                                               tracingApplication, token: Token);
            return data;
        }

        public async Task<TracingApplicationData> UpdateTracingApplication(TracingApplicationData tracingApplication)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            var data = await ApiHelper.PutData<TracingApplicationData, TracingApplicationData>($"api/v1/tracings/{key}",
                                                                                              tracingApplication, token: Token);
            return data;
        }

        public async Task<TracingApplicationData> CancelTracingApplication(TracingApplicationData tracingApplication)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            var data = await ApiHelper.PutData<TracingApplicationData, TracingApplicationData>($"api/v1/tracings/{key}",
                                                                                              tracingApplication, token: Token);
            return data;
        }

        public async Task<TracingApplicationData> TransferTracingApplication(TracingApplicationData tracingApplication,
                                                                 string newRecipientSubmitter,
                                                                 string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                           $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = await ApiHelper.PutData<TracingApplicationData, TracingApplicationData>(apiCall,
                                                                                              tracingApplication, token: Token);
            return data;
        }

        public async Task<List<TracingOutgoingFederalData>> GetOutgoingFederalTracingRequests(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalTracingRequests";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return await ApiHelper.GetData<List<TracingOutgoingFederalData>>(apiCall, token: Token);
        }

        public async Task<TracingOutgoingProvincialData> GetOutgoingProvincialTracingData(int maxRecords,
                                                                                  string activeState,
                                                                                  string recipientCode)
        {
            string baseCall = "api/v1/OutgoingProvincialTracingResults";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}&isXML=true";
            return await ApiHelper.GetData<TracingOutgoingProvincialData>(apiCall, token: Token);
        }

    }
}
