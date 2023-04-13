using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OutgoingFederalSinsController : FoaeaControllerBase
{    
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("OutgoingFederalSins API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet("")]
    public async Task<ActionResult<List<SINOutgoingFederalData>>> GetFederalOutgoingData(
                                                                    [FromQuery] int maxRecords,
                                                                    [FromQuery] string activeState,
                                                                    [FromQuery] int lifeState,
                                                                    [FromQuery] string enfServiceCode,
                                                                    [FromServices] IRepositories repositories)
    {
        var appl = new ApplicationData();
        var applManager = new ApplicationManager(appl, repositories, config, User);
        var manager = new ApplicationSINManager(appl, applManager);

        var data = await manager.GetFederalOutgoingDataAsync(maxRecords, activeState, (ApplicationState)lifeState, enfServiceCode);

        return Ok(data);
    }
}
