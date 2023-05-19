using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IApplicationSearchRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }
        string LastError { get; set; }

        Task<(List<ApplicationSearchResultData>, int)> QuickSearchAsync(QuickSearchData searchData,
                                                      int page = 1, int perPage = 1000, string orderBy = "A.Appl_EnfSrv_Cd, A.Appl_CtrlCd");
    }

}
