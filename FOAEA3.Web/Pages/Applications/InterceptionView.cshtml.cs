using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FOAEA3.Web.Pages.Applications
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
                EnfServiceDescription = submitterProfileApi.GetSubmitterProfileAsync(submitter).Result.EnfSrv_Nme;

                LoadReferenceData();
            }
        }
    }
}
