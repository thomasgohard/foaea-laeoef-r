using FileBroker.Common.Helpers;
using FOAEA3.Resources;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace FileBroker.Business
{
    public class IncomingProvincialElectronicSummonsManager
    {
        private string FileName { get; }
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }
        private ConfigurationHelper Config { get; }
        private Dictionary<string, string> Translations { get; }
        private bool IsFrench { get; }
        private string EnfSrv_Cd { get; }

        private IncomingProvincialHelper IncomingFileHelper { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public IncomingProvincialElectronicSummonsManager(RepositoryList db,
                                                          APIBrokerList foaeaApis,
                                                          string fileName,
                                                          ConfigurationHelper config)
        {
            FileName = fileName;
            APIs = foaeaApis;
            DB = db;
            Config = config;

            string provinceCode = fileName[0..2].ToUpper();
            IsFrench = Config.AuditConfig.FrenchAuditProvinceCodes?.Contains(provinceCode) ?? false;

            Translations = LoadTranslations();

            EnfSrv_Cd = provinceCode + "01"; // e.g. ON01

            string provCode = FileName[..2].ToUpper();
            IncomingFileHelper = new IncomingProvincialHelper(config, provCode);

            FoaeaAccess = new FoaeaSystemAccess(foaeaApis, Config.FoaeaLogin);
        }

        private Dictionary<string, string> LoadTranslations()
        {
            var translations = new Dictionary<string, string>();

            if (IsFrench)
            {
                var Translations = DB.TranslationTable.GetTranslationsAsync().Result;
                foreach (var translation in Translations)
                    translations.Add(translation.EnglishText, translation.FrenchText);

                APIs.InterceptionApplications.ApiHelper.CurrentLanguage = LanguageHelper.FRENCH_LANGUAGE;
                LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);
            }

            return translations;
        }

        private string Translate(string englishText)
        {
            if (IsFrench && Translations.ContainsKey(englishText))
            {
                return Translations[englishText];
            }
            else
                return englishText;
        }

        public async Task<MessageDataList> ExtractAndProcessRequestsInFileAsync(string sourceFilePath)
        {
            var result = new MessageDataList();

            var fileAuditManager = new FileAuditManager(DB.FileAudit, Config.AuditConfig, DB.MailService);

            int errorCount = 0;
            int warningCount = 0;
            int successCount = 0;

            await FoaeaAccess.SystemLoginAsync();

            try
            {
                string fileName = Path.GetFileName(sourceFilePath).ToUpper();

                if (fileName.StartsWith("NF"))
                {
                    string oldBaseName = fileName;
                    fileName = fileName.Replace("NF", "NL");
                    sourceFilePath = sourceFilePath.Replace(oldBaseName, fileName);
                }

                if (!IsValidZIPname(fileName))
                {
                    result.AddSystemError($"Invalid ESD File Name Format:: {Path.GetFileName(fileName)}");
                    return result;
                }

                if (await APIs.InterceptionApplications.ESD_CheckIfAlreadyLoaded(fileName))
                {
                    result.AddSystemError($"ESD File Currently Exists: {Path.GetFileName(fileName)}");
                    return result;
                }

                string baseName = string.Empty;
                if (fileName.Length > 5)
                    baseName = fileName[0..5];
                var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(baseName);
                var fileInfo = new FileInfo(sourceFilePath);

                var newESD = await APIs.InterceptionApplications.ESD_Create(fileTableData.PrcId, sourceFilePath, fileInfo.CreationTime);

                var pdfs = ExtractPDFlist(sourceFilePath);

                if (pdfs.Count == 0)
                {
                    result.AddSystemError($"No PDFs found in ESD file or invalid file: {sourceFilePath}");
                    return result;
                }

                string destinationPath = fileTableData.Path.AppendToPath(Path.GetFileName(sourceFilePath), isFileName: true);
                try
                {
                    File.Copy(sourceFilePath, destinationPath);
                }
                catch (Exception e)
                {
                    result.AddSystemError(e.Message);
                    return result;
                }

                string enfServiceCode = string.Empty;
                string controlCode = string.Empty;
                string sourceRef = string.Empty;
                string message = string.Empty;

                foreach (var pdfName in pdfs)
                {
                    string error = string.Empty;
                    string warning = string.Empty;

                    if (!IsValidESDname(pdfName, fileName))
                        error = Translate("Invalid PDF name");
                    else
                    {
                        string pdfNameNoExtension = Path.GetFileNameWithoutExtension(pdfName);

                        enfServiceCode = pdfNameNoExtension[..2] + "01";
                        controlCode = pdfNameNoExtension[5..(pdfNameNoExtension.Length - 1)];

                        var appl = await APIs.Applications.GetApplicationAsync(enfServiceCode, controlCode);

                        if ((appl == null) || (appl.Appl_CtrlCd != controlCode))
                            warning = Translate("Warning: Appl Record does not exist");
                        else
                            sourceRef = appl.Appl_Source_RfrNr?.Trim();

                        var newPDF = new ElectronicSummonsDocumentPdfData
                        {
                            EnfSrv = enfServiceCode,
                            Ctrl = controlCode,
                            ZipID = newESD.ZipID,
                            PDFName = pdfName
                        };

                        newPDF = await APIs.InterceptionApplications.ESDPDF_Create(newPDF);

                        if (newPDF.PDFid == 0)
                            error = Translate("Error: Could not add PDF to database");

                        if (error != string.Empty)
                        {
                            message = error;
                            errorCount++;
                        }
                        else if (warning != string.Empty)
                        {
                            message = warning;
                            warningCount++;
                        }
                        else
                        {
                            message = LanguageResource.AUDIT_SUCCESS;
                            successCount++;
                        }

                    }

                    var fileAuditData = new FileAuditData()
                    {
                        Appl_EnfSrv_Cd = enfServiceCode,
                        Appl_CtrlCd = controlCode,
                        Appl_Source_RfrNr = sourceRef ?? string.Empty,
                        ApplicationMessage = message,
                        InboundFilename = fileName,
                        Timestamp = DateTime.Now,
                        IsCompleted = true
                    };

                    await DB.FileAudit.InsertFileAuditDataAsync(fileAuditData);

                }

                await fileAuditManager.GenerateAuditFileAsync(fileName, null, errorCount, warningCount, successCount);
                await fileAuditManager.SendStandardAuditEmailAsync(fileName, Config.AuditConfig.AuditRecipients,
                                                                   errorCount, warningCount, successCount, 0);

            }
            finally
            {
                await FoaeaAccess.SystemLogoutAsync();
            }

            return result;
        }

        private static List<string> ExtractPDFlist(string sourceFilePath)
        {
            var pdfs = new List<string>();

            using ZipArchive archive = ZipFile.OpenRead(sourceFilePath);

            foreach (ZipArchiveEntry entry in archive.Entries)
                pdfs.Add(entry.FullName?.Trim()?.ToUpper()); // e.g. ONESDM11773O.pdf

            return pdfs;
        }

        private static bool IsValidESDname(string pdfName, string baseFileName)
        {
            if (pdfName == null) return false;

            string regex = "(?i)[" + baseFileName[..1] + "][" + baseFileName[1..2] + "][E][S][D][a-zA-Z0-9]{3,6}[A|O][.pdf]";

            return Regex.IsMatch(pdfName, regex);
        }

        private static bool IsValidZIPname(string baseFileName)
        {
            string regex = "(?i)[" + baseFileName[..1] + "][" + baseFileName[1..2] + "][E][S][D][0-9]{10}[.zip]";

            return Regex.IsMatch(baseFileName, regex);
        }

    }
}
