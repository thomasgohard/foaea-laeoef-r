using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialResponseAPIBroker
    {
        void InsertBulkData(List<LicenceDenialResponseData> responseData);
        void MarkTraceResultsAsViewed(string enfService);
    }
}
