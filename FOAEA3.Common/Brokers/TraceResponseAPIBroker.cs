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

        public async Task InsertBulkData(List<TraceResponseData> responseData)
        {
            _ = await ApiHelper.PostData<TraceResponseData, List<TraceResponseData>>("api/v1/traceResponses/bulk",
                                                                                    responseData, token: Token);
        }

        public async Task AddTraceFinancialResponseData(TraceFinancialResponseData traceFinancialResultData)
        {
            string apiCall = "api/v1/traceFinancialResponses";
            _ = await ApiHelper.PostData<TraceFinancialResponseData, TraceFinancialResponseData>(apiCall,
                                                                                         traceFinancialResultData, token: Token);
        }

        public async Task MarkTraceResultsAsViewed(string recipientSubmCd)
        {
            await ApiHelper.SendCommand("api/v1/traceResponses/MarkResultsAsViewed?recipientSubmCd=" + recipientSubmCd, token: Token);
        }
    }
}
