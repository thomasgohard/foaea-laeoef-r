using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Tracing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TraceFinancialResponsesController : FoaeaControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<DataList<TraceFinancialResponseData>>> GetTraceFiancialResults([FromRoute] ApplKey id,
                                                                                    [FromServices] IRepositories repositories)
        {
            var manager = new TracingManager(repositories, config);

            if (await manager.LoadApplicationAsync(id.EnfSrv, id.CtrlCd))
                return Ok(await manager.GetTraceFinancialResultsAsync());
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateTraceFinancialResponses([FromServices] IRepositories repositories)
        {
            var responseData = await APIBrokerHelper.GetDataFromRequestBodyAsync<TraceFinancialResponseData>(Request);

            var tracingManager = new TracingManager(repositories, config);

            await tracingManager.CreateFinancialResponseDataAsync(responseData);

            var rootPath = "https://" + HttpContext.Request.Host.ToString();

            return Created(rootPath, new TraceResponseData());
        }
    }
}
