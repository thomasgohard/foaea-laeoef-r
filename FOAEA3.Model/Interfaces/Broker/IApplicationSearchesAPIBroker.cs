using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationSearchesAPIBroker
    {
        List<ApplicationSearchResultData> Search(QuickSearchData searchCriteria);
    }
}
