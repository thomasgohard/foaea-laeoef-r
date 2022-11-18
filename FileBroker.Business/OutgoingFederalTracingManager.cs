using FileBroker.Common.Helpers;
using System.Text;

namespace FileBroker.Business;

public class OutgoingFederalTracingManager : IOutgoingFileManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }
    private FoaeaSystemAccess FoaeaAccess { get; }

    public OutgoingFederalTracingManager(APIBrokerList apis, RepositoryList repositories, ConfigurationHelper config)
    {
        APIs = apis;
        DB = repositories;

        FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
    }

    public async Task<string> CreateOutputFileAsync(string fileBaseName, List<string> errors)
    {
        errors = new List<string>();

        bool fileCreated = false;

        var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileBaseName);

        var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);

        string newCycle;
        if (processCodes.EnfSrv_Cd == "RC01") // TODO: should be a flag in the FileTable...
            newCycle = fileTableData.Cycle.ToString("000");
        else
            newCycle = fileTableData.Cycle.ToString("000000");

        try
        {
            string newFilePath = fileTableData.Path + fileBaseName + "." + newCycle;
            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }
            await FoaeaAccess.SystemLoginAsync();

            try
            {
                var data = await GetOutgoingDataAsync(fileTableData, processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                         processCodes.EnfSrv_Cd);

                var eventIds = new List<int>();
                string fileContent = GenerateOutputFileContentFromData(data, newCycle, processCodes.EnfSrv_Cd, ref eventIds);

                await File.WriteAllTextAsync(newFilePath, fileContent);
                fileCreated = true;

                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now, fileCreated,
                                                                         "Outbound File created successfully.");

                await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, newCycle.Length);

                await APIs.ApplicationEvents.UpdateOutboundEventDetailAsync(processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                                                            processCodes.EnfSrv_Cd,
                                                                            "OK: Written to " + newFilePath, eventIds);
            }
            finally
            {
                await FoaeaAccess.SystemLogoutAsync();
            }

            return newFilePath;

        }
        catch (Exception e)
        {
            string error = "Error Creating Outbound Data File: " + e.Message;
            errors.Add(error);

            await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now, fileCreated, error);

            await DB.ErrorTrackingTable.MessageBrokerErrorAsync($"File Error: {fileTableData.PrcId} {fileBaseName}",
                                                             "Error creating outbound file", e, displayExceptionError: true);

            return string.Empty;
        }

    }

    private async Task<List<TracingOutgoingFederalData>> GetOutgoingDataAsync(FileTableData fileTableData, string actvSt_Cd,
                                                           int appLiSt_Cd, string enfSrvCode)
    {
        var recMax = await DB.ProcessParameterTable.GetValueForParameterAsync(fileTableData.PrcId, "rec_max");
        int maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);

        var data = await APIs.TracingApplications.GetOutgoingFederalTracingRequestsAsync(maxRecords, actvSt_Cd,
                                                                                         appLiSt_Cd, enfSrvCode);
        return data;
    }

    private static string GenerateOutputFileContentFromData(List<TracingOutgoingFederalData> data,
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

}
