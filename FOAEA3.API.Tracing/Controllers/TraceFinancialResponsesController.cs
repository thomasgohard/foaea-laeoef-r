using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

                    string province = id.EnfSrv[0..2];

                    string headerLanguage = GetLanguageFromHeader(Request.Headers);

                    string formLanguage;
                    string templateLanguage;
                    if (headerLanguage == "fr")
                    {
                        formLanguage = "F";
                        templateLanguage = "French";
                    }
                    else
                    {
                        formLanguage = "E";
                        templateLanguage = "English";
                    }

                    string formShortName = FormHelper.ConvertTaxFormFullNameToAbbreviation(form);

                    string templateName = craForms.Where(m => m.CRAFormProvince == province && m.CRAFormLanguage == formLanguage &&
                                                              m.CRAFormYear == year && m.CRAFormSchedule == formShortName)
                                                  .FirstOrDefault()?
                                                  .CRAFormPDFName;

                    string template = config.TaxFormsRootPath.AppendToPath(@$"{templateLanguage}\{year}\{templateName}.pdf", isFileName: true);
                    var values = PdfHelper.GetValuesForPDF(year, finValues, craFields);

                    // TODO: send email to FLAS-IT-SO about missing fields?

                    (var fileContent, var missingFields) = PdfHelper.FillPdf(template, values, formLanguage == "E");

                    return File(fileContent.ToArray(), "application/pdf", $"{templateName}-{year}-{cycle}.pdf");
                }
            }

            return NotFound();
        }

        private string GetLanguageFromHeader(IHeaderDictionary headers)
        {
            if ((headers is not null) && headers.ContainsKey("Accept-Language"))
            {
                var languageHeader = Request.Headers["Accept-Language"];
                return languageHeader[0]?.ToLower() ?? "en";
            }
            else
                return "en";
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
