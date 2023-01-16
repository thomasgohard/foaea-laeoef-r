using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Security
{
    public class TermsOfReferenceModel : PageModel
    {
        private ApiConfig ApiConfig { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public TermsOfReferenceModel([FromServices] IOptions<ApiConfig> apiConfigOption)
        {
            ApiConfig = apiConfigOption.Value;
        }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAccept()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, currentToken);

            var result = await loginAPIs.AcceptTerms();

            if ((result == null) || string.IsNullOrEmpty(result.Token))
            {
                Message = "Error: accept failed.";
                return null;
            }
            else
            {
                HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
                HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

                return RedirectToPage("SelectSubmitter");
            }
        }
    }
}
