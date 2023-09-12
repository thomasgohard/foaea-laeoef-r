using FileBroker.Common.Helpers;
using System.Text;

namespace FileBroker.Business;

public class OutgoingProvincialTracingManager : IOutgoingFileManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }
    private IFileBrokerConfigurationHelper Config { get; }

    private FoaeaSystemAccess FoaeaAccess { get; }

    public OutgoingProvincialTracingManager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
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

        string newCycle = fileTableData.Cycle.ToString("000000");

        try
        {
            var processCodes = await DB.ProcessParameterTable.GetProcessCodes(fileTableData.PrcId);

            string newFilePath = fileTableData.Path + fileBaseName + "." + newCycle + ".xml";
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
                var data = await GetOutgoingDataFromFoaea(fileTableData, processCodes.ActvSt_Cd, processCodes.SubmRecptCd);

                string fileContent = GenerateOutputFileContentFromData(data, newCycle);

                await File.WriteAllTextAsync(newFilePath, fileContent, Encoding.UTF8);
                fileCreated = true;

                string errorDoingBackup = await FileHelper.BackupFile(newFilePath, DB, Config);

                if (!string.IsNullOrEmpty(errorDoingBackup))
                    await DB.ErrorTrackingTable.MessageBrokerError($"File Error: {newFilePath}",
                                                                   "Error creating backup of outbound file: " + errorDoingBackup);

                await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now, fileCreated,
                                                                    "Outbound File created successfully.");

                await DB.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

                await APIs.TracingResponses.MarkTraceResultsAsViewed(processCodes.EnfSrv_Cd);
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

    private async Task<TracingOutgoingProvincialData> GetOutgoingDataFromFoaea(FileTableData fileTableData, string actvSt_Cd,
                                                                string recipientCode)
    {
        var recMax = await DB.ProcessParameterTable.GetValueForParameter(fileTableData.PrcId, "rec_max");
        int maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);

        var data = await APIs.TracingApplications.GetOutgoingProvincialTracingData(maxRecords, actvSt_Cd, recipientCode);
        return data;
    }


    private static string GenerateOutputFileContentFromData(TracingOutgoingProvincialData data, string newCycle)
    {
        var result = new StringBuilder();

        result.AppendLine("<?xml version='1.0' encoding='utf-8'?>");
        result.AppendLine("<ProvincialOutboundXMLFileTraceResults>");

        result.AppendLine(GenerateHeaderLine(newCycle));

        int totalCount = 0;
        if (data.TracingData is not null)
        {
            totalCount += data.TracingData.Count;
            foreach (var item in data.TracingData)
                result.AppendLine(GenerateDetailLine80(item));
        }
        if (data.FinancialData is not null)
        {
            totalCount += data.FinancialData.Count;
            foreach (var item in data.FinancialData)
                result.AppendLine(GenerateDetailLine81(item));
        }

        result.AppendLine(GenerateFooterLine(totalCount));

        result.Append("</ProvincialOutboundXMLFileTraceResults>");

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

    private static string GenerateDetailLine80(TracingOutgoingProvincialTracingData item)
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
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Result_Code", item.Prcs_RecType.ToString()));
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

    private static string GenerateDetailLine81(TracingOutgoingProvincialFinancialData item)
    {
        string xmlReceiptDate = item.TrcRsp_Rcpt_Dte.ToString("o");

        var output = new StringBuilder();
        output.AppendLine($"<Trace_Result>");
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Record_Type_Code", "81"));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Enforcement_Service_Code", item.Appl_EnfSrv_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Issuing_Submitter_Code", item.Subm_SubmCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Control_Code", item.Appl_CtrlCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Source_Reference_Number", item.Appl_Source_RfrNr));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Recipient_Submitter_Code", item.Subm_Recpt_SubmCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Receipt_Date", xmlReceiptDate));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Sequence_Number", item.TrcRsp_SeqNr));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Status_Code", item.TrcSt_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Result_Code", item.Prcs_RecType.ToString()));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Trace_Source_Service_Code", item.EnfSrv_Cd));

        if (item.TraceFinancialDetails is not null)
        {
            output.AppendLine("   <Tax_Response>");
            foreach (var taxYear in item.TraceFinancialDetails)
            {
                short year = taxYear.FiscalYear;
                string taxForm = taxYear.TaxForm;
                output.AppendLine($"      <Tax_Data year='{year}' form='{taxForm}'>");
                foreach (var taxValue in taxYear.TraceDetailValues)
                    output.AppendLine($"         <field name='{taxValue.FieldName}'>{taxValue.FieldValue}</field>");
                output.AppendLine($"      </Tax_Data>");
            }
            output.AppendLine("   </Tax_Response>");
        }
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
