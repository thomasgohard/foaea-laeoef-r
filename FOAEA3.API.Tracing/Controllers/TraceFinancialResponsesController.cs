using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc;
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

                    var values = new Dictionary<string, string>();

                    foreach (var value in finValues)
                    {
                        string fieldName = value.FieldName;
                        string fieldValue = value.FieldValue;

                        var thisCraField = craFields.Where(m => m.CRAFieldName == fieldName).FirstOrDefault();
                        if (thisCraField is not null)
                        {
                            string pdfFieldName;

                            if (year >= 2019)
                                pdfFieldName = thisCraField.CRAFieldCode;
                            else
                                pdfFieldName = thisCraField.CRAFieldCodeOld;

                            if (pdfFieldName == "MaritalStatus")
                            {
                                switch (fieldValue)
                                {
                                    case "01": pdfFieldName = "Married"; break;
                                    case "02": pdfFieldName = "CommonLaw"; break;
                                    case "03": pdfFieldName = "Widowed"; break;
                                    case "04": pdfFieldName = "Divorced"; break;
                                    case "05": pdfFieldName = "Separated"; break;
                                    case "06": pdfFieldName = "Single"; break;
                                }
                                fieldValue = "1";
                            }
                            if (pdfFieldName == "PreferredLanguage")
                            {
                                switch (fieldValue)
                                {
                                    case "E": pdfFieldName = "English"; break;
                                    case "F": pdfFieldName = "French"; break;
                                }
                                fieldValue = "1";
                            }

                            if (!string.IsNullOrEmpty(pdfFieldName))
                            {
                                pdfFieldName = pdfFieldName.ToUpper();

                                if (!values.ContainsKey(pdfFieldName))
                                    values.Add(pdfFieldName, fieldValue);
                            }
                        }
                    }

                    (var fileContent, _) = PdfHelper.FillPdf(template, values, formLanguage == "E");

                    byte[] bytes = fileContent.ToArray();

                    return File(bytes, "application/pdf", $"{templateName}-{year}-{cycle}.pdf");
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
