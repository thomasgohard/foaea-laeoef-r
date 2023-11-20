using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TracingsController : FoaeaControllerBase
{
    [AllowAnonymous]
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Tracings API Version 1.1");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{key}", Name = "GetTracing")]
    public async Task<ActionResult<TracingApplicationData>> GetApplication([FromRoute] string key,
                                                                           [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var manager = new TracingManager(repositories, config, User);

        bool success = await manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
        if (success)
        {
            if (manager.TracingApplication.AppCtgy_Cd == "T01")
                return Ok(manager.TracingApplication);
            else
                return NotFound($"Error: requested T01 application but found {manager.TracingApplication.AppCtgy_Cd} application.");
        }
        else
            return NotFound();

    }

    [HttpGet("AtState6")]
    public async Task<ActionResult<List<TracingApplicationData>>> GetTracingApplicationsAtState6([FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config, User);

        var data = await manager.GetTracingApplicationsWaitingAtState6();

        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<TracingApplicationData>> CreateApplication([FromServices] IRepositories db)
    {
        var tracingData = await APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

        if (!APIHelper.ValidateRequest(tracingData, applKey: null, out string error))
            return UnprocessableEntity(error);

        var tracingManager = new TracingManager(tracingData, db, config, User);

        var submitter = (await db.SubmitterTable.GetSubmitter(tracingData.Subm_SubmCd)).FirstOrDefault();
        if (submitter is not null)
        {
            tracingManager.CurrentUser.Submitter = submitter;
            db.CurrentSubmitter = submitter.Subm_SubmCd;
        }

        var appl = tracingManager.TracingApplication;

        bool isCreated = await tracingManager.CreateApplication();
        if (isCreated)
        {
            var appKey = $"{appl.Appl_EnfSrv_Cd}-{appl.Appl_CtrlCd}";

            return CreatedAtRoute("GetTracing", new { key = appKey }, appl);
        }
        else
        {
            return UnprocessableEntity(appl);
        }

    }

    [HttpPut("{key}")]
    [Produces("application/json")]
    public async Task<ActionResult<TracingApplicationData>> UpdateApplication([FromRoute] string key,
                                                                              [FromQuery] string command,
                                                                              [FromQuery] FederalSource fedSource,
                                                                              [FromServices] IRepositories db)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey, out string error))
            return UnprocessableEntity(error);

        var tracingManager = new TracingManager(application, db, config, User);

        if ((application.Medium_Cd == "FTP") && (tracingManager.CurrentUser.SubjectName?.ToLower() == "system_support"))
        {
            // NOTE: when request came via FTP, the Appl_LastUpdate_Usr will contain the dat_Update_SubmCd code
            //       that was sent in the FTP file
            var submitter = (await db.SubmitterTable.GetSubmitter(application.Appl_LastUpdate_Usr)).FirstOrDefault();
            if (submitter is not null)
            {
                tracingManager.CurrentUser.Submitter = submitter;
                db.CurrentSubmitter = submitter.Subm_SubmCd;
            }
        }

        if (string.IsNullOrEmpty(command))
            command = "";

        switch (command.ToLower())
        {
            case "":
                await tracingManager.UpdateApplication();
                break;

            case "partiallyserviceapplication":
                await tracingManager.PartiallyServiceApplication(fedSource);
                break;

            case "fullyserviceapplication":
                await tracingManager.FullyServiceApplication(fedSource);
                break;

            default:
                application.Messages.AddSystemError($"Unknown command: {command}");
                return UnprocessableEntity(application);
        }

        if (!tracingManager.TracingApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            return Ok(application);
        else
            return UnprocessableEntity(application);

    }

    [HttpPut("{key}/AcceptApplication")]
    public async Task<ActionResult<TracingApplicationData>> AcceptApplication([FromRoute] string key,
                                                                              [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.AcceptApplication();

        return Ok(application);
    }

    [HttpPut("{key}/RejectApplication")]
    public async Task<ActionResult<TracingApplicationData>> RejectApplication([FromRoute] string key,
                                                                              [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.AcceptApplication();

        return Ok(application);
    }

    [HttpPut("{key}/Transfer")]
    public async Task<ActionResult<TracingApplicationData>> Transfer([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories,
                                                                     [FromQuery] string newRecipientSubmitter,
                                                                     [FromQuery] string newIssuingSubmitter)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

        if (!APIHelper.ValidateRequest(application, applKey, out string error))
            return UnprocessableEntity(error);

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.TransferApplication(newIssuingSubmitter, newRecipientSubmitter);

        return Ok(application);
    }

    [HttpPut("{key}/SINbypass")]
    public async Task<ActionResult<TracingApplicationData>> SINbypass([FromRoute] string key,
                                                                      [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var sinBypassData = await APIBrokerHelper.GetDataFromRequestBody<SINBypassData>(Request);

        var application = new TracingApplicationData();

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

        var sinManager = new ApplicationSINManager(application, appManager);
        await sinManager.SINconfirmationBypass(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

        return Ok(application);
    }

    [HttpPut("{key}/CertifyAffidavit")]
    public async Task<ActionResult<TracingApplicationData>> CertifyAffidavit([FromRoute] string key,
                                                                             [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = await APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

        var affidavitDate = application.Appl_RecvAffdvt_Dte;
        var affidavitSubm = application.Subm_Affdvt_SubmCd;

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

        application.Appl_RecvAffdvt_Dte = affidavitDate;
        application.Subm_Affdvt_SubmCd = affidavitSubm;

        await appManager.CertifyAffidavit(repositories.CurrentSubmitter);

        return Ok(application);
    }

    [HttpPut("{key}/RejectAffidavit")]
    public async Task<ActionResult<TracingApplicationData>> RejectAffidavit([FromRoute] string key,
                                                                            [FromServices] IRepositories repositories)
    {
        var applKey = new ApplKey(key);

        var application = new TracingApplicationData();

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

        await appManager.RejectAffidavit(repositories.CurrentSubmitter);

        return Ok(application);
    }

    [HttpPost("InsertAffidavit")]
    public async Task<ActionResult<TracingApplicationData>> InsertAffidavit([FromServices] IRepositories repositories)
    {
        var affidavitData = await APIBrokerHelper.GetDataFromRequestBody<AffidavitData>(Request);

        var application = new TracingApplicationData();

        var appManager = new TracingManager(application, repositories, config, User);

        await appManager.InsertAffidavitData(affidavitData);

        return Ok(application);
    }

    [HttpGet("WaitingForAffidavits")]
    public async Task<ActionResult<DataList<TracingApplicationData>>> GetApplicationsWaitingForAffidavit(
                                                                                        [FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config, User);

        var data = await manager.GetApplicationsWaitingForAffidavit();

        return Ok(data);
    }

    [HttpGet("TraceToApplication")]
    public async Task<ActionResult<List<TraceCycleQuantityData>>> GetTraceToApplData([FromServices] IRepositories repositories)
    {
        var manager = new TracingManager(repositories, config, User);

        var data = await manager.GetTraceToApplData();

        return Ok(data);
    }

}
