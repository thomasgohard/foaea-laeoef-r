using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OutgoingProvincialTracingResultsController : ControllerBase
{
    private readonly CustomConfig config;

    public OutgoingProvincialTracingResultsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("OutgoingProvincialTracingResults API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("")]
    public async Task<ActionResult<List<TracingOutgoingProvincialData>>> GetProvincialOutgoingData(
                                                            [FromQuery] int maxRecords,
                                                            [FromQuery] string activeState,
                                                            [FromQuery] string recipientCode,
                                                            [FromQuery] bool isXML,
                                                            [FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config);

        var data = await manager.GetProvincialOutgoingDataAsync(maxRecords, activeState, recipientCode, isXML);

        return Ok(data);
    }
}
