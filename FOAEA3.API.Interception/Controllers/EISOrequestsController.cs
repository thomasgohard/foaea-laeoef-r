using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EISOrequestsController : FoaeaControllerBase
    {
        // Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        [HttpGet("")]
        [Authorize(Policy = Policies.ApplicationReadAccess)]
        public async Task<ActionResult<List<ProcessEISOOUTHistoryData>>> GetBlockFunds([FromServices] IRepositories db,
                                                                                       [FromServices] IRepositories_Finance dbFinance)
        {
            var manager = new InterceptionManager(db, dbFinance, config);
            return await manager.GetEISOvalidApplications();
        }
    }
}
