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
using System.Collections.Generic;
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

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("LicenceDenialsFiles API Version 1.0");

        [HttpGet("{key}")]
        public ActionResult<LicenceDenialApplicationData> GetApplication([FromRoute] string key,
                                                                         [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var manager = new LicenceDenialManager(repositories, config);

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

        [HttpGet("{key}/LicenceSuspensionHistory")]
        public ActionResult<List<LicenceSuspensionHistoryData>> GetLicenceSuspensionHistory([FromRoute] string key,
                                                                                            [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var manager = new LicenceDenialManager(repositories, config);

            bool success = manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
            if (success)
            {
                if (manager.LicenceDenialApplication.AppCtgy_Cd == "L01")
                {
                    var suspensionHistory = manager.GetLicenceSuspensionHistory();

                    return Ok(suspensionHistory);
                }
                else
                    return NotFound($"Error: requested L01 application but found {manager.LicenceDenialApplication.AppCtgy_Cd} application.");
            }
            else
                return NotFound();

        }

        [HttpPost]
        public ActionResult<LicenceDenialApplicationData> CreateApplication([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var application = APIBrokerHelper.GetDataFromRequestBody<LicenceDenialApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey: null, out string error))
                return UnprocessableEntity(error);

            var licenceDenialManager = new LicenceDenialManager(application, repositories, config);

            bool isCreated = licenceDenialManager.CreateApplication();
            if (isCreated)
            {
                var appKey = $"{application.Appl_EnfSrv_Cd}-{application.Appl_CtrlCd}";
                var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + appKey;
                var getURI = new Uri("http://" + HttpContext.Request.Host.ToString() + actionPath);

                return Created(getURI, application);
            }
            else
            {
                return UnprocessableEntity(application);
            }

        }
               
        [HttpPut("{key}")]
        [Produces("application/json")]
        public ActionResult<TracingApplicationData> UpdateApplication(
                                                        [FromRoute] string key,
                                                        [FromQuery] string command,
                                                        [FromQuery] string enforcementServiceCode,
                                                        [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<LicenceDenialApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey, out string error))
                return UnprocessableEntity(error);

            var licenceDenialManager = new LicenceDenialManager(application, repositories, config);

            if (string.IsNullOrEmpty(command))
                command = "";

            switch (command.ToLower())
            {
                case "":
                    licenceDenialManager.UpdateApplication();
                    break;

                default:
                    application.Messages.AddSystemError($"Unknown command: {command}");
                    return UnprocessableEntity(application);
            }

            if (!application.Messages.ContainsMessagesOfType(MessageType.Error))
                return Ok(application);
            else
                return UnprocessableEntity(application);

        }

        [HttpPut("{key}/SINbypass")]
        public ActionResult<LicenceDenialApplicationData> SINbypass([FromRoute] string key,
                                                         [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var sinBypassData = APIBrokerHelper.GetDataFromRequestBody<SINBypassData>(Request);

            var application = new LicenceDenialApplicationData();

            var appManager = new LicenceDenialManager(application, repositories, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            if (!APIHelper.ValidateApplication(appManager.LicenceDenialApplication, applKey, out string error))
                return UnprocessableEntity(error);

            var sinManager = new ApplicationSINManager(application, appManager);
            sinManager.SINconfirmationBypass(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

            return Ok(application);
        }

        [HttpPut("{key}/ProcessLicenceDenialResponse")]
        public ActionResult<LicenceDenialApplicationData> ProcessLicenceDenialResponse([FromRoute] string key,
                                                                                       [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = new LicenceDenialApplicationData();

            var appManager = new LicenceDenialManager(application, repositories, config);
            if (appManager.ProcessLicenceDenialResponse(applKey.EnfSrv, applKey.CtrlCd))
                return Ok(application);
            else
                return UnprocessableEntity(application);
        }
           
        [HttpGet("LicenceDenialToApplication")]
        public ActionResult<List<TraceCycleQuantityData>> GetLicenceDenialToApplData([FromQuery] string federalSource,
                                                                [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new LicenceDenialManager(repositories, config);

            var data = manager.GetLicenceDenialToApplData(federalSource);

            return Ok(data);

        }
    }
}
