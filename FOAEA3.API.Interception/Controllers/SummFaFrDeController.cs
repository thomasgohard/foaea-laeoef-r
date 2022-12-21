using FOAEA3.Business.Areas.Financials;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SummFaFrDeController : ControllerBase
    {
        [HttpGet("{summFaFrId}")]
        public async Task<ActionResult<SummFAFR_DE_Data>> GetFaFrDe([FromRoute] int summFaFrDeId,
                                                                    [FromServices] IRepositories db,
                                                                    [FromServices] IRepositories_Finance dbFinance)
        {
            var manager = new TransactionManager(db, dbFinance);
            var summFaFrDe = await manager.GetFaFrDe(summFaFrDeId);

            if (summFaFrDe is not null)
                return Ok(summFaFrDe);
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResult>> CreateFaFrDe([FromQuery] string transactionType, 
                                                                       [FromServices] IRepositories db,
                                                                       [FromServices] IRepositories_Finance dbFinance)
        {
            var fafrdeData = await APIBrokerHelper.GetDataFromRequestBodyAsync<SummFAFR_DE_Data>(Request);

            var manager = new TransactionManager(db, dbFinance);
            var result = await manager.CreateFaFrDe(transactionType, fafrdeData);

            if (result.ReturnCode == Model.Enums.ReturnCode.Valid)
            {
                var rootPath = $"https://{HttpContext.Request.Host}/{fafrdeData.SummFAFR_Id}";
                return Created(rootPath, result);
            }
            else
                return BadRequest(result);
        }
               
    }
}
