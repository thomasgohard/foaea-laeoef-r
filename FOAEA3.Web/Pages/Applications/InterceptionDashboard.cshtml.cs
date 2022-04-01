using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

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

            string currentSubmitter = "";  // need to put something here, for testing/prototype
            string currentUser = "";       // need to put something here, for testing/prototype
            var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaApplicationRootAPI, currentSubmitter, currentUser);
            var apiBroker = new ApplicationLifeStatesAPIBroker(apiHelper);

            ApplLifeStates = apiBroker.GetApplicationLifeStates().Items;

            SearchCriteria = new ApplicationSearchCriteriaData
            {
                Category = "I01"
            };
        }

        public void OnGet()
        {
            ViewData["ApplLifeStates"] = ApplLifeStates;
        }

        public IActionResult OnPostAdvancedSearch()
        {
            if (!ModelState.IsValid)
            {
                // call API to do search and display result
                string currentSubmitter = "";
                string currentUser = "";
                var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaApplicationRootAPI, currentSubmitter, currentUser);
                var apiBroker = new ApplicationSearchesAPIBroker(apiHelper);

                var quickSearch = new QuickSearchData
                {
                    FirstName = SearchCriteria.FirstName,
                    LastName = SearchCriteria.LastName,
                    SIN = SearchCriteria.SIN,
                    State = SearchCriteria.State,
                    Status = SearchCriteria.Status,
                    Category = SearchCriteria.Category,
                    ControlCode = SearchCriteria.ControlCode,
                    EnfService = "ON01",
                    EnfSourceRefNumber = SearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = SearchCriteria.JusticeNumber
                };

                ViewData["SearchResult"] = apiBroker.Search(quickSearch);
                ViewData["ApplLifeStates"] = ApplLifeStates;

                return Page();
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostMySearch()
        {
            if (!ModelState.IsValid)
            {
                // call API to do search and display result
                string currentSubmitter = "";
                string currentUser = "";
                var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaApplicationRootAPI, currentSubmitter, currentUser);
                var apiBroker = new ApplicationSearchesAPIBroker(apiHelper);

                var quickSearch = new QuickSearchData
                {
                    FirstName = MySearchCriteria.FirstName,
                    LastName = MySearchCriteria.LastName,
                    SIN = MySearchCriteria.SIN,
                    State = MySearchCriteria.State,
                    Status = MySearchCriteria.Status,
                    Category = MySearchCriteria.Category,
                    ControlCode = MySearchCriteria.ControlCode,
                    EnfService = "ON01",
                    EnfSourceRefNumber = MySearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = MySearchCriteria.JusticeNumber
                };

                ViewData["SearchResult"] = apiBroker.Search(quickSearch);
                ViewData["ApplLifeStates"] = ApplLifeStates;

                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}

