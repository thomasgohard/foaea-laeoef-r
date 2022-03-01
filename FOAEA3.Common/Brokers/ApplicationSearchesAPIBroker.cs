using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationSearchesAPIBroker : IApplicationSearchesAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationSearchesAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public List<ApplicationSearchResultData> Search(QuickSearchData searchCriteria)
        {
            string apiCall = $"api/v1/applicationSearches";
            return ApiHelper.PostDataAsync<List<ApplicationSearchResultData>, QuickSearchData>(apiCall, searchCriteria).Result;
        }
    }
}
