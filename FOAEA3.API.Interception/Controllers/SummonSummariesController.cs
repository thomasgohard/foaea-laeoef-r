using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SummonSummariesController : FoaeaControllerBase
    {
        [HttpGet("{key}")]
        public async Task<ActionResult<SummonsSummaryData>> GetSummonsSummaryForApplication([FromRoute] string key,
                                                                                            [FromServices] IRepositories repositories,
                                                                                            [FromServices] IRepositories_Finance repositoriesFinance)
        {
            var applKey = new ApplKey(key);

            var manager = new InterceptionManager(repositories, repositoriesFinance, config, User);

            var data = (await manager.GetSummonsSummaryAsync(applKey.EnfSrv, applKey.CtrlCd)).FirstOrDefault();

            return Ok(data);
        }
    }
}
