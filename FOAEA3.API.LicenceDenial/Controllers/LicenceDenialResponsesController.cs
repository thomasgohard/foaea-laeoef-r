using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FOAEA3.API.LicenceDenial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenceDenialResponsesController : ControllerBase
    {
        private readonly CustomConfig config;

        public LicenceDenialResponsesController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("LicenceDenialResponses API Version 1.0");

        //[HttpGet("{id}")]
        //public ActionResult<DataList<LicenceDenialResponseData>> GetLicenceDenialResults([FromRoute] string id,
        //                                                                                 [FromServices] IRepositories repositories)
        //{
        //    APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
        //    APIHelper.PrepareResponseHeaders(Response.Headers);

        //    var applKey = new ApplKey(id);

        //    var manager = new LicenceDenialManager(repositories, config);

        //    if (manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd))
        //        return Ok(manager.GetLicenceDenialResults());
        //    else
        //        return NotFound();
        //}

        [HttpPost("bulk")]
        public ActionResult<int> CreateLicenceDenialResponsesBulk([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var responseData = APIBrokerHelper.GetDataFromRequestBody<List<LicenceDenialResponseData>>(Request);

            var licenceDenialManager = new LicenceDenialManager(repositories, config);

            licenceDenialManager.CreateResponseData(responseData);

            var rootPath = "http://" + HttpContext.Request.Host.ToString();

            return Created(rootPath, new TraceResponseData());

        }

        [HttpPut("MarkResultsAsViewed")]
        public ActionResult<int> MarkLicenceDenialResponsesAsViewed([FromServices] IRepositories repositories,
                                                                    [FromQuery] string enfService)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var licenceDenialManager = new LicenceDenialManager(repositories, config);

            licenceDenialManager.MarkResponsesAsViewed(enfService);

            return Ok();
        }
    }
}
