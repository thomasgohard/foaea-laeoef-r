using FOAEA3.Business.Utilities;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EnfServicesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("EnfServices API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("{enfServiceCode}")]
    public async Task<ActionResult<EnfSrvData>> GetEnforcementService([FromRoute] string enfServiceCode, [FromServices] IRepositories repositories)
    {
        var enfSrvManager = new EnforcementServiceManager(repositories);
        EnfSrvData enfSrvData = await enfSrvManager.GetEnforcementServiceAsync(enfServiceCode);

        if (enfSrvData != null)
            return Ok(enfSrvData);
        else
            return UnprocessableEntity();
    }

    [HttpGet]
    public async Task<ActionResult<EnfSrvData>> GetEnforcementServices([FromServices] IRepositories repositories,
                                                           [FromQuery] string enforcementServiceCode = null, [FromQuery] string enforcementServiceName = null,
                                                           [FromQuery] string enforcementServiceProvince = null, [FromQuery] string enforcementServiceCategory = null)
    {
        return Ok(await repositories.EnfSrvTable.GetEnfServiceAsync(enforcementServiceCode, enforcementServiceName, enforcementServiceProvince, enforcementServiceCategory));
    }
}
