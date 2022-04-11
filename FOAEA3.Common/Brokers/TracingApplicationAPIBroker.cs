using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{

    public class TracingApplicationAPIBroker : ITracingApplicationAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public TracingApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public TracingApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}";
            return ApiHelper.GetDataAsync<TracingApplicationData>(apiCall).Result;
        }

        public List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/traceCycles?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return ApiHelper.GetDataAsync<List<TraceCycleQuantityData>>(apiCall).Result;
        }

        public List<TraceToApplData> GetTraceToApplData()
        {
            string apiCall = $"api/v1/tracings/TraceToApplication";
            return ApiHelper.GetDataAsync<List<TraceToApplData>>(apiCall).Result;
        }

        public TracingApplicationData FullyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}?command=FullyServiceApplication&EnforcementServiceCode={enfSrvCd}";
            return ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>(apiCall, tracingApplication).Result;
        }

        public TracingApplicationData PartiallyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/tracings/{key}?command=PartiallyServiceApplication&EnforcementServiceCode={enfSrvCd}";
            return ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>(apiCall, tracingApplication).Result;
        }

        public TracingApplicationData CreateTracingApplication(TracingApplicationData tracingApplication)
        {
            var data = ApiHelper.PostDataAsync<TracingApplicationData, TracingApplicationData>("api/v1/tracings",
                                                                                               tracingApplication).Result;
            return data;
        }

        public TracingApplicationData UpdateTracingApplication(TracingApplicationData tracingApplication)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            var data = ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>($"api/v1/tracings/{key}",
                                                                                              tracingApplication).Result;
            return data;
        }

        public TracingApplicationData CloseTracingApplication(TracingApplicationData tracingApplication)
        {
            string key = ApplKey.MakeKey(tracingApplication.Appl_EnfSrv_Cd, tracingApplication.Appl_CtrlCd);
            var data = ApiHelper.PutDataAsync<TracingApplicationData, TracingApplicationData>($"api/v1/tracings/{key}",
                                                                                              tracingApplication).Result;
            return data;
        }

        public List<TracingOutgoingFederalData> GetOutgoingFederalTracingRequests(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalTracingRequests";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return ApiHelper.GetDataAsync<List<TracingOutgoingFederalData>>(apiCall).Result;
        }
         
        public List<TracingOutgoingProvincialData> GetOutgoingProvincialTracingData(int maxRecords,
                                                                                  string activeState,
                                                                                  string recipientCode)
        {
            string baseCall = "api/v1/OutgoingProvincialTracingResults";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                                        $"&recipientCode={recipientCode}&isXML=true";
            return ApiHelper.GetDataAsync<List<TracingOutgoingProvincialData>>(apiCall).Result;
        }

    }
}
