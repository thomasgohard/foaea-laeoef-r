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
        public ActionResult<string> Version() => Ok("Interceptions API Version 1.0");

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

        [HttpGet("GetApplicationsForVariationAutoAccept")]
        public ActionResult<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAccept(
                                                                            [FromServices] IRepositories repositories,
                                                                            [FromServices] IRepositories_Finance repositoriesFinance,
                                                                            [FromQuery] string enfService)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var interceptionManager = new InterceptionManager(repositories, repositoriesFinance, config);
            var data = interceptionManager.GetApplicationsForVariationAutoAccept(enfService);

            return Ok(data);
        }

        [HttpPost]
        public ActionResult<InterceptionApplicationData> CreateApplication([FromServices] IRepositories repositories,
                                                                           [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey: null, out string error))
                return UnprocessableEntity(error);

            var interceptionManager = new InterceptionManager(application, repositories, repositoriesFinance, config);

            bool isCreated = interceptionManager.CreateApplication();
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
        public ActionResult<InterceptionApplicationData> UpdateApplication(
                                                        [FromRoute] string key,
                                                        [FromQuery] string command,
                                                        [FromQuery] string enforcementServiceCode,
                                                        [FromServices] IRepositories repositories,
                                                        [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey, out string error))
                return UnprocessableEntity(error);

            var interceptionManager = new InterceptionManager(application, repositories, repositoriesFinance, config);

            if (string.IsNullOrEmpty(command))
                command = "";

            switch (command.ToLower())
            {
                case "":
                    interceptionManager.UpdateApplication();
                    break;

                case "partiallyserviceapplication":
                    // TODO: interceptionManager.PartiallyServiceApplication(enforcementServiceCode);
                    break;

                case "fullyserviceapplication":
                    // TODO: interceptionManager.FullyServiceApplication(enforcementServiceCode);
                    break;

                default:
                    application.Messages.AddSystemError($"Unknown command: {command}");
                    return UnprocessableEntity(application);
            }

            if (!interceptionManager.InterceptionApplication.Messages.ContainsMessagesOfType(MessageType.Error))
                return Ok(application);
            else
                return UnprocessableEntity(application);

        }

        [HttpPut("ValidateFinancialCoreValues")]
        public ActionResult<ApplicationData> ValidateFinancialCoreValues([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var appl = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);
            var interceptionValidation = new InterceptionValidation(appl, repositories, config);

            bool isValid = interceptionValidation.ValidateFinancialCoreValues();

            if (isValid)
                return Ok(appl);
            else
                return UnprocessableEntity(appl);
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

        [HttpPut("{key}/Vary")]
        public ActionResult<InterceptionApplicationData> Vary([FromRoute] string key,
                                                              [FromServices] IRepositories repositories,
                                                              [FromServices] IRepositories_Finance repositoriesFinance)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey, out string error))
                return UnprocessableEntity(error);

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

            if (!APIHelper.ValidateApplication(application, applKey, out string error))
                return UnprocessableEntity(error);

            var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);

            if (appManager.AcceptInterception(supportingDocsReceiptDate))
                return Ok(application);
            else
                return UnprocessableEntity(application);
        }

        [HttpPut("{key}/AcceptVariation")]
        public ActionResult<InterceptionApplicationData> AcceptVariation([FromRoute] string key,
                                                                         [FromServices] IRepositories repositories,
                                                                         [FromServices] IRepositories_Finance repositoriesFinance,
                                                                         [FromQuery] DateTime supportingDocsReceiptDate)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey, out string error))
                return UnprocessableEntity(error);

            var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);

            if (appManager.AcceptVariation(supportingDocsReceiptDate))
                return Ok(application);
            else
                return UnprocessableEntity(application);
        }

        [HttpPut("{key}/RejectVariation")]
        public ActionResult<InterceptionApplicationData> RejectVariation([FromRoute] string key,
                                                                         [FromServices] IRepositories repositories,
                                                                         [FromServices] IRepositories_Finance repositoriesFinance,
                                                                         [FromQuery] string applicationRejectReasons)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var applKey = new ApplKey(key);

            var application = APIBrokerHelper.GetDataFromRequestBody<InterceptionApplicationData>(Request);

            if (!APIHelper.ValidateApplication(application, applKey, out string error))
                return UnprocessableEntity(error);

            var appManager = new InterceptionManager(application, repositories, repositoriesFinance, config);

            if (appManager.RejectVariation(applicationRejectReasons))
                return Ok(application);
            else
                return UnprocessableEntity(application);
        }

    }
}
