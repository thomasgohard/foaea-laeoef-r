using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GendersController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Genders API Version 1.0");

    [HttpGet]
    public async Task<ActionResult<DataList<GenderData>>> GetGenders([FromServices] IGenderRepository genderRepository)
    {
        if (Request.Headers.ContainsKey("CurrentSubmitter"))
            genderRepository.CurrentSubmitter = Request.Headers["CurrentSubmitter"];

        if (Request.Headers.ContainsKey("CurrentSubject"))
            genderRepository.UserId = Request.Headers["CurrentSubject"];

        var data = await genderRepository.GetGendersAsync();

        return Ok(data);
    }
}
