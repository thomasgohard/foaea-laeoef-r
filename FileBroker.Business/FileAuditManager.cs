using FOAEA3.Resources;
using System.Text;

namespace FileBroker.Business;

public class FileAuditManager
{
    private IFileAuditRepository FileAuditDB { get; }
    private IMailServiceRepository MailService { get; }
    private ProvincialAuditFileConfig AuditConfiguration { get; }
    private List<string> FrenchProvinceCodes { get; }

    public FileAuditManager(IFileAuditRepository fileAuditDB, IFileBrokerConfigurationHelper config, IMailServiceRepository mailService)
    {
        FileAuditDB = fileAuditDB;
        MailService = mailService;
        AuditConfiguration = config.AuditConfig;
        FrenchProvinceCodes = new(config.ProvinceConfig.FrenchAuditProvinceCodes);
    }

    private bool IsFrench(string provCd) => FrenchProvinceCodes.Contains(provCd);

    public async Task<int> GenerateAuditFileAsync(string fileName, List<UnknownTag> unknownTags, int errorCount, int warningCount, int successCount)
    {
        string provCd = fileName[..2].ToUpper();
        bool isFrench = IsFrench(provCd);
        var auditFileContent = new StringBuilder();

        var auditData = await FileAuditDB.GetFileAuditDataForFileAsync(fileName);

        int fileNotLoadedCount = auditData.GroupBy(p => new { p.Appl_EnfSrv_Cd, p.Appl_CtrlCd })
                                          .Select(g => g.First())
                                          .Where(g => !g.ApplicationMessage.StartsWith(LanguageResource.AUDIT_SUCCESS, StringComparison.InvariantCultureIgnoreCase))
                                          .Count();

        var auditErrorsData = new List<FileAuditData>();

        if (isFrench)
            LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);

        if (isFrench)
            auditFileContent.AppendLine("Code de l'autorité provinciale\tCode de contrôle\tNumero réf du ministère payeur\tMessage de l'application");
        else
            auditFileContent.AppendLine("Enforcement Service Code\tControl Code\tSource Reference Number\tApplication Message");

        foreach (var auditRow in auditData)
            if (auditRow.ApplicationMessage.StartsWith(LanguageResource.AUDIT_SUCCESS, StringComparison.InvariantCultureIgnoreCase))
            {
                if (isFrench)
                    auditFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-30}\t{auditRow.Appl_CtrlCd,-16}\t" +
                                                $"{auditRow.Appl_Source_RfrNr,-30}\t{auditRow.ApplicationMessage}");
                else
                    auditFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-24}\t{auditRow.Appl_CtrlCd,-12}\t" +
                                                $"{auditRow.Appl_Source_RfrNr,-23}\t{auditRow.ApplicationMessage}");
            }
            else
                auditErrorsData.Add(auditRow);

        if (auditErrorsData.Any())
        {
            auditFileContent.AppendLine($"");
            auditFileContent.AppendLine($"{LanguageResource.RECORDS_REMOVED_FROM_FILE}: {fileName}");
            foreach (var auditError in auditErrorsData)
                if (isFrench)
                    auditFileContent.AppendLine($"{auditError.Appl_EnfSrv_Cd,-30}\t{auditError.Appl_CtrlCd,-16}\t" +
                                                $"{auditError.Appl_Source_RfrNr,-30}\t{auditError.ApplicationMessage}");
                else
                    auditFileContent.AppendLine($"{auditError.Appl_EnfSrv_Cd,-24}\t{auditError.Appl_CtrlCd,-12}\t" +
                                                $"{auditError.Appl_Source_RfrNr,-23}\t{auditError.ApplicationMessage}");
        }

        if (unknownTags is not null && unknownTags.Any())
        {
            auditFileContent.AppendLine("");
            auditFileContent.AppendLine($"{LanguageResource.XML_VALIDATION_WARNINGS}: {fileName}");
            foreach (var unknownTag in unknownTags)
                auditFileContent.AppendLine($"{LanguageResource.AUDIT_SECTION} '{unknownTag.Section}' {LanguageResource.CONTAINS_INVALID_TAG} '{unknownTag.Tag}'");
            auditFileContent.AppendLine("");
        }

        auditFileContent.AppendLine("");
        auditFileContent.AppendLine($"{LanguageResource.NUMBER_OF_RECORDS}: {errorCount + warningCount + successCount}");
        auditFileContent.AppendLine($"{LanguageResource.LOADED_RECORDS}: {successCount + warningCount}");
        auditFileContent.AppendLine($"{LanguageResource.RECORDS_NOT_LOADED}: {fileNotLoadedCount}");
        auditFileContent.AppendLine($"{LanguageResource.ERRORS_DETECTED}: {errorCount}");
        if (unknownTags is not null && unknownTags.Any())
            auditFileContent.AppendLine($"{LanguageResource.TOTAL_XML_VALIDATION_WARNINGS}: {unknownTags.Count}");

        // save file to proper location
        string fullFileName = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.txt";
        await File.WriteAllTextAsync(fullFileName, auditFileContent.ToString());

        return fileNotLoadedCount;
    }

    public async Task SendStandardAuditEmailAsync(string fileName, string recipients, int errorCount, int warningCount,
                                                  int successCount, int xmlWarningCount, int totalFilesCount)
    {
        string provCd = fileName.Substring(0, 2).ToUpper();
        bool isFrench = IsFrench(provCd);

        string bodyContent;

        if (isFrench)
        {
            bodyContent = @"<b>SVP NE PAS RÉPONDRE À CE MESSAGE – CETTE BOÎTE COURRIEL N'EST PAS SURVEILLÉE DE PRÈS<b><br /><br /><br />" +
                          @$"Nombre d'enregistrements: {errorCount + warningCount + successCount}<br /><br />" +
                          @$"Enregistrements chargés: {successCount + warningCount}<br /><br />" +
                          @$"Enregistrements non chargés: {totalFilesCount}<br /><br />" +
                          @$"Erreurs décelées: {errorCount}<br /><br />" +
                          @"Pour toutes questions concernant le contenu de ce courriel, SVP envoyez un courriel à <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
            if (xmlWarningCount > 0)
                bodyContent += $"Nombre total avec avertissements de validation XML: {xmlWarningCount}";
        }
        else
        {
            bodyContent = @"<b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED<b><br /><br /><br />" +
                          @$"Number of records: {errorCount + warningCount + successCount}<br /><br />" +
                          @$"Loaded records: {successCount + warningCount}<br /><br />" +
                          @$"Records not loaded: {totalFilesCount}<br /><br />" +
                          @$"Errors detected: {errorCount}<br /><br />" +
                          @"For any questions regarding the content of this email, please contact <a href='email:FLAS-IT-SO@justice.gc.ca'>FLAS-IT-SO@justice.gc.ca</a>";
            if (xmlWarningCount > 0)
                bodyContent += $"Total XML validation warnings: {xmlWarningCount}";
        }

        string auditFileLocation = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.txt";

        await MailService.SendEmailAsync($"Audit {fileName}", recipients, bodyContent, auditFileLocation);

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

        await MailService.SendEmailAsync($"Audit {fileName}", recipients, bodyContent);

    }
}
