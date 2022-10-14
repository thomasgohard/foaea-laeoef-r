using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ApplicationLifeStatesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ApplicationLifeStates API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public async Task<ActionResult<DataList<ApplicationLifeStateData>>> GetApplicationLifeStates(
                            [FromServices] IApplicationLifeStateRepository applicationLifeStateRepository)
    {
        return Ok(await applicationLifeStateRepository.GetApplicationLifeStatesAsync());
    }
}
