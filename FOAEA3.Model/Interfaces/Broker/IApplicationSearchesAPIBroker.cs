using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationSearchesAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<List<ApplicationSearchResultData>> SearchAsync(QuickSearchData searchCriteria);
    }
}
