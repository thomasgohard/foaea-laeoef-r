using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class TraceResponseAPIBroker : ITraceResponseAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public TraceResponseAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public void InsertBulkData(List<TraceResponseData> responseData)
        {
            _ = ApiHelper.PostDataAsync<TraceResponseData, List<TraceResponseData>>("api/v1/traceResponses/bulk",
                                                                                    responseData).Result;
        }

        public void MarkTraceResultsAsViewed(string enfService)
        {
            ApiHelper.SendCommand("api/v1/traceResponses/MarkResultsAsViewed?enfService=" + enfService);
        }
    }
}
