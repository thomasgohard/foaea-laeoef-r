using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace FOAEA3.API.broker
{
    public class TracingsAPI : BaseAPI
    {

        public TracingApplicationData GetApplication(string enfSrv, string ctrlCd)
        {
            var data = GetDataAsync<TracingApplicationData>($"api/v1/tracings/{enfSrv}-{ctrlCd}", APIroot_Tracing).Result;
            return data;
        }

        public TracingApplicationData CreateApplication(TracingApplicationData tracing)
        {
            var data = PostDataAsync<TracingApplicationData, TracingApplicationData>("api/v1/tracings", tracing, APIroot_Tracing).Result;
            return data;
        }

        public TracingApplicationData UpdateApplication(TracingApplicationData tracing)
        {
            var data = PutDataAsync<TracingApplicationData, TracingApplicationData>("api/v1/tracings", tracing, APIroot_Tracing).Result;
            return data;
        }

        public object GetTraceResults(string enfSrv, string ctrlCd)
        {
            return GetDataAsync<DataList<TraceResponseData>>($"api/v1/traceResponses/{enfSrv}-{ctrlCd}", APIroot_Tracing).Result;
        }

        public List<TracingApplicationData> GetApplicationsWaitingForAffidavit()
        {
            var data = GetDataAsync<DataList<TracingApplicationData>>($"api/v1/tracings/WaitingForAffidavits", APIroot_Tracing).Result;

            if (data.Messages.ContainsSystemMessagesOfType(MessageType.Error)) {
                return new List<TracingApplicationData>(); // error occurred -- should display -- TODO!
            }
            else
            {
                return data.Items;
            }

        }
    }
}
