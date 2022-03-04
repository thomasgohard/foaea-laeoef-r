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

namespace FOAEA3.API.Interception.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InterceptionsController : ControllerBase
    {
        private readonly CustomConfig config;

        public InterceptionsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> Version()
        {
            return Ok("FOAEA3.API.Interception API Version 1.4");
        }

        [HttpGet("{key}")]
        public ActionResult<InterceptionApplicationData> GetApplication([FromRoute] string key,
                                                                        [FromServices] IRepositories repositories,
                                                                        [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var manager = new InterceptionManager(repositories, repositoriesFinance, config);

            bool success = manager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);
            if (success)
            {
                if (manager.InterceptionApplication.AppCtgy_Cd == "I01")
                    return Ok(manager.InterceptionApplication);
                else
                    return NotFound($"Error: requested I01 application but found {manager.InterceptionApplication.AppCtgy_Cd} application.");
            }
            else
                return NotFound();

        }

        [HttpPost]
        public ActionResult<InterceptionApplicationData> CreateApplication([FromServices] IRepositories repositories, 
                                                                           [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var interceptionData = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if (interceptionData is null)
                return UnprocessableEntity("Missing or invalid request body.");

            var interceptionManager = new InterceptionManager(interceptionData, repositories, repositoriesFinance, config);
            var appl = interceptionManager.InterceptionApplication;

            bool isCreated = interceptionManager.CreateApplication();
            if (isCreated)
            {
                var appKey = $"{appl.Appl_EnfSrv_Cd}-{appl.Appl_CtrlCd}";
                var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + appKey;
                var rootPath = "http://" + HttpContext.Request.Host.ToString();
                var apiGetURIForNewlyCreatedTracing = new Uri(rootPath + actionPath);

                return Created(apiGetURIForNewlyCreatedTracing, appl);
            }
            else
            {
                return UnprocessableEntity(appl.Messages.GetMessagesForType(MessageType.Error));
            }

        }

        [HttpPut("{key}/Vary")]
        public ActionResult<InterceptionApplicationData> Vary([FromRoute] string key,
                                                              [FromServices] IRepositories repositories,
                                                              [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if ((applKey.EnfSrv != application.Appl_EnfSrv_Cd) || (applKey.CtrlCd != application.Appl_CtrlCd))
                return UnprocessableEntity("Key does not match body.");

            var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
            if (appManager.VaryApplication())
                return Ok(application);
            else
                return UnprocessableEntity(application);
        }

        [HttpPut("{key}/AcceptApplication")]
        public ActionResult<InterceptionApplicationData> AcceptInterception([FromRoute] string key,
                                                                            [FromServices] IRepositories repositories,
                                                                            [FromServices] IRepositories_Finance repositoriesFinance,
                                                                            [FromQuery] DateTime supportingDocsReceiptDate)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if ((applKey.EnfSrv != application.Appl_EnfSrv_Cd) || (applKey.CtrlCd != application.Appl_CtrlCd))
                return UnprocessableEntity("Key does not match body.");
            
            var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);

            if (appManager.AcceptInterception(supportingDocsReceiptDate))
                return Ok(appManager.InterceptionApplication);
            else
                return UnprocessableEntity(appManager.InterceptionApplication);
        }

        [HttpPut("{key}/SINbypass")]
        public ActionResult<InterceptionApplicationData> SINbypass([FromRoute] string key,
                                                                   [FromServices] IRepositories repositories,
                                                                   [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var sinBypassData = APIBrokerHelper.GetDataFromRequestBody<SINBypassData>(Request);

            var application = new InterceptionApplicationData();

            var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);
            appManager.LoadApplication(applKey.EnfSrv, applKey.CtrlCd);

            var sinManager = new ApplicationSINManager(application, appManager);
            sinManager.SINconfirmationBypass(sinBypassData.NewSIN, repositories.CurrentSubmitter, false, sinBypassData.Reason);

            return Ok(application);
        }

    }
}
