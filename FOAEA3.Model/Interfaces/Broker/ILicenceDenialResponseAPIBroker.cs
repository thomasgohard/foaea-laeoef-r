using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialResponseAPIBroker
    {
        Task InsertBulkDataAsync(List<LicenceDenialResponseData> responseData);
        Task MarkTraceResultsAsViewedAsync(string enfService);
    }
}
