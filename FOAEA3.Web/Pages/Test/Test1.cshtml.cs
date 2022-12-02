using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Test
{
    public class Test1Model : PageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public FoaeaLoginData LoginData { get; set; }

        private ApiConfig ApiConfig { get; set; }

        public Test1Model([FromServices] IOptions<ApiConfig> apiConfigOption)
        {
            ApiConfig = apiConfigOption.Value;
        }

        public void OnGet()
        {
        }

        public async Task OnPostLogin()
        {
            string token = string.Empty;
            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, token);

            var result = await loginAPIs.LoginAsync(LoginData);

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("RefreshToken", result.RefreshToken);

            Message = "Token: " + result.Token;
        }

        public async Task OnPostLogout()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken");

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, currentToken);

            await loginAPIs.LogoutAsync(LoginData);

            HttpContext.Session.SetString("Token", "");
            HttpContext.Session.SetString("RefreshToken", "");

            Message = "Logged out";
        }

        public async Task OnPostVerify()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(currentToken))
                Message = "No logged in user.";
            else
            {
                var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI,
                                                    getRefreshedToken: OnRefreshTokenAsync);
                var loginAPIs = new LoginsAPIBroker(apiHelper, currentToken);

                Message = await loginAPIs.LoginVerificationAsync(new FoaeaLoginData { });
            }

        }

        public async Task OnPostGetAppAPIVersion()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken");

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI,
                                                getRefreshedToken: OnRefreshTokenAsync);
            var appAPIs = new ApplicationAPIBroker(apiHelper, currentToken);

            var result = await appAPIs.GetVersionAsync();

            Message = result;
        }

        public async Task OnPostGetIntAPIVersion()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken");

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaInterceptionRootAPI,
                                                getRefreshedToken: OnRefreshTokenAsync);
            var interceptionAPIs = new InterceptionApplicationAPIBroker(apiHelper, currentToken);

            var result = await interceptionAPIs.GetVersionAsync();

            Message = result;
        }

        private async Task<string> OnRefreshTokenAsync()
        {
            string oldToken = HttpContext.Session.GetString("Token");
            string oldRefreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(oldToken))
                return string.Empty;

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, oldToken);

            var result = await loginAPIs.RefreshTokenAsync(oldToken, oldRefreshToken);

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("RefreshToken", result.RefreshToken);

            return result.Token;
        }

    }
}
