using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TracingEventDetailsController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("TracingEventDetails API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{id}/SIN")]
    public async Task<ActionResult<ApplicationEventDetailsList>> GetSINEvents([FromRoute] ApplKey id, [FromServices] IRepositories repositories)
    {
        return await GetEventsForQueue(id, repositories, EventQueue.EventSIN_dtl);
    }

    [HttpGet("{id}/Trace")]
    public async Task<ActionResult<ApplicationEventDetailsList>> GetTraceEvents([FromRoute] ApplKey id, [FromServices] IRepositories repositories)
    {
        //var result = await GetEventsForQueue(id, repositories, EventQueue.EventTrace_dtl);
        //return result;
        var manager = new ApplicationManager(new ApplicationData(), repositories, config, User);

        if (await manager.LoadApplication(id.EnfSrv, id.CtrlCd))
        {
            var result = await manager.EventDetailManager.GetApplicationEventDetailsForQueue(EventQueue.EventTrace_dtl);
            return Ok(result);
        }
        else
            return NotFound();
    }

    [HttpPost("")]
    public async Task<ActionResult<ApplicationEventDetailData>> SaveEventDetail([FromServices] IRepositories repositories)
    {
        var applicationEventDetail = await APIBrokerHelper.GetDataFromRequestBody<ApplicationEventDetailData>(Request);

        var eventDetailManager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

        await eventDetailManager.SaveEventDetail(applicationEventDetail);

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
        var eventIds = await APIBrokerHelper.GetDataFromRequestBody<List<int>>(Request);

        var eventDetailManager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

        if (command?.ToLower() == "markoutboundprocessed")
        {
            await eventDetailManager.UpdateOutboundEventDetail(activeState, applicationState, enfSrvCode, writtenFile, eventIds);
        }

        return Ok();

    }

    private async Task<ActionResult<ApplicationEventDetailsList>> GetEventsForQueue(ApplKey id,
                                                                IRepositories repositories, EventQueue queue)
    {
        var manager = new ApplicationManager(new ApplicationData(), repositories, config, User);

        if (await manager.LoadApplication(id.EnfSrv, id.CtrlCd))
        {
            var result = manager.EventDetailManager.GetApplicationEventDetailsForQueue(queue);
            return Ok(result);
        }
        else
            return NotFound();
    }
}
