using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications.Interception
{
    public class InterceptionViewModel : InterceptionFoaeaPageModel
    {
        public InterceptionViewModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
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
                EnfServiceDescription = submitterProfileApi.GetSubmitterProfile(submitter).Result.EnfSrv_Nme;

                LoadReferenceData();
            }
        }

        public async Task OnGet([FromRoute] ApplKey id)
        {
            var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
            var application = await interceptionApi.GetApplication(id.EnfSrv, id.CtrlCd);
            if ((application != null) && (application.Appl_EnfSrv_Cd.Trim() == id.EnfSrv) &&
                                         (application.Appl_CtrlCd.Trim() == id.CtrlCd))
            {
                InterceptionApplication = application;
            }
        }
    }
}
