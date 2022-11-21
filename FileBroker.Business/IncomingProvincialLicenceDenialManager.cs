using DBHelper;
using FileBroker.Common.Helpers;
using Newtonsoft.Json;

namespace FileBroker.Business;

public class IncomingProvincialLicenceDenialManager
{
    private string FileName { get; }
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }
    private FileBrokerConfigurationHelper Config { get; }
    private Dictionary<string, string> Translations { get; }
    private bool IsFrench { get; }

    private IncomingProvincialHelper IncomingFileHelper { get; }

    private FoaeaSystemAccess FoaeaAccess { get; }

    public IncomingProvincialLicenceDenialManager(RepositoryList db,
                                                  APIBrokerList foaeaApis,
                                                  string fileName,
                                                  FileBrokerConfigurationHelper config)
    {
        FileName = fileName;
        APIs = foaeaApis;
        DB = db;
        Config = config;

        string provinceCode = fileName[0..2].ToUpper();
        IsFrench = Config.AuditConfig.FrenchAuditProvinceCodes?.Contains(provinceCode) ?? false;

        Translations = LoadTranslations();

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

    public async Task<MessageDataList> ExtractAndProcessRequestsInFileAsync(string sourceLicenceDenialData,
                                            List<UnknownTag> unknownTags, bool includeInfoInMessages = false)
    {
        var result = new MessageDataList();

        var fileAuditManager = new FileAuditManager(DB.FileAudit, Config.AuditConfig, DB.MailService);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(FileName);
        var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

        await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, true);

        bool isValid = true;

        // convert data from json into object
        var licenceDenialFileData = ExtractLicenceDenialDataFromJson(sourceLicenceDenialData, out string error);
        var licenceDenialFile = licenceDenialFileData.NewDataSet;

        if (!string.IsNullOrEmpty(error))
        {
            isValid = false;
            result.AddSystemError(error);
        }
        else
        {
            ValidateHeader(licenceDenialFile.LICAPPIN01, ref result, ref isValid);
            ValidateFooter(licenceDenialFile, ref result, ref isValid);

            if (isValid)
            {
                var counts = new ResultTracking();

                await FoaeaAccess.SystemLoginAsync();
                try
                {
                    if (licenceDenialFile.LICAPPIN30 is not null)
                        foreach (var data in licenceDenialFile.LICAPPIN30)
                            await ProcessLicenceApplicationsAsync(data, licenceDenialFile, result,
                                                                  includeInfoInMessages, counts,
                                                                  isTermination: false);

                    if (licenceDenialFile.LICAPPIN40 is not null)
                        foreach (var data in licenceDenialFile.LICAPPIN40)
                            await ProcessLicenceApplicationsAsync(data, licenceDenialFile, result,
                                                                  includeInfoInMessages, counts,
                                                                  isTermination: true);

                    int totalFilesCount = await fileAuditManager.GenerateAuditFileAsync(FileName + ".XML", unknownTags, counts.ErrorCount, counts.WarningCount, counts.SuccessCount);
                    await fileAuditManager.SendStandardAuditEmailAsync(FileName + ".XML", Config.AuditConfig.AuditRecipients,
                                                            counts.ErrorCount, counts.WarningCount, counts.SuccessCount, 
                                                            unknownTags.Count, totalFilesCount);
                }
                finally
                {
                    await FoaeaAccess.SystemLogoutAsync();
                }
            }

        }

        if (!isValid)
        {
            result.AddSystemError($"One of more error(s) occured in file ({FileName}.XML)");

            await fileAuditManager.SendSystemErrorAuditEmailAsync(FileName, Config.AuditConfig.AuditRecipients, result);
        }

        await DB.FileAudit.MarkFileAuditCompletedForFileAsync(FileName);
        await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, false);
        await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData);

        return result;
    }

    private async Task ProcessLicenceApplicationsAsync(MEPLicenceDenial_RecTypeBase data,
                                                     MEPLicenceDenial_LicenceDenialDataSet licenceDenialFile,
                                                     MessageDataList result, bool includeInfoInMessages,
                                                     ResultTracking counts, bool isTermination)
    {
        bool isValidRequest = true;

        var fileAuditData = new FileAuditData
        {
            Appl_EnfSrv_Cd = data.dat_Appl_EnfSrvCd,
            Appl_CtrlCd = data.dat_Appl_CtrlCd,
            Appl_Source_RfrNr = data.dat_Appl_Source_RfrNr,
            InboundFilename = FileName + ".XML"
        };

        var requestError = new MessageDataList();

        ValidateActionCode(data, ref requestError, ref isValidRequest, isTermination);

        if (isValidRequest)
        {
            MessageData<LicenceDenialApplicationData> licenceDenialMessage;
            MessageDataList messages;

            if (!isTermination)
            {
                var licenceDenialData = licenceDenialFile.LICAPPIN31.Find(t => t.dat_Appl_CtrlCd == data.dat_Appl_CtrlCd);

                licenceDenialMessage = new MessageData<LicenceDenialApplicationData>
                {
                    Application = GetLicenceDenialApplicationDataFromRequest(data, licenceDenialData),
                    MaintenanceAction = data.Maintenance_ActionCd,
                    MaintenanceLifeState = data.dat_Appl_LiSt_Cd,
                    NewRecipientSubmitter = data.dat_New_Owner_RcptSubmCd,
                    NewIssuingSubmitter = data.dat_New_Owner_SubmCd,
                    NewUpdateSubmitter = data.dat_Update_SubmCd
                };

                var requestLogData = new RequestLogData
                {
                    MaintenanceAction = licenceDenialMessage.MaintenanceAction,
                    MaintenanceLifeState = licenceDenialMessage.MaintenanceLifeState,
                    Appl_EnfSrv_Cd = licenceDenialMessage.Application.Appl_EnfSrv_Cd,
                    Appl_CtrlCd = licenceDenialMessage.Application.Appl_CtrlCd,
                    LoadedDateTime = DateTime.Now
                };

                _ = await DB.RequestLogTable.AddAsync(requestLogData);

                messages = await ProcessApplicationRequestAsync(licenceDenialMessage);
            }
            else
            {
                var licenceDenialData = licenceDenialFile.LICAPPIN41.Find(t => t.dat_Appl_CtrlCd == data.dat_Appl_CtrlCd);

                licenceDenialMessage = new MessageData<LicenceDenialApplicationData>
                {
                    Application = GetLicenceDenialTerminationApplicationDataFromRequest(data, licenceDenialData),
                    MaintenanceAction = data.Maintenance_ActionCd,
                    MaintenanceLifeState = data.dat_Appl_LiSt_Cd,
                    NewRecipientSubmitter = data.dat_New_Owner_RcptSubmCd,
                    NewIssuingSubmitter = data.dat_New_Owner_SubmCd,
                    NewUpdateSubmitter = data.dat_Update_SubmCd
                };

                var requestLogData = new RequestLogData
                {
                    MaintenanceAction = licenceDenialMessage.MaintenanceAction,
                    MaintenanceLifeState = licenceDenialMessage.MaintenanceLifeState,
                    Appl_EnfSrv_Cd = licenceDenialMessage.Application.Appl_EnfSrv_Cd,
                    Appl_CtrlCd = licenceDenialMessage.Application.Appl_CtrlCd,
                    LoadedDateTime = DateTime.Now
                };

                _ = await DB.RequestLogTable.AddAsync(requestLogData);

                messages = await ProcessTerminationApplicationRequestAsync(licenceDenialMessage);
            }

            if (messages.ContainsMessagesOfType(MessageType.Error))
            {
                var errors = messages.FindAll(m => m.Severity == MessageType.Error);

                fileAuditData.ApplicationMessage = Translate(errors[0].Description);
                counts.ErrorCount++;
            }
            else if (messages.ContainsMessagesOfType(MessageType.Warning))
            {
                var warnings = messages.FindAll(m => m.Severity == MessageType.Warning);

                fileAuditData.ApplicationMessage = Translate(warnings[0].Description);
                counts.WarningCount++;
            }
            else
            {
                if (includeInfoInMessages)
                {
                    var infos = messages.FindAll(m => m.Severity == MessageType.Information);

                    result.AddRange(infos);
                }

                fileAuditData.ApplicationMessage = Translate("Success");
                counts.SuccessCount++;
            }

        }
        else
        {
            fileAuditData.ApplicationMessage = requestError[0].Description;
            counts.ErrorCount++;
        }

        await DB.FileAudit.InsertFileAuditDataAsync(fileAuditData);
    }

    public async Task<MessageDataList> ProcessApplicationRequestAsync(MessageData<LicenceDenialApplicationData> licenceDenialMessageData)
    {
        LicenceDenialApplicationData licenceDenial;

        if (licenceDenialMessageData.MaintenanceAction == "A")
        {
            licenceDenial = await APIs.LicenceDenialApplications.CreateLicenceDenialApplicationAsync(licenceDenialMessageData.Application);
        }
        else // if (tracingMessageData.MaintenanceAction == "C")
        {
            switch (licenceDenialMessageData.MaintenanceLifeState)
            {
                case "00": // change
                case "0":
                    licenceDenial = await APIs.LicenceDenialApplications.UpdateLicenceDenialApplicationAsync(licenceDenialMessageData.Application);
                    break;

                case "29": // transfer
                    licenceDenial = await APIs.LicenceDenialApplications.TransferLicenceDenialApplicationAsync(licenceDenialMessageData.Application,
                                                                                  licenceDenialMessageData.NewRecipientSubmitter,
                                                                                  licenceDenialMessageData.NewIssuingSubmitter);
                    break;

                default:
                    licenceDenial = licenceDenialMessageData.Application;
                    licenceDenial.Messages.AddError($"Unknown dat_Appl_LiSt_Cd ({licenceDenialMessageData.MaintenanceLifeState})" +
                                              $" for Maintenance_ActionCd ({licenceDenialMessageData.MaintenanceAction})");
                    break;
            }
        }

        return licenceDenial.Messages;
    }

    public async Task<MessageDataList> ProcessTerminationApplicationRequestAsync(MessageData<LicenceDenialApplicationData> licenceDenialMessageData)
    {
        LicenceDenialApplicationData licenceDenial;

        if (licenceDenialMessageData.MaintenanceAction == "A")
        {
            licenceDenial = await APIs.LicenceDenialTerminationApplications.CreateLicenceDenialTerminationApplicationAsync(licenceDenialMessageData.Application,
                                                                                    licenceDenialMessageData.Application.LicSusp_Appl_CtrlCd);
        }
        else // if (tracingMessageData.MaintenanceAction == "C")
        {
            switch (licenceDenialMessageData.MaintenanceLifeState)
            {
                case "00": // change
                case "0":
                    licenceDenial = await APIs.LicenceDenialTerminationApplications.UpdateLicenceDenialTerminationApplicationAsync(licenceDenialMessageData.Application);
                    break;

                case "14": // cancellation
                    licenceDenial = await APIs.LicenceDenialTerminationApplications.CancelLicenceDenialTerminationApplicationAsync(licenceDenialMessageData.Application);
                    break;

                case "29": // transfer
                    licenceDenial = await APIs.LicenceDenialTerminationApplications.TransferLicenceDenialTerminationApplicationAsync(licenceDenialMessageData.Application,
                                                                                  licenceDenialMessageData.NewRecipientSubmitter,
                                                                                  licenceDenialMessageData.NewIssuingSubmitter);
                    break;

                default:
                    licenceDenial = licenceDenialMessageData.Application;
                    licenceDenial.Messages.AddError($"Unknown dat_Appl_LiSt_Cd ({licenceDenialMessageData.MaintenanceLifeState})" +
                                              $" for Maintenance_ActionCd ({licenceDenialMessageData.MaintenanceAction})");
                    break;
            }
        }

        return licenceDenial.Messages;

    }

    private void ValidateHeader(MEPLicenceDenial_RecType01 licenceDenialFile, ref MessageDataList result, ref bool isValid)
    {
        int cycle = FileHelper.GetCycleFromFilename(FileName);
        if (int.Parse(licenceDenialFile.Cycle) != cycle)
        {
            isValid = false;
            result.AddSystemError($"Cycle in file [{licenceDenialFile.Cycle}] does not match cycle of file [{cycle}]");
        }
        if (!IncomingFileHelper.IsValidTermsAccepted(licenceDenialFile.TermsAccepted))
        {
            isValid = false;
            result.AddSystemError($"type 01 Terms Accepted invalid text: {licenceDenialFile.TermsAccepted}");
        }
    }

    private static void ValidateFooter(MEPLicenceDenial_LicenceDenialDataSet licenceDenialFile, ref MessageDataList result, ref bool isValid)
    {
        int totalCount = 0;
        if (licenceDenialFile.LICAPPIN30 is not null && licenceDenialFile.LICAPPIN30.Any() && !EmptyAction(licenceDenialFile.LICAPPIN30))
            totalCount += licenceDenialFile.LICAPPIN30.Count;
        if (licenceDenialFile.LICAPPIN40 is not null && licenceDenialFile.LICAPPIN40.Any() && !EmptyAction(licenceDenialFile.LICAPPIN40))
            totalCount += licenceDenialFile.LICAPPIN40.Count;

        if (int.Parse(licenceDenialFile.LICAPPIN99.ResponseCnt) != totalCount)
        {
            isValid = false;
            result.AddSystemError("Invalid ResponseCnt in section 99");
        }
    }

    private static bool EmptyAction(List<MEPLicenceDenial_RecTypeBase> baseData)
    {
        bool result = true;

        foreach (var item in baseData)
        {
            if (!string.IsNullOrEmpty(item.Maintenance_ActionCd))
            {
                result = false;
                break;
            }
        }

        return result;
    }

    private static void ValidateActionCode(MEPLicenceDenial_RecTypeBase data, ref MessageDataList result, ref bool isValid,
                                          bool isTermination)
    {
        bool validActionLifeState = true;

        string actionCode = data.Maintenance_ActionCd?.Trim();
        string actionState = data.dat_Appl_LiSt_Cd?.Trim();

        if (!string.IsNullOrEmpty(actionCode) && !string.IsNullOrEmpty(actionState))
        {
            if ((actionCode == "A") && actionState.NotIn("00", "0"))
                validActionLifeState = false;
            else if ((actionCode == "C") && (actionState.NotIn("00", "0", "14", "29")))
                validActionLifeState = false;
            else if (actionCode.NotIn("A", "C"))
                validActionLifeState = false;

            if (!isTermination && (actionCode == "C") && (actionState == "14"))
                validActionLifeState = false; // licence denial applications cannot be cancelled!
        }
        else validActionLifeState = false;

        if (!validActionLifeState)
        {
            isValid = false;
            result.AddSystemError($"Invalid MaintenanceAction [{actionCode}] and MaintenanceLifeState [{actionState}] combination.");
        }
    }

    private static MEPLicenceDenialFileData ExtractLicenceDenialDataFromJson(string sourceLicenceDenialData,
                                                                                            out string error)
    {
        error = string.Empty;

        MEPLicenceDenialFileData result;
        try
        {
            result = JsonConvert.DeserializeObject<MEPLicenceDenialFileData>(sourceLicenceDenialData);
        }
        catch
        {
            try
            {
                var single = JsonConvert.DeserializeObject<MEPLicenceDenialFileDataSingle>(sourceLicenceDenialData);
                if (single is null)
                    throw new NullReferenceException("json conversion failed for MEPLicenceDenialFileDataSingle");

                result = new MEPLicenceDenialFileData();
                result.NewDataSet.LICAPPIN01 = single.NewDataSet.LICAPPIN01;
                result.NewDataSet.LICAPPIN30.Add(single.NewDataSet.LICAPPIN30);
                result.NewDataSet.LICAPPIN31.Add(single.NewDataSet.LICAPPIN31);
                result.NewDataSet.LICAPPIN40.Add(single.NewDataSet.LICAPPIN40);
                result.NewDataSet.LICAPPIN41.Add(single.NewDataSet.LICAPPIN41);
                result.NewDataSet.LICAPPIN99 = single.NewDataSet.LICAPPIN99;
            }
            catch (Exception ee)
            {
                error = ee.Message;
                result = new MEPLicenceDenialFileData();
            }
        }

        return result;
    }

    private static LicenceDenialApplicationData GetLicenceDenialApplicationDataFromRequest(MEPLicenceDenial_RecTypeBase baseData, MEPLicenceDenial_RecType31 licenceDenialData)
    {
        var licenceDenialApplication = new LicenceDenialApplicationData
        {
            Appl_EnfSrv_Cd = baseData.dat_Appl_EnfSrvCd,
            Appl_CtrlCd = baseData.dat_Appl_CtrlCd,
            Appl_Source_RfrNr = baseData.dat_Appl_Source_RfrNr,
            Subm_Recpt_SubmCd = baseData.dat_Subm_Rcpt_SubmCd,
            Subm_SubmCd = baseData.dat_Subm_SubmCd,
            Appl_Lgl_Dte = baseData.dat_Appl_Lgl_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? default,
            Appl_Dbtr_SurNme = baseData.dat_Appl_Dbtr_SurNme,
            Appl_Dbtr_FrstNme = baseData.dat_Appl_Dbtr_FrstNme,
            Appl_Dbtr_MddleNme = baseData.dat_Appl_Dbtr_MddleNme,
            Appl_Dbtr_Brth_Dte = baseData.dat_Appl_Dbtr_Brth_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date,
            Appl_Dbtr_Gendr_Cd = baseData.dat_Appl_Dbtr_Gendr_Cd.Trim() == "" ? "M" : baseData.dat_Appl_Dbtr_Gendr_Cd.Trim(),
            Appl_Dbtr_Entrd_SIN = baseData.dat_Appl_Dbtr_Entrd_SIN,
            Appl_Dbtr_Parent_SurNme_Birth = baseData.dat_Appl_Dbtr_Parent_SurNme_Birth,
            Appl_CommSubm_Text = baseData.dat_Appl_CommSubm_Text,
            Appl_Rcptfrm_Dte = baseData.dat_Appl_Rcptfrm_dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? default,
            AppCtgy_Cd = baseData.dat_Appl_AppCtgy_Cd,
            Appl_Group_Batch_Cd = baseData.dat_Appl_Group_Batch_Cd,
            Medium_Cd = baseData.dat_Appl_Medium_Cd,
            Appl_Affdvt_DocTypCd = baseData.dat_Appl_Affdvt_Doc_TypCd,
            AppReas_Cd = baseData.dat_Appl_Reas_Cd,
            Appl_Reactv_Dte = baseData.dat_Appl_Reactv_Dte.ConvertToDateTimeIgnoringTimeZone(),
            AppLiSt_Cd = (ApplicationState)int.Parse(baseData.dat_Appl_LiSt_Cd),
            Appl_SIN_Cnfrmd_Ind = 0,
            ActvSt_Cd = "A",

            Appl_Crdtr_FrstNme = licenceDenialData.dat_Appl_Crdtr_FrstNme,
            Appl_Crdtr_MddleNme = licenceDenialData.dat_Appl_Crdtr_MddleNme,
            Appl_Crdtr_SurNme = licenceDenialData.dat_Appl_Crdtr_SurNme,
            Appl_Dbtr_LngCd = licenceDenialData.dat_Appl_Dbtr_LngCd,
            LicSusp_Dbtr_PhoneNumber = licenceDenialData.dat_LicSup_Dbtr_PhoneNumber,
            LicSusp_Dbtr_EmailAddress = licenceDenialData.dat_LicSup_Dbtr_EmailAddress,

            LicSusp_SupportOrder_Dte = licenceDenialData.dat_LicSup_SupportOrder_Dte.ConvertToDateTimeIgnoringTimeZone() ?? default,
            LicSusp_NoticeSentToDbtr_Dte = licenceDenialData.dat_LicSup_NoticeSntTDbtr_Dte.ConvertToDateTimeIgnoringTimeZone() ?? default,
            LicSusp_CourtNme = licenceDenialData.dat_LicSup_CourtNme,
            PymPr_Cd = licenceDenialData.dat_LicSup_PymPr_Cd,
            LicSusp_NrOfPymntsInDefault = ConvertToShortOrNull(licenceDenialData.dat_LicSup_NrOfPymntsInDefault),
            LicSusp_AmntOfArrears = ConvertToDecimalOrNull(licenceDenialData.dat_LicSup_AmntOfArrears),
            LicSusp_Dbtr_EmplNme = licenceDenialData.dat_LicSup_Dbtr_EmplNme,
            LicSusp_Dbtr_EmplAddr_Ln = licenceDenialData.dat_LicSup_Dbtr_EmplAddr_Ln,
            LicSusp_Dbtr_EmplAddr_Ln1 = licenceDenialData.dat_LicSup_Dbtr_EmplAddr_Ln1,
            LicSusp_Dbtr_EmplAddr_CityNme = licenceDenialData.dat_LicSup_Dbtr_EmplAddr_CtyNme,
            LicSusp_Dbtr_EmplAddr_PrvCd = licenceDenialData.dat_LicSup_Dbtr_EmplAddr_PrvCd,
            LicSusp_Dbtr_EmplAddr_CtryCd = licenceDenialData.dat_LicSup_Dbtr_EmplAddr_CtryCd,
            LicSusp_Dbtr_EmplAddr_PCd = licenceDenialData.dat_LicSup_Dbtr_EmplAddr_PCd,
            LicSusp_Dbtr_EyesColorCd = licenceDenialData.dat_LicSup_Dbtr_EyesColorCd,
            LicSusp_Dbtr_HeightUOMCd = licenceDenialData.dat_LicSup_Dbtr_HeightUOMCd,
            LicSusp_Dbtr_HeightQty = ConvertToIntOrNull(licenceDenialData.dat_LicSup_Dbtr_HeightQty),
            LicSusp_Dbtr_Brth_CityNme = licenceDenialData.dat_LicSup_Dbtr_Brth_CityNme,
            LicSusp_Dbtr_Brth_CtryCd = licenceDenialData.dat_LicSup_Dbtr_Brth_CtryCd,
            Appl_Dbtr_Addr_Ln = licenceDenialData.dat_Appl_Dbtr_Addr_Ln,
            Appl_Dbtr_Addr_Ln1 = licenceDenialData.dat_Appl_Dbtr_Addr_Ln1,
            Appl_Dbtr_Addr_CityNme = licenceDenialData.dat_Appl_Dbtr_Addr_CityNme,
            Appl_Dbtr_Addr_PrvCd = licenceDenialData.dat_Appl_Dbtr_Addr_PrvCd,
            Appl_Dbtr_Addr_CtryCd = licenceDenialData.dat_Appl_Dbtr_Addr_CtryCd,
            Appl_Dbtr_Addr_PCd = licenceDenialData.dat_Appl_Dbtr_Addr_PCd,

            LicSusp_Declaration = licenceDenialData.dat_LicSup_Declaration
        };

        return licenceDenialApplication;
    }

    private static short? ConvertToShortOrNull(string value)
    {
        if (short.TryParse(value, out short result))
            return result;
        else
            return null;
    }

    private static int? ConvertToIntOrNull(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (int.TryParse(value, out int result))
            return result;
        else
            return null;
    }

    private static decimal? ConvertToDecimalOrNull(string value)
    {
        if (decimal.TryParse(value, out decimal result))
            return result / 100M;
        else
            return null;
    }

    private static LicenceDenialApplicationData GetLicenceDenialTerminationApplicationDataFromRequest(MEPLicenceDenial_RecTypeBase baseData, MEPLicenceDenial_RecType41 licenceDenialData)
    {
        var licenceDenialTerminationApplication = new LicenceDenialApplicationData
        {
            Appl_EnfSrv_Cd = baseData.dat_Appl_EnfSrvCd,
            Appl_CtrlCd = baseData.dat_Appl_CtrlCd,
            Appl_Source_RfrNr = baseData.dat_Appl_Source_RfrNr,
            Subm_Recpt_SubmCd = baseData.dat_Subm_Rcpt_SubmCd,
            Subm_SubmCd = baseData.dat_Subm_SubmCd,
            Appl_Lgl_Dte = baseData.dat_Appl_Lgl_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? default,
            Appl_Dbtr_SurNme = baseData.dat_Appl_Dbtr_SurNme,
            Appl_Dbtr_FrstNme = baseData.dat_Appl_Dbtr_FrstNme,
            Appl_Dbtr_MddleNme = baseData.dat_Appl_Dbtr_MddleNme,
            Appl_Dbtr_Brth_Dte = baseData.dat_Appl_Dbtr_Brth_Dte.ConvertToDateTimeIgnoringTimeZone()?.Date,
            Appl_Dbtr_Gendr_Cd = baseData.dat_Appl_Dbtr_Gendr_Cd.Trim() == "" ? "M" : baseData.dat_Appl_Dbtr_Gendr_Cd.Trim(),
            Appl_Dbtr_Entrd_SIN = baseData.dat_Appl_Dbtr_Entrd_SIN,
            Appl_Dbtr_Parent_SurNme_Birth = baseData.dat_Appl_Dbtr_Parent_SurNme_Birth,
            Appl_CommSubm_Text = baseData.dat_Appl_CommSubm_Text,
            Appl_Rcptfrm_Dte = baseData.dat_Appl_Rcptfrm_dte.ConvertToDateTimeIgnoringTimeZone()?.Date ?? default,
            AppCtgy_Cd = baseData.dat_Appl_AppCtgy_Cd,
            Appl_Group_Batch_Cd = baseData.dat_Appl_Group_Batch_Cd,
            Medium_Cd = baseData.dat_Appl_Medium_Cd,
            Appl_Affdvt_DocTypCd = baseData.dat_Appl_Affdvt_Doc_TypCd,
            AppReas_Cd = baseData.dat_Appl_Reas_Cd,
            Appl_Reactv_Dte = baseData.dat_Appl_Reactv_Dte.ConvertToDateTimeIgnoringTimeZone(),
            AppLiSt_Cd = (ApplicationState)int.Parse(baseData.dat_Appl_LiSt_Cd),
            Appl_SIN_Cnfrmd_Ind = 0,
            ActvSt_Cd = "A",

            LicSusp_Dbtr_LastAddr_Ln = licenceDenialData.dat_Appl_Dbtr_Last_Addr_Ln,
            LicSusp_Dbtr_LastAddr_Ln1 = licenceDenialData.dat_Appl_Dbtr_Last_Addr_Ln1,
            LicSusp_Dbtr_LastAddr_CityNme = licenceDenialData.dat_Appl_Dbtr_Last_Addr_CityNme,
            LicSusp_Dbtr_LastAddr_PrvCd = licenceDenialData.dat_Appl_Dbtr_Last_Addr_PrvCd,
            LicSusp_Dbtr_LastAddr_CtryCd = licenceDenialData.dat_Appl_Dbtr_Last_Addr_CtryCd,
            LicSusp_Dbtr_LastAddr_PCd = licenceDenialData.dat_Appl_Dbtr_Last_Addr_PCd,

            LicSusp_Appl_CtrlCd = licenceDenialData.RefSusp_Appl_CtrlNr,

            // WARNING: RefSusp_Issuing_SubmCd doesn't seem to be used even though it is mandatory?

        };

        return licenceDenialTerminationApplication;
    }
}
