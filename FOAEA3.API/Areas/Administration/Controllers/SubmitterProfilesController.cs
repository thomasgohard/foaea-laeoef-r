using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SubmitterProfilesController : ControllerBase
{

    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("SubmitterProfiles API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{submCd}")]
    public async Task<ActionResult<SubmitterProfileData>> GetSubmitterProfile([FromRoute] string submCd, [FromServices] IRepositories repositories)
    {
        var submitterProfileManager = new SubmitterProfileManager(repositories);
        var submitter = await submitterProfileManager.GetSubmitterProfileAsync(submCd);

        if (submitter != null)
        {
            return Ok(submitter);
        }
        else
        {
            return NotFound();
        }

    }

}
