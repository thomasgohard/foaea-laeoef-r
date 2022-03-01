using FOAEA3.API.Areas.Application.Controllers;
using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FOAEA3.API.Tracing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TracingEventsController : ControllerBase
    {
        private readonly CustomConfig config;
        private readonly ApplicationEventsController appEventsController;

        public TracingEventsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;

            appEventsController = new ApplicationEventsController(config);

        }

        [HttpGet("queues")]
        public ActionResult<Dictionary<int, string>> GetQueueNames()
        {
            return appEventsController.GetQueueNames();
        }
        
        [HttpGet("{id}")]
        public ActionResult<List<ApplicationEventData>> GetEvents([FromRoute] string id, 
                                                                  [FromQuery] int? queue,
                                                                  [FromServices] IRepositories repositories)
        {
            return appEventsController.GetEvents(id, queue, repositories);
        }
                
        [HttpGet("RequestedTRCIN")]
        public ActionResult<ApplicationEventData> GetRequestedTRCINTracingEvents([FromQuery] string enforcementServiceCode,
                                                                                 [FromQuery] string fileCycle,
                                                                                 [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new TracingManager(repositories, config);

            if (string.IsNullOrEmpty(enforcementServiceCode))
                return BadRequest("Missing enforcementServiceCode parameter");

            if (string.IsNullOrEmpty(fileCycle))
                return BadRequest("Missing fileCycle parameter");

            var result = manager.GetRequestedTRCINTracingEvents(enforcementServiceCode, fileCycle);
            return Ok(result);

        }

        [HttpGet("Details/Active")]
        public ActionResult<ApplicationEventDetailData> GetActiveTracingEventDetails([FromQuery] string enforcementServiceCode,
                                                                                     [FromQuery] string fileCycle,
                                                                                     [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new TracingManager(repositories, config);

            var result = manager.GetActiveTracingEventDetails(enforcementServiceCode, fileCycle);

            return Ok(result);
        }

    }
}
