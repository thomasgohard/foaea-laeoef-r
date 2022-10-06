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
        public string Token { get; set; }

        public LicenceDenialResponseAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task InsertBulkDataAsync(List<LicenceDenialResponseData> responseData)
        {
            _ = await ApiHelper.PostDataAsync<LicenceDenialResponseData, List<LicenceDenialResponseData>>("api/v1/licenceDenialResponses/bulk",
                                                                                                    responseData, token: Token);
        }

        public async Task MarkTraceResultsAsViewedAsync(string enfService)
        {
            await ApiHelper.SendCommandAsync("api/v1/licenceDenialResponses/MarkResultsAsViewed?enfService=" + enfService, token: Token);
        }
    }
}
