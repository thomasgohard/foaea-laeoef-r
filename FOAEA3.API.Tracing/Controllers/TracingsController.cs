using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace FOAEA3.API.Tracing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TracingsController : ControllerBase
    {
        private readonly CustomConfig config;

        public TracingsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("{key}")]
        public ActionResult<TracingApplicationData> GetApplication([FromRoute] string key,
                                                                   [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var manager = new TracingManager(repositories, config);

            bool success = manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
            if (success)
            {
                if (manager.TracingApplication.AppCtgy_Cd == "T01")
                    return Ok(manager.TracingApplication);
                else
                    return NotFound($"Error: requested T01 application but found {manager.TracingApplication.AppCtgy_Cd} application.");
            }
            else
                return NotFound();

        }

        [HttpPost]
        public ActionResult<TracingApplicationData> CreateApplication([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var tracingData = APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

            if (tracingData is null)
                return UnprocessableEntity("Missing or invalid request body.");

            var tracingManager = new TracingManager(tracingData, repositories, config);

            bool isCreated = tracingManager.CreateApplication();
            if (isCreated)
            {
                var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + tracingManager.TracingApplication.Appl_EnfSrv_Cd + "-" + tracingManager.TracingApplication.Appl_CtrlCd;
                var rootPath = "http://" + HttpContext.Request.Host.ToString();
                var apiGetURIForNewlyCreatedTracing = new Uri(rootPath + actionPath);

                return Created(apiGetURIForNewlyCreatedTracing, tracingManager.TracingApplication);
            }
            else
            {
                return UnprocessableEntity(tracingManager.TracingApplication.Messages.GetMessagesForType(MessageType.Error));
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

            var tracingData = APIBrokerHelper.GetDataFromRequestBody<TracingApplicationData>(Request);

            var applKey = new ApplKey(key);
            if ((applKey.EnfSrv.Trim() != tracingData.Appl_EnfSrv_Cd.Trim()) || (applKey.CtrlCd.Trim() != tracingData.Appl_CtrlCd.Trim()))
            {
                tracingData.Messages.AddSystemError($"id [{key}] does not match content " +
                                                    $"[{tracingData.Appl_EnfSrv_Cd.Trim()}-{tracingData.Appl_CtrlCd.Trim()}]");
                return UnprocessableEntity(tracingData);
            }

            var tracingManager = new TracingManager(tracingData, repositories, config);

            if (string.IsNullOrEmpty(command))
                command = "";

            switch (command.ToLower())
            {
                case "":
                    tracingManager.UpdateApplication();
                    break;

                case "partiallyserviceapplication":
                    tracingManager.PartiallyServiceApplication(enforcementServiceCode);
                    break;

                case "fullyserviceapplication":
                    tracingManager.FullyServiceApplication(enforcementServiceCode);
                    break;

                default:
                    tracingData.Messages.AddSystemError($"Unknown command: {command}");
                    return UnprocessableEntity(tracingManager.TracingApplication);
            }

            if (!tracingManager.TracingApplication.Messages.ContainsMessagesOfType(MessageType.Error))
                return Ok(tracingManager.TracingApplication);
            else
                return UnprocessableEntity(tracingManager.TracingApplication);

        }

        [HttpPut("{key}/SINbypass")]
        public ActionResult<TracingApplicationData> SINbypass([FromRoute] string key,
                                                              [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var sinBypassData = APIBrokerHelper.GetDataFromRequestBody<SINBypassData>(Request);

            var application = new TracingApplicationData();

            var appManager = new TracingManager(application, repositories, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            var sinManager = new ApplicationSINManager(application, appManager);
            sinManager.SINconfirmationBypass(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

            return Ok(application);
        }

        [HttpPut("{key}/CertifyAffidavit")]
        public ActionResult<TracingApplicationData> CertifyAffidavit([FromRoute] string key,
                                                                     [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = new TracingApplicationData();

            var appManager = new TracingManager(application, repositories, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            appManager.CertifyAffidavit(repositories.CurrentSubmitter);

            return Ok(application);
        }

        [HttpGet("WaitingForAffidavits")]
        public ActionResult<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavit(
                                                                [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new TracingManager(repositories, config);

            var data = manager.GetApplicationsWaitingForAffidavit();

            return Ok(data);
        }

        [HttpGet("TraceToApplication")]
        public ActionResult<List<TraceCycleQuantityData>> GetTraceToApplData(
                                                                [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var manager = new TracingManager(repositories, config);

            var data = manager.GetTraceToApplData();

            return Ok(data);

        }
        [HttpGet("Version")]
        public ActionResult<string> Version()
        {
            return Ok("FOAEA3.API.Tracing API Version 1.4");
        }

    }
}
