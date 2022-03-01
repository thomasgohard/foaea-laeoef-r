using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class TracingEventAPIBroker : ITracingEventAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public TracingEventAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public void CloseNETPTraceEvents()
        {
            ApiHelper.SendCommand($"api/v1/tracingEvents/CloseNETPTraceEvents");
        }

        public List<ApplicationEventDetailData> GetActiveTracingEventDetails(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/tracingEvents/Details/Active?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return ApiHelper.GetDataAsync<List<ApplicationEventDetailData>>(apiCall).Result;
        }

        public List<ApplicationEventData> GetRequestedTRCINEvents(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/tracingEvents/RequestedTRCIN?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return ApiHelper.GetDataAsync<List<ApplicationEventData>>(apiCall).Result;
        }
    }
}
