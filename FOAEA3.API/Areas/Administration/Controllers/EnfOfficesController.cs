using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EnfOfficesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("EnfOffices API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public async Task<ActionResult<List<EnfOffData>>> GetEnfOffices([FromServices] IRepositories repositories,
                                                        [FromQuery] string enfOffName = null, [FromQuery] string enfOffCode = null,
                                                        [FromQuery] string province = null, [FromQuery] string enfServCode = null)
    {
        return Ok(await repositories.EnfOffTable.GetEnfOffAsync(enfOffName, enfOffCode, province, enfServCode));
    }

}
