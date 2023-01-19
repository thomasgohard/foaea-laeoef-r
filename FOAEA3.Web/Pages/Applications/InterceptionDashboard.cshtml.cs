using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications;

public class InterceptionDashboardModel : FoaeaPageModel
{
    public string ErrorMessage { get; set; }
    public List<ApplicationLifeStateData> LifeStates { get; set; }
    public List<ApplicationSearchResultData> SearchResults { get; set; }

    [BindProperty]
    public ApplicationSearchCriteriaData SearchCriteria { get; set; }

    [BindProperty]
    public ApplicationSearchCriteriaData MySearchCriteria { get; set; }

    public InterceptionDashboardModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
        var apiRefBroker = new ApplicationLifeStatesAPIBroker(BaseAPIs);
        LifeStates = apiRefBroker.GetApplicationLifeStatesAsync().Result;

        SearchCriteria = new ApplicationSearchCriteriaData
        {
            Category = "I01" // default
        };
    }

    public async Task<IActionResult> OnPostAdvancedSearch()
    {
        if (!ModelState.IsValid)
        {
            SearchResults = await GetSearchResults(SearchCriteria);

            if (BaseAPIs.ErrorData.Any())
                ErrorMessage = BaseAPIs.BuildErrorMessage();

            return Page();
        }

        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostMySearch()
    {
        if (!ModelState.IsValid)
        {
            SearchResults = await GetSearchResults(MySearchCriteria);

            if (BaseAPIs.ErrorData.Any())
                ErrorMessage = BaseAPIs.BuildErrorMessage();

            return Page();
        }

        return RedirectToPage("./Index");
    }

    private async Task<List<ApplicationSearchResultData>> GetSearchResults(ApplicationSearchCriteriaData searchCriteria)
    {
        string enfService = HttpContext.Session.GetString(SessionValue.ENF_SERVICE);

        var quickSearch = new QuickSearchData
        {
            DebtorFirstName = searchCriteria.FirstName,
            DebtorSurname = searchCriteria.LastName,
            SIN = searchCriteria.SIN,
            State = searchCriteria.State,
            Status = searchCriteria.Status,
            Category = searchCriteria.Category,
            ControlCode = searchCriteria.ControlCode,
            EnforcementService = enfService,
            ReferenceNumber = searchCriteria.EnfSourceRefNumber,
            JusticeNumber = searchCriteria.JusticeNumber
        };

        var searchApi = new ApplicationSearchesAPIBroker(BaseAPIs);
        return await searchApi.SearchAsync(quickSearch);
    }

}

