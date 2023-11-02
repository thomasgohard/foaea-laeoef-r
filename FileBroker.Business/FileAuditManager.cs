using DBHelper;
using FileBroker.Model.Enums;
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

    public async Task<int> GenerateProvincialAuditFile(string fileName, List<UnknownTag> unknownTags,
                                                       int errorCount, int warningCount, int successCount,
                                                       AuditFileFormat outputFormat = AuditFileFormat.TextFormat)
    {
        string provCd = fileName[..2].ToUpper();
        bool isFrench = IsFrench(provCd);
        var auditTextFileContent = new StringBuilder();
        var auditXmlFileContent = new StringBuilder();

        var auditData = await FileAuditDB.GetFileAuditDataForFile(fileName);

        int fileNotLoadedCount = auditData.GroupBy(p => new { p.Appl_EnfSrv_Cd, p.Appl_CtrlCd })
                                          .Select(g => g.First())
                                          .Where(g => !g.ApplicationMessage.StartsWith(LanguageResource.AUDIT_SUCCESS, StringComparison.InvariantCultureIgnoreCase))
                                          .Count();

        var auditErrorsData = new List<FileAuditData>();

        if (isFrench)
            LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);

        if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
        {
            auditTextFileContent.AppendLine($"{LanguageResource.ENFORCEMENT_SERVICE_CODE}\t{LanguageResource.CONTROL_CODE}\t{LanguageResource.SOURCE_REF_NUMBER}\t{LanguageResource.APP_MESSAGE}");
        }
        if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
        {
            string cycle = FileHelper.ExtractCycleAsStringFromFilename(fileName);

            auditXmlFileContent.AppendLine($"<?xml version=\"1.0\" standalone=\"yes\" ?>");
            auditXmlFileContent.AppendLine($"<NewDataSet>");
            auditXmlFileContent.AppendLine($"  <Header>");
            auditXmlFileContent.AppendLine($"    <RecType>01</RecType>");
            auditXmlFileContent.AppendLine($"    <Cycle>{cycle}</Cycle>");
            auditXmlFileContent.AppendLine($"    <FileDate>{DateTime.Now.ToString("o")}</FileDate>");
            auditXmlFileContent.AppendLine($"    <EnfSrv_Cd>{provCd}01</EnfSrv_Cd>");
            auditXmlFileContent.AppendLine($"  </Header>");
        }

        foreach (var auditRow in auditData)
            if (auditRow.ApplicationMessage.StartsWith(LanguageResource.AUDIT_SUCCESS, StringComparison.InvariantCultureIgnoreCase))
            {
                if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
                {
                    if (isFrench)
                        auditTextFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-30}\t{auditRow.Appl_CtrlCd,-16}\t" +
                                                    $"{auditRow.Appl_Source_RfrNr,-30}\t{auditRow.ApplicationMessage}");
                    else
                        auditTextFileContent.AppendLine($"{auditRow.Appl_EnfSrv_Cd,-24}\t{auditRow.Appl_CtrlCd,-12}\t" +
                                                    $"{auditRow.Appl_Source_RfrNr,-23}\t{auditRow.ApplicationMessage}");
                }
                if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
                {
                    auditXmlFileContent.AppendLine($"  <AuditDetail>");
                    auditXmlFileContent.AppendLine($"    <RecType>70</RecType>");
                    auditXmlFileContent.AppendLine($"    <EnfSrv_Cd>{auditRow.Appl_EnfSrv_Cd}</EnfSrv_Cd>");
                    auditXmlFileContent.AppendLine($"    <Appl_CtrlCd>{auditRow.Appl_CtrlCd}</Appl_CtrlCd>");
                    auditXmlFileContent.AppendLine($"    <Source_Reference_Number>{auditRow.Appl_Source_RfrNr}</Source_Reference_Number>");
                    auditXmlFileContent.AppendLine($"    <Application_Message>{auditRow.ApplicationMessage}</Application_Message>");
                    auditXmlFileContent.AppendLine($"  </AuditDetail>");
                }
            }
            else
                auditErrorsData.Add(auditRow);

        if (auditErrorsData.Any())
        {
            if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
            {
                auditTextFileContent.AppendLine($"");
                auditTextFileContent.AppendLine($"{LanguageResource.RECORDS_REMOVED_FROM_FILE}: {fileName}");
                foreach (var auditError in auditErrorsData)
                    if (isFrench)
                        auditTextFileContent.AppendLine($"{auditError.Appl_EnfSrv_Cd,-30}\t{auditError.Appl_CtrlCd,-16}\t" +
                                                    $"{auditError.Appl_Source_RfrNr,-30}\t{auditError.ApplicationMessage}");
                    else
                        auditTextFileContent.AppendLine($"{auditError.Appl_EnfSrv_Cd,-24}\t{auditError.Appl_CtrlCd,-12}\t" +
                                                    $"{auditError.Appl_Source_RfrNr,-23}\t{auditError.ApplicationMessage}");
            }
            if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
            {
                foreach (var auditError in auditErrorsData)
                {
                    auditXmlFileContent.AppendLine($"  <ErrorDetail>");
                    auditXmlFileContent.AppendLine($"    <RecType>71</RecType>");
                    auditXmlFileContent.AppendLine($"    <EnfSrv_Cd>{auditError.Appl_EnfSrv_Cd}</EnfSrv_Cd>");
                    auditXmlFileContent.AppendLine($"    <Appl_CtrlCd>{auditError.Appl_CtrlCd}</Appl_CtrlCd>");
                    auditXmlFileContent.AppendLine($"    <Source_Reference_Number>{auditError.Appl_Source_RfrNr}</Source_Reference_Number>");
                    auditXmlFileContent.AppendLine($"    <Application_Message>{auditError.ApplicationMessage}</Application_Message>");
                    auditXmlFileContent.AppendLine($"  </ErrorDetail>");
                }
            }
        }

        if (unknownTags is not null && unknownTags.Any())
        {
            if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
            {
                auditTextFileContent.AppendLine("");
                auditTextFileContent.AppendLine($"{LanguageResource.XML_VALIDATION_WARNINGS}: {fileName}");
                foreach (var unknownTag in unknownTags)
                    auditTextFileContent.AppendLine($"{LanguageResource.AUDIT_SECTION} '{unknownTag.Section}' {LanguageResource.CONTAINS_INVALID_TAG} '{unknownTag.Tag}'");
                auditTextFileContent.AppendLine("");
            }
            if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
            {
                foreach (var unknownTag in unknownTags)
                {
                    auditXmlFileContent.AppendLine($"  <XmlWarningDetail>");
                    auditXmlFileContent.AppendLine($"    <RecType>72</RecType>");
                    auditXmlFileContent.AppendLine($"    <Section>{unknownTag.Section}</Section>");
                    auditXmlFileContent.AppendLine($"    <InvalidTag>{unknownTag.Tag}</InvalidTag>");
                    auditXmlFileContent.AppendLine($"  </XmlWarningDetail>");
                }
            }
        }

        if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
        {
            auditTextFileContent.AppendLine("");
            auditTextFileContent.AppendLine($"{LanguageResource.NUMBER_OF_RECORDS}: {errorCount + warningCount + successCount}");
            auditTextFileContent.AppendLine($"{LanguageResource.LOADED_RECORDS}: {successCount + warningCount}");
            auditTextFileContent.AppendLine($"{LanguageResource.RECORDS_NOT_LOADED}: {fileNotLoadedCount}");
            auditTextFileContent.AppendLine($"{LanguageResource.ERRORS_DETECTED}: {errorCount}");
            if (unknownTags is not null && unknownTags.Any())
                auditTextFileContent.AppendLine($"{LanguageResource.TOTAL_XML_VALIDATION_WARNINGS}: {unknownTags.Count}");
        }
        if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
        {
            auditXmlFileContent.AppendLine($"  <Trailer>");
            auditXmlFileContent.AppendLine($"    <RecType>99</RecType>");
            auditXmlFileContent.AppendLine($"    <NumberOfRecords>{errorCount + warningCount + successCount}</NumberOfRecords>");
            auditXmlFileContent.AppendLine($"    <LoadedRecords>{successCount + warningCount}</LoadedRecords>");
            auditXmlFileContent.AppendLine($"    <RecordsNotLoaded>{fileNotLoadedCount}</RecordsNotLoaded>");
            auditXmlFileContent.AppendLine($"    <ErrorsDetected>{errorCount}</ErrorsDetected>");
            if (unknownTags is not null && unknownTags.Any())
                auditXmlFileContent.AppendLine($"    <XmlValidationWarnings>{unknownTags.Count}</XmlValidationWarnings>");
            auditXmlFileContent.AppendLine($"  </Trailer>");
            auditXmlFileContent.AppendLine($"</NewDataSet>");
        }

        // save file to proper location

        if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
        {
            string fullFileName = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.txt";
            await File.WriteAllTextAsync(fullFileName, auditTextFileContent.ToString());
        }
        if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
        {
            string fullFileName = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.XML";
            await File.WriteAllTextAsync(fullFileName, auditXmlFileContent.ToString());
        }

        return fileNotLoadedCount;
    }

    public async Task GenerateCraAuditFile(string fileName, List<InboundAuditData> inboundAudit)
    {
        var auditFileContent = new StringBuilder();

        auditFileContent.AppendLine($"{LanguageResource.ENFORCEMENT_SERVICE_CODE}\t{LanguageResource.CONTROL_CODE}\t{LanguageResource.SOURCE_REF_NUMBER}\t{LanguageResource.APP_MESSAGE}");

        int successCount = 0;
        int failureCount = 0;
        foreach (var auditRow in inboundAudit)
        {
            auditFileContent.AppendLine($"{auditRow.EnforcementServiceCode,-24}\t{auditRow.ControlCode,-12}\t" +
                                        $"{auditRow.SourceReferenceNumber.Trim(),-23}\t{auditRow.ApplicationMessage}");

            if (auditRow.ApplicationMessage.ToUpper().IndexOf("SUCC") > -1)
                successCount++;
            else
                failureCount++;
        }

        auditFileContent.AppendLine();

        auditFileContent.AppendLine($"Total records: {successCount + failureCount}");
        auditFileContent.AppendLine($"Total success: {successCount}");
        auditFileContent.AppendLine($"Total failed: {failureCount}");

        string fullFileName = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.txt";
        await File.WriteAllTextAsync(fullFileName, auditFileContent.ToString());

        string recipients = AuditConfiguration.AuditRecipients;
        string bodyContent = $"Total records: {successCount + failureCount}<br/><br/>" +
                             $"Total success: {successCount}<br/><br/>" +
                             $"Total failed: {failureCount}<br/><br/>";
        await MailService.SendEmail($"Audit {fileName}", recipients, bodyContent, fullFileName);

        if (failureCount >= successCount)
        {
            bodyContent = $"Multiple failures loading file: {fileName}<br/><br/>" + bodyContent;
            await MailService.SendEmail($"Multiple failures loading file: {fileName}", recipients, bodyContent);
        }
    }

    public async Task SendStandardAuditEmail(string fileName, string recipients, int errorCount, int warningCount,
                                             int successCount, int xmlWarningCount, int totalFilesCount, 
                                             AuditFileFormat outputFormat = AuditFileFormat.TextFormat)
    {
        string provCd = fileName[..2].ToUpper();
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

        if (outputFormat.In(AuditFileFormat.TextFormat, AuditFileFormat.Both))
        {
            string auditFileLocation = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.txt";
            await MailService.SendEmail($"Audit {fileName}", recipients, bodyContent, auditFileLocation);
        }

        if (outputFormat.In(AuditFileFormat.XmlFormat, AuditFileFormat.Both))
        {
            string auditFileLocation = AuditConfiguration.AuditRootPath + @"\" + fileName + ".audit.XML";
            await MailService.SendEmail($"Audit {fileName}", recipients, bodyContent, auditFileLocation);
        }
    }

    public async Task SendSystemErrorAuditEmail(string fileName, string recipients, MessageDataList errors)
    {
        string provCd = fileName[..2].ToUpper();
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

        await MailService.SendEmail($"Audit {fileName}", recipients, bodyContent);

    }
}
