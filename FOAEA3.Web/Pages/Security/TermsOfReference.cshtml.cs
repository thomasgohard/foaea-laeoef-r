using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Security;

public class TermsOfReferenceModel : FoaeaPageModel
{
    [BindProperty]
    public string Message { get; set; }

    public TermsOfReferenceModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                          base(httpContextAccessor, apiConfig.Value)
    {
    }

    public void OnGet()
    {
    }

    public async Task<ActionResult> OnPostAccept()
    {
        var loginAPIs = new LoginsAPIBroker(APIs);

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
