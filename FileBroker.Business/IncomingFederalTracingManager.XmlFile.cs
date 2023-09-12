using DBHelper;
using Newtonsoft.Json;

namespace FileBroker.Business;

public partial class IncomingFederalTracingManager
{
    public async Task<List<string>> ProcessXmlData(string xmlFileContent, string flatFileName)
    {
        var errors = new List<string>();
        string sourceTracingDataAsJson = FileHelper.ConvertXmlToJson(xmlFileContent, errors);

        if (errors.Any())
            return errors;

        var result = new MessageDataList();

        short cycle = (short)FileHelper.ExtractCycleFromFilename(flatFileName);
        var fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);
        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

        var tracingFileData = ExtractTracingFinancialDataFromJson(sourceTracingDataAsJson, out string error);
        var tracingFile = tracingFileData.CRATraceIn;

        bool isValid = true;

        if (!string.IsNullOrEmpty(error))
        {
            result.AddSystemError(error);
        }
        else
        {
            ValidateXmlHeader(tracingFile.Header, flatFileName, ref result, ref isValid);
            ValidateXmlFooter(tracingFile, ref result, ref isValid);

            if (isValid)
            {
                if (!await FoaeaAccess.SystemLogin())
                {
                    result.AddError("Failed to login to FOAEA!");
                    return result.Select(m => m.Description).ToList();
                }
                try
                {
                    await SendTraceFinancialResultToFoaea(tracingFile.TraceResponse, fileTableData.PrcId, "RC02", cycle, fileNameNoCycle);
                }
                finally
                {
                    await FoaeaAccess.SystemLogout();
                }
            }
        }

        return result.Select(m => m.Description).ToList();
    }

    private static void ValidateXmlHeader(FedTracingFinancial_Header header, string flatFileName, ref MessageDataList result, ref bool isValid)
    {
        int cycle = FileHelper.ExtractCycleFromFilename(flatFileName);
        if (header.Cycle != cycle)
        {
            result.AddError($"Cycle in file [{header.Cycle}] does not match cycle of file [{cycle}]");
            isValid = false;
        }
    }

    private static void ValidateXmlFooter(FedTracingFinancial_CRATraceIn tracingFile, ref MessageDataList result, ref bool isValid)
    {
        if (tracingFile.TraceResponse.Count != tracingFile.Footer.ResponseCount)
        {
            result.AddError("Invalid ResponseCount in footer section");
            isValid = false;
        }
    }

    private async Task SendTraceFinancialResultToFoaea(List<FedTracingFinancial_TraceResponse> traceResponses,
                                                       int processId, string enfSrvCd, short fileCycle,
                                                       string flatFileName)
    {
        string cutOffDaysValue = await DB.ProcessParameterTable.GetValueForParameter(processId, "evnt_cutoff");
        int cutOffDays = int.Parse(cutOffDaysValue);

        var activeTraceEvents = await APIs.TracingEvents.GetRequestedTRCINEvents(enfSrvCd, fileCycle.ToString());

        // WARNING: ˅ ˅ ˅ for testing only! ˅ ˅ ˅ */
        activeTraceEvents.RemoveAll(m => m.Appl_CtrlCd.Trim().NotIn("A44", "A488", "A489", "A490", "A491", "A492", "A493", "A494", "A495", "A496", "A497"));
        // WARNING: ˄ ˄ ˄ for testing only! ˄ ˄ ˄ */

        var activeTraceEventDetails = await APIs.TracingEvents.GetActiveTracingEventDetails(enfSrvCd, fileCycle.ToString());

        foreach (var response in traceResponses)
        {
            var item = ConvertCraResponseToFoaeaResponseData(response, 0);
            var appl = await APIs.TracingApplications.GetApplication(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);
            item.TrcRsp_Trace_CyclNr = (short)appl.Trace_Cycl_Qty;

            await APIs.TracingResponses.AddTraceFinancialResponseData(item);
            await MarkTraceEventsAsProcessed(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd, flatFileName, (short)appl.AppLiSt_Cd,
                                             activeTraceEvents, activeTraceEventDetails);
        }

        await CloseOrInactivateTraceEventDetails(cutOffDays, activeTraceEventDetails);
        await UpdateTracingApplications(enfSrvCd, fileCycle.ToString(), FederalSource.CRA_TracingFinancials);
        await CloseOrInactivateTraceEventDetails(cutOffDays, activeTraceEventDetails);
    }

    private static FedTracingFinancialFileBase ExtractTracingFinancialDataFromJson(string sourceTracingData, out string error)
    {
        error = string.Empty;

        FedTracingFinancialFileBase result;
        try
        {
            result = JsonConvert.DeserializeObject<FedTracingFinancialFileBase>(sourceTracingData);
        }
        catch (Exception e)
        {
            error = e.Message;
            result = new FedTracingFinancialFileBase();
        }

        return result;
    }

    private static TraceFinancialResponseData ConvertCraResponseToFoaeaResponseData(FedTracingFinancial_TraceResponse traceResponse, 
                                                                                    short cycle)
    {
        var result = new TraceFinancialResponseData
        {
            Appl_EnfSrv_Cd = traceResponse.Appl_EnfSrvCd,
            Appl_CtrlCd = traceResponse.Appl_CtrlCd,
            EnfSrv_Cd = "RC02",
            TrcRsp_Rcpt_Dte = DateTime.Now,
            TrcRsp_SeqNr = 0,
            TrcRsp_Trace_CyclNr = cycle,
            ActvSt_Cd = "A",
            Sin = traceResponse.SIN,
            SinXref = traceResponse.SIN_XRef,
            TrcSt_Cd = traceResponse.ResponseCode
        };

        if (traceResponse.Tax_Response.Tax_Data is not null)
        {
            result.TraceFinancialDetails = new List<TraceFinancialResponseDetailData>();

            foreach (var taxData in traceResponse.Tax_Response.Tax_Data)
            {
                string thisForm = taxData.Form.ToUpper();
                if (thisForm == "ID")
                    thisForm = "T1";
                if (short.TryParse(taxData.Year, out short thisFiscalYear))
                {
                    TraceFinancialResponseDetailData details;
                    bool isNewDetails = false;
                    var existingDetails = result.TraceFinancialDetails.Find(m => m.TaxForm == thisForm && m.FiscalYear == thisFiscalYear);
                    if (existingDetails is null)
                    {
                        isNewDetails = true;
                        details = new TraceFinancialResponseDetailData
                        {
                            TaxForm = thisForm,
                            FiscalYear = thisFiscalYear
                        };
                    }
                    else
                        details = existingDetails;

                    if (taxData.Field is not null)
                    {
                        if (isNewDetails || details.TraceDetailValues is null)
                            details.TraceDetailValues = new List<TraceFinancialResponseDetailValueData>();

                        foreach (var valueData in taxData.Field)
                        {
                            var detailValue = new TraceFinancialResponseDetailValueData
                            {
                                FieldName = valueData.Name,
                                FieldValue = valueData.Value
                            };
                            details.TraceDetailValues.Add(detailValue);
                        }
                    }

                    if (isNewDetails)
                        result.TraceFinancialDetails.Add(details);  
                }
            }
        }

        return result;
    }


}
