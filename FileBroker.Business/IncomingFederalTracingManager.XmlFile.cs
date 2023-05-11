using Newtonsoft.Json;

namespace FileBroker.Business;

public partial class IncomingFederalTracingManager
{
    public async Task<MessageDataList> ProcessXmlFileAsync(string sourceTracingDataAsJson, string flatFileName)
    {
        var result = new MessageDataList();

        short cycle = (short)FileHelper.ExtractCycleFromFilename(flatFileName);
        var fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);
        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

        await DB.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

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
                    return result;
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

        return result;
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
        string cutOffDaysValue = await DB.ProcessParameterTable.GetValueForParameterAsync(processId, "evnt_cutoff");
        int cutOffDays = int.Parse(cutOffDaysValue);

        var activeTraceEvents = await APIs.TracingEvents.GetRequestedTRCINEventsAsync(enfSrvCd, fileCycle.ToString());
        var activeTraceEventDetails = await APIs.TracingEvents.GetActiveTracingEventDetailsAsync(enfSrvCd, fileCycle.ToString());

        foreach (var response in traceResponses)
        {
            var item = ConvertCraResponseToFoaeaResponseData(response, fileCycle);
            await APIs.TracingResponses.AddTraceFinancialResponseData(item);

            var appl = await APIs.TracingApplications.GetApplicationAsync(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);
            await MarkTraceEventsAsProcessedAsync(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd, flatFileName, (short)appl.AppLiSt_Cd,
                                                  activeTraceEvents, activeTraceEventDetails);
        }

        await CloseOrInactivateTraceEventDetailsAsync(cutOffDays, activeTraceEventDetails);
        await UpdateTracingApplicationsAsync(enfSrvCd, fileCycle.ToString());
        await CloseOrInactivateTraceEventDetailsAsync(cutOffDays, activeTraceEventDetails);
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

    private static TraceFinancialResponseData ConvertCraResponseToFoaeaResponseData(FedTracingFinancial_TraceResponse traceResponse, short cycle)
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
                var details = new TraceFinancialResponseDetailData
                {
                    TaxForm = taxData.Form
                };
                if (short.TryParse(taxData.Year, out short fiscalYear))
                    details.FiscalYear = fiscalYear;

                if (taxData.Field is not null)
                {
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

                result.TraceFinancialDetails.Add(details);
            }
        }

        return result;
    }


}
