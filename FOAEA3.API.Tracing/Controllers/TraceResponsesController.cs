using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Tracing.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TraceResponsesController : ControllerBase
{
    private readonly CustomConfig config;

    public TraceResponsesController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("TraceResponses API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{id}")]
    public async Task<ActionResult<DataList<TraceResponseData>>> GetTraceResults([FromRoute] string id,
                                                                     [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(id);

        var manager = new TracingManager(repositories, config);

        if (await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd))
            return Ok(await manager.GetTraceResultsAsync());
        else
            return NotFound();
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<int>> CreateTraceResponsesBulk([FromServices] IRepositories repositories)
    {
        var responseData = await APIBrokerHelper.GetDataFromRequestBodyAsync<List<TraceResponseData>>(Request);

        var tracingManager = new TracingManager(repositories, config);

        await tracingManager.CreateResponseDataAsync(responseData);

        var rootPath = "http://" + HttpContext.Request.Host.ToString();

        return Created(rootPath, new TraceResponseData());

    }

    [HttpPut("MarkResultsAsViewed")]
    public async Task<ActionResult<int>> MarkTraceResponsesAsViewed([FromServices] IRepositories repositories,
                                                                    [FromQuery] string enfService)
    {
        var tracingManager = new TracingManager(repositories, config);

        await tracingManager.MarkResponsesAsViewedAsync(enfService);

        return Ok();
    }
}
