using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications
{
    public class InterceptionDashboardModel : PageModel
    {
        private readonly ApiConfig ApiRoots;

        public List<ApplicationLifeStateData> ApplLifeStates { get; }

        [BindProperty]
        public ApplicationSearchCriteriaData SearchCriteria { get; set; }

        [BindProperty]
        public ApplicationSearchCriteriaData MySearchCriteria { get; set; }

        public InterceptionDashboardModel(IOptions<ApiConfig> apiConfig)
        {
            ApiRoots = apiConfig.Value;

            string currentSubmitter = "ON2D68";  // need to put something here, for testing/prototype
            string currentUser = "system_support";       // need to put something here, for testing/prototype
            var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaApplicationRootAPI, currentSubmitter, currentUser);
            var apiBroker = new ApplicationLifeStatesAPIBroker(apiHelper);

            ApplLifeStates = (apiBroker.GetApplicationLifeStatesAsync().Result).Items;

            SearchCriteria = new ApplicationSearchCriteriaData
            {
                Category = "I01"
            };
        }

        public void OnGet()
        {
            ViewData["ApplLifeStates"] = ApplLifeStates;
        }

        public async Task<IActionResult> OnPostAdvancedSearch()
        {
            if (!ModelState.IsValid)
            {
                // call API to do search and display result
                string currentSubmitter = "ON2D68";
                string currentUser = "system_support";
                var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaApplicationRootAPI, currentSubmitter, currentUser);
                var apiBroker = new ApplicationSearchesAPIBroker(apiHelper);

                var quickSearch = new QuickSearchData
                {
                    DebtorFirstName = SearchCriteria.FirstName,
                    DebtorSurname = SearchCriteria.LastName,
                    SIN = SearchCriteria.SIN,
                    State = SearchCriteria.State,
                    Status = SearchCriteria.Status,
                    Category = SearchCriteria.Category,
                    ControlCode = SearchCriteria.ControlCode,
                    EnforcementService = "ON01",
                    ReferenceNumber = SearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = SearchCriteria.JusticeNumber
                };

                ViewData["SearchResult"] = await apiBroker.SearchAsync(quickSearch);
                ViewData["ApplLifeStates"] = ApplLifeStates;

                return Page();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostMySearch()
        {
            if (!ModelState.IsValid)
            {
                // call API to do search and display result
                string currentSubmitter = "ON2D68";
                string currentUser = "system_support";
                var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaApplicationRootAPI, currentSubmitter, currentUser);
                var apiBroker = new ApplicationSearchesAPIBroker(apiHelper);

                var quickSearch = new QuickSearchData
                {
                    DebtorFirstName = MySearchCriteria.FirstName,
                    DebtorSurname = MySearchCriteria.LastName,
                    SIN = MySearchCriteria.SIN,
                    State = MySearchCriteria.State,
                    Status = MySearchCriteria.Status,
                    Category = MySearchCriteria.Category,
                    ControlCode = MySearchCriteria.ControlCode,
                    EnforcementService = "ON01",
                    ReferenceNumber = MySearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = MySearchCriteria.JusticeNumber
                };

                ViewData["SearchResult"] = await apiBroker.SearchAsync(quickSearch);
                ViewData["ApplLifeStates"] = ApplLifeStates;

                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}

