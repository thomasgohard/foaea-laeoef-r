using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ApplicationCommentsController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ApplicationComments API Version 1.0");

    [HttpGet]
    public async Task<ActionResult<DataList<ApplicationCommentsData>>> GetApplicationComments([FromServices] IApplicationCommentsRepository applicationCommentsRepository)
    {
        if (Request.Headers.ContainsKey("CurrentSubmitter"))
            applicationCommentsRepository.CurrentSubmitter = Request.Headers["CurrentSubmitter"];

        if (Request.Headers.ContainsKey("CurrentSubject"))
            applicationCommentsRepository.UserId = Request.Headers["CurrentSubject"];

        return Ok(await applicationCommentsRepository.GetApplicationCommentsAsync());
    }
}
