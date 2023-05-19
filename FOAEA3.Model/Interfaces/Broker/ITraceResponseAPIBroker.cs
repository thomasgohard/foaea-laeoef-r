using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ITraceResponseAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task InsertBulkDataAsync(List<TraceResponseData> responseData);
        Task AddTraceFinancialResponseData(TraceFinancialResponseData traceFinancialResultData);
        Task MarkTraceResultsAsViewedAsync(string enfService);
    }
}
