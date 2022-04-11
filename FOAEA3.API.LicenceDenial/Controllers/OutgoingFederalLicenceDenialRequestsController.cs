using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FOAEA3.API.LicenceDenial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OutgoingFederalLicenceDenialRequestsController : ControllerBase
    {
        private readonly CustomConfig config;

        public OutgoingFederalLicenceDenialRequestsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("OutgoingFederalTracingRequests API Version 1.0");

        [HttpGet("")]
        public ActionResult<List<TracingOutgoingFederalData>> GetFederalOutgoingData(
                                                                [FromQuery] int maxRecords,
                                                                [FromQuery] string activeState,
                                                                [FromQuery] int lifeState,
                                                                [FromQuery] string enfServiceCode,
                                                                [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new LicenceDenialManager(repositories, config);

            var data = manager.GetFederalOutgoingData(maxRecords, activeState, (ApplicationState)lifeState,
                                                      enfServiceCode);

            return Ok(data);
        }
    }
}
