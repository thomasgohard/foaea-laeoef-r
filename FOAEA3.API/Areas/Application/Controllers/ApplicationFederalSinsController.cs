using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Application.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ApplicationFederalSinsController : ControllerBase
    {
        private readonly CustomConfig config;

        public ApplicationFederalSinsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("ApplicationFederalSins API Version 1.0");

        [HttpGet("DB")]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [HttpPost("bulk")]
        public ActionResult<int> CreateSinResultBulk([FromServices] IRepositories repositories)
        {
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
            var manager = new ApplicationEventManager(new ApplicationData(), repositories);

            return manager.GetRequestedSINEventDataForFile("HR01", fileName).Items;
        }

        [HttpGet("RequestedEventDetailsForFile")]
        public ActionResult<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile([FromQuery] string fileName,
                                                                                              [FromServices] IRepositories repositories)
        {
            var manager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

            return manager.GetRequestedSINEventDetailDataForFile("HR01", fileName).Items;
        }

    }
}
