using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
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
    public class ApplicationEventDetailsController : ControllerBase
    {
        private readonly CustomConfig config;

        public ApplicationEventDetailsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("ApplicationEventDetails API Version 1.0");

        [HttpGet("{id}/SIN")]
        public ActionResult<List<ApplicationEventDetailData>> GetSINEvents([FromRoute] string id, [FromServices] IRepositories repositories)
        {
            return GetEventsForQueue(id, repositories, EventQueue.EventSIN_dtl);
        }

        [HttpGet("{id}/Trace")]
        public ActionResult<List<ApplicationEventDetailData>> GetTraceEvents([FromRoute] string id, [FromServices] IRepositories repositories)
        {
            return GetEventsForQueue(id, repositories, EventQueue.EventTrace_dtl);
        }

        [HttpPost("")]
        public ActionResult<ApplicationEventDetailData> SaveEventDetail([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applicationEventDetail = APIBrokerHelper.GetDataFromRequestBody<ApplicationEventDetailData>(Request);

            var eventDetailManager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

            eventDetailManager.SaveEventDetail(applicationEventDetail);

            return Ok();

        }

        [HttpPut("")]
        public ActionResult<ApplicationEventDetailData> UpdateEventDetail([FromServices] IRepositories repositories,
                                                                          [FromQuery] string command,
                                                                          [FromQuery] string activeState,
                                                                          [FromQuery] string applicationState,
                                                                          [FromQuery] string enfSrvCode,
                                                                          [FromQuery] string writtenFile)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var eventIds = APIBrokerHelper.GetDataFromRequestBody<List<int>>(Request);

            var eventDetailManager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

            if (command?.ToLower() == "markoutboundprocessed")
            {
                eventDetailManager.UpdateOutboundEventDetail(activeState, applicationState, enfSrvCode, writtenFile, eventIds);
            }

            return Ok();

        }

        private ActionResult<List<ApplicationEventDetailData>> GetEventsForQueue(string id, IRepositories repositories, EventQueue queue)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(id);

            var manager = new ApplicationManager(new ApplicationData(), repositories, config);

            if (manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd))
                return Ok(manager.EventDetailManager.GetApplicationEventDetailsForQueue(queue));
            else
                return NotFound();
        }


    }
}
