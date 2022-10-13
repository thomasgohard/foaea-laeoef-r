using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace FileBroker.Web.Pages.Tools
{
    public class TestFoaeaLoginModel : PageModel
    {
        public string Message { get; set; }

        public async Task OnPostLogin(IConfiguration config)
        {
            var client = new HttpClient();

            var userLoginInfo = new FoaeaLoginData
            {
                UserName = config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                Password = config["FOAEA:password"].ReplaceVariablesWithEnvironmentValues(),
                Submitter = config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
            };

            string api = "https://localhost:12011/api/v1/logins/TestLogin";
            var response = await client.PostAsJsonAsync(api, userLoginInfo);

            Message = await response.Content.ReadAsStringAsync();

        }

        public async Task OnPostLogout()
        {
            
            var client = new HttpClient();

            var userLoginInfo = new FoaeaLoginData();

            string api = "https://localhost:12011/api/v1/logins/TestLogout";
            var response = await client.PostAsJsonAsync(api, userLoginInfo);

            Message = await response.Content.ReadAsStringAsync();

        }

        public async Task OnPostValidate()
        {            
            var client = new HttpClient();

            var userLoginInfo = new FoaeaLoginData();

            string api = "https://localhost:12011/api/v1/logins/TestVerify";
            var response = await client.PostAsJsonAsync(api, userLoginInfo);

            if (response.IsSuccessStatusCode)
                Message = await response.Content.ReadAsStringAsync();
            else
                Message = "Failed with status code: " + response.StatusCode.ToString();

        }

    }
}
