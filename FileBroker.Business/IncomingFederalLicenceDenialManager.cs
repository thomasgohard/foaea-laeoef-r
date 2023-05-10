using FileBroker.Common.Helpers;
using Newtonsoft.Json;

namespace FileBroker.Business
{
    public class IncomingFederalLicenceDenialManager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        private List<ApplicationEventData> ValidEvents { get; set; }

        private List<ApplicationEventDetailData> ValidEventDetails { get; set; }

        public IncomingFederalLicenceDenialManager(APIBrokerList apis, RepositoryList repositories,
                                                   IFileBrokerConfigurationHelper config)
        {
            APIs = apis;
            DB = repositories;
            ValidEvents = new List<ApplicationEventData>();
            ValidEventDetails = new List<ApplicationEventDetailData>();

            FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
        }

        public async Task<List<string>> ProcessJsonFileAsync(string jsonFileContent, string fileName)
        {
            var fileTableData = await GetFileTableDataAsync(fileName);

            var errors = new List<string>();

            if (fileTableData is null)
            {
                errors.Add($"fileTableData is empty for fileName [{fileName}].");
                return errors;
            }

            var fedSource = fileTableData.Name[0..2].ToUpper() switch
            {
                "PA" => "PA01",
                "TC" => "TC01",
                _ => ""
            };


            if (string.IsNullOrEmpty(fedSource))
                errors.Add($"Federal source could not be determined for {fileName}");

            if (!fileTableData.Active.HasValue || !fileTableData.Active.Value)
                errors.Add($"[{fileTableData.Name}] is not active.");

            if (errors.Any())
                return errors;

            await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, true);

