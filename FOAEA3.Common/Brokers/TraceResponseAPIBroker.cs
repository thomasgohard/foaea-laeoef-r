using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class TraceResponseAPIBroker : ITraceResponseAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public TraceResponseAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task InsertBulkDataAsync(List<TraceResponseData> responseData)
        {
            _ = await ApiHelper.PostDataAsync<TraceResponseData, List<TraceResponseData>>("api/v1/traceResponses/bulk",
                                                                                    responseData, token: Token);
        }

        public async Task MarkTraceResultsAsViewedAsync(string enfService)
        {
            await ApiHelper.SendCommandAsync("api/v1/traceResponses/MarkResultsAsViewed?enfService=" + enfService, token: Token);
        }
    }
}
