using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications
{
    public class ViewEventsModel : FoaeaPageModel
    {
        public ApplicationEventsList ApplicationEvents { get; set; }

        public ViewEventsModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                     base(httpContextAccessor, apiConfig.Value)
        {
            if ((httpContextAccessor is not null) && (httpContextAccessor.HttpContext is not null))
            {
                LoadReferenceData();
            }
        }

        public async Task OnGet([FromRoute] ApplKey id)
        {
            var eventsApi = new ApplicationEventAPIBroker(BaseAPIs);
            ApplicationEvents = await eventsApi.GetEvents(id.EnfSrv, id.CtrlCd);
        }
    }
}
