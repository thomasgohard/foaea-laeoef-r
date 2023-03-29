using FOAEA3.Business.Areas.Application;
using FOAEA3.Business.Areas.Financials;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ControlBatchesController : FoaeaControllerBase
{
    [HttpGet("{batchId}")]
    public async Task<ActionResult<ControlBatchData>> GetControlBatch([FromRoute] string batchId,
                                                                      [FromServices] IRepositories db,
                                                                      [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new ControlBatchManager(db, dbFinance);
        var controlBatchData = await manager.GetControlBatch(batchId);

        if (controlBatchData is not null)
            return Ok(controlBatchData);
        else
            return NotFound();
    }

    [HttpPut("{batchId}/close")]
    public async Task<ActionResult> CloseControlBatch([FromRoute] string batchId,
                                                      [FromServices] IRepositories db,
                                                      [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new ControlBatchManager(db, dbFinance);
        await manager.CloseControlBatchAsync(batchId);

        return Ok();
    }

    [HttpPut("{batchId}/markAsLoaded")]
    public async Task<ActionResult> MarkBatchAsLoaded([FromRoute] string batchId,
                                                      [FromServices] IRepositories db,
                                                      [FromServices] IRepositories_Finance dbFinance)
    {
        var controlBatchData = await APIBrokerHelper.GetDataFromRequestBodyAsync<ControlBatchData>(Request);
       
        var manager = new ControlBatchManager(db, dbFinance);
        if (controlBatchData.BatchType_Cd == "FA")
            await manager.UpdateBatchStateFtpProcessedAsync(batchId, -1);

        return Ok();
    }

    [HttpGet("LastUiBatchLoaded")]
    public async Task<ActionResult<DateTime>> GetDateLastUIBatchLoaded([FromServices] IRepositories db,
                                                                       [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new ControlBatchManager(db, dbFinance);
        return Ok(await manager.GetDateLastUIBatchLoaded());
    }

    [HttpGet("readyDivertFunds")]
    public async Task<ActionResult<List<BatchSimpleData>>> GetReadyDivertFunds([FromQuery] string enfSrv, [FromQuery] string enfSrvLoc,
                                                                   [FromServices] IRepositories db,
                                                                   [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new ControlBatchManager(db, dbFinance);
        return Ok(await manager.GetReadyDivertFundsBatches(enfSrv, enfSrvLoc));
    }

    [HttpPost]
    public async Task<ActionResult<ControlBatchData>> CreateControlBatch([FromServices] IRepositories db,
                                                                         [FromServices] IRepositories_Finance dbFinance)
    {
        var controlBatchData = await APIBrokerHelper.GetDataFromRequestBodyAsync<ControlBatchData>(Request);

        var manager = new ControlBatchManager(db, dbFinance);
        controlBatchData = await manager.CreateControlBatchAsync(controlBatchData);

        if (controlBatchData is not null)
        {
            var rootPath = $"https://{HttpContext.Request.Host}/{controlBatchData.Batch_Id}";
            return Created(rootPath, controlBatchData);
        }
        else
            return BadRequest();
    }

}
