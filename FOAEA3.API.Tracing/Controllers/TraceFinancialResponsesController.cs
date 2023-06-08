using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Outgoing.FileCreator.Fed.Tracing;

namespace FOAEA3.API.Tracing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TraceFinancialResponsesController : FoaeaControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<DataList<TraceFinancialResponseData>>> GetTraceFinancialResults([FromRoute] ApplKey id,
                                                                                    [FromServices] IRepositories repositories)
        {
            var manager = new TracingManager(repositories, config, User);

            if (await manager.LoadApplication(id.EnfSrv, id.CtrlCd))
                return Ok(await manager.GetTraceFinancialResults());
            else
                return NotFound();
        }

        [HttpGet("{id}/pdf")]
        public async Task<ActionResult> GetCRAform([FromRoute] ApplKey id, [FromServices] IRepositories repositories,
                                                   [FromQuery] short year, [FromQuery] string form, [FromQuery] short cycle)
        {
            var manager = new TracingManager(repositories, config, User);

            if (await manager.LoadApplication(id.EnfSrv, id.CtrlCd))
            {
                var finResults = (await manager.GetTraceFinancialResults()).Items;
                var finDetails = finResults?.Where(m => m.TrcRsp_Trace_CyclNr == cycle)?.FirstOrDefault()?.TraceFinancialDetails;
                var finValues = finDetails?.Where(m => m.FiscalYear == year && m.TaxForm == form)?.FirstOrDefault()?.TraceDetailValues;

                if (finValues is not null)
                {                    
                    var craFields = await manager.GetCraFields();
                    var craForms = await manager.GetCraForms();

                    string province = "ON";
                    string language = "E";
                    
                    string templateName = craForms.Where(m => m.CRAFormProvince == province && m.CRAFormLanguage == language && 
                                                              m.CRAFormYear == year && m.CRAFormSchedule == form)
                                                  .FirstOrDefault()?
                                                  .CRAFormPDFName;

                    string template = @$"C:\CRATaxForms\{year}\{templateName}.pdf";

                    var values = new Dictionary<string, string>();

                    foreach (var value in finValues)
                    {
                        string fieldName = value.FieldName;
                        string fieldValue = value.FieldValue;

                        var thisCraField = craFields.Where(m => m.CRAFieldName == fieldName).FirstOrDefault();
                        if (thisCraField is not null)
                        {
                            string thisLineNumber;
                            
                            if (year >= 2019)
                                thisLineNumber = thisCraField.CRAFieldCode;
                            else
                                thisLineNumber = thisCraField.CRAFieldCodeOld;

                            if (!string.IsNullOrEmpty(thisLineNumber))
                            {
                                if (!values.ContainsKey(thisLineNumber))
                                    values.Add(thisLineNumber, fieldValue);
                            }
                        }                        
                    }

                    (var fileContent, _) = PdfHelper.FillPdf(template, values);

                    byte[] bytes = fileContent.ToArray();

                    return File(bytes, "application/pdf", $"{templateName}-{year}-{cycle}.pdf");
                }
            }

            return null;
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateTraceFinancialResponses([FromServices] IRepositories repositories)
        {
            var responseData = await APIBrokerHelper.GetDataFromRequestBody<TraceFinancialResponseData>(Request);

            var tracingManager = new TracingManager(repositories, config, User);

            await tracingManager.CreateFinancialResponseData(responseData);

            var rootPath = "https://" + HttpContext.Request.Host.ToString();

            return Created(rootPath, new TraceResponseData());
        }
    }
}
