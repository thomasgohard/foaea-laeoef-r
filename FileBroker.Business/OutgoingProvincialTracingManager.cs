using FileBroker.Common.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace FileBroker.Business;

public class OutgoingProvincialTracingManager : IOutgoingFileManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }
    private FoaeaSystemAccess FoaeaAccess { get; }

    public OutgoingProvincialTracingManager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
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

        string newCycle = fileTableData.Cycle.ToString("000000");

        try
        {
            var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);

            string newFilePath = fileTableData.Path + fileBaseName + "." + newCycle + ".xml";
            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            await FoaeaAccess.SystemLoginAsync();

            try
            {
                var data = await GetOutgoingDataAsync(fileTableData, processCodes.ActvSt_Cd, processCodes.SubmRecptCd);

                string fileContent = GenerateOutputFileContentFromData(data, newCycle);

                await File.WriteAllTextAsync(newFilePath, fileContent);
                fileCreated = true;

                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now, fileCreated,
                                                                     "Outbound File created successfully.");

                await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, newCycle.Length);

                await APIs.TracingResponses.MarkTraceResultsAsViewedAsync(processCodes.EnfSrv_Cd);
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

    private async Task<List<TracingOutgoingProvincialData>> GetOutgoingDataAsync(FileTableData fileTableData, string actvSt_Cd,
                                                                string recipientCode)
    {
        var recMax = await DB.ProcessParameterTable.GetValueForParameterAsync(fileTableData.PrcId, "rec_max");
        int maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);

        var data = await APIs.TracingApplications.GetOutgoingProvincialTracingDataAsync(maxRecords, actvSt_Cd,
                                                                                     recipientCode);
        return data;
    }


    private static string GenerateOutputFileContentFromData(List<TracingOutgoingProvincialData> data,
                                                            string newCycle)
    {
        var result = new StringBuilder();

        result.AppendLine("<?xml version='1.0' encoding='utf-8'?>");
        result.AppendLine("<ProvincialOutboundXMLFileTraceResults80>");

        result.AppendLine(GenerateHeaderLine(newCycle));

        foreach (var item in data)
            result.AppendLine(GenerateDetailLine(item));

        result.AppendLine(GenerateFooterLine(data.Count));

        result.Append("</ProvincialOutboundXMLFileTraceResults80>");

        return result.ToString();
    }

    private static string GenerateHeaderLine(string newCycle)
    {
        string xmlCreationDateTime = DateTime.Now.ToString("o");

        var output = new StringBuilder();
        output.AppendLine($"<Header>");
        output.AppendLine($"  <Record_Type>01</Record_Type>");
        output.AppendLine($"  <Cycle_Number>{newCycle}</Cycle_Number>");
        output.AppendLine($"  <File_Creation_Date>{xmlCreationDateTime}</File_Creation_Date>");
        output.Append($"</Header>");

        return output.ToString();
    }

    private static string GenerateDetailLine(TracingOutgoingProvincialData item)
    {
        string xmlReceiptDate = item.TrcRsp_Rcpt_Dte.ToString("o");
        string xmlLastUpdateDate = item.TrcRsp_Addr_LstUpdte.ToString("o");

        var output = new StringBuilder();
        output.AppendLine($"<Trace_Result>");
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Record_Type_Code", "80"));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Enforcement_Service_Code", item.Appl_EnfSrv_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Issuing_Submitter_Code", item.Subm_SubmCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Control_Code", item.Appl_CtrlCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Source_Reference_Number", item.Appl_Source_RfrNr));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Recipient_Submitter_Code", item.Subm_Recpt_SubmCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Receipt_Date", xmlReceiptDate));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Sequence_Number", item.TrcRsp_SeqNr));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Status_Code", item.TrcSt_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Result_Code", item.Prcs_RecType.ToString())); // or item.Recordtype?
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Source_Service_Code", item.EnfSrv_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_Type_Code", item.AddrTyp_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Employer_Name1", item.TrcRsp_EmplNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Employer_Name2", item.TrcRsp_EmplNme1));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_Line1", item.TrcRsp_Addr_Ln));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_Line2", item.TrcRsp_Addr_Ln1));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_City", item.TrcRsp_Addr_CityNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_Province_Code", item.TrcRsp_Addr_PrvCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_Country_Code", item.TrcRsp_Addr_CtryCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Address_Postal_Code", item.TrcRsp_Addr_PCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Last_Update_Date ", xmlLastUpdateDate));
        output.Append($"</Trace_Result>");

        return output.ToString();
    }

    private static string GenerateFooterLine(int rowCount)
    {
        var output = new StringBuilder();
        output.AppendLine($"<Trailer>");
        output.AppendLine($"  <Record_Type>99</Record_Type>");
        output.AppendLine($"  <Detail_Record_Count>{rowCount:000000}</Detail_Record_Count>");
        output.Append($"</Trailer>");

        return output.ToString();
    }

}
