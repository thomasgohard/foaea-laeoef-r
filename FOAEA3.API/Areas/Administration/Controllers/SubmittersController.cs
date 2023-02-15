using FOAEA3.Business.Areas.Administration;
using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FOAEA3.API.Areas.Administration;

[ApiController]
[Route("api/v1/[controller]")]
public class SubmittersController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Submitters API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public async Task<ActionResult<List<SubmitterData>>> GetSubmitters([FromServices] IRepositories repositories,
                                                           [FromQuery] string provCd = null, [FromQuery] string enfOffCode = null,
                                                           [FromQuery] string enfSrvCode = null, [FromQuery] string onlyActive = null)
    {
        var submitterManager = new SubmitterManager(repositories);

        if ((enfSrvCode is null) && (enfOffCode is null))
            return Ok(await submitterManager.GetSubmittersForProvinceAsync(provCd, onlyActive == "true"));
        else
            return Ok(await submitterManager.GetSubmittersForProvinceAndOfficeAsync(provCd, enfOffCode, enfSrvCode, onlyActive == "true"));
    }

    [HttpGet("{submCd}", Name = "GetSubmitter")]
    public async Task<ActionResult<SubmitterData>> GetSubmitter([FromRoute] string submCd, [FromServices] IRepositories repositories)
    {
        var submitterManager = new SubmitterManager(repositories);
        var submitter = await submitterManager.GetSubmitterAsync(submCd);

        if (submitter != null)
        {
            return Ok(submitter);
        }
        else
        {
            return NotFound();
        }

    }

    [HttpGet("Office/{office}")]
    public async Task<ActionResult<List<string>>> GetSubmitterCodesForOffice([FromQuery] string service, [FromRoute] string office, 
                                                                             [FromServices] IRepositories db)
    {
        var submitterManager = new SubmitterManager(db);
        var submitterCodes = await submitterManager.GetSubmitterCodesForOffice(service, office);

        return Ok(submitterCodes);
    }

    [HttpGet("{submCd}/Declarant")]
    public async Task<ActionResult<string>> GetDeclarant([FromRoute] string submCd, [FromServices] IRepositories repositories)
    {
        var submitterManager = new SubmitterManager(repositories);
        var submitter = await submitterManager.GetSubmitterAsync(submCd);

        string declarant = string.Empty;
        if (submitter != null)
        {
            declarant = submitter.Subm_FrstNme.Trim() + " " + submitter.Subm_SurNme.Trim();
        }

        return declarant;
    }
    
    [HttpGet("{submCd}/RecentActivity")]
    public async Task<ActionResult<List<ApplicationModificationActivitySummaryData>>> GetRecentActivity([FromServices] IRepositories db, 
                                                                                                        [FromRoute] string submCd, 
                                                                                                        [FromQuery] int days = 0)
    {
        var applicationManager = new ApplicationManager(new ApplicationData(), db, config);
        return Ok(await applicationManager.GetApplicationRecentActivityForSubmitter(submCd, days));
    }

    [HttpGet("{submCd}/ApplicationsAtState/{state}")]
    public async Task<ActionResult<List<ApplicationModificationActivitySummaryData>>> GetSinNotConfirmed([FromServices] IRepositories db, 
                                                                                                         [FromRoute] string submCd,
                                                                                                         [FromRoute] int state)
    {
        var applicationManager = new ApplicationManager(new ApplicationData(), db, config);
        return Ok(await applicationManager.GetApplicationAtStateForSubmitter(submCd, (ApplicationState) state));
    }

    [HttpGet("commissioners/{enfOffLocCode}")]
    public async Task<ActionResult<List<CommissionerData>>> GetCommissioners([FromServices] IRepositories repositories,
                                                                 [FromRoute] string enfOffLocCode = null)
    {
        var submitterManager = new SubmitterManager(repositories);

        return Ok(await submitterManager.GetCommissionersAsync(enfOffLocCode, repositories.CurrentSubmitter));

    }

    [HttpPost]
    public async Task<ActionResult<TracingApplicationData>> CreateSubmitter([FromBody] string sourceSubmitterData, [FromQuery] string suffixCode, [FromQuery] string readOnlyAccess,
                                                                [FromServices] IRepositories repositories)
    {
        var submitterData = JsonConvert.DeserializeObject<SubmitterData>(sourceSubmitterData);

        var submitterManager = new SubmitterManager(repositories);
        submitterData = await submitterManager.CreateSubmitterAsync(submitterData, suffixCode, readOnlyAccess == "true");

        if (!submitterData.Messages.ContainsMessagesOfType(MessageType.Error))
        {
            var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + submitterData.Subm_SubmCd;
            var getURI = new Uri("https://" + HttpContext.Request.Host.ToString() + actionPath);

            return CreatedAtRoute("GetSubmitter", new { submCd = submitterData.Subm_SubmCd }, submitterData);
        }
        else
        {
            return UnprocessableEntity(submitterData);
        }

    }

    [HttpPut]
    public async Task<ActionResult<TracingApplicationData>> UpdateSubmitter([FromServices] IRepositories repositories)
    {
        var submitterData = await APIBrokerHelper.GetDataFromRequestBodyAsync<SubmitterData>(Request);

        bool readOnly = ((submitterData.Subm_Class == "RO") || (submitterData.Subm_Class == "R1"));

        var submitterManager = new SubmitterManager(repositories);
        submitterData = await submitterManager.UpdateSubmitterAsync(submitterData, readOnly);

        return Ok(submitterData);

    }

}
