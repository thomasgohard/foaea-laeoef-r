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

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("Applications API Version 1.0");

        [HttpGet("{id}")]
        public ActionResult<DataList<SINResultData>> GetApplication([FromRoute] string id, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(id);

            var appl = new ApplicationData();
            var applManager = new ApplicationManager(appl, repositories, config);

            if (applManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd))
                return Ok(appl);
            else
                return NotFound();
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

        [HttpPut("{key}/SinConfirmation")]
        public ActionResult<ApplicationData> SINconfirmation([FromRoute] string key,
                                                             [FromServices] IRepositories repositories,
                                                             [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var sinConfirmationData = APIBrokerHelper.GetDataFromRequestBody<SINConfirmationData>(Request);

            var application = new ApplicationData();

            var appManager = new ApplicationManager(application, repositories, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            ApplicationSINManager sinManager;

            switch (application.AppCtgy_Cd)
            {
                case "T01":
                    var tracingManager = new TracingManager(repositories, config);
                    tracingManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
                    sinManager = new ApplicationSINManager(tracingManager.TracingApplication, tracingManager);
                    break;
                case "I01":
                    var interceptionManager = new InterceptionManager(repositories, repositoriesFinance, config);
                    interceptionManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
                    sinManager = new ApplicationSINManager(interceptionManager.InterceptionApplication, interceptionManager);
                    break;
                case "L01":
                    var licenceDenialManager = new LicenceDenialManager(repositories, config);
                    licenceDenialManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
                    sinManager = new ApplicationSINManager(licenceDenialManager.LicenceDenialApplication, licenceDenialManager);
                    break;
                default:
                    sinManager = new ApplicationSINManager(application, appManager);
                    break;
            }

            sinManager.SINconfirmation(isSinConfirmed: sinConfirmationData.IsSinConfirmed,
                                       confirmedSin: sinConfirmationData.ConfirmedSIN,
                                       lastUpdateUser: repositories.CurrentSubmitter);

            return Ok(application);
        }
    }
}
