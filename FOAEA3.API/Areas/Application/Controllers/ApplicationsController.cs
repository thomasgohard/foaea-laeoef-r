using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Areas.Application.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly CustomConfig config;

        public ApplicationsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("{id}/SINresults")]
        public ActionResult<DataList<SINResultData>> GetSINResults([FromRoute] string id, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(id);

            var appl = new ApplicationData();
            var applManager = new ApplicationManager(appl, repositories, config);
            var sinManager = new ApplicationSINManager(appl, applManager);

            if (applManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd))
                return Ok(sinManager.GetSINResults());
            else
                return NotFound();
        }

        [HttpGet("{id}/SINresultsWithHistory")]
        public ActionResult<DataList<SINResultWithHistoryData>> GetSINResultsWithHistory([FromRoute] string id, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(id);

            var appl = new ApplicationData();
            var applManager = new ApplicationManager(appl, repositories, config);
            var sinManager = new ApplicationSINManager(appl, applManager);

            if (applManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd))
                return Ok(sinManager.GetSINResultsWithHistory());
            else
                return NotFound();
        }

    }
}
