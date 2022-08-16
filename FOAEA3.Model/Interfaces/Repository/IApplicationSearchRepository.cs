using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationSearchRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }
        string LastError { get; set; }

        List<ApplicationSearchResultData> QuickSearch(QuickSearchData searchData, out int totalCount,
                                                      int page = 1, int perPage = 1000, string orderBy = "A.Appl_EnfSrv_Cd, A.Appl_CtrlCd");
    }

}
