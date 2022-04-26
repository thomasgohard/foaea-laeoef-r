using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FOAEA3.API.LicenceDenial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutgoingProvincialLicenceDenialResultsController : ControllerBase
    {
        private readonly CustomConfig config;

        public OutgoingProvincialLicenceDenialResultsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("OutgoingProvincialLicenceDenialResults API Version 1.0");

        [HttpGet("")]
        public ActionResult<List<LicenceDenialOutgoingProvincialData>> GetProvincialOutgoingData(
                                                                [FromQuery] int maxRecords,
                                                                [FromQuery] string activeState,
                                                                [FromQuery] string recipientCode,
                                                                [FromQuery] bool isXML,
                                                                [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new LicenceDenialManager(repositories, config);

            var data = manager.GetProvincialOutgoingData(maxRecords, activeState, recipientCode, isXML);

            return Ok(data);
        }
    }
}
