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
    [AllowAnonymous]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialsFiles API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{key}", Name = "GetLicenceDenial")]
    public async Task<ActionResult<LicenceDenialApplicationData>> GetApplication([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var manager = new LicenceDenialManager(repositories, config, User);

        bool success = await manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
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

        var manager = new LicenceDenialManager(repositories, config, User);

        bool success = await manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
        if (success)
        {
            if (manager.LicenceDenialApplication.AppCtgy_Cd == "L01")
            {
                var suspensionHistory = manager.GetLicenceSuspensionHistory();

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
        var application = await APIBrokerHelper.GetDataFromRequestBody<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey: null, out string error))
            return UnprocessableEntity(error);

        var licenceDenialManager = new LicenceDenialManager(application, db, config, User);
        var submitter = (await db.SubmitterTable.GetSubmitter(application.Subm_SubmCd)).FirstOrDefault();
        if (submitter is not null)
        {
            licenceDenialManager.CurrentUser.Submitter = submitter;
            db.CurrentSubmitter = submitter.Subm_SubmCd;
        }

        bool isCreated = await licenceDenialManager.CreateApplication();
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

        var application = await APIBrokerHelper.GetDataFromRequestBody<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey, out string error))
            return UnprocessableEntity(error);

        var licenceDenialManager = new LicenceDenialManager(application, repositories, config, User);
        await licenceDenialManager.UpdateApplication();

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

        var application = await APIBrokerHelper.GetDataFromRequestBody<LicenceDenialApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new LicenceDenialManager(application, repositories, config, User);

        await appManager.TransferApplication(newIssuingSubmitter, newRecipientSubmitter);

        return Ok(application);
    }

    [HttpPut("{key}/SINbypass")]
    public async Task<ActionResult<LicenceDenialApplicationData>> SINbypass([FromRoute] string key,
                                                     [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var sinBypassData = await APIBrokerHelper.GetDataFromRequestBody<SINBypassData>(Request);

        var application = new LicenceDenialApplicationData();

        var appManager = new LicenceDenialManager(application, repositories, config, User);

        await appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

        if (!APIHelper.ValidateRequest(appManager.LicenceDenialApplication, applKey, out string error))
            return UnprocessableEntity(error);

        var sinManager = new ApplicationSINManager(application, appManager);
        await sinManager.SINconfirmationBypass(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

        return Ok(application);
    }

    [HttpPut("{key}/ProcessLicenceDenialResponse")]
    public async Task<ActionResult<LicenceDenialApplicationData>> ProcessLicenceDenialResponse([FromRoute] string key,
                                                                                   [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = new LicenceDenialApplicationData();

        var appManager = new LicenceDenialManager(application, repositories, config, User);

        if (await appManager.ProcessLicenceDenialResponse(applKey.EnfSrv, applKey.CtrlCd))
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

    [HttpGet("LicenceDenialToApplication")]
    public async Task<ActionResult<List<TraceCycleQuantityData>>> GetLicenceDenialToApplData([FromQuery] string federalSource,
                                                            [FromServices] IRepositories repositories)
    {
        var manager = new LicenceDenialManager(repositories, config, User);

        var data = await manager.GetLicenceDenialToApplData(federalSource);

        return Ok(data);

    }
}
