using FOAEA3.API.Filters;
using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.Areas.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly CustomConfig config;

    public ApplicationsController(IOptions<CustomConfig> config)
    {
        this.config = config.Value;
    }

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Applications API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = "Admin")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{id}")]
    [HttpGet("{id}/friendly")]
    [ApplicationDataFriendlyResultFilter]
    public async Task<ActionResult<ApplicationData>> GetApplication(
                                [FromRoute] ApplKey id,
                                [FromServices] IRepositories repositories)
    {
        var appl = new ApplicationData();
        var applManager = new ApplicationManager(appl, repositories, config);

        if (await applManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd))
            return Ok(appl);
        else
            return NotFound();
    }

    [HttpPut("{id}/ValidateCoreValues")]
    public async Task<ActionResult<ApplicationData>> ValidateCoreValues([FromRoute] ApplKey id,
                                                            [FromServices] IRepositories repositories)
    {
        var appl = await APIBrokerHelper.GetDataFromRequestBodyAsync<InterceptionApplicationData>(Request);
        var applicationValidation = new ApplicationValidation(appl, repositories, config);

        bool isValid = await applicationValidation.ValidateCodeValues();

        if (isValid)
            return Ok(appl);
        else
            return UnprocessableEntity(appl);
    }

    [HttpGet("{id}/SINresults")]
    public async Task<ActionResult<DataList<SINResultData>>> GetSINResults([FromRoute] ApplKey id,
                                                                    [FromServices] IRepositories repositories)
    {
        var appl = new ApplicationData();
        var applManager = new ApplicationManager(appl, repositories, config);
        var sinManager = new ApplicationSINManager(appl, applManager);

        if (await applManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd))
            return Ok(await sinManager.GetSINResultsAsync());
        else
            return NotFound();
    }

    [HttpGet("{id}/SINresultsWithHistory")]
    public async Task<ActionResult<DataList<SINResultWithHistoryData>>> GetSINResultsWithHistory([FromRoute] ApplKey id,
                                                                                [FromServices] IRepositories repositories)
    {
        var appl = new ApplicationData();
        var applManager = new ApplicationManager(appl, repositories, config);
        var sinManager = new ApplicationSINManager(appl, applManager);

        if (await applManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd))
            return Ok(await sinManager.GetSINResultsWithHistoryAsync());
        else
            return NotFound();
    }

    [HttpPut("{id}/SinConfirmation")]
    public async Task<ActionResult<ApplicationData>> SINconfirmation([FromRoute] ApplKey id,
                                                         [FromServices] IRepositories repositories,
                                                         [FromServices] IRepositories_Finance repositoriesFinance)
    {
        var sinConfirmationData = await APIBrokerHelper.GetDataFromRequestBodyAsync<SINConfirmationData>(Request);

        var application = new ApplicationData();

        var appManager = new ApplicationManager(application, repositories, config);
        await appManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd);

        ApplicationSINManager sinManager;

        switch (application.AppCtgy_Cd)
        {
            case "T01":
                var tracingManager = new TracingManager(repositories, config);
                await tracingManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd);
                sinManager = new ApplicationSINManager(tracingManager.TracingApplication, tracingManager);
                break;
            case "I01":
                var interceptionManager = new InterceptionManager(repositories, repositoriesFinance, config);
                await interceptionManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd);
                sinManager = new ApplicationSINManager(interceptionManager.InterceptionApplication, interceptionManager);
                break;
            case "L01":
                var licenceDenialManager = new LicenceDenialManager(repositories, config);
                await licenceDenialManager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd);
                sinManager = new ApplicationSINManager(licenceDenialManager.LicenceDenialApplication, licenceDenialManager);
                break;
            default:
                sinManager = new ApplicationSINManager(application, appManager);
                break;
        }

        await sinManager.SINconfirmationAsync(isSinConfirmed: sinConfirmationData.IsSinConfirmed,
                                              confirmedSin: sinConfirmationData.ConfirmedSIN,
                                              lastUpdateUser: repositories.CurrentSubmitter);

        return Ok(application);
    }

    [HttpGet("Stats")]
    public async Task<ActionResult<List<StatsOutgoingProvincialData>>> GetOutgoingProvincialStatusData([FromServices] IRepositories repositories,
                                                               [FromQuery] int maxRecords,
                                                               [FromQuery] string activeState,
                                                               [FromQuery] string recipientCode)
    {
        var appl = new ApplicationData();
        var applManager = new ApplicationManager(appl, repositories, config);

        return await applManager.GetProvincialStatsOutgoingDataAsync(maxRecords, activeState, recipientCode);
    }


}
