using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using FOAEA3.Web.Helpers;
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

            HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
            HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

            Message = "Token: " + result.Token;
        }

        public async Task OnPostLogout()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            string currentRefreshToken = HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, currentToken);

            await loginAPIs.LogoutAsync(LoginData);

            HttpContext.Session.SetString(SessionValue.TOKEN, "");
            HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, "");

            Message = "Logged out";
        }

        public async Task OnPostVerify()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            string currentRefreshToken = HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

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
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            string currentRefreshToken = HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI,
                                                getRefreshedToken: OnRefreshTokenAsync);
            var appAPIs = new ApplicationAPIBroker(apiHelper, currentToken);

            var result = await appAPIs.GetVersionAsync();

            Message = result;
        }

        public async Task OnPostGetIntAPIVersion()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            string currentRefreshToken = HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaInterceptionRootAPI,
                                                getRefreshedToken: OnRefreshTokenAsync);
            var interceptionAPIs = new InterceptionApplicationAPIBroker(apiHelper, currentToken);

            var result = await interceptionAPIs.GetVersionAsync();

            Message = result;
        }

        private async Task<string> OnRefreshTokenAsync()
        {
            string oldToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            string oldRefreshToken = HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

            if (string.IsNullOrEmpty(oldToken))
                return string.Empty;

            var apiHelper = new APIBrokerHelper(apiRoot: ApiConfig.FoaeaRootAPI);
            var loginAPIs = new LoginsAPIBroker(apiHelper, oldToken);

            var result = await loginAPIs.RefreshTokenAsync(oldToken, oldRefreshToken);

            HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
            HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

            return result.Token;
        }

    }
}
