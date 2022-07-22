using System.Text;

namespace FileBroker.Business;

public class OutgoingFederalLicenceDenialManager : IOutgoingFileManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList Repositories { get; }

    public OutgoingFederalLicenceDenialManager(APIBrokerList apiBrokers, RepositoryList repositories)
    {
        APIs = apiBrokers;
        Repositories = repositories;
    }

    public string CreateOutputFile(string fileBaseName, out List<string> errors)
    {
        errors = new List<string>();

        bool fileCreated = false;

        var fileTableData = Repositories.FileTable.GetFileTableDataForFileName(fileBaseName);
        string newCycle = (fileTableData.Cycle + 1).ToString("000000");

        try
        {
            var processCodes = Repositories.ProcessParameterTable.GetProcessCodes(fileTableData.PrcId);

            string newFilePath = fileTableData.Path + fileBaseName + "." + newCycle + ".XML";
            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            var outgoingData = GetOutgoingData(fileTableData, processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                       processCodes.EnfSrv_Cd);

            var eventDetailIds = new List<int>();
            foreach (var item in outgoingData)
                eventDetailIds.Add(item.Event_dtl_Id);

            string fileContent = GenerateOutputFileContentFromData(outgoingData, newCycle, processCodes.EnfSrv_Cd);

            File.WriteAllText(newFilePath, fileContent);
            fileCreated = true;

            Repositories.OutboundAuditDB.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now, fileCreated,
                                                                 "Outbound File created successfully.");

            Repositories.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

            APIs.ApplicationEvents.UpdateOutboundEventDetail(processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                                             processCodes.EnfSrv_Cd,
                                                             "OK: Written to " + newFilePath, eventDetailIds);

            return newFilePath;

        }
        catch (Exception e)
        {
            string error = "Error Creating Outbound Data File: " + e.Message;
            errors.Add(error);

            Repositories.OutboundAuditDB.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now, fileCreated, error);

            Repositories.ErrorTrackingDB.MessageBrokerError($"File Error: {fileTableData.PrcId} {fileBaseName}", 
                                                                        "Error creating outbound file", e, displayExceptionError: true);

            return string.Empty;
        }

    }

    private List<LicenceDenialOutgoingFederalData> GetOutgoingData(FileTableData fileTableData, string actvSt_Cd,
                                                           int appLiSt_Cd, string enfSrvCode)
    {
        var recMax = Repositories.ProcessParameterTable.GetValueForParameter(fileTableData.PrcId, "rec_max");
        int maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);

        var data = APIs.LicenceDenialApplications.GetOutgoingFederalLicenceDenialRequests(maxRecords, actvSt_Cd,
                                                                                          appLiSt_Cd, enfSrvCode);
        return data;
    }

    private static string GenerateOutputFileContentFromData(List<LicenceDenialOutgoingFederalData> data,
                                                            string newCycle, string enfSrvCode)
    {
        var result = new StringBuilder();

        result.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        result.AppendLine("<NewDataSet>");

        result.AppendLine(GenerateHeaderLine(newCycle));

        foreach (var item in data)
            result.AppendLine(GenerateDetailLine(enfSrvCode, item));

        result.AppendLine(GenerateFooterLine(data.Count));

        result.Append("</NewDataSet>");

        return result.ToString();
    }

    private static string GenerateHeaderLine(string newCycle)
    {
        string xmlCreationDateTime = DateTime.Now.ToString("o");

        var output = new StringBuilder();
        output.AppendLine($"<LICOUT01>");
        output.AppendLine($"  <RecType>01</RecType>");
        output.AppendLine($"  <Cycle>{newCycle}</Cycle>");
        output.AppendLine($"  <FileDate>{xmlCreationDateTime}</FileDate>");
        output.Append($"</LICOUT01>");

        return output.ToString();
    }

    public static string FormatDBDateString(string dateString)
    {
        if (!string.IsNullOrEmpty(dateString.Trim()) && (dateString.Length == 8))
        {
            try
            {
                int day = int.Parse(dateString[0..2]);
                int month = int.Parse(dateString[2..4]);
                int year = int.Parse(dateString[4..8]);
                var dateTime = new DateTime(year, month, day);
                return dateTime.ToString("o");
            }
            catch
            {
                return string.Empty;
            }
        }
        return string.Empty;
    }

    private static string GenerateDetailLine(string enfSource, LicenceDenialOutgoingFederalData item)
    {
        string xmlDebtorDOB = FormatDBDateString(item.Appl_Dbtr_Brth_Dte);

        var output = new StringBuilder();
        output.AppendLine($"<LICOUT02>");
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("RecType", item.Recordtype));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("RequestType", item.RequestType));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_EnfSrv_Cd", item.Appl_EnfSrv_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_CtrlCd", item.Appl_CtrlCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_FrstNme", item.Appl_Dbtr_FrstNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_MddleNme", item.Appl_Dbtr_MddleNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_SurNme", item.Appl_Dbtr_SurNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_MotherMaiden_SurNme", item.Appl_Dbtr_Parent_SurNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Gendr_Cd", item.Appl_Dbtr_Gendr_Cd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Brth_Dte", xmlDebtorDOB));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Brth_CityNme", item.LicSusp_Dbtr_Brth_CityNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Brth_CtryCd", item.LicSusp_Dbtr_Brth_CtryCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EyesColorCd", item.LicSusp_Dbtr_EyesColorCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_HeightQty", item.LicSusp_Dbtr_HeightQty));

        if (enfSource == "TC01")
        {
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Addr_Ln", item.Dbtr_Addr_Ln));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Addr_Ln1", item.Dbtr_Addr_Ln1));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Addr_CityNme", item.Dbtr_Addr_CityNme));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Addr_PrvCd", item.Dbtr_Addr_PrvCd));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Addr_PCd", item.Dbtr_Addr_PCd));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_Addr_CtryCd", item.Dbtr_Addr_CtryCd));
        }
        else // PA01
        {
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Addr_Ln", item.Dbtr_Addr_Ln));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Addr_Ln1", item.Dbtr_Addr_Ln1));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Addr_CityNme", item.Dbtr_Addr_CityNme));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Addr_PrvCd", item.Dbtr_Addr_PrvCd));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Addr_PCd", item.Dbtr_Addr_PCd));
            output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Appl_Dbtr_Addr_CtryCd", item.Dbtr_Addr_CtryCd));
        }

        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplNme", item.LicSusp_Dbtr_EmplNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplAddr_Ln", item.LicSusp_Dbtr_EmplAddr_Ln));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplAddr_Ln1", item.LicSusp_Dbtr_EmplAddr_Ln1));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplAddr_CityNme", item.LicSusp_Dbtr_EmplAddr_CityNme));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplAddr_PrvCd", item.LicSusp_Dbtr_EmplAddr_PrvCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplAddr_PCd", item.LicSusp_Dbtr_EmplAddr_PCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("LicSusp_Dbtr_EmplAddr_CtryCd", item.LicSusp_Dbtr_EmplAddr_CtryCd));
        output.AppendLine(XmlHelper.GenerateXMLTagWithValue("Source_RefNo", ""));
        output.Append($"</LICOUT02>");

        return output.ToString();
    }

    private static string GenerateFooterLine(int rowCount)
    {
        var output = new StringBuilder();
        output.AppendLine($"<LICOUT99>");
        output.AppendLine($"  <RecType>99</RecType>");
        output.AppendLine($"  <ResponseCnt>{rowCount:000000}</ResponseCnt>");
        output.Append($"</LICOUT99>");

        return output.ToString();
    }


}
