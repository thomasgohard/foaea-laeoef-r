using FOAEA3.Resources;
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

    public async Task GenerateAuditFileAsync(string fileName, List<UnknownTag> unknownTags, int errorCount, int warningCount, int successCount)
    {
        string provCd = fileName[..2].ToUpper();
        bool isFrench = IsFrench(provCd);
        var auditFileContent = new StringBuilder();

        var auditData = await FileAuditDB.GetFileAuditDataForFileAsync(fileName);
        var auditErrorsData = new List<FileAuditData>();
        if (isFrench)
        {
            LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);
            auditFileContent.AppendLine($"Code de l'autorité provinciale\tCode de contrôle\tNumero réf du ministère payeur\tMessage de l'application");
            foreach (var auditRow in auditData)
                if (auditRow.ApplicationMessage.StartsWith(LanguageResource.AUDIT_SUCCESS))
                    auditFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-30}\t{auditRow.Appl_CtrlCd,-16}\t" +
                                                $"{auditRow.Appl_Source_RfrNr,-30}\t{auditRow.ApplicationMessage}");
                else
                    auditErrorsData.Add(auditRow);

            if (auditErrorsData.Any())
            {
                auditFileContent.AppendLine($"");
                auditFileContent.AppendLine($"Entrées enlevées du fichier: {fileName}");
                foreach (var auditError in auditErrorsData)
                    auditFileContent.AppendLine($"{auditError.Appl_EnfSrv_Cd,-30}\t{auditError.Appl_CtrlCd,-16}\t" +
                                                $"{auditError.Appl_Source_RfrNr,-30}\t{auditError.ApplicationMessage}");
            }

            if (unknownTags.Any())
            {
                auditFileContent.AppendLine($"");
                auditFileContent.AppendLine($"Codes XML invalides: {fileName}");
                foreach (var unknownTag in unknownTags)
                    auditFileContent.AppendLine($"La section '{unknownTag.Section}' inclues le code invalide '{unknownTag.Tag}'");
                auditFileContent.AppendLine($"");
            }

            auditFileContent.AppendLine("");
            auditFileContent.AppendLine($"Nombre total de records: {errorCount + warningCount + successCount}");
            auditFileContent.AppendLine($"Nombre total avec succès: {successCount + warningCount}");
            auditFileContent.AppendLine($"Nombre total sans succès: {errorCount}");
            if (unknownTags.Any())
                auditFileContent.AppendLine($"Nombre total avec avertissements de validation XML: {unknownTags.Count}");

        }
        else
        {
            auditFileContent.AppendLine($"Enforcement Service Code\tControl Code\tSource Reference Number\tApplication Message");
            foreach (var auditRow in auditData)
                if (auditRow.ApplicationMessage.StartsWith(LanguageResource.AUDIT_SUCCESS))
                    auditFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-24}\t{auditRow.Appl_CtrlCd,-12}\t" +
                                            $"{auditRow.Appl_Source_RfrNr,-23}\t{auditRow.ApplicationMessage}");
                else
                    auditErrorsData.Add(auditRow);

            if (auditErrorsData.Any())
            {
                auditFileContent.AppendLine($"");
                auditFileContent.AppendLine($"Records removed from file: {fileName}");
                foreach (var auditError in auditErrorsData)
                    auditFileContent.AppendLine($"{auditError.Appl_EnfSrv_Cd,-24}\t{auditError.Appl_CtrlCd,-12}\t" +
                                                $"{auditError.Appl_Source_RfrNr,-23}\t{auditError.ApplicationMessage}");
            }

            if (unknownTags.Any())
            {
                auditFileContent.AppendLine($"");
                auditFileContent.AppendLine($"XML validation warnings: {fileName}");
                foreach (var unknownTag in unknownTags)
                    auditFileContent.AppendLine($"Section '{unknownTag.Section}' contains invalid tag '{unknownTag.Tag}'");
                auditFileContent.AppendLine($"");
            }

            auditFileContent.AppendLine("");
            auditFileContent.AppendLine($"Total records: {errorCount + warningCount + successCount}");
            auditFileContent.AppendLine($"Total success: {successCount + warningCount}");
            auditFileContent.AppendLine($"Total failed: {errorCount}");
            if (unknownTags.Any())
                auditFileContent.AppendLine($"Total XML validation warnings: {unknownTags.Count}");
        }

        // save file to proper location
        string fullFileName = AuditConfiguration.AuditRootPath + @"\" + fileName + ".xml.audit.txt";
        await File.WriteAllTextAsync(fullFileName, auditFileContent.ToString());
    }

    public async Task SendStandardAuditEmailAsync(string fileName, string recipients, int errorCount, int warningCount, int successCount, int xmlWarningCount)
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
            if (xmlWarningCount > 0)
                bodyContent += $"Nombre total avec avertissements de validation XML: {xmlWarningCount}";
        }
        else
        {
            bodyContent = @"<b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED<b><br /><br /><br />" +
                          @$"Total records: {errorCount + warningCount + successCount}<br /><br />" +
                          @$"Total success: {successCount + warningCount}<br /><br />" +
                          @$"Total failed: {errorCount}<br /><br />" +
                          @"For any questions regarding the content of this email, please contact <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
            if (xmlWarningCount > 0)
                bodyContent += $"Total XML validation warnings: {xmlWarningCount}";
        }

        string auditFileLocation = AuditConfiguration.AuditRootPath + @"\" + fileName + ".xml.audit.txt";

        await MailService.SendEmailAsync($"Audit {fileName}.xml", recipients, bodyContent, auditFileLocation);

    }

    public async Task SendSystemErrorAuditEmailAsync(string fileName, string recipients, MessageDataList errors)
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

        await MailService.SendEmailAsync($"Audit {fileName}.xml", recipients, bodyContent);

    }
}
