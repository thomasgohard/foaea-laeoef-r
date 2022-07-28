using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SubjectRolesController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("SubjectRoles API Version 1.0");

        [HttpGet]
        public ActionResult<List<SubjectRoleData>> GetSubjectRoles([FromServices] IRepositories repositories, [FromQuery] string subjectName)
        {
            return Ok(repositories.SubjectRoleRepository.GetSubjectRoles(subjectName));
        }

        [HttpGet("{subjectName}")]
        public ActionResult<List<string>> GetAssumedRolesForSubject([FromServices] IRepositories repositories, [FromRoute] string subjectName)
        {
            return Ok(repositories.SubjectRoleRepository.GetAssumedRolesForSubject(subjectName));
        }
    }
}
