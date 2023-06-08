using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Tracing.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TraceResponsesController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("TraceResponses API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{id}")]
    public async Task<ActionResult<DataList<TraceResponseData>>> GetTraceResults([FromRoute] ApplKey id,
                                                                     [FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config, User);

        if (await manager.LoadApplication(id.EnfSrv, id.CtrlCd))
            return Ok(await manager.GetTraceResults());
        else
            return NotFound();
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<int>> CreateTraceResponsesBulk([FromServices] IRepositories repositories)
    {
        var responseData = await APIBrokerHelper.GetDataFromRequestBody<List<TraceResponseData>>(Request);

        var tracingManager = new TracingManager(repositories, config, User);

        await tracingManager.CreateResponseData(responseData);

        var rootPath = "https://" + HttpContext.Request.Host.ToString();

        return Created(rootPath, new TraceResponseData());

    }

    [HttpPut("MarkResultsAsViewed")]
    public async Task<ActionResult<int>> MarkTraceResponsesAsViewed([FromServices] IRepositories repositories,
                                                                    [FromQuery] string enfService)
    {
        var tracingManager = new TracingManager(repositories, config, User);

        await tracingManager.MarkResponsesAsViewed(enfService);

        return Ok();
    }
}
