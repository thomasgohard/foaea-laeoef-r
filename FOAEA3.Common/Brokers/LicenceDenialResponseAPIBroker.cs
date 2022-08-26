using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialResponseAPIBroker : ILicenceDenialResponseAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialResponseAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task InsertBulkDataAsync(List<LicenceDenialResponseData> responseData)
        {
            _ = await ApiHelper.PostDataAsync<LicenceDenialResponseData, List<LicenceDenialResponseData>>("api/v1/licenceDenialResponses/bulk",
                                                                                                    responseData);
        }

        public async Task MarkTraceResultsAsViewedAsync(string enfService)
        {
            await ApiHelper.SendCommandAsync("api/v1/licenceDenialResponses/MarkResultsAsViewed?enfService=" + enfService);
        }
    }
}
