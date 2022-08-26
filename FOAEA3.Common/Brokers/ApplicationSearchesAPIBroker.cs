using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationSearchesAPIBroker : IApplicationSearchesAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationSearchesAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<List<ApplicationSearchResultData>> SearchAsync(QuickSearchData searchCriteria)
        {
            string apiCall = $"api/v1/applicationSearches";
            return await ApiHelper.PostDataAsync<List<ApplicationSearchResultData>, QuickSearchData>(apiCall, searchCriteria);
        }
    }
}
