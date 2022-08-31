using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OutgoingFederalLicenceDenialRequestsController : ControllerBase
{
    private readonly CustomConfig config;

    public OutgoingFederalLicenceDenialRequestsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("OutgoingFederalTracingRequests API Version 1.0");

    [HttpGet("")]
    public async Task<ActionResult<List<TracingOutgoingFederalData>>> GetFederalOutgoingData(
                                                            [FromQuery] int maxRecords,
                                                            [FromQuery] string activeState,
                                                            [FromQuery] int lifeState,
                                                            [FromQuery] string enfServiceCode,
                                                            [FromServices] IRepositories repositories)
    {
        var manager = new LicenceDenialManager(repositories, config);

        var data = await manager.GetFederalOutgoingDataAsync(maxRecords, activeState, (ApplicationState)lifeState,
                                                             enfServiceCode);

        return Ok(data);
    }
}
