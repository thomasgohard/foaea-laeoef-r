using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{key}", Name = "GetApplication")]
    public async Task<ActionResult<LicenceDenialApplicationData>> GetApplication([FromRoute] ApplKey key,
                                                                     [FromServices] IRepositories repositories)
    {
        var manager = new LicenceDenialTerminationManager(repositories, config);
        await manager.SetCurrentUserAsync(User);

        bool success = await manager.LoadApplicationAsync(key.EnfSrv, key.CtrlCd);
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
                                                                        [FromQuery] string controlCodeForL01)
    {
        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<LicenceDenialApplicationData>(Request);
        var requestDate = DateTime.Now; // or should it be a different date?

        if (!APIHelper.ValidateApplication(application, applKey: null, out string error))
            return UnprocessableEntity(error);

        var licenceDenialTerminationManager = new LicenceDenialTerminationManager(application, repositories, config);
        await licenceDenialTerminationManager.SetCurrentUserAsync(User);

        bool isCreated = await licenceDenialTerminationManager.CreateApplicationAsync(controlCodeForL01, requestDate);
        if (isCreated)
        {
            var appKey = $"{application.Appl_EnfSrv_Cd}-{application.Appl_CtrlCd}";
            return CreatedAtRoute("GetApplication", new { key = appKey }, application);
        }
        else
        {
            return UnprocessableEntity(application);
        }

    }

    [HttpPut("{key}")]
    [Produces("application/json")]
    public async Task<ActionResult<LicenceDenialApplicationData>> UpdateApplication(
                                                [FromRoute] string key,
                                                [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var licenceDenialManager = new LicenceDenialTerminationManager(application, repositories, config);
        await licenceDenialManager.SetCurrentUserAsync(User);

        await licenceDenialManager.UpdateApplicationAsync();

        if (!application.Messages.ContainsMessagesOfType(MessageType.Error))
            return Ok(application);
        else
            return UnprocessableEntity(application);

    }

    [HttpPut("{key}/Transfer")]
    public async Task<ActionResult<LicenceDenialApplicationData>> Transfer([FromRoute] string key,
                                                 [FromServices] IRepositories repositories,
                                                 [FromQuery] string newRecipientSubmitter,
                                                 [FromQuery] string newIssuingSubmitter)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new LicenceDenialTerminationManager(application, repositories, config);
        await appManager.SetCurrentUserAsync(User);

        await appManager.TransferApplicationAsync(newIssuingSubmitter, newRecipientSubmitter);

        return Ok(application);
    }

    [HttpPut("{key}/cancel")]
    [Produces("application/json")]
    public async Task<ActionResult<InterceptionApplicationData>> CancelApplication([FromRoute] string key,
                                                                       [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var licenceDenialTerminationManager = new LicenceDenialTerminationManager(application, repositories, config);
        await licenceDenialTerminationManager.SetCurrentUserAsync(User);
        await licenceDenialTerminationManager.CancelApplication();

        if (!licenceDenialTerminationManager.LicenceDenialTerminationApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            return Ok(application);
        else
            return UnprocessableEntity(application);

    }


    [HttpPut("{key}/ProcessLicenceDenialTerminationResponse")]
    public async Task<ActionResult<LicenceDenialApplicationData>> ProcessLicenceDenialTerminationResponse(
                                                                        [FromRoute] ApplKey key,
                                                                        [FromServices] IRepositories repositories)
    {
        var application = new LicenceDenialApplicationData();

        var appManager = new LicenceDenialTerminationManager(application, repositories, config);
        await appManager.SetCurrentUserAsync(User);

        if (await appManager.ProcessLicenceDenialTerminationResponseAsync(key.EnfSrv, key.CtrlCd))
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

}
