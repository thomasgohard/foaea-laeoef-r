using System.Text;

namespace FileBroker.Business;

public class FileAuditManager
{
    private IFileAuditRepository FileAuditDB { get; }
    private IMailServiceRepository MailService { get; }
    private ProvincialAuditFileConfig AuditConfiguration { get; }
    private List<string> FrenchProvinceCodes { get; }

    public FileAuditManager(IFileAuditRepository fileAuditDB, ProvincialAuditFileConfig auditConfig, IMailServiceRepository mailService)
    {
        FileAuditDB = fileAuditDB;
        MailService = mailService;
        AuditConfiguration = auditConfig;
        FrenchProvinceCodes = new(auditConfig.FrenchAuditProvinceCodes);
    }

    private bool IsFrench(string provCd) => FrenchProvinceCodes.Contains(provCd);

    public void GenerateAuditFile(string fileName, int errorCount, int warningCount, int successCount)
    {
        string provCd = fileName[..2].ToUpper();
        bool isFrench = IsFrench(provCd);
        var auditFileContent = new StringBuilder();

        var auditData = FileAuditDB.GetFileAuditDataForFile(fileName);
        if (isFrench)
        {
            auditFileContent.AppendLine($"Code de l'autorité provinciale\tCode de contrôle\tNumero réf du ministère payeur\tMessage de l'application");
            foreach (var auditRow in auditData)
                auditFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-30}\t{auditRow.Appl_CtrlCd,-16}\t" +
                                            $"{auditRow.Appl_Source_RfrNr,-30}\t{auditRow.ApplicationMessage}");

            auditFileContent.AppendLine("");
            auditFileContent.AppendLine($"Nombre total de records: {errorCount + warningCount + successCount}");
            auditFileContent.AppendLine($"Nombre total avec succès: {successCount + warningCount}");
            auditFileContent.AppendLine($"Nombre total sans succès: {errorCount}");
        }
        else
        {
            auditFileContent.AppendLine($"Enforcement Service Code\tControl Code\tSource Reference Number\tApplication Message");
            foreach (var auditRow in auditData)
                auditFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-24}\t{auditRow.Appl_CtrlCd,-12}\t" +
                                            $"{auditRow.Appl_Source_RfrNr,-23}\t{auditRow.ApplicationMessage}");

            auditFileContent.AppendLine("");
            auditFileContent.AppendLine($"Total records: {errorCount + warningCount + successCount}");
            auditFileContent.AppendLine($"Total success: {successCount + warningCount}");
            auditFileContent.AppendLine($"Total failed: {errorCount}");
        }


        // save file to proper location
        string fullFileName = AuditConfiguration.AuditRootPath + @"\" + fileName + ".xml.audit.txt";
        File.WriteAllText(fullFileName, auditFileContent.ToString());
    }

    public void SendStandardAuditEmail(string fileName, string recipients, int errorCount, int warningCount, int successCount)
    {
        string provCd = fileName.Substring(0, 2).ToUpper();
        bool isFrench = IsFrench(provCd);

        string bodyContent;

        if (isFrench)
        {
            bodyContent = @"<b>SVP NE PAS RÉPONDRE À CE MESSAGE – CETTE BOÎTE COURRIEL N'EST PAS SURVEILLÉE DE PRÈS<b><br /><br /><br />" +
                          @$"Nombre total de records: {errorCount + warningCount + successCount}<br /><br />" +
                          @$"Nombre total avec succès: {successCount + warningCount}<br /><br />" +
                          @$"Nombre total sans succès: {errorCount}<br /><br />" +
                          @"Pour toutes questions concernant le contenu de ce courriel, SVP envoyez un courriel à <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
        }
        else
        {
            bodyContent = @"<b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED<b><br /><br /><br />" +
                          @$"Total records: {errorCount + warningCount + successCount}<br /><br />" +
                          @$"Total success: {successCount + warningCount}<br /><br />" +
                          @$"Total failed: {errorCount}<br /><br />" +
                          @"For any questions regarding the content of this email, please contact <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
        }

        string auditFileLocation = AuditConfiguration.AuditRootPath + @"\" + fileName + ".xml.audit.txt";

        MailService.SendEmail($"Audit {fileName}.xml", recipients, bodyContent, auditFileLocation);

    }

    public void SendSystemErrorAuditEmail(string fileName, string recipients, MessageDataList errors)
    {
        string provCd = fileName.Substring(0, 2).ToUpper();
        bool isFrench = IsFrench(provCd);

        string multipleErrors = (errors.Count) > 1 ? "s" : "";

        string bodyContent;
        if (isFrench)
        {
            bodyContent = @"<b>SVP NE PAS RÉPONDRE À CE MESSAGE – CETTE BOÎTE COURRIEL N'EST PAS SURVEILLÉE DE PRÈS<b><br /><br /><br />";
            bodyContent += @$"The incoming file could not be processed due to the following error{multipleErrors}:<br />";
            foreach (var error in errors)
                bodyContent += @$"&nbsp;&nbsp;{error.Description}<br />";
            bodyContent += @"<br />Pour toutes questions concernant le contenu de ce courriel, SVP envoyez un courriel à <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
        }
        else
        {
            bodyContent = @"<b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED<b><br /><br /><br />";
            bodyContent += @$"The incoming file could not be processed due to the following error{multipleErrors}:<br />";
            foreach (var error in errors)
                bodyContent += @$"&nbsp;&nbsp;{error.Description}<br />";
            bodyContent += @"<br />For any questions regarding the content of this email, please contact <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
        }

        MailService.SendEmail($"Audit {fileName}.xml", recipients, bodyContent);

    }
}
