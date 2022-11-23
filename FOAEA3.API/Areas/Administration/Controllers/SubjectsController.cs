using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SubjectsController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Subjects API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public async Task<ActionResult<List<SubjectData>>> GetSubjects([FromServices] IRepositories repositories, [FromQuery] string submCd)
    {
        if (submCd != null)
            return Ok(await repositories.SubjectTable.GetSubjectsForSubmitterAsync(submCd));
        else
            return UnprocessableEntity("Missing submCd parameter"); // not allowed to get all subjects currently
    }

    [HttpGet("{subjectName}")]
    public async Task<ActionResult<SubjectData>> GetSubject([FromServices] IRepositories repositories, [FromRoute] string subjectName)
    {
        var data = await repositories.SubjectTable.GetSubjectAsync(subjectName);
        return Ok(data);
    }

    [HttpPut("AcceptTermsOfReference")]
    public async Task<ActionResult<SubjectData>> AcceptTermsOfReference([FromServices] IRepositories repositories, [FromBody] SubjectData subject)
    {
        var loginManager = new LoginManager(repositories);
        await loginManager.AcceptNewTermsOfReferernceAsync(subject.SubjectName);

        return Ok(subject);
    }

}
