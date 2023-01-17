using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Security;

public class LoginModel : FoaeaPageModel
{
    public string Message { get; set; }

    [BindProperty]
    public FoaeaLoginData LoginData { get; set; }

    public LoginModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                          base(httpContextAccessor, apiConfig.Value)
    {
    }

    public void OnGet()
    {
    }

    public async Task<ActionResult> OnPostLogin()
    {
        var loginAPIs = new LoginsAPIBroker(APIs);

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
