using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using Spire.Pdf.Lists;

namespace FOAEA3.Common.Brokers
{
    public class TracingEventAPIBroker : ITracingEventAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public TracingEventAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task CloseNETPTraceEvents()
        {
            await ApiHelper.SendCommand($"api/v1/tracingEvents/CloseNETPTraceEvents", token: Token);
        }

        public async Task<ApplicationEventDetailsList> GetActiveTracingEventDetails(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/tracingEvents/Details/Active?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            var data = await ApiHelper.GetData<List<ApplicationEventDetailData>>(apiCall, token: Token);
            return new ApplicationEventDetailsList(data);
        }

        public async Task<ApplicationEventsList> GetRequestedTRCINEvents(string enfSrvCd, string fileCycle)
        {
            string apiCall = $"api/v1/tracingEvents/RequestedTRCIN?enforcementServiceCode={enfSrvCd}&fileCycle={fileCycle}";
            var data = await ApiHelper.GetData<List<ApplicationEventData>>(apiCall, token: Token);
            return new ApplicationEventsList(data);
        }
    }
}
