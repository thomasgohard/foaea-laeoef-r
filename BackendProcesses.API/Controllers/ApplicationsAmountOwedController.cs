using BackendProcesses.Business;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace BackendProcess.API.Controllers
{
    [Route("api/v1/applicationsAmountOwed")]
    [ApiController]
    public class ApplicationsAmountOwedController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> Version([FromServices] IRepositories repositories) => 
            Ok($"Applications Amount Owed API Version 1.5\nDB: {repositories.MainDB.ConnectionString}");

        [HttpPut("")]
        public ActionResult<string> RunAmountOwed([FromServices] IRepositories repositories, [FromServices] IRepositories_Finance repositoriesFinance)
        {
            repositories.CurrentSubmitter = "";

            ILogger log = Log.ForContext("APIpath", HttpContext.Request.Path.Value);
            log.Information("(PUT) method RunAmountOwed(): referer = {referer}", Request.Headers["Referer"]);

            var startTime = DateTime.Now;

            var amountOwedProcess = new AmountOwedProcess(repositories, repositoriesFinance);
            amountOwedProcess.Run();

            var endTime = DateTime.Now;

            var duration = endTime - startTime;

            return Ok($"{duration.Hours} hour(s) and {duration.Minutes} minute(s)");
        }

        [HttpPut("{id}")]
        public ActionResult<SummonsSummaryData> CalculateAmountOwed(string id, [FromServices] IRepositories repositories, [FromServices] IRepositories_Finance repositoriesFinance)
        {
            repositories.CurrentSubmitter = "";

            ILogger log = Log.ForContext("APIpath", HttpContext.Request.Path.Value);
            log.Information("(PUT) method CalculateAmountOwed({id}): referer = {referer}", id, Request.Headers["Referer"]);

            string[] values = id.Split("-");
            if (values.Length == 2)
            {
                string enfSrv = values[0];
                string ctrlCd = values[1];

                var amountOwedProcess = new AmountOwedProcess(repositories, repositoriesFinance);
                var (summSmryNewData, _) = amountOwedProcess.CalculateAndUpdateAmountOwedForVariation(enfSrv, ctrlCd);

                if (summSmryNewData != null)
                {
                    Response.Headers.Add("get-amount-owed", "GET " + HttpContext.Request.Path.Value);

                    return Ok(summSmryNewData);
                }
                else
                    return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
