using FOAEA3.Common.Helpers;
using FOAEA3.Model;
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

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/tracings/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/tracings/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<TracingApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}";
            return await ApiHelper.GetDataAsync<TracingApplicationData>(apiCall, token: Token);
        }

        public async Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/traceCycles?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return await ApiHelper.GetDataAsync<List<TraceCycleQuantityData>>(apiCall, token: Token);
        }

        public async Task<List<TraceToApplData>> GetTraceToApplDataAsync()
        {
            string apiCall = $"api/v1/tracings/TraceToApplication";
            return await ApiHelper.GetDataAsync<List<TraceToApplData>>(apiCall, token: Token);
        }

        public async Task<TracingApplicationData> FullyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}?command=FullyServiceApplication&EnforcementServiceCode={enfSrvCd}";
            return await ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>(apiCall, tracingApplication, token: Token);
        }

        public async Task<TracingApplicationData> PartiallyServiceApplicationAsync(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}?command=PartiallyServiceApplication&EnforcementServiceCode={enfSrvCd}";
            return await ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>(apiCall, tracingApplication, token: Token);
        }

        public async Task<TracingApplicationData> CreateTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            var data = await ApiHelper.PostDataAsync<TracingApplicationData, TracingApplicationData>("api/v1/tracings",
                                                                                               tracingApplication, token: Token);
            return data;
        }

        public async Task<TracingApplicationData> UpdateTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            var data = await ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>($"api/v1/tracings/{key}",
                                                                                              tracingApplication, token: Token);
            return data;
        }

        public async Task<TracingApplicationData> CloseTracingApplicationAsync(TracingApplicationData tracingApplication)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            var data = await ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>($"api/v1/tracings/{key}",
                                                                                              tracingApplication, token: Token);
            return data;
        }

        public async Task<TracingApplicationData> TransferTracingApplicationAsync(TracingApplicationData tracingApplication,
                                                                 string newRecipientSubmitter,
                                                                 string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                           $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = await ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>(apiCall,
                                                                                              tracingApplication, token: Token);
            return data;
        }

        public async Task<List<TracingOutgoingFederalData>> GetOutgoingFederalTracingRequestsAsync(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalTracingRequests";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return await ApiHelper.GetDataAsync<List<TracingOutgoingFederalData>>(apiCall, token: Token);
        }

        public async Task<TracingOutgoingProvincialData> GetOutgoingProvincialTracingDataAsync(int maxRecords,
                                                                                  string activeState,
                                                                                  string recipientCode)
        {
            string baseCall = "api/v1/OutgoingProvincialTracingResults";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}&isXML=true";
            return await ApiHelper.GetDataAsync<TracingOutgoingProvincialData>(apiCall, token: Token);
        }

    }
}
