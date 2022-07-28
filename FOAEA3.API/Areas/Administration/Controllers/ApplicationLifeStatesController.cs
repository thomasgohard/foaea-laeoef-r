using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ApplicationLifeStatesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ApplicationLifeStates API Version 1.0");

    [HttpGet]
    public ActionResult<DataList<ApplicationLifeStateData>> GetApplicationLifeStates([FromServices] IApplicationLifeStateRepository applicationLifeStateRepository)
    {
        if (Request.Headers.ContainsKey("CurrentSubmitter"))
            applicationLifeStateRepository.CurrentSubmitter = Request.Headers["CurrentSubmitter"];

        if (Request.Headers.ContainsKey("CurrentSubject"))
            applicationLifeStateRepository.UserId = Request.Headers["CurrentSubject"];

        return Ok(applicationLifeStateRepository.GetApplicationLifeStates());
    }
}
