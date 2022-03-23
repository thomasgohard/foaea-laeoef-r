using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.LicenceDenial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LicenceDenialTerminationsController : ControllerBase
    {
        private readonly CustomConfig config;

        public LicenceDenialTerminationsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("{key}")]
        public ActionResult<LicenceDenialApplicationData> GetApplication([FromRoute] string key, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var manager = new LicenceDenialTerminationManager(repositories, config);

            bool success = manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
            if (success)
            {
                if (manager.LicenceDenialTerminationApplication.AppCtgy_Cd == "L03")
                    return Ok(manager.LicenceDenialTerminationApplication);
                else
                    return NotFound($"Error: requested L03 application but found {manager.LicenceDenialTerminationApplication.AppCtgy_Cd} application.");
            }
            else
                return NotFound();

        }
    }
}
