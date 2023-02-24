using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using System.Linq;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Web.Helpers;
using FOAEA3.Common.Helpers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FOAEA3.Web.Pages.Applications
{
    public class InterceptionEditModel : InterceptionFoaeaPageModel
    {
        private InterceptionFinancialHoldbackData IntFinHOriginal { get; set; }
        private List<HoldbackConditionData> HldbCndOriginal { get; set; }

        public InterceptionEditModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
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

        public async Task OnGet([FromRoute] ApplKey id)
        {
            var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
            var application = await interceptionApi.GetApplicationAsync(id.EnfSrv, id.CtrlCd);
            if ((application != null) && (application.Appl_EnfSrv_Cd.Trim() == id.EnfSrv) &&
                                         (application.Appl_CtrlCd.Trim() == id.CtrlCd))
            {
                InterceptionApplication = application;
            }

            // make deep copy of IntFinH and HldbCnd to see if they have changed (in which case, they are doing a variation)
            IntFinHOriginal = new InterceptionFinancialHoldbackData
            {
                Appl_EnfSrv_Cd = InterceptionApplication.IntFinH.Appl_EnfSrv_Cd,
                Appl_CtrlCd = InterceptionApplication.IntFinH.Appl_CtrlCd,
                IntFinH_Dte = InterceptionApplication.IntFinH.IntFinH_Dte,
                IntFinH_RcvtAffdvt_Dte = InterceptionApplication.IntFinH.IntFinH_RcvtAffdvt_Dte,
                IntFinH_Affdvt_SubmCd = InterceptionApplication.IntFinH.IntFinH_Affdvt_SubmCd,
                PymPr_Cd = InterceptionApplication.IntFinH.PymPr_Cd,
                IntFinH_NextRecalcDate_Cd = InterceptionApplication.IntFinH.IntFinH_NextRecalcDate_Cd,
                HldbTyp_Cd = InterceptionApplication.IntFinH.HldbTyp_Cd,
                IntFinH_DefHldbAmn_Money = InterceptionApplication.IntFinH.IntFinH_DefHldbAmn_Money,
                IntFinH_DefHldbPrcnt = InterceptionApplication.IntFinH.IntFinH_DefHldbPrcnt,
                HldbCtg_Cd = InterceptionApplication.IntFinH.HldbCtg_Cd,
                IntFinH_CmlPrPym_Ind = InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind,
                IntFinH_MxmTtl_Money = InterceptionApplication.IntFinH.IntFinH_MxmTtl_Money,
                IntFinH_PerPym_Money = InterceptionApplication.IntFinH.IntFinH_PerPym_Money,
                IntFinH_LmpSum_Money = InterceptionApplication.IntFinH.IntFinH_LmpSum_Money,
                IntFinH_TtlAmn_Money = InterceptionApplication.IntFinH.IntFinH_TtlAmn_Money,
                IntFinH_VarIss_Dte = InterceptionApplication.IntFinH.IntFinH_VarIss_Dte,
                IntFinH_CreateUsr = InterceptionApplication.IntFinH.IntFinH_CreateUsr,
                IntFinH_LiStCd = InterceptionApplication.IntFinH.IntFinH_LiStCd,
                ActvSt_Cd = InterceptionApplication.IntFinH.ActvSt_Cd,
                IntFinH_DefHldbAmn_Period = InterceptionApplication.IntFinH.IntFinH_DefHldbAmn_Period
            };

            HldbCndOriginal = new List<HoldbackConditionData>();
            foreach (var holdback in InterceptionApplication.HldbCnd)
            {
                HldbCndOriginal.Add(new HoldbackConditionData
                {
                    Appl_EnfSrv_Cd = holdback.Appl_EnfSrv_Cd,
                    Appl_CtrlCd = holdback.Appl_CtrlCd,
                    IntFinH_Dte = holdback.IntFinH_Dte,
                    EnfSrv_Cd = holdback.EnfSrv_Cd,
                    HldbCnd_MxmPerChq_Money = holdback.HldbCnd_MxmPerChq_Money,
                    HldbCnd_SrcHldbAmn_Money = holdback.HldbCnd_SrcHldbAmn_Money,
                    HldbCnd_SrcHldbPrcnt = holdback.HldbCnd_SrcHldbPrcnt,
                    HldbCnd_LiStCd = holdback.HldbCnd_LiStCd,
                    HldbCtg_Cd = holdback.HldbCtg_Cd,
                    ActvSt_Cd = holdback.ActvSt_Cd
                });
            }
        }

        public async Task<IActionResult> OnPostSubmitEditApplication()
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

            if (FinancialTermsModified())
            {
                var variationIssueDate = DateTime.Now;
                InterceptionApplication.Appl_Lgl_Dte = variationIssueDate;
            }

            var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
            var newApplication = await interceptionApi.UpdateInterceptionApplicationAsync(InterceptionApplication);

            SetDisplayMessages(newApplication.Messages);

            InterceptionApplication = newApplication;

            if (!InterceptionApplication.HldbCnd.Any())
                InterceptionApplication.HldbCnd.Add(new HoldbackConditionData());

            return RedirectToPagePermanent("InterceptionDashboard");
        }

        private bool FinancialTermsModified()
        {
            if (!InterceptionApplication.IntFinH.ValuesEqual(IntFinHOriginal))
                return true;

            if (InterceptionApplication.HldbCnd.Count != HldbCndOriginal.Count)
                return true;

            foreach (var holdback in InterceptionApplication.HldbCnd)
            {
                var originalHoldback = HldbCndOriginal.Where(m => m.ValuesEqual(holdback));
                if (!originalHoldback.Any())
                    return true;
            }

            return false;
        }

    }
}
