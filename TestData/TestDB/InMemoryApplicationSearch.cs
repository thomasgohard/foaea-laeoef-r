using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationSearch : IApplicationSearchRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }
        public string LastError { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<(List<ApplicationSearchResultData>, int)> QuickSearchAsync(QuickSearchData searchData, int page = 1, int perPage = 1000, string orderBy = "A.Appl_EnfSrv_Cd, A.Appl_CtrlCd")
        {
            throw new NotImplementedException();
        }
    }
}
