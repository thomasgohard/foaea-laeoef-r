using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Security
{
    public class SelectSubmitterModel : PageModel
    {
        public const string AvailableSubmitters = "AvailableSubmitters";

        private ApiConfig ApiConfig { get; set; }

        public SelectSubmitterModel([FromServices] IOptions<ApiConfig> apiConfigOption)
        {
            ApiConfig = apiConfigOption.Value;
        }

        [BindProperty]
        public string Submitter { get; set; }
        public string Message { get; set; }

        public List<string> Submitters { get; set; }

        public async Task OnGet()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, currentToken);

            Submitters = await loginAPIs.GetAvailableSubmittersAsync();
        }

        public async Task<ActionResult> OnPostSelectSubmitter()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, currentToken);

            var result = await loginAPIs.SelectSubmitterAsync(Submitter);

            if ((result.Token == null) || string.IsNullOrEmpty(result.Token))
            {
                Message = "Error: Could not select submitter";
                return null;
            }
            else
            {
                var submitterAPIs = new SubmitterAPIBroker(apiHelper, currentToken);
                var submitter = await submitterAPIs.GetSubmitterAsync(Submitter);

                HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
                HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

                HttpContext.Session.SetString(SessionValue.USER_NAME, TokenDataHelper.UserName(result.Token));
                HttpContext.Session.SetString(SessionValue.SUBMITTER, Submitter);
                HttpContext.Session.SetString(SessionValue.ENF_SERVICE, submitter.EnfSrv_Cd);

                return RedirectToPage("/Applications/InterceptionDashboard");
            }
        }
    }
}
