using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications.Interception;

public class InterceptionNewModel : InterceptionFoaeaPageModel
{

    public InterceptionNewModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                     base(httpContextAccessor, apiConfig.Value)
    {
        if ((httpContextAccessor is not null) && (httpContextAccessor.HttpContext is not null))
        {
            string submitter = httpContextAccessor.HttpContext.Session.GetString(SessionValue.SUBMITTER);

            InterceptionApplication = new InterceptionApplicationData
            {
                Appl_EnfSrv_Cd = httpContextAccessor.HttpContext.Session.GetString(SessionValue.ENF_SERVICE),
                Subm_SubmCd = submitter,
                Appl_Dbtr_Addr_CtryCd = "CAN",
                Subm_Recpt_SubmCd = submitter,
                Medium_Cd = "ONL"
            };

            var submitterProfileApi = new SubmitterProfileAPIBroker(BaseAPIs);
            EnfServiceDescription = submitterProfileApi.GetSubmitterProfileAsync(submitter).Result.EnfSrv_Nme;

            LoadReferenceData();
        }
    }

    public async Task<IActionResult> OnPostSubmitNewApplication()
    {
        if (InterceptionApplication.IntFinH.IntFinH_NextRecalcDate_Cd == 0)
            InterceptionApplication.IntFinH.IntFinH_NextRecalcDate_Cd = null;

        if (InterceptionApplication.IntFinH.HldbCtg_Cd == "0")
            InterceptionApplication.IntFinH.HldbTyp_Cd = null;
        else if (InterceptionApplication.IntFinH.HldbCtg_Cd == "1")
            InterceptionApplication.IntFinH.HldbTyp_Cd = "T";
        else // "2"
            InterceptionApplication.IntFinH.HldbTyp_Cd = "P";

        if (InterceptionApplication.Appl_Dbtr_Brth_Dte == DateTime.MinValue)
            InterceptionApplication.Appl_Dbtr_Brth_Dte = null;

        if (!ModelState.IsValid)
        {
            ErrorMessage = GetValidationErrors();

            return Page();
        }

        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var newApplication = await interceptionApi.CreateInterceptionApplicationAsync(InterceptionApplication);

        SetDisplayMessages(newApplication.Messages);

        InterceptionApplication = newApplication;

        if (!InterceptionApplication.HldbCnd.Any())
            InterceptionApplication.HldbCnd.Add(new HoldbackConditionData());

        return RedirectPermanent(PageRoute.INTERCEPTION_DASHBOARD);
    }

}
