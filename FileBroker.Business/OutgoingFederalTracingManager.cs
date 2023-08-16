using FileBroker.Common.Helpers;
using FOAEA3.Common.Helpers;
using System.Text;

namespace FileBroker.Business;

public class OutgoingFederalTracingManager : IOutgoingFileManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }
    private FoaeaSystemAccess FoaeaAccess { get; }
    private IFileBrokerConfigurationHelper Config { get; }

    public OutgoingFederalTracingManager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
    {
        APIs = apis;
        DB = repositories;
        Config = config;

        FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
    }

    public async Task<(string, List<string>)> CreateOutputFile(string fileBaseName)
    {
        var errors = new List<string>();

        bool fileCreated = false;

        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileBaseName);

        var processCodes = await DB.ProcessParameterTable.GetProcessCodes(fileTableData.PrcId);
        string federalEnfServiceCode = processCodes.EnfSrv_Cd;

        string newCycle;
        if (federalEnfServiceCode == "RC01")
            newCycle = fileTableData.Cycle.ToString("000");
        else
            newCycle = fileTableData.Cycle.ToString("000000");

        try
        {
            string newFilePath;
            if (federalEnfServiceCode != "RC02")
                newFilePath = $"{fileTableData.Path}{fileBaseName}.{newCycle}";
            else
                newFilePath = $"{fileTableData.Path}{fileBaseName}.{newCycle}.XML";

            if (File.Exists(newFilePath))
            {
                errors.Add($"** Error: File Already Exists ({newFilePath})");
                return (newFilePath, errors);
            }

            if (!await FoaeaAccess.SystemLogin())
            {
                errors.Add("Failed to login to FOAEA!");
                return (newFilePath, errors);
            }
            try
            {
                var data = await GetOutgoingDataFromFoaea(fileTableData, processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                                          federalEnfServiceCode);
                if (!data.Any())
                {
                    errors.Add("No data found!?");
                    return ("", errors);
                }

                var eventIds = new List<int>();
                string fileContent = string.Empty;

                if (federalEnfServiceCode != "RC02")
                    (fileContent, eventIds) = GenerateTraceOutputFileContentFromData(data, newCycle, federalEnfServiceCode);
                else
                    (fileContent, eventIds) = await GenerateFinancialOutputFileContentFromData(data, newCycle);

                await File.WriteAllTextAsync(newFilePath, fileContent);
                fileCreated = true;

                string errorMessage = await FileHelper.BackupFile(newFilePath, DB, Config);

                if (!string.IsNullOrEmpty(errorMessage))
                    await DB.ErrorTrackingTable.MessageBrokerError($"File Error: {newFilePath}",
                                                                   "Error creating backup of outbound file: " + errorMessage);

                await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now, fileCreated,
                                                                    "Outbound File created successfully.");

                await DB.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

                await APIs.ApplicationEvents.UpdateOutboundEventDetail(processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd, federalEnfServiceCode,
                                                                       "OK: Written to " + newFilePath, eventIds);
            }
            finally
            {
                await FoaeaAccess.SystemLogout();
            }

            return (newFilePath, errors);
        }
        catch (Exception e)
        {
            string error = "Error Creating Outbound Data File: " + e.Message;
            errors.Add(error);

            await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now, fileCreated, error);

            await DB.ErrorTrackingTable.MessageBrokerError($"File Error: {fileTableData.PrcId} {fileBaseName}",
                                                           "Error creating outbound file", e, displayExceptionError: true);

            return (string.Empty, errors);
        }
    }

    private async Task<List<TracingOutgoingFederalData>> GetOutgoingDataFromFoaea(FileTableData fileTableData, string actvSt_Cd,
                                                                                  int appLiSt_Cd, string enfSrvCode)
    {
        int maxRecords;
        try
        {
            var recMax = await DB.ProcessParameterTable.GetValueForParameter(fileTableData.PrcId, "rec_max");
            maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);
        }
        catch
        {
            throw new Exception("Error getting rec_max value. Set to 0.");
        }

        var data = await APIs.TracingApplications.GetOutgoingFederalTracingRequests(maxRecords, actvSt_Cd, appLiSt_Cd, enfSrvCode);

        return data;
    }

    private static (string, List<int>) GenerateTraceOutputFileContentFromData(List<TracingOutgoingFederalData> data,
                                                                              string newCycle, string enfSrvCode)
    {
        var result = new StringBuilder();
        var eventIds = new List<int>();

        result.AppendLine(GenerateTraceHeaderLine(newCycle));
        foreach (var item in data)
        {
            result.AppendLine(GenerateTraceDetailLine(item, enfSrvCode));
            eventIds.Add(item.Event_dtl_Id);
        }
        result.AppendLine(GenerateTraceFooterLine(data.Count));

        return (result.ToString(), eventIds);
    }

    private static string GenerateTraceHeaderLine(string newCycle)
    {
        string julianDate = DateTime.Now.AsJulianString();

        return $"01{newCycle}{julianDate}";
    }

    private static string GenerateTraceDetailLine(TracingOutgoingFederalData item, string enfSrvCode)
    {
        string result = $"02{item.Appl_Dbtr_Cnfrmd_SIN,9}{item.Appl_EnfSrv_Cd,6}{item.Appl_CtrlCd,6}";

        if (enfSrvCode == "RC01")
            result += $"{item.ReturnType}";

        return result;
    }

    private static string GenerateTraceFooterLine(int rowCount)
    {
        return $"99{rowCount:000000}";
    }

    private async Task<(string, List<int>)> GenerateFinancialOutputFileContentFromData(List<TracingOutgoingFederalData> data,
                                                                                       string newCycle)
    {
        var result = new StringBuilder();
        var eventIds = new List<int>();

        result.AppendLine("<?xml version='1.0' encoding='utf-8'?>");
        result.AppendLine("<CRATraceOut>");

        result.AppendLine(GenerateFinancialsHeaderLine(newCycle));
        foreach (var item in data)
        {
            var appl = await APIs.TracingApplications.GetApplication(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);
            result.AppendLine(GenerateFinancialsDetailLine(appl));
            eventIds.Add(item.Event_dtl_Id);
        }
        result.AppendLine(GenerateFinancialsFooterLine(data.Count));

        result.AppendLine("</CRATraceOut>");

        return (result.ToString(), eventIds);
    }

    private static string GenerateFinancialsHeaderLine(string newCycle)
    {
        string xmlCreationDateTime = DateTime.Now.ToString("o");

        var output = new StringBuilder();
        output.AppendLine($"<Header>");
        output.AppendLine($"  <Cycle>{newCycle}</Cycle>");
        output.AppendLine($"  <FileDate>{xmlCreationDateTime}</FileDate>");
        output.Append($"</Header>");

        return output.ToString();
    }

    private static string GenerateFinancialsDetailLine(TracingApplicationData appl)
    {
        string xmlDebtorBirthDate = appl.Appl_Dbtr_Brth_Dte?.ToString("o");

        bool wantSinOnly = false;
        string entity;
        switch (appl.Subm_SubmCd[2])
        {
            case 'C': // court
                if (appl.AppReas_Cd == "1")
                    entity = "01";
                else
                    entity = "02";
                break;

            case 'S': // Provincial Child Support Services
                entity = "03";
                break;

            default: // MEP
                entity = "04";
                wantSinOnly = !appl.IncludeFinancialInformation && appl.IncludeSinInformation;
                break;
        }

        var output = new StringBuilder();
        output.AppendLine($"<TraceRequest>");
        if (wantSinOnly)
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("EntityType", "01")); // special case, send a 01 for entity but treat it like a MEP (04)
        else
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("EntityType", entity));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_SurNme", appl.Appl_Dbtr_SurNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_FrstNme", appl.Appl_Dbtr_FrstNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Brth_Dte", xmlDebtorBirthDate));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Cnfrmd_SIN", appl.Appl_Dbtr_Cnfrmd_SIN));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_EnfSrvCd", appl.Appl_EnfSrv_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_CtrlCd", appl.Appl_CtrlCd));

        if (entity == "01")
        {
            if (appl.YearsAndTaxForms is not null)
            {
                foreach (var taxYear in appl.YearsAndTaxForms)
                {
                    short year = taxYear.Key;
                    var taxForms = taxYear.Value;
                    output.AppendLine($"      <Tax_Data>");
                    output.AppendLine($"         <Tax_Year>{year}</Tax_Year>");
                    foreach (var form in taxForms)
                    {
                        string shortName = FormHelper.ConvertTaxFormFullNameToAbbreviation(form);
                        output.AppendLine($"         <Tax_Form>{shortName}</Tax_Form>");
                    }
                    output.AppendLine($"      </Tax_Data>");
                }
            }
        }
        else
        {
            output.AppendLine($"   <Tax_Year />");
        }

        output.Append($"</TraceRequest>");

        return output.ToString();
    }

    private static string GenerateFinancialsFooterLine(int count)
    {
        var output = new StringBuilder();
        output.AppendLine($"<Footer>");
        output.AppendLine($"  <RequestCount>{count:000000}</RequestCount>");
        output.Append($"</Footer>");

        return output.ToString();
    }

}
