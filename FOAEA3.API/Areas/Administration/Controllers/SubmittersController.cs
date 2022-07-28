using FOAEA3.Business.Areas.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SubmittersController : ControllerBase
    {

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("Submitters API Version 1.0");

        [HttpGet]
        public ActionResult<List<SubmitterData>> GetSubmitters([FromServices] IRepositories repositories,
                                                               [FromQuery] string provCd = null, [FromQuery] string enfOffCode = null,
                                                               [FromQuery] string enfSrvCode = null, [FromQuery] string onlyActive = null)
        {
            var submitterManager = new SubmitterManager(repositories);

            if ((enfSrvCode is null) && (enfOffCode is null))
                return Ok(submitterManager.GetSubmittersForProvince(provCd, onlyActive == "true"));
            else
                return Ok(submitterManager.GetSubmittersForProvinceAndOffice(provCd, enfOffCode, enfSrvCode, onlyActive == "true"));
        }

        [HttpGet("{submCd}")]
        public ActionResult<SubmitterData> GetSubmitter([FromRoute] string submCd, [FromServices] IRepositories repositories)
        {
            var submitterManager = new SubmitterManager(repositories);
            var submitter = submitterManager.GetSubmitter(submCd);

            if (submitter != null)
            {
                return Ok(submitter);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet("{submCd}/Declarant")]
        public ActionResult<string> GetDeclarant([FromRoute] string submCd, [FromServices] IRepositories repositories)
        {
            var submitterManager = new SubmitterManager(repositories);
            var submitter = submitterManager.GetSubmitter(submCd);

            string declarant = string.Empty;
            if (submitter != null)
            {
                declarant = submitter.Subm_FrstNme.Trim() + " " + submitter.Subm_SurNme.Trim();
            }

            return declarant;
        }

        [HttpGet("commissioners/{enfOffLocCode}")]
        public ActionResult<List<CommissionerData>> GetCommissioners([FromServices] IRepositories repositories,
                                                                     [FromRoute] string enfOffLocCode = null)
        {
            var submitterManager = new SubmitterManager(repositories);

            return Ok(submitterManager.GetCommissioners(enfOffLocCode, repositories.CurrentSubmitter));

        }

        [HttpPost]
        public ActionResult<TracingApplicationData> CreateSubmitter([FromBody] string sourceSubmitterData, [FromQuery] string suffixCode, [FromQuery] string readOnlyAccess,
                                                                    [FromServices] IRepositories repositories)
        {
            var submitterData = JsonConvert.DeserializeObject<SubmitterData>(sourceSubmitterData);

            var submitterManager = new SubmitterManager(repositories);
            submitterData = submitterManager.CreateSubmitter(submitterData, suffixCode, readOnlyAccess == "true");

            if (!submitterData.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                var actionPath = HttpContext.Request.Path.Value + Path.AltDirectorySeparatorChar + submitterData.Subm_SubmCd;
                var getURI = new Uri("http://" + HttpContext.Request.Host.ToString() + actionPath);

                return Created(getURI, submitterData);
            }
            else
            {
                return UnprocessableEntity(submitterData);
            }

        }

        [HttpPut]
        public ActionResult<TracingApplicationData> UpdateSubmitter([FromServices] IRepositories repositories)
        {
            var submitterData = APIBrokerHelper.GetDataFromRequestBody<SubmitterData>(Request);

            bool readOnly = ((submitterData.Subm_Class == "RO") || (submitterData.Subm_Class == "R1"));

            var submitterManager = new SubmitterManager(repositories);
            submitterData = submitterManager.UpdateSubmitter(submitterData, readOnly);

            return Ok(submitterData);

        }

    }
}
