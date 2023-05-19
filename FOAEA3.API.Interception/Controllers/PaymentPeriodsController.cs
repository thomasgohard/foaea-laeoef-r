using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentPeriodsController : FoaeaControllerBase
    {
        // GetPaymentPeriods
        [HttpGet]
        public async Task<ActionResult<List<PaymentPeriodData>>> GetPeriodicPeriods([FromServices] IRepositories db, [FromServices] IRepositories_Finance dbFinance)
        {
            var manager = new InterceptionManager(db, dbFinance, config, User);
            return Ok(await manager.GetPaymentPeriods());
        }
    }
}
