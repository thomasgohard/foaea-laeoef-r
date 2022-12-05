using FOAEA3.Business.Areas.Financials;
using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Financials.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PADReventsController : FoaeaControllerBase
{
    [HttpGet("active")]
    [Authorize(Policy = Policies.ApplicationReadAccess)]
    public async Task<ActionResult<List<CR_PADReventData>>> GetActiveCR_PADRevents([FromQuery] string enfSrv,
                                                                                   [FromServices] IRepositories db,
                                                                                   [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new FinancialManager(db, dbFinance);
        return Ok(await manager.GetActiveCR_PADReventsAsync(enfSrv));
    }

    [HttpPost("close")]
    [Authorize(Policy = Policies.ApplicationReadAccess)]
    public async Task<ActionResult> CloseCR_PADRevents([FromQuery] string batchId,
                                                       [FromQuery] string enfSrv,
                                                       [FromServices] IRepositories db,
                                                       [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new FinancialManager(db, dbFinance);
        await manager.CloseCR_PADReventsAsync(batchId, enfSrv);

        return Ok();
    }

}
