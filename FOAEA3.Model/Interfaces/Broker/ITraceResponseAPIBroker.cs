using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ITraceResponseAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task InsertBulkDataAsync(List<TraceResponseData> responseData);
        Task MarkTraceResultsAsViewedAsync(string enfService);
    }
}