            string fileCycle = Path.GetExtension(fileName)[1..];
            try
            {
                await FoaeaAccess.SystemLoginAsync();
                try
                {
                    var licenceDenialResponses = ExtractLicenceDenialResponsesFromJson(jsonFileContent, ref errors);

                    if (errors.Any())
                        return errors;

                    var result = new MessageDataList();

                    ValidateHeader(licenceDenialResponses.NewDataSet, fileName, ref errors);
                    ValidateFooter(licenceDenialResponses.NewDataSet, ref errors);
                    await ValidateDetailsAsync(licenceDenialResponses.NewDataSet, fedSource, errors);

                    if (errors.Any())
                        return errors;

                    var licenceDenialFoaeaResponseData = GenerateLicenceDenialResponseDataFromIncomingResponses(
                                                                                                        licenceDenialResponses,
                                                                                                        fileName, fedSource);

                    if ((licenceDenialFoaeaResponseData != null) && (licenceDenialFoaeaResponseData.Count > 0))
                    {
                        await SendLicenceDenialResponsesToFOAEAAsync(licenceDenialFoaeaResponseData, fedSource, fileName, errors);

                        await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, fileCycle.Length);
                    }
                }
                finally
                {
                    await FoaeaAccess.SystemLogoutAsync();
                }

            }
            catch (Exception e)
            {
                errors.Add("An exception occurred: " + e.Message);
            }
            finally
            {
                await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, false);
            }

            return errors;
        }

        private static FedLicenceDenialFileData ExtractLicenceDenialResponsesFromJson(string sourceLicenceDenialData,
                                                                                      ref List<string> errors)
        {

            FedLicenceDenialFileData result;
            try
            {
                result = JsonConvert.DeserializeObject<FedLicenceDenialFileData>(sourceLicenceDenialData);
            }
            catch
            {
                try
                {
                    var single = JsonConvert.DeserializeObject<FedLicenceDenialFileDataSingle>(sourceLicenceDenialData);
                    if (single is null)
                        throw new NullReferenceException("json conversion failed for FedLicenceDenialFileDataSingle");

                    result = new FedLicenceDenialFileData();
                    result.NewDataSet.LICIN01 = single.NewDataSet.LICIN01;
                    result.NewDataSet.LICIN02.Add(single.NewDataSet.LICIN02);
                    result.NewDataSet.LICIN99 = single.NewDataSet.LICIN99;
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);
                    result = new FedLicenceDenialFileData();
                }
            }

            return result;
        }

        private List<LicenceDenialResponseData> GenerateLicenceDenialResponseDataFromIncomingResponses(
                                                           FedLicenceDenialFileData licenceDenialResponses, string fileName,
                                                           string fedSource)
        {
            var responseData = new List<LicenceDenialResponseData>();
            short sequence = 0;
            foreach (var detail in licenceDenialResponses.NewDataSet.LICIN02)
            {
                var existingEventsForAppl = ValidEvents.Where(m => m.Appl_EnfSrv_Cd.Equals(detail.Appl_EnfSrv_Cd) &&
                                                                   m.Appl_CtrlCd.Equals(detail.Appl_CtrlCd)).FirstOrDefault();

                var responseType = GetResponseType(existingEventsForAppl);

                responseData.Add(new LicenceDenialResponseData
                {
                    Appl_EnfSrv_Cd = detail.Appl_EnfSrv_Cd,
                    Appl_CtrlCd = detail.Appl_CtrlCd,
                    EnfSrv_Cd = fedSource,
                    LicRsp_Rcpt_Dte = DateTime.Now,
                    LicRsp_SeqNr = sequence++,
                    RqstStat_Cd = short.Parse(detail.RqstStat_Cd),
                    LicRspSource_RefNo = detail.Source_RefNo,
                    LicRsp_Comments = detail.LicRsp_Comments,
                    LicRspFilename = fileName,
                    LicRspType = responseType
                });
            }
            return responseData;
        }

        private async Task<FileTableData> GetFileTableDataAsync(string fileName)
        {
            string fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);

            return await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        }

        private static void ValidateHeader(FedLicenceDenial_DataSet licenceDenialFile, string fileName, ref List<string> result)
        {
            int cycle = FileHelper.ExtractCycleFromFilename(fileName);
            if (int.Parse(licenceDenialFile.LICIN01.Cycle) != cycle)
                result.Add($"Cycle in file [{licenceDenialFile.LICIN01.Cycle}] does not match cycle of file [{cycle}]");
        }

        private static void ValidateFooter(FedLicenceDenial_DataSet licenceDenialFile, ref List<string> result)
        {
            if (licenceDenialFile.LICIN99.ResponseCnt != licenceDenialFile.LICIN02.Count)
                result.Add("Invalid ResponseCnt in section 99");
        }

        private async Task ValidateDetailsAsync(FedLicenceDenial_DataSet newDataSet, string fedSource, List<string> result)
        {
            var invalidApplications = new List<string>();
            ValidEvents.Clear();
            ValidEventDetails.Clear();

            foreach (var detail in newDataSet.LICIN02)
            {
                string applEnfSrv = detail.Appl_EnfSrv_Cd;
                string applControlCode = detail.Appl_CtrlCd;

                var events = await APIs.LicenceDenialEvents.GetRequestedLICINEventsAsync(fedSource, applEnfSrv, applControlCode);
                if (events.Count == 0)
                {
                    string errorMsg = $"type 02 {applEnfSrv} {applControlCode} no active event found. File was not loaded.";
                    result.Add(errorMsg);
                    invalidApplications.Add(applEnfSrv + " " + applControlCode);
                }
                else
                {
                    var existingEventsForAppl = ValidEvents.Where(m => m.Appl_EnfSrv_Cd.Equals(applEnfSrv) && m.Appl_CtrlCd.Equals(applControlCode));
                    if (!existingEventsForAppl.Any())
                        ValidEvents.AddRange(events);

                    var eventDetails = await APIs.LicenceDenialEvents.GetRequestedLICINEventDetailsAsync(fedSource, applEnfSrv, applControlCode);
                    if (eventDetails.Count == 0)
                    {
                        string errorMsg = $"type 02 {applEnfSrv} {applControlCode} no active event found. File was not loaded.";
                        result.Add(errorMsg);
                        invalidApplications.Add(applEnfSrv + " " + applControlCode);
                    }
                    else
                    {
                        foreach (var eventData in events)
                        {
                            var existingEventDetailsForAppl = eventDetails.Where(m => m.Event_Id == eventData.Event_Id).ToList();
                            if (existingEventDetailsForAppl.Any())
                                ValidEventDetails.AddRange(existingEventDetailsForAppl);
                        }
                    }
                }

            }
        }

        public async Task SendLicenceDenialResponsesToFOAEAAsync(List<LicenceDenialResponseData> licenceDenialResponseData,
                                                      string fedSource, string fileName,
                                                      List<string> errors)
        {
            try
            {
                foreach (var item in licenceDenialResponseData)
                    await VerifyReceivedDataForErrorsAsync(item, fedSource, fileName);

                var dataToSave = licenceDenialResponseData.Where(m => (m.RqstStat_Cd != 5) || (fedSource != "PA01")).ToList();
                await APIs.LicenceDenialResponses.InsertBulkDataAsync(dataToSave);

                foreach (var item in licenceDenialResponseData)
                    await UpdateLicenceEventTablesAsync(item, newState: 2, fileName);

                await ProcessLicenceApplicationsAsync(fedSource, fileName, errors);
            }
            catch (Exception e)
            {
                errors.Add("Error sending licence denial responses to FOAEA: " + e.Message);
            }
        }

        private async Task ProcessLicenceApplicationsAsync(string fedSource, string fileName, List<string> errors)
        {
            var dataToAppl = await APIs.LicenceDenialApplications.GetLicenceDenialToApplDataAsync(fedSource);

            foreach (var item in dataToAppl)
                await UpdateLicenceApplicationAsync(item, fileName, errors);
        }

        private async Task UpdateLicenceApplicationAsync(LicenceDenialToApplData item, string fileName, List<string> errors)
        {
            var licenceEvent = ValidEvents.Where(m => m.Event_Id == item.Event_Id).FirstOrDefault();
            var licenseDetailEvent = ValidEventDetails.Where(m => m.Event_Id == item.Event_Id).FirstOrDefault();

            var responseType = GetResponseType(licenceEvent);

            bool fullyProcessed = true;

            if ((responseType == "L01") && (item.ActvSt_Cd == "A"))
            {
                var applLicenceDenial = await APIs.LicenceDenialApplications.ProcessLicenceDenialResponseAsync(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);

                if (applLicenceDenial.Messages.ContainsMessagesOfType(MessageType.Error))
                {
                    foreach (var error in applLicenceDenial.Messages.GetMessagesForType(MessageType.Error))
                    {
                        errors.Add(error.Description);
                        if (error.Description == SystemMessage.APPLICATION_NOT_FOUND)
                        {
                            string errorMsg = $"{item.Appl_EnfSrv_Cd}-{item.Appl_CtrlCd} no record found. File {fileName} was loaded";
                            await DB.ErrorTrackingTable.MessageBrokerErrorAsync("MessageBrokerService", "File Broker Service Error",
                                                                                       new Exception(errorMsg), displayExceptionError: true);
                        }
                    }
                }
            }
            else if (responseType == "L03")
            {
                var applLicenceDenialTermination = await APIs.LicenceDenialTerminationApplications.ProcessLicenceDenialResponseAsync(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);

                if (applLicenceDenialTermination.Messages.ContainsMessagesOfType(MessageType.Error))
                {
                    foreach (var error in applLicenceDenialTermination.Messages.GetMessagesForType(MessageType.Error))
                    {
                        errors.Add(error.Description);
                        if (error.Description == SystemMessage.APPLICATION_NOT_FOUND)
                        {
                            string errorMsg = $"{item.Appl_EnfSrv_Cd}-{item.Appl_CtrlCd} no record found. File {fileName} was loaded";
                            await DB.ErrorTrackingTable.MessageBrokerErrorAsync("MessageBrokerService", "File Broker Service Error",
                                                                                        new Exception(errorMsg), displayExceptionError: true);
                        }
                    }
                }

                fullyProcessed = applLicenceDenialTermination.AppLiSt_Cd == ApplicationState.EXPIRED_15;

            }

            if ((licenceEvent is not null) && fullyProcessed)
            {
                licenceEvent.ActvSt_Cd = item.ActvSt_Cd;
                licenceEvent.Event_Compl_Dte = DateTime.Now;
                await APIs.ApplicationEvents.SaveEventAsync(licenceEvent);
            }

            if (licenseDetailEvent is not null)
            {
                licenseDetailEvent.ActvSt_Cd = item.ActvSt_Cd;
                licenseDetailEvent.AppLiSt_Cd = item.Dtl_List;
                licenseDetailEvent.Event_Compl_Dte = DateTime.Now;
                await APIs.ApplicationEvents.SaveEventDetailAsync(licenseDetailEvent);
            }

        }

        private async Task VerifyReceivedDataForErrorsAsync(LicenceDenialResponseData item, string fedSource, string fileName)
        {
            var eventForAppl = ValidEvents.Where(m => (m.Appl_EnfSrv_Cd == item.Appl_EnfSrv_Cd) &&
                                                      (m.Appl_CtrlCd == item.Appl_CtrlCd)).FirstOrDefault();

            if (eventForAppl is not null)
            {
                // CR 932 remove code 05 from Passport Canada  
                if ((item.RqstStat_Cd == 5) && (fedSource == "PA01"))
                {
                    string errorMsg = $"{item.Appl_EnfSrv_Cd}-{item.Appl_CtrlCd} code 05 not loaded Data In Foaea. File Name: {fileName}";
                    await DB.ErrorTrackingTable.MessageBrokerErrorAsync("MessageBrokerService", "File Broker Service Error",
                                                                    new Exception(errorMsg), displayExceptionError: true);
                }
            }

        }

        private async Task UpdateLicenceEventTablesAsync(LicenceDenialResponseData item, short newState, string fileName)
        {
            string eventReason = $"[FileNm:{fileName}][RecPos:{item.LicRsp_SeqNr}][ErrDes:000000MSGBRO]" +
                                 $"[(EnfSrv:{item.Appl_EnfSrv_Cd.Trim()})(CtrlCd:{item.Appl_CtrlCd.Trim()})]" +
                                 $"[RqStat:{item.RqstStat_Cd}]";

            var eventForAppl = ValidEvents.Where(m => (m.Appl_EnfSrv_Cd.Trim() == item.Appl_EnfSrv_Cd.Trim()) &&
                                                      (m.Appl_CtrlCd.Trim() == item.Appl_CtrlCd.Trim())).FirstOrDefault();
            if (eventForAppl != null)
            {
                int eventId = eventForAppl.Event_Id;

                var eventDetailForAppl = ValidEventDetails.Where(m => m.Event_Id == eventId).FirstOrDefault();
                if (eventDetailForAppl != null)
                {
                    var responseType = GetResponseType(eventForAppl);

                    switch (responseType)
                    {
                        case "L01":
                            if ((eventForAppl.ActvSt_Cd != "C") && (item.RqstStat_Cd != 8))
                            {
                                eventDetailForAppl.Event_Reas_Text = eventReason;
                                eventDetailForAppl.Event_Effctv_Dte = DateTime.Now;
                                eventDetailForAppl.AppLiSt_Cd = newState;
                                eventDetailForAppl.ActvSt_Cd = "P";
                            }
                            break;

                        case "L03":
                            if ((eventForAppl.ActvSt_Cd != "C") && (eventDetailForAppl.ActvSt_Cd != "C"))
                            {
                                eventDetailForAppl.Event_Reas_Text = eventReason;
                                eventDetailForAppl.Event_Effctv_Dte = DateTime.Now;
                                eventDetailForAppl.AppLiSt_Cd = newState;
                                eventDetailForAppl.ActvSt_Cd = "P";
                            }
                            break;
                    }

                    await APIs.ApplicationEvents.SaveEventDetailAsync(eventDetailForAppl);
                }
            }
        }

        private static string GetResponseType(ApplicationEventData existingEventsForAppl)
        {
            var responseType = string.Empty;
            if (existingEventsForAppl != null)
            {
                if (existingEventsForAppl.Event_Reas_Cd == EventCode.C50780_APPLICATION_ACCEPTED)
                    responseType = "L01";
                else if (existingEventsForAppl.Event_Reas_Cd == EventCode.C50781_L03_ACCEPTED)
                    responseType = "L03";
            }

            return responseType;
        }

    }
}
