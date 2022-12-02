using FOAEA3.Business.Areas.Financials;
using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Financials.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FinancialEventsController : FoaeaControllerBase
    {
        [HttpGet("PADRevents/active")]
        [Authorize(Policy = Policies.ApplicationReadAccess)]
        public async Task<ActionResult<List<CR_PADReventData>>> GetActiveCR_PADReventsAsync([FromQuery] string enfSrv,
                                                                                            [FromServices] IRepositories db,
                                                                                            [FromServices] IRepositories_Finance dbFinance)
        {
            var manager = new FinancialEventManager(db, dbFinance, config);
            return Ok(await manager.GetActiveCR_PADReventsAsync(enfSrv));
        }

        [HttpGet("IFMS")]
        [Authorize(Policy = Policies.ApplicationReadAccess)]
        public async Task<ActionResult<List<IFMSdata>>> GetIFMS([FromQuery] string batchId,
                                                                [FromServices] IRepositories db,
                                                                [FromServices] IRepositories_Finance dbFinance)
        {
            var manager = new FinancialEventManager(db, dbFinance, config);
            return Ok(await manager.GetIFMSdataAsync(batchId));
        }
    }
}
