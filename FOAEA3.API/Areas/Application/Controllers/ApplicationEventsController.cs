using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Application.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ApplicationEventsController : ControllerBase
    {
        private readonly CustomConfig config;

        public ApplicationEventsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("ApplicationEvents API Version 1.0");

        [HttpGet("DB")]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [HttpGet("queues")]
        public ActionResult<Dictionary<int, string>> GetQueueNames()
        {
            var values = new Dictionary<int, string>();
            foreach (var g in Enum.GetValues(typeof(EventQueue)))
                values.Add((int)g, g?.ToString()?.Replace("Event", "Evnt"));

            return Ok(values);
        }

        [HttpGet("{id}")]
        public ActionResult<List<ApplicationEventData>> GetEvents([FromRoute] string id,
                                                                  [FromQuery] int? queue,
                                                                  [FromServices] IRepositories repositories)
        {
            EventQueue eventQueue;
            if (queue.HasValue)
                eventQueue = (EventQueue)queue.Value;
            else
                eventQueue = EventQueue.EventSubm;

            return GetEventsForQueue(id, repositories, eventQueue);
        }

        [HttpPost]
        public ActionResult<ApplicationEventData> SaveEvent([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applicationEvent = APIBrokerHelper.GetDataFromRequestBody<ApplicationEventData>(Request);

            var eventManager = new ApplicationEventManager(new ApplicationData(), repositories);

            eventManager.SaveEvent(applicationEvent);

            return Ok();

        }

        private ActionResult<List<ApplicationEventData>> GetEventsForQueue(string id, IRepositories repositories, EventQueue queue)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(id);

            var manager = new ApplicationManager(new ApplicationData(), repositories, config);

            if (manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd))
                return Ok(manager.EventManager.GetApplicationEventsForQueue(queue));
            else
                return NotFound();
        }

        [HttpGet("GetLatestSinEventDataSummary")]
        public ActionResult<List<SinInboundToApplData>> GetLatestSinEventDataSummary([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applManager = new ApplicationEventManager(new ApplicationData(), repositories);

            return applManager.GetLatestSinEventDataSummary();
        }

    }
}
