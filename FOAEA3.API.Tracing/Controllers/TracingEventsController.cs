using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Tracing.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TracingEventsController : ControllerBase
{
    private readonly CustomConfig config;

    public TracingEventsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("TracingEvents API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
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
    public async Task<ActionResult<List<ApplicationEventData>>> GetEvents([FromRoute] ApplKey id,
                                                              [FromQuery] int? queue,
                                                              [FromServices] IRepositories repositories)
    {
        EventQueue eventQueue;
        if (queue.HasValue)
            eventQueue = (EventQueue)queue.Value;
        else
            eventQueue = EventQueue.EventSubm;

        return await GetEventsForQueueAsync(id, repositories, eventQueue);
    }

    [HttpGet("RequestedTRCIN")]
    public async Task<ActionResult<ApplicationEventData>> GetRequestedTRCINTracingEvents([FromQuery] string enforcementServiceCode,
                                                                             [FromQuery] string fileCycle,
                                                                             [FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config);

        if (string.IsNullOrEmpty(enforcementServiceCode))
            return BadRequest("Missing enforcementServiceCode parameter");

        if (string.IsNullOrEmpty(fileCycle))
            return BadRequest("Missing fileCycle parameter");

        var result = await manager.GetRequestedTRCINTracingEventsAsync(enforcementServiceCode, fileCycle);
        return Ok(result);

    }

    [HttpGet("Details/Active")]
    public async Task<ActionResult<ApplicationEventDetailData>> GetActiveTracingEventDetails([FromQuery] string enforcementServiceCode,
                                                                                 [FromQuery] string fileCycle,
                                                                                 [FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config);

        var result = await manager.GetActiveTracingEventDetailsAsync(enforcementServiceCode, fileCycle);

        return Ok(result);
    }

    private async Task<ActionResult<List<ApplicationEventData>>> GetEventsForQueueAsync(ApplKey id, IRepositories repositories, EventQueue queue)
    {
        var manager = new ApplicationManager(new ApplicationData(), repositories, config);

        if (await manager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd))
            return Ok(manager.EventManager.GetApplicationEventsForQueueAsync(queue));
        else
            return NotFound();
    }
}
