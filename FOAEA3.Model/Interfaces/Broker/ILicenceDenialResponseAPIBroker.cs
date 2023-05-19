using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialResponseAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task InsertBulkDataAsync(List<LicenceDenialResponseData> responseData);
        Task MarkTraceResultsAsViewedAsync(string enfService);
    }
}
