using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LicenceDenialsController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialsFiles API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{key}", Name = "GetLicenceDenial")]
    public async Task<ActionResult<LicenceDenialApplicationData>> GetApplication([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var manager = new LicenceDenialManager(repositories, config);
        await manager.SetCurrentUserAsync(User);

        bool success = await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd);
        if (success)
        {
            if (manager.LicenceDenialApplication.AppCtgy_Cd == "L01")
                return Ok(manager.LicenceDenialApplication);
            else
                return NotFound($"Error: requested L01 application but found {manager.LicenceDenialApplication.AppCtgy_Cd} application.");
        }
        else
            return NotFound();

    }

    [HttpGet("{key}/LicenceSuspensionHistory")]
    public async Task<ActionResult<List<LicenceSuspensionHistoryData>>> GetLicenceSuspensionHistory([FromRoute] string key,
                                                                                        [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var manager = new LicenceDenialManager(repositories, config);
        await manager.SetCurrentUserAsync(User);

        bool success = await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd);
        if (success)
        {
            if (manager.LicenceDenialApplication.AppCtgy_Cd == "L01")
            {
                var suspensionHistory = manager.GetLicenceSuspensionHistoryAsync();

                return Ok(suspensionHistory);
            }
            else
                return NotFound($"Error: requested L01 application but found {manager.LicenceDenialApplication.AppCtgy_Cd} application.");
        }
        else
            return NotFound();

    }

    [HttpPost]
    public async Task<ActionResult<LicenceDenialApplicationData>> CreateApplication([FromServices] IRepositories db)
    {
        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey: null, out string error))
            return UnprocessableEntity(error);

        var licenceDenialManager = new LicenceDenialManager(application, db, config);
        await licenceDenialManager.SetCurrentUserAsync(User);
        var submitter = (await db.SubmitterTable.GetSubmitterAsync(application.Subm_SubmCd)).FirstOrDefault();
        if (submitter is not null)
        {
            licenceDenialManager.CurrentUser.Submitter = submitter;
            db.CurrentSubmitter = submitter.Subm_SubmCd;
        }

        bool isCreated = await licenceDenialManager.CreateApplicationAsync();
        if (isCreated)
        {
            var appKey = $"{application.Appl_EnfSrv_Cd}-{application.Appl_CtrlCd}";

            return CreatedAtRoute("GetLicenceDenial", new { key = appKey }, application);
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

        var licenceDenialManager = new LicenceDenialManager(application, repositories, config);
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

        var appManager = new LicenceDenialManager(application, repositories, config);
        await appManager.SetCurrentUserAsync(User);

        await appManager.TransferApplicationAsync(newIssuingSubmitter, newRecipientSubmitter);

        return Ok(application);
    }

    [HttpPut("{key}/SINbypass")]
    public async Task<ActionResult<LicenceDenialApplicationData>> SINbypass([FromRoute] string key,
                                                     [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var sinBypassData = await APIBrokerHelper.GetDataFromRequestBodyAsync<SINBypassData>(Request);

        var application = new LicenceDenialApplicationData();

        var appManager = new LicenceDenialManager(application, repositories, config);
        await appManager.SetCurrentUserAsync(User);

        await appManager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd);

        if (!APIHelper.ValidateApplication(appManager.LicenceDenialApplication, applKey, out string error))
            return UnprocessableEntity(error);

        var sinManager = new ApplicationSINManager(application, appManager);
        await sinManager.SINconfirmationBypassAsync(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

        return Ok(application);
    }

    [HttpPut("{key}/ProcessLicenceDenialResponse")]
    public async Task<ActionResult<LicenceDenialApplicationData>> ProcessLicenceDenialResponse([FromRoute] string key,
                                                                                   [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = new LicenceDenialApplicationData();

        var appManager = new LicenceDenialManager(application, repositories, config);
        await appManager.SetCurrentUserAsync(User);

        if (await appManager.ProcessLicenceDenialResponseAsync(applKey.EnfSrv, applKey.CtrlCd))
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

    [HttpGet("LicenceDenialToApplication")]
    public async Task<ActionResult<List<TraceCycleQuantityData>>> GetLicenceDenialToApplData([FromQuery] string federalSource,
                                                            [FromServices] IRepositories repositories)
    {
        var manager = new LicenceDenialManager(repositories, config);
        await manager.SetCurrentUserAsync(User);

        var data = await manager.GetLicenceDenialToApplDataAsync(federalSource);

        return Ok(data);

    }
}
