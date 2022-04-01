using FOAEA3.Business.Security;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SubmitterProfilesController : ControllerBase
    {

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("SubmitterProfiles API Version 1.0");

        [HttpGet("{submCd}")]
        public ActionResult<SubmitterProfileData> GetSubmitterProfile([FromRoute] string submCd, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var submitterProfileManager = new SubmitterProfileManager(repositories);
            var submitter = submitterProfileManager.GetSubmitterProfile(submCd);

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
}
