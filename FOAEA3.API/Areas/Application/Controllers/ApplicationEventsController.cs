using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Areas.Application.Controllers;

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
    public async Task<ActionResult<List<ApplicationEventData>>> GetEvents([FromRoute] string id,
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

    [HttpPost]
    public async Task<ActionResult<ApplicationEventData>> SaveEvent([FromServices] IRepositories repositories)
    {
        var applicationEvent = await APIBrokerHelper.GetDataFromRequestBodyAsync<ApplicationEventData>(Request);

        var eventManager = new ApplicationEventManager(new ApplicationData(), repositories);

        await eventManager.SaveEventAsync(applicationEvent);

        return Ok();

    }

    private async Task<ActionResult<List<ApplicationEventData>>> GetEventsForQueueAsync(string id, IRepositories repositories, EventQueue queue)
    {
        var applKey = new ApplKey(id);

        var manager = new ApplicationManager(new ApplicationData(), repositories, config);

        if (await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd))
            return Ok(await manager.EventManager.GetApplicationEventsForQueueAsync(queue));
        else
            return NotFound();
    }

    [HttpGet("GetLatestSinEventDataSummary")]
    public async Task<ActionResult<List<SinInboundToApplData>>> GetLatestSinEventDataSummary([FromServices] IRepositories repositories)
    {
        var applManager = new ApplicationEventManager(new ApplicationData(), repositories);

        return await applManager.GetLatestSinEventDataSummaryAsync();
    }

}
