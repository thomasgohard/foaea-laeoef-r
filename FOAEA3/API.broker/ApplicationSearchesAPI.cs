using FOAEA3.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class ApplicationSearchesAPI : BaseAPI
    {

        internal List<ApplicationSearchResultData> Search(QuickSearchData searchCriteria)
        {
            return PostDataAsync<List<ApplicationSearchResultData>, QuickSearchData>("api/v1/applicationSearches", searchCriteria).Result;
        }

    }
}
