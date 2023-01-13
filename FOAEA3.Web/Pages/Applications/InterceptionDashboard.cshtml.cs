using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications
{
    public class InterceptionDashboardModel : PageModel
    {
        private readonly ApiConfig ApiRoots;

        private List<ApplicationLifeStateData> ApplLifeStates { get; set; }

        public string UserName { get; set; }
        public string Submitter { get; set; }

        [BindProperty]
        public ApplicationSearchCriteriaData SearchCriteria { get; set; }

        [BindProperty]
        public ApplicationSearchCriteriaData MySearchCriteria { get; set; }

        public InterceptionDashboardModel(IOptions<ApiConfig> apiConfig)
        {
            ApiRoots = apiConfig.Value;

            SearchCriteria = new ApplicationSearchCriteriaData
            {
                Category = "I01"
            };

            
        }

        public void OnGet()
        {
            ViewData["ApplLifeStates"] = ReferenceData.Instance().ApplicationLifeStates;

            string currentToken = HttpContext.Session.GetString("Token");

            UserName = TokenDataHelper.UserName(currentToken);
            Submitter = TokenDataHelper.SubmitterCode(currentToken);
        }

        public async Task<IActionResult> OnPostAdvancedSearch()
        {
            if (!ModelState.IsValid)
            {
                // TODO: fix token
                string currentToken = HttpContext.Session.GetString("Token");
                string refreshToken = HttpContext.Session.GetString("RefreshToken");
                // call API to do search and display result
                string currentSubmitter = "ON2D68"; // TODO: fix when log in is done
                string currentUser = "system_support"; // TODO: fix when log in is done
                var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaRootAPI, currentSubmitter, currentUser);
                var apiBroker = new ApplicationSearchesAPIBroker(apiHelper, currentToken);

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

                apiBroker.Token = currentToken;
                ViewData["SearchResult"] = await apiBroker.SearchAsync(quickSearch);
                ViewData["ApplLifeStates"] = ReferenceData.Instance().ApplicationLifeStates;

                return Page();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostMySearch()
        {
            if (!ModelState.IsValid)
            {
                // TODO: fix token
                string token = "";
                // call API to do search and display result
                string currentSubmitter = "ON2D68"; // TODO: fix when log in is done
                string currentUser = "system_support"; // TODO: fix when log in is done
                var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaRootAPI, currentSubmitter, currentUser);
                var apiBroker = new ApplicationSearchesAPIBroker(apiHelper, token);

                var quickSearch = new QuickSearchData
                {
                    DebtorFirstName = MySearchCriteria.FirstName,
                    DebtorSurname = MySearchCriteria.LastName,
                    SIN = MySearchCriteria.SIN,
                    State = MySearchCriteria.State,
                    Status = MySearchCriteria.Status,
                    Category = MySearchCriteria.Category,
                    ControlCode = MySearchCriteria.ControlCode,
                    EnforcementService = "ON01", // TODO: fix when log in is done
                    ReferenceNumber = MySearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = MySearchCriteria.JusticeNumber
                };

                ViewData["SearchResult"] = await apiBroker.SearchAsync(quickSearch);
                ViewData["ApplLifeStates"] = ReferenceData.Instance().ApplicationLifeStates;

                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}

