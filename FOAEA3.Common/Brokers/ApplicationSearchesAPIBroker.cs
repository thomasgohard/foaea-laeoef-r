using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await ApiHelper.PostDataAsync<List<ApplicationSearchResultData>, QuickSearchData>(apiCall, searchCriteria, token: Token);
        }
    }
}
