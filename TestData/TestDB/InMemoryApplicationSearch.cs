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

        public List<ApplicationSearchResultData> QuickSearch(QuickSearchData searchData, out int totalCount, int page = 1, int perPage = 1000, string orderBy = "A.Appl_EnfSrv_Cd, A.Appl_CtrlCd")
        {
            throw new NotImplementedException();
        }
    }
}
