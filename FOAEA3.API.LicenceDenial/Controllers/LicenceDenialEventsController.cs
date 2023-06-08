using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.LicenceDenial.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LicenceDenialEventsController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialEvents API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("RequestedLICIN")]
    public async Task<ActionResult<ApplicationEventData>> GetRequestedLICINTracingEvents([FromQuery] string enforcementServiceCode,
                                                                             [FromQuery] string appl_EnfSrv_Cd,
                                                                             [FromQuery] string appl_CtrlCd,
                                                                             [FromServices] IRepositories repositories)
    {
        var manager = new LicenceDenialManager(repositories, config, User);

        if (string.IsNullOrEmpty(enforcementServiceCode))
            return BadRequest("Missing enforcementServiceCode parameter");

        if (string.IsNullOrEmpty(appl_EnfSrv_Cd))
            return BadRequest("Missing appl_EnfSrv_Cd parameter");

        if (string.IsNullOrEmpty(appl_CtrlCd))
            return BadRequest("Missing appl_CtrlCd parameter");

        var result = await manager.GetRequestedLICINLicenceDenialEvents(enforcementServiceCode, appl_EnfSrv_Cd, appl_CtrlCd);
        return Ok(result);

    }

}
