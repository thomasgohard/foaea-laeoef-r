using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Test
{
    public class Test1Model : PageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public LoginData2 LoginData { get; set; }

        private IConfiguration Config { get; set; }

        public Test1Model([FromServices] IConfiguration config)
        {
            Config = config;
        }

        public void OnGet()
        {
        }

        public async Task OnPostLogin()
        {
            string apiRoot = Config["APIroot:FoaeaApplicationRootAPI"].ReplaceVariablesWithEnvironmentValues();
            var apiHelper = new APIBrokerHelper(apiRoot: apiRoot);
            var loginAPIs = new LoginsAPIBroker(apiHelper);

            var result = await loginAPIs.LoginAsync(LoginData);

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("RefreshToken", result.RefreshToken);

            Message = "Token: " + result.Token;
        }

        private async Task<string> RefreshTokenAsync()
        {
            string oldToken = HttpContext.Session.GetString("Token");
            string oldRefreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(oldToken))
                return string.Empty;

            string apiRoot = Config["APIroot:FoaeaApplicationRootAPI"].ReplaceVariablesWithEnvironmentValues();
            var apiHelper = new APIBrokerHelper(apiRoot: apiRoot);
            var loginAPIs = new LoginsAPIBroker(apiHelper); 
            
            var result = await loginAPIs.RefreshTokenAsync(oldToken, oldRefreshToken);

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("RefreshToken", result.RefreshToken);

            return result.Token;
        }

        public async Task OnPostLogout()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken"); 
            
            string apiRoot = Config["APIroot:FoaeaApplicationRootAPI"].ReplaceVariablesWithEnvironmentValues();
            var apiHelper = new APIBrokerHelper(apiRoot: apiRoot);
            var loginAPIs = new LoginsAPIBroker(apiHelper);

            var result = await loginAPIs.LogoutAsync(LoginData, currentToken);

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
            else {
                string apiRoot = Config["APIroot:FoaeaApplicationRootAPI"].ReplaceVariablesWithEnvironmentValues();
                var apiHelper = new APIBrokerHelper(apiRoot: apiRoot, getRefreshedToken: RefreshTokenAsync);
                var loginAPIs = new LoginsAPIBroker(apiHelper);

                Message = await loginAPIs.LoginVerificationAsync(new LoginData2 { }, currentToken);
            }

        }

        public async Task OnPostGetAppAPIVersion()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken");

            string apiRoot = Config["APIroot:FoaeaApplicationRootAPI"].ReplaceVariablesWithEnvironmentValues();
            var apiHelper = new APIBrokerHelper(apiRoot: apiRoot, getRefreshedToken: RefreshTokenAsync);
            var appAPIs = new ApplicationAPIBroker(apiHelper);

            var result = await appAPIs.GetVersionAsync(currentToken);

            Message = result;
        }

        public async Task OnPostGetIntAPIVersion()
        {
            string currentToken = HttpContext.Session.GetString("Token");
            string currentRefreshToken = HttpContext.Session.GetString("RefreshToken");

            string apiRoot = Config["APIroot:FoaeaInterceptionRootAPI"].ReplaceVariablesWithEnvironmentValues();
            var apiHelper = new APIBrokerHelper(apiRoot: apiRoot, getRefreshedToken: RefreshTokenAsync);
            var interceptionAPIs = new InterceptionApplicationAPIBroker(apiHelper);

            var result = await interceptionAPIs.GetVersionAsync(currentToken);

            Message = result;
        }

    }
}
