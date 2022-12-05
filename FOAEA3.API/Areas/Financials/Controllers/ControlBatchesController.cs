﻿using FOAEA3.Business.Areas.Financials;
using FOAEA3.Common;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Financials.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ControlBatchesController : FoaeaControllerBase
{
    [HttpPost("close")]
    [Authorize(Policy = Policies.ApplicationReadAccess)]
    public async Task<ActionResult> CloseControlBatch([FromQuery] string batchId,
                                                      [FromServices] IRepositories db,
                                                      [FromServices] IRepositories_Finance dbFinance)
    {
        var manager = new FinancialManager(db, dbFinance);
        await manager.CloseControlBatchAsync(batchId);

        return Ok();
    }
}
