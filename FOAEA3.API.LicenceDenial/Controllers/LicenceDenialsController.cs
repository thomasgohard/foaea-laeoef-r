using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace FOAEA3.API.LicenceDenial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LicenceDenialsController : ControllerBase
    {
        private readonly CustomConfig config;

        public LicenceDenialsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("{key}")]
        public ActionResult<LicenceDenialData> GetApplication([FromRoute] string key, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var manager = new LicenceDenialManager(new LicenceDenialData(), repositories, config);

            bool success = manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
            if (success)
            {
                if (manager.LicenceDenialApplication.AppCtgy_Cd == "L01")
                    return Ok(manager.LicenceDenialApplication);
                else
                    return NotFound($"Error: requested L01 application but found {manager.LicenceDenialApplication.AppCtgy_Cd} application.");
            }
            else
                return NotFound();

        }

        [HttpPost]
        public ActionResult<LicenceDenialData> CreateApplication([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var LicenceDenialData = APIBrokerHelper.GetDataFromRequestBody<LicenceDenialData>(Request);

            var LicenceDenialManager = new LicenceDenialManager(LicenceDenialData, repositories, config);
            if (!LicenceDenialManager.LicenceDenialApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                LicenceDenialManager.CreateApplication();
                var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + LicenceDenialManager.LicenceDenialApplication.Appl_EnfSrv_Cd + "-" + LicenceDenialManager.LicenceDenialApplication.Appl_CtrlCd;
                var getURI = new Uri("http://" + HttpContext.Request.Host.ToString() + actionPath);

                return Created(getURI, LicenceDenialManager.LicenceDenialApplication);
            }
            else
            {
                return UnprocessableEntity(LicenceDenialManager.LicenceDenialApplication);
            }

        }

        [HttpPut]
        [Produces("application/json")]
        public ActionResult<LicenceDenialData> UpdateApplication([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var LicenceDenialData = APIBrokerHelper.GetDataFromRequestBody<LicenceDenialData>(Request);

            var LicenceDenialManager = new LicenceDenialManager(LicenceDenialData, repositories, config);
            if (!LicenceDenialManager.LicenceDenialApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                LicenceDenialManager.UpdateApplication();

                return Ok(LicenceDenialManager.LicenceDenialApplication);
            }
            else
            {
                return UnprocessableEntity(LicenceDenialManager.LicenceDenialApplication);
            }

        }

        [HttpPut("{key}/SINbypass")]
        public ActionResult<LicenceDenialData> SINbypass([FromRoute] string key,
                                                         [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var sinBypassData = APIBrokerHelper.GetDataFromRequestBody<SINBypassData>(Request);

            var application = new LicenceDenialData();

            var appManager = new LicenceDenialManager(application, repositories, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            var sinManager = new ApplicationSINManager(application, appManager);
            sinManager.SINconfirmationBypass(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

            return Ok(application);
        }

        [HttpGet("Version")]
        public ActionResult<string> Version()
        {
            return Ok("FOAEA3.API.LicenceDenial API Version 1.4");
        }
    }
}
