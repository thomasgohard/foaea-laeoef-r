using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationSearchesAPIBroker
    {
        Task<List<ApplicationSearchResultData>> SearchAsync(QuickSearchData searchCriteria);
    }
}
