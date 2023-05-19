using FileBroker.Common.Helpers;
using System.Diagnostics.Eventing.Reader;
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

    public async Task<(string, List<string>)> CreateOutputFileAsync(string fileBaseName)
    {
        var errors = new List<string>();

        bool fileCreated = false;

        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileBaseName);

        var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);

        string newCycle;
        if (processCodes.EnfSrv_Cd == "RC01") // TODO: should be a flag in the FileTable...
            newCycle = fileTableData.Cycle.ToString("000");
        else
            newCycle = fileTableData.Cycle.ToString("000000");

        try
        {
            string newFilePath = fileTableData.Path + fileBaseName + "." + newCycle;
            if (processCodes.EnfSrv_Cd == "RC02")
                newFilePath += ".XML";
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
                var data = await GetOutgoingDataAsync(fileTableData, processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                                      processCodes.EnfSrv_Cd);

                if (!data.Any())
                {
                    return ("", errors); /// nothing to generate
                }

                var eventIds = new List<int>();
                string fileContent = string.Empty;

                if (processCodes.EnfSrv_Cd != "RC02")
                {
                    // tracing requests
                    fileContent = GenerateTraceOutputFileContentFromData(data, newCycle, processCodes.EnfSrv_Cd, ref eventIds);
                }
                else
                {
                    // financial requests
                    (fileContent, eventIds) = await GenerateFinancialOutputFileContentFromData(data, newCycle, processCodes.EnfSrv_Cd, eventIds);
                }

                await File.WriteAllTextAsync(newFilePath, fileContent);
                fileCreated = true;

                string errorDoingBackup = await FileHelper.BackupFile(newFilePath, DB, Config);

                if (!string.IsNullOrEmpty(errorDoingBackup))
                    await DB.ErrorTrackingTable.MessageBrokerErrorAsync($"File Error: {newFilePath}",
                                                                        "Error creating backup of outbound file: " + errorDoingBackup);

                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now, fileCreated,
                                                                         "Outbound File created successfully.");

                await DB.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

                await APIs.ApplicationEvents.UpdateOutboundEventDetailAsync(processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                                                            processCodes.EnfSrv_Cd,
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

            await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now, fileCreated, error);

            await DB.ErrorTrackingTable.MessageBrokerErrorAsync($"File Error: {fileTableData.PrcId} {fileBaseName}",
                                                             "Error creating outbound file", e, displayExceptionError: true);

            return (string.Empty, errors);
        }

    }

    private async Task<List<TracingOutgoingFederalData>> GetOutgoingDataAsync(FileTableData fileTableData, string actvSt_Cd,
                                                           int appLiSt_Cd, string enfSrvCode)
    {
        int maxRecords = 0;
        try
        {
            var recMax = await DB.ProcessParameterTable.GetValueForParameterAsync(fileTableData.PrcId, "rec_max");
            maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);
        }
        catch (Exception e)
        {
            throw new Exception("Error getting rec_max value. Set to 0.");
        }

        var data = await APIs.TracingApplications.GetOutgoingFederalTracingRequestsAsync(maxRecords, actvSt_Cd,
                                                                                         appLiSt_Cd, enfSrvCode);
        return data;
    }

    private static string GenerateTraceOutputFileContentFromData(List<TracingOutgoingFederalData> data,
                                                            string newCycle, string enfSrvCode, ref List<int> eventIds)
    {
        var result = new StringBuilder();

        result.AppendLine(GenerateHeaderLine(newCycle));
        foreach (var item in data)
        {
            result.AppendLine(GenerateDetailLine(item, enfSrvCode));
            eventIds.Add(item.Event_dtl_Id);
        }
        result.AppendLine(GenerateFooterLine(data.Count));

        return result.ToString();
    }

    private static string GenerateHeaderLine(string newCycle)
    {
        string julianDate = DateTime.Now.AsJulianString();

        return $"01{newCycle}{julianDate}";
    }

    private static string GenerateDetailLine(TracingOutgoingFederalData item, string enfSrvCode)
    {
        string result = $"02{item.Appl_Dbtr_Cnfrmd_SIN,9}{item.Appl_EnfSrv_Cd,6}{item.Appl_CtrlCd,6}";

        if (enfSrvCode == "RC01")
            result += $"{item.ReturnType}";

        return result;
    }

    private static string GenerateFooterLine(int rowCount)
    {
        return $"99{rowCount:000000}";
    }

    private async Task<(string, List<int>)> GenerateFinancialOutputFileContentFromData(List<TracingOutgoingFederalData> data,
                                                                     string newCycle, string enfSrvCode, List<int> eventIds)
    {
        var result = new StringBuilder();

        result.AppendLine("<?xml version='1.0' encoding='utf-8'?>");
        result.AppendLine("<CRATraceOut>");

        result.AppendLine(GenerateFinHeaderLine(newCycle));
        foreach (var item in data)
        {
            var appl = await APIs.TracingApplications.GetApplicationAsync(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);

            result.AppendLine(GenerateFinDetailLine(item, appl));
            eventIds.Add(item.Event_dtl_Id);
        }
        result.AppendLine(GenerateFinFooterLine(data.Count));

        return (result.ToString(), eventIds);
    }

    private static string GenerateFinHeaderLine(string newCycle)
    {
        string xmlCreationDateTime = DateTime.Now.ToString("o");

        var output = new StringBuilder();
        output.AppendLine($"<Header>");
        output.AppendLine($"  <Cycle>{newCycle}</Cycle>");
        output.AppendLine($"  <FileDate>{xmlCreationDateTime}</FileDate>");
        output.Append($"</Header>");

        return output.ToString();
    }

    private static string GenerateFinDetailLine(TracingOutgoingFederalData item, TracingApplicationData appl)
    {
        string xmlDebtorBirthDate = appl.Appl_Dbtr_Brth_Dte?.ToString("o");

        string entity = "04"; // 04 for MEP, courts can be 01 or 02 -- not sure how to identify this
        if (appl.Subm_SubmCd[2] == 'C')
            entity = "01"; // court

        bool wantSinOnly = !appl.IncludeFinancialInformation && appl.IncludeSinInformation;

        var output = new StringBuilder();
        output.AppendLine($"<Trace_Request>");
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
                    output.AppendLine($"      </Tax_Data>");
                    output.AppendLine($"          <Tax_Year>{year}</Tax_Year>");
                    foreach (var form in taxForms)
                        output.AppendLine($"         <Tax_Form>{form}</Tax_Form>");
                    output.AppendLine($"      </Tax_Data>");
                }
            }
        }
        else
        {
            output.AppendLine($"   <Tax_Year />");
        }

        output.Append($"</Trace_Request>");

        return output.ToString();
    }

    private static string GenerateFinFooterLine(int count)
    {
        var output = new StringBuilder();
        output.AppendLine($"<Footer>");
        output.AppendLine($"  <RequestCount>{count:000000}</RequestCount>");
        output.Append($"</Footer>");

        return output.ToString();
    }
}
