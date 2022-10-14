using FOAEA3.Business.Areas.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SubmittersController : ControllerBase
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

    [HttpGet("{submCd}")]
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

            return Created(getURI, submitterData);
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
