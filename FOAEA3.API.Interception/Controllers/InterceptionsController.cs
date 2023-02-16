using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = Duties.Interception + "," + Roles.FlasUser + "," + Roles.Admin)]
public class InterceptionsController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> Version()
    {
        return Ok("Interceptions API Version 1.0");
    }

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{key}", Name = "GetInterception")]
    public async Task<ActionResult<InterceptionApplicationData>> GetApplication([FromRoute] string key,
                                                                    [FromServices] IRepositories repositories,
                                                                    [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var manager = new InterceptionManager(repositories, repositoriesFinance, config);
        await manager.SetCurrentUserAsync(User);

        bool success = await manager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd);
        if (success)
        {
            if (manager.InterceptionApplication.AppCtgy_Cd == "I01")
                return Ok(manager.InterceptionApplication);
            else
                return NotFound($"Error: requested I01 application but found {manager.InterceptionApplication.AppCtgy_Cd} application.");
        }
        else
            return NotFound();

    }

    [HttpGet("AutoAcceptVariations")]
    [Authorize(Policy = Policies.ApplicationReadAccess)]
    public async Task<ActionResult> AutoAcceptVariations([FromServices] IRepositories repositories,
                                                         [FromServices] IRepositories_Finance repositoriesFinance,
                                                         [FromQuery] string enfService)
    {
        var interceptionManager = new InterceptionManager(repositories, repositoriesFinance, config);
        await interceptionManager.SetCurrentUserAsync(User);
        await interceptionManager.AutoAcceptVariationsAsync(enfService);

        return Ok();
    }

    [HttpPost]
    [Authorize(Policy = Policies.ApplicationModifyAccess)]
    public async Task<ActionResult<InterceptionApplicationData>> CreateApplication([FromServices] IRepositories db,
                                                                                   [FromServices] IRepositories_Finance dbFinance)
    {
        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey: null, out string error))
            return UnprocessableEntity(error);

        var interceptionManager = new InterceptionManager(application, db, dbFinance, config);
        await interceptionManager.SetCurrentUserAsync(User);
        var submitter = (await db.SubmitterTable.GetSubmitterAsync(application.Subm_SubmCd)).FirstOrDefault();
        if (submitter is not null)
        {
            interceptionManager.CurrentUser.Submitter = submitter;
            db.CurrentSubmitter = submitter.Subm_SubmCd;
        }

        bool isCreated = await interceptionManager.CreateApplicationAsync();
        if (isCreated)
        {
            var appKey = $"{application.Appl_EnfSrv_Cd}-{application.Appl_CtrlCd}";

            return CreatedAtRoute("GetInterception", new { key = appKey }, application);
        }
        else
        {
            return UnprocessableEntity(application);
        }

    }

    [HttpPut("{key}")]
    [Produces("application/json")]
    public async Task<ActionResult<InterceptionApplicationData>> UpdateApplication(
                                                    [FromRoute] string key,
                                                    [FromServices] IRepositories repositories,
                                                    [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var interceptionManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await interceptionManager.SetCurrentUserAsync(User);
        await interceptionManager.UpdateApplicationAsync();

        if (!interceptionManager.InterceptionApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            return Ok(application);
        else
            return UnprocessableEntity(application);

    }

    [HttpPut("{key}/Transfer")]
    public async Task<ActionResult<InterceptionApplicationData>> Transfer([FromRoute] string key,
                                                     [FromServices] IRepositories repositories,
                                                     [FromServices] IRepositories_Finance repositories_finance,
                                                     [FromQuery] string newRecipientSubmitter,
                                                     [FromQuery] string newIssuingSubmitter)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new InterceptionManager(application, repositories, repositories_finance, config);
        await appManager.SetCurrentUserAsync(User);
        await appManager.TransferApplicationAsync(newIssuingSubmitter, newRecipientSubmitter);

        return Ok(application);
    }


    [HttpPut("{key}/cancel")]
    [Produces("application/json")]
    public async Task<ActionResult<InterceptionApplicationData>> CancelApplication([FromRoute] string key,
                                                                       [FromServices] IRepositories repositories,
                                                                       [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var interceptionManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await interceptionManager.SetCurrentUserAsync(User);
        await interceptionManager.CancelApplication();

        if (!interceptionManager.InterceptionApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            return Ok(application);
        else
            return UnprocessableEntity(application);

    }

    [HttpPut("{key}/suspend")]
    [Produces("application/json")]
    public async Task<ActionResult<InterceptionApplicationData>> SuspendApplication([FromRoute] string key,
                                                                        [FromServices] IRepositories repositories,
                                                                        [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var interceptionManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await interceptionManager.SetCurrentUserAsync(User);
        await interceptionManager.SuspendApplicationAsync();

        if (!interceptionManager.InterceptionApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            return Ok(application);
        else
            return UnprocessableEntity(application);

    }

    [HttpPut("ValidateFinancialCoreValues")]
    public async Task<ActionResult<ApplicationData>> ValidateFinancialCoreValues([FromServices] IRepositories repositories)
    {
        var appl = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);
        var currentUser = await UserHelper.ExtractDataFromUser(User, repositories);
        var interceptionValidation = new InterceptionValidation(appl, repositories, config, currentUser);

        bool isValid = interceptionValidation.ValidateFinancialCoreValues();

        if (isValid)
            return Ok(appl);
        else
            return UnprocessableEntity(appl);
    }

    [HttpPut("{key}/SINbypass")]
    public async Task<ActionResult<InterceptionApplicationData>> SINbypass([FromRoute] string key,
                                                       [FromServices] IRepositories repositories,
                                                       [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var sinBypassData = await APIBrokerHelper.GetDataFromRequestBodyAsync<SINBypassData>(Request);

        var application = new InterceptionApplicationData();

        var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await appManager.SetCurrentUserAsync(User);
        await appManager.LoadApplicationAsync(applKey.EnfSrv, applKey.CtrlCd);

        var sinManager = new ApplicationSINManager(application, appManager);
        await sinManager.SINconfirmationBypassAsync(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

        return Ok(application);
    }

    [HttpPut("{key}/Vary")]
    public async Task<ActionResult<InterceptionApplicationData>> Vary([FromRoute] string key,
                                                          [FromServices] IRepositories repositories,
                                                          [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await appManager.SetCurrentUserAsync(User);

        if (await appManager.VaryApplicationAsync())
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

    [HttpPut("{key}/AcceptApplication")]
    public async Task<ActionResult<InterceptionApplicationData>> AcceptInterception([FromRoute] string key,
                                                                        [FromServices] IRepositories repositories,
                                                                        [FromServices] IRepositories_Finance repositoriesFinance,
                                                                        [FromQuery] DateTime supportingDocsReceiptDate)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await appManager.SetCurrentUserAsync(User);

        if (await appManager.AcceptInterceptionAsync(supportingDocsReceiptDate))
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

    [HttpPut("{key}/AcceptVariation")]
    public async Task<ActionResult<InterceptionApplicationData>> AcceptVariation([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories,
                                                                     [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await appManager.SetCurrentUserAsync(User);

        if (await appManager.AcceptVariationAsync())
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

    [HttpPut("{key}/RejectVariation")]
    public async Task<ActionResult<InterceptionApplicationData>> RejectVariation([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories,
                                                                     [FromServices] IRepositories_Finance repositoriesFinance,
                                                                     [FromQuery] string applicationRejectReasons)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);

        if (!APIHelper.ValidateApplication(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
        await appManager.SetCurrentUserAsync(User);

        if (await appManager.RejectVariationAsync(applicationRejectReasons))
            return Ok(application);
        else
            return UnprocessableEntity(application);
    }

}
