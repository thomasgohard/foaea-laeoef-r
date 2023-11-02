using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialResponseAPIBroker : ILicenceDenialResponseAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public LicenceDenialResponseAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task InsertBulkData(List<LicenceDenialResponseData> responseData)
        {
            _ = await ApiHelper.PostData<LicenceDenialResponseData, List<LicenceDenialResponseData>>("api/v1/licenceDenialResponses/bulk",
                                                                                                    responseData, token: Token);
        }

        public async Task MarkTraceResultsAsViewed(string enfService)
        {
            await ApiHelper.SendCommand("api/v1/licenceDenialResponses/MarkResultsAsViewed?enfService=" + enfService, token: Token);
        }
    }
}
