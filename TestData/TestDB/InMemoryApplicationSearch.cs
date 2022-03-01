using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    public class InMemoryApplicationSearch : IApplicationSearchRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public List<ApplicationSearchResultData> QuickSearch(QuickSearchData searchData, out int totalCount)
        {
            throw new NotImplementedException();
        }
    }
}
