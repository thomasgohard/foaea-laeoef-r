using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ESDsController : ControllerBase
    {
        private readonly RecipientsConfig config;

        public ESDsController()
        {
            var configHelper = new FoaeaConfigurationHelper();
            config = configHelper.RecipientsConfig;
        }

        [HttpGet("Version")]
        public ActionResult<string> Version()
        {
            return Ok("ESDs API Version 1.0");
        }

        [HttpGet("DB")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [HttpGet("{fileName}")]
        public async Task<ActionResult<ElectronicSummonsDocumentZipData>> GetESD([FromRoute] string fileName,
                                                                                 [FromServices] IRepositories repositories)
        {
            var manager = new ElectronicSummonsDocumentManager(repositories);
            await manager.SetCurrentUserAsync(User);

            var electronicSummons = await manager.GetESDasync(fileName);
            if (electronicSummons is not null)
                return Ok(electronicSummons);
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ElectronicSummonsDocumentZipData>> CreateESD([FromServices] IRepositories repositories)
        {
            var zipData = await APIBrokerHelper.GetDataFromRequestBodyAsync<ElectronicSummonsDocumentZipData>(Request);

            var manager = new ElectronicSummonsDocumentManager(repositories);
            await manager.SetCurrentUserAsync(User);

            var electronicSummons = await manager.CreateESD(zipData);
            if (electronicSummons is not null)
                return Ok(electronicSummons);
            else
                return NotFound();
        }
    }
}
