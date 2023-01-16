using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationSearchesAPIBroker : IApplicationSearchesAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ApplicationSearchesAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<ApplicationSearchResultData>> SearchAsync(QuickSearchData searchCriteria)
        {
            string apiCall = $"api/v1/applicationSearches";
            var result = await ApiHelper.PostDataAsync<List<ApplicationSearchResultData>, QuickSearchData>(apiCall, searchCriteria, token: Token);

            return result;
        }
    }
}
