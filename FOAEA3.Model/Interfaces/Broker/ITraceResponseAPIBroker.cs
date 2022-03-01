using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ITraceResponseAPIBroker
    {
        void InsertBulkData(List<TraceResponseData> responseData);
        void MarkTraceResultsAsViewed(string enfService);
    }
}
