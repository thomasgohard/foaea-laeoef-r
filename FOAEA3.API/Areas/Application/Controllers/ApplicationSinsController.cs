using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Application.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ApplicationSinsController : ControllerBase
    {
        private readonly CustomConfig config;

        public ApplicationSinsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpPost("bulk")]
        public ActionResult<int> CreateSinResultBulk([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var responseData = APIBrokerHelper.GetDataFromRequestBody<List<SINResultData>>(Request);

            var application = new ApplicationData();

            var applManager = new ApplicationManager(application, repositories, config);
            var sinManager = new ApplicationSINManager(application, applManager);

            sinManager.CreateResultData(responseData);

            var rootPath = "http://" + HttpContext.Request.Host.ToString();

            return Created(rootPath, new SINResultData());
        }

        [HttpGet("RequestedEventsForFile")]
        public ActionResult<List<ApplicationEventData>> GetRequestedSINEventDataForFile([FromQuery] string fileName,
                                                                                        [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new ApplicationEventManager(new ApplicationData(), repositories);

            return manager.GetRequestedSINEventDataForFile("HR01", fileName).Items;
        }

        [HttpGet("RequestedEventDetailsForFile")]
        public ActionResult<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile([FromQuery] string fileName,
                                                                                              [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

            return manager.GetRequestedSINEventDetailDataForFile("HR01", fileName).Items;
        }

        [HttpPut("{key}/SinConfirmation")]
        public ActionResult<ApplicationData> SINconfirmation([FromRoute] string key,
                                                             [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var sinConfirmationData = APIBrokerHelper.GetDataFromRequestBody<SINConfirmationData>(Request);

            var application = new ApplicationData();

            var appManager = new ApplicationManager(application, repositories, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            var sinManager = new ApplicationSINManager(application, appManager);
            sinManager.SINconfirmation(isSinConfirmed: sinConfirmationData.IsSinConfirmed, 
                                       confirmedSin: sinConfirmationData.ConfirmedSIN,
                                       lastUpdateUser: repositories.CurrentSubmitter);

            return Ok(application);
        }
    }
}
