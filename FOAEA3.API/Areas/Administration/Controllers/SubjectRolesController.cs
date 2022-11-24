using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SubjectRolesController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("SubjectRoles API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public async Task<ActionResult<List<SubjectRoleData>>> GetSubjectRoles([FromServices] IRepositories repositories, [FromQuery] string subjectName)
    {
        return Ok(await repositories.SubjectRoleTable.GetSubjectRolesAsync(subjectName));
    }

    [HttpGet("{subjectName}")]
    public async Task<ActionResult<List<string>>> GetAssumedRolesForSubject([FromServices] IRepositories repositories, [FromRoute] string subjectName)
    {
        return Ok(await repositories.SubjectRoleTable.GetAssumedRolesForSubjectAsync(subjectName));
    }
}
