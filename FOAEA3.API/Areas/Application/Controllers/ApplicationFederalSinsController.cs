using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Areas.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationFederalSinsController : ControllerBase
{
    private readonly CustomConfig config;

    public ApplicationFederalSinsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ApplicationFederalSins API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpPost("bulk")]
    public async Task<ActionResult<int>> CreateSinResultBulk([FromServices] IRepositories repositories)
    {
        var responseData = await APIBrokerHelper.GetDataFromRequestBodyAsync<List<SINResultData>>(Request);

        var application = new ApplicationData();

        var applManager = new ApplicationManager(application, repositories, config);
        var sinManager = new ApplicationSINManager(application, applManager);

        await sinManager.CreateResultDataAsync(responseData);

        var rootPath = "https://" + HttpContext.Request.Host.ToString();

        return Created(rootPath, new SINResultData());
    }

    [HttpGet("RequestedEventsForFile")]
    public async Task<ActionResult<List<ApplicationEventData>>> GetRequestedSINEventDataForFile([FromQuery] string fileName,
                                                                                    [FromServices] IRepositories repositories)
    {
        var manager = new ApplicationEventManager(new ApplicationData(), repositories);

        return (await manager.GetRequestedSINEventDataForFileAsync("HR01", fileName)).Items;
    }

    [HttpGet("RequestedEventDetailsForFile")]
    public async Task<ActionResult<List<ApplicationEventDetailData>>> GetRequestedSINEventDetailDataForFile([FromQuery] string fileName,
                                                                                          [FromServices] IRepositories repositories)
    {
        var manager = new ApplicationEventDetailManager(new ApplicationData(), repositories);

        return (await manager.GetRequestedSINEventDetailDataForFileAsync("HR01", fileName)).Items;
    }

}
