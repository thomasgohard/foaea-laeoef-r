using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.LicenceDenial.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OutgoingProvincialLicenceDenialResultsController : ControllerBase
{
    private readonly CustomConfig config;

    public OutgoingProvincialLicenceDenialResultsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("OutgoingProvincialLicenceDenialResults API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("")]
    public async Task<ActionResult<List<LicenceDenialOutgoingProvincialData>>> GetProvincialOutgoingData(
                                                            [FromQuery] int maxRecords,
                                                            [FromQuery] string activeState,
                                                            [FromQuery] string recipientCode,
                                                            [FromQuery] bool isXML,
                                                            [FromServices] IRepositories repositories)
    {
        var manager = new LicenceDenialManager(repositories, config);

        var data = await manager.GetProvincialOutgoingDataAsync(maxRecords, activeState, recipientCode, isXML);

        return Ok(data);
    }
}
