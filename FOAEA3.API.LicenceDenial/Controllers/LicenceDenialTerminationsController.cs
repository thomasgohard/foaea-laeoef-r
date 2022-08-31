using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LicenceDenialTerminationsController : ControllerBase
{
    private readonly CustomConfig config;

    public LicenceDenialTerminationsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialTerminations API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{key}")]
    public async Task<ActionResult<LicenceDenialApplicationData>> GetApplication([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var manager = new LicenceDenialTerminationManager(repositories, config);

        bool success = await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd);
        if (success)
        {
            if (manager.LicenceDenialTerminationApplication.AppCtgy_Cd == "L03")
                return Ok(manager.LicenceDenialTerminationApplication);
            else
                return NotFound($"Error: requested L03 application but found {manager.LicenceDenialTerminationApplication.AppCtgy_Cd} application.");
        }
        else
            return NotFound();

    }

    [HttpPost]
    public async Task<ActionResult<LicenceDenialApplicationData>> CreateApplication([FromServices] IRepositories repositories,
                                                                        [FromQuery] string controlCodeForL01,
                                                                        [FromQuery] DateTime requestDate)
    {
        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey: null, out string error))
            return UnprocessableEntity(error);

        var licenceDenialTerminationManager = new LicenceDenialTerminationManager(application, repositories, config);

        bool isCreated = await licenceDenialTerminationManager.CreateApplicationAsync(controlCodeForL01, requestDate);
        if (isCreated)
        {
            var appKey = $"{application.Appl_EnfSrv_Cd}-{application.Appl_CtrlCd}";
            var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + appKey;
            var getURI = new Uri("http://" + HttpContext.Request.Host.ToString() + actionPath);

            return Created(getURI, application);
        }
        else
        {
            return UnprocessableEntity(application);
        }

    }

    [HttpPut("{key}/ProcessLicenceDenialTerminationResponse")]
    public async Task<ActionResult<LicenceDenialApplicationData>> ProcessLicenceDenialTerminationResponse([FromRoute] string key,
                                                                                              [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = new LicenceDenialApplicationData();

        var appManager = new LicenceDenialTerminationManager(application, repositories, config);
        if (await appManager.ProcessLicenceDenialTerminationResponseAsync(applKey.EnfSrv, applKey.CtrlCd))
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

}
