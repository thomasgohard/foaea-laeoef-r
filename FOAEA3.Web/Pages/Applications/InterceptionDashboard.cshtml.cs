using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications
{
    public class InterceptionDashboardModel : FoaeaPageModel
    {
        public string ErrorMessage { get; set; }

        [BindProperty]
        public ApplicationSearchCriteriaData SearchCriteria { get; set; }

        [BindProperty]
        public ApplicationSearchCriteriaData MySearchCriteria { get; set; }

        public InterceptionDashboardModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) : base(httpContextAccessor, apiConfig.Value)
        {
            SearchCriteria = new ApplicationSearchCriteriaData
            {
                Category = "I01"
            };
        }

        public async Task OnGet()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            var apiRefBroker = new ApplicationLifeStatesAPIBroker(APIs, currentToken);
            
            var data = await apiRefBroker.GetApplicationLifeStatesAsync();
            ViewData["ApplLifeStates"] = data;
        }

        public async Task<IActionResult> OnPostAdvancedSearch()
        {
            if (!ModelState.IsValid)
            {
                string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
                string enfService = HttpContext.Session.GetString(SessionValue.ENF_SERVICE);

                var apiBroker = new ApplicationSearchesAPIBroker(APIs, currentToken);
                var apiRefBroker = new ApplicationLifeStatesAPIBroker(APIs, currentToken);

                var quickSearch = new QuickSearchData
                {
                    DebtorFirstName = SearchCriteria.FirstName,
                    DebtorSurname = SearchCriteria.LastName,
                    SIN = SearchCriteria.SIN,
                    State = SearchCriteria.State,
                    Status = SearchCriteria.Status,
                    Category = SearchCriteria.Category,
                    ControlCode = SearchCriteria.ControlCode,
                    EnforcementService = enfService,
                    ReferenceNumber = SearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = SearchCriteria.JusticeNumber
                };

                apiBroker.Token = currentToken;
                ViewData["SearchResult"] = await apiBroker.SearchAsync(quickSearch);
                var data = await apiRefBroker.GetApplicationLifeStatesAsync();
                ViewData["ApplLifeStates"] = data;

                if (base.APIs.Messages.Any())
                    ErrorMessage = APIs.Messages[0].Description;

                return Page();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostMySearch()
        {
            if (!ModelState.IsValid)
            {
                string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
                string enfService = HttpContext.Session.GetString(SessionValue.ENF_SERVICE);

                var apiBroker = new ApplicationSearchesAPIBroker(APIs, currentToken);
                var apiRefBroker = new ApplicationLifeStatesAPIBroker(APIs, currentToken);

                var quickSearch = new QuickSearchData
                {
                    DebtorFirstName = MySearchCriteria.FirstName,
                    DebtorSurname = MySearchCriteria.LastName,
                    SIN = MySearchCriteria.SIN,
                    State = MySearchCriteria.State,
                    Status = MySearchCriteria.Status,
                    Category = MySearchCriteria.Category,
                    ControlCode = MySearchCriteria.ControlCode,
                    EnforcementService = enfService,
                    ReferenceNumber = MySearchCriteria.EnfSourceRefNumber,
                    JusticeNumber = MySearchCriteria.JusticeNumber
                };

                ViewData["SearchResult"] = await apiBroker.SearchAsync(quickSearch);
                var data = await apiRefBroker.GetApplicationLifeStatesAsync();
                ViewData["ApplLifeStates"] = data;

                if (base.APIs.Messages.Any())
                    ErrorMessage = APIs.Messages[0].Description;
                
                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}

