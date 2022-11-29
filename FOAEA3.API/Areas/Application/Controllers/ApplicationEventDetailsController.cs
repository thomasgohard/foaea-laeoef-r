using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationEventDetailsController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ApplicationEventDetails API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{id}/SIN")]
    public async Task<ActionResult<List<ApplicationEventDetailData>>> GetSINEvents([FromRoute] string id, [FromServices] IRepositories repositories)
    {
        return await GetEventsForQueueAsync(id, repositories, EventQueue.EventSIN_dtl);
    }

    [HttpGet("{id}/Trace")]
    public async Task<ActionResult<List<ApplicationEventDetailData>>> GetTraceEvents([FromRoute] string id, [FromServices] IRepositories repositories)
    {
        return await GetEventsForQueueAsync(id, repositories, EventQueue.EventTrace_dtl);
    }

    [HttpPost("")]
    public async Task<ActionResult<ApplicationEventDetailData>> SaveEventDetail([FromServices] IRepositories repositories)
    {
        var applicationEventDetail = await APIBrokerHelper.GetDataFromRequestBodyAsync<ApplicationEventDetailData>(Request);

        var eventDetailManager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

        await eventDetailManager.SaveEventDetailAsync(applicationEventDetail);

        return Ok();

    }

    [HttpPut("")]
    public async Task<ActionResult<ApplicationEventDetailData>> UpdateEventDetail([FromServices] IRepositories repositories,
                                                                      [FromQuery] string command,
                                                                      [FromQuery] string activeState,
                                                                      [FromQuery] string applicationState,
                                                                      [FromQuery] string enfSrvCode,
                                                                      [FromQuery] string writtenFile)
    {
        var eventIds = await APIBrokerHelper.GetDataFromRequestBodyAsync<List<int>>(Request);

        var eventDetailManager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

        if (command?.ToLower() == "markoutboundprocessed")
        {
            await eventDetailManager.UpdateOutboundEventDetailAsync(activeState, applicationState, enfSrvCode, writtenFile, eventIds);
        }

        return Ok();

    }

    private async Task<ActionResult<List<ApplicationEventDetailData>>> GetEventsForQueueAsync(string id, IRepositories repositories, EventQueue queue)
    {
        var applKey = new ApplKey(id);

        var manager = new ApplicationManager(new ApplicationData(), repositories, config);
        await manager.SetCurrentUserAsync(User);

        if (await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd))
            return Ok(await manager.EventDetailManager.GetApplicationEventDetailsForQueueAsync(queue));
        else
            return NotFound();
    }


}
