using BackendProcesses.Business;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System;

namespace BackendProcesses.API.Controllers
{
    [Route("api/v1/bringforwardevents")]
    [ApiController]
    public class BringForwardEventsController : Controller
    {
        private readonly CustomConfig config;

        public BringForwardEventsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPut("")]
        public ActionResult<string> RunBringForward([FromServices] IRepositories repositories)
        {
            repositories.CurrentSubmitter = "";

            ILogger log = Log.ForContext("APIpath", HttpContext.Request.Path.Value);
            log.Information("(PUT) method RunBringForward(): referer = {referer}", Request.Headers["Referer"]);

            var startTime = DateTime.Now;

            var bringForwardProcess = new BringForwardEventProcess(repositories, config);
            bringForwardProcess.Run();

            var endTime = DateTime.Now;

            var duration = endTime - startTime;

            return Ok($"{duration.Hours} hour(s) and {duration.Minutes} minute(s)");
        }
    }
}
