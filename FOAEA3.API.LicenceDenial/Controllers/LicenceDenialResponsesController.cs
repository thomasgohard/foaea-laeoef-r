using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.LicenceDenial.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LicenceDenialResponsesController : ControllerBase
{
    private readonly CustomConfig config;

    public LicenceDenialResponsesController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialResponses API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpPost("bulk")]
    public async Task<ActionResult<int>> CreateLicenceDenialResponsesBulk([FromServices] IRepositories repositories)
    {
        var responseData = await APIBrokerHelper.GetDataFromRequestBodyAsync<List<LicenceDenialResponseData>>(Request);

        var licenceDenialManager = new LicenceDenialManager(repositories, config);

        licenceDenialManager.CreateResponseData(responseData);

        var rootPath = "http://" + HttpContext.Request.Host.ToString();

        return Created(rootPath, new TraceResponseData());

    }

    [HttpPut("MarkResultsAsViewed")]
    public ActionResult<int> MarkLicenceDenialResponsesAsViewed([FromServices] IRepositories repositories,
                                                                [FromQuery] string enfService)
    {
        var licenceDenialManager = new LicenceDenialManager(repositories, config);

        licenceDenialManager.MarkResponsesAsViewed(enfService);

        return Ok();
    }
}
