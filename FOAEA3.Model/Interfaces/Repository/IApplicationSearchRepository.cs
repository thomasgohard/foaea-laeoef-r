using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationSearchRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<ApplicationSearchResultData> QuickSearch(QuickSearchData searchData, out int totalCount);
    }

}
