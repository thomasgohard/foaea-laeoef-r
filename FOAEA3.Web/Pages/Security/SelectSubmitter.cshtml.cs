using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Security;

public class SelectSubmitterModel : FoaeaPageModel
{
    public const string AvailableSubmitters = "AvailableSubmitters";

    public SelectSubmitterModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                          base(httpContextAccessor, apiConfig.Value)
    {
    }

    [BindProperty]
    public string Submitter { get; set; }
    public string Message { get; set; }

    public List<string> Submitters { get; set; }

    public async Task OnGet()
    {
        var loginAPIs = new LoginsAPIBroker(BaseAPIs);

        Submitters = await loginAPIs.GetAvailableSubmittersAsync();
    }

    public async Task<ActionResult> OnPostSelectSubmitter()
    {
        var loginAPIs = new LoginsAPIBroker(BaseAPIs);

        var result = await loginAPIs.SelectSubmitterAsync(Submitter);

        if ((result.Token == null) || string.IsNullOrEmpty(result.Token))
        {
            Message = "Error: Could not select submitter";
            return null;
        }
        else
        {
            var submitterAPIs = new SubmitterAPIBroker(BaseAPIs);
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
