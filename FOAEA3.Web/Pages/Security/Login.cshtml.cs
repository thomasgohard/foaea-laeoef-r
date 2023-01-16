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
    public class LoginModel : PageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public FoaeaLoginData LoginData { get; set; }

        private ApiConfig ApiConfig { get; set; }

        public LoginModel([FromServices] IOptions<ApiConfig> apiConfigOption)
        {
            ApiConfig = apiConfigOption.Value;
        }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostLogin()
        {
            string token = string.Empty; // no token since not logged in yet
            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, token);

            var result = await loginAPIs.SubjectLoginAsync(LoginData);

            if ((result == null) || string.IsNullOrEmpty(result.Token))
            {
                Message = "Error: login failed.";
                return null;
            }
            else
            {
                HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
                HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

                return RedirectToPage("TermsOfReference");
            }
        }
    }
}
