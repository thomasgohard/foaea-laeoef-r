using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class TracingEventAPIBroker : ITracingEventAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public TracingEventAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task CloseNETPTraceEventsAsync()
        {
            await ApiHelper.SendCommandAsync($"api/v1/tracingEvents/CloseNETPTraceEvents", token: Token);
        }

        public async Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/tracingEvents/Details/Active?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return await ApiHelper.GetDataAsync<List<ApplicationEventDetailData>>(apiCall, token: Token);
        }

        public async Task<List<ApplicationEventData>> GetRequestedTRCINEventsAsync(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/tracingEvents/RequestedTRCIN?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            return await ApiHelper.GetDataAsync<List<ApplicationEventData>>(apiCall, token: Token);
        }
    }
}
