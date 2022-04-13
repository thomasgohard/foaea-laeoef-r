using Newtonsoft.Json;

namespace FileBroker.Business
{
    public class IncomingFederalLicenceDenialManager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList Repositories { get; }

        public IncomingFederalLicenceDenialManager(APIBrokerList apiBrokers, RepositoryList repositories)
        {
            APIs = apiBrokers;
            Repositories = repositories;
        }

        public List<string> ProcessJsonFile(string flatFileContent, string fileName)
        {
            var fileTableData = GetFileTableData(fileName);
            var fedSource = fileTableData?.Name[0..1].ToUpper() switch
            {
                "PA" => "PA01",
                "TC" => "TC01",
                _ => ""
            };

            var errors = new List<string>();

            if (!fileTableData.Active.HasValue || !fileTableData.Active.Value)
                errors.Add($"[{fileTableData.Name}] is not active.");

            if (errors.Any())
                return errors;

            Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

            string fileCycle = Path.GetExtension(fileName)[1..];
            try
            {
                var licenceDenialResponses = ExtractLicenceDenialDataFromJson(flatFileContent, out string error);

                if (!string.IsNullOrEmpty(error))
                    errors.Add(error);

                if (errors.Any())
                    return errors;

                MessageDataList result = new MessageDataList();

                ValidateHeader(licenceDenialResponses.NewDataSet, fileName, ref errors);
                ValidateFooter(licenceDenialResponses.NewDataSet, ref errors);
                ValidateDetails(licenceDenialResponses.NewDataSet, fedSource, ref errors,
                                out List<ApplicationEventData> validEvents,
                                out List<ApplicationEventDetailData> validEventDetails);

                if (errors.Any())
                    return errors;

                var licenceDenialResponseData = ExtractFoaeaResponseDataFromIncomingResponseData(licenceDenialResponses, 
                                                                                                 fileName, validEvents);

                if ((licenceDenialResponseData != null) && (licenceDenialResponseData.Count > 0))
                    SendLicenceDenialResponsesToFOAEA(licenceDenialResponseData, fileTableData.PrcId,
                                                      fedSource, fileCycle, fileName, validEvents, validEventDetails,
                                                      ref errors);

            }
            catch (Exception e)
            {
                errors.Add("An error occurred: " + e.Message);
            }
            finally
            {
                Repositories.FileTable.SetNextCycleForFileType(fileTableData, fileCycle.Length);
                Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, false);
            }

            return errors;

        }

        private List<LicenceDenialResponseData> ExtractFoaeaResponseDataFromIncomingResponseData(FedLicenceDenialFileData licenceDenialResponses, string fileName,
                                                                                                 List<ApplicationEventData> validEvents)
        {
            string enfSrvCode = (fileName[0..2].ToUpper() == "PA3") ? "PA01" : "TC01";
            var responseData = new List<LicenceDenialResponseData>();
            short recordID = 0;
            foreach (var item in licenceDenialResponses.NewDataSet.LICIN02)
            {
                var existingEventsForAppl = validEvents.Where(m => m.Appl_EnfSrv_Cd.Equals(item.Appl_EnfSrv_Cd) &&
                                                                   m.Appl_CtrlCd.Equals(item.Appl_CtrlCd)).FirstOrDefault();

                var responseType = string.Empty;
                if (existingEventsForAppl != null)
                {
                    if (existingEventsForAppl.Event_Reas_Cd == EventCode.C50780_APPLICATION_ACCEPTED)
                        responseType = "L01";
                    else if (existingEventsForAppl.Event_Reas_Cd == EventCode.C50781_L03_ACCEPTED)
                        responseType = "L03";
                }

                responseData.Add(new LicenceDenialResponseData
                {
                    Appl_EnfSrv_Cd = item.Appl_EnfSrv_Cd,
                    Appl_CtrlCd = item.Appl_CtrlCd,
                    EnfSrv_Cd = enfSrvCode,
                    LicRsp_Rcpt_Dte = DateTime.Now,
                    LicRsp_SeqNr = recordID++,
                    RqstStat_Cd = short.Parse(item.RqstStat_Cd),
                    LicRspSource_RefNo = item.Source_RefNo,
                    LicRsp_Comments = item.LicRsp_Comments,
                    LicRspFilename = fileName,
                    LicRspType = responseType
                });
            }
            return responseData;
        }

        private static FedLicenceDenialFileData ExtractLicenceDenialDataFromJson(string sourceLicenceDenialData, out string error)
        {
            error = string.Empty;

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

                    result = new FedLicenceDenialFileData();
                    result.NewDataSet.LICIN01 = single.NewDataSet.LICIN01;
                    result.NewDataSet.LICIN02.Add(single.NewDataSet.LICIN02);
                    result.NewDataSet.LICIN99 = single.NewDataSet.LICIN99;
                }
                catch (Exception ee)
                {
                    error = ee.Message;
                    result = new FedLicenceDenialFileData();
                }
            }

            return result;
        }

        private FileTableData GetFileTableData(string flatFileName)
        {
            string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

            return Repositories.FileTable.GetFileTableDataForFileName(fileNameNoCycle);
        }

        private void ValidateHeader(FedLicenceDenial_DataSet licenceDenialFile, string fileName, ref List<string> result)
        {
            int cycle = FileHelper.GetCycleFromFilename(fileName);
            if (int.Parse(licenceDenialFile.LICIN01.Cycle) != cycle)
                result.Add($"Cycle in file [{licenceDenialFile.LICIN01.Cycle}] does not match cycle of file [{cycle}]");
        }

        private static void ValidateFooter(FedLicenceDenial_DataSet licenceDenialFile, ref List<string> result)
        {
            if (licenceDenialFile.LICIN99.ResponseCnt != licenceDenialFile.LICIN02.Count)
                result.Add("Invalid ResponseCnt in section 99");
        }

        private void ValidateDetails(FedLicenceDenial_DataSet newDataSet, string fedSource, ref List<string> result,
                                     out List<ApplicationEventData> validEvents,
                                     out List<ApplicationEventDetailData> validEventDetails)
        {
            var invalidApplications = new List<string>();
            validEvents = new List<ApplicationEventData>();
            validEventDetails = new List<ApplicationEventDetailData>();

            foreach (var detail in newDataSet.LICIN02)
            {
                string applEnfSrv = detail.Appl_EnfSrv_Cd;
                string applControlCode = detail.Appl_CtrlCd;

                var events = APIs.LicenceDenialEventAPIBroker.GetRequestedLICINEvents(fedSource, applEnfSrv, applControlCode);
                if (events.Count == 0)
                {
                    string errorMsg = $"type 02 {applEnfSrv} {applControlCode} no active event found. File was not loaded.";
                    result.Add(errorMsg);
                    invalidApplications.Add(applEnfSrv + " " + applControlCode);
                }
                else
                {
                    var existingEventsForAppl = validEvents.Where(m => m.Appl_EnfSrv_Cd.Equals(applEnfSrv) && m.Appl_CtrlCd.Equals(applControlCode));
                    if (!existingEventsForAppl.Any())
                        validEvents.AddRange(events);

                    var eventDetails = APIs.LicenceDenialEventAPIBroker.GetRequestedLICINEventDetails(fedSource, applEnfSrv, applControlCode);
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
                            if (!existingEventDetailsForAppl.Any())
                                validEventDetails.AddRange(existingEventDetailsForAppl);
                        }
                    }
                }

            }
        }

        public void SendLicenceDenialResponsesToFOAEA(List<LicenceDenialResponseData> licenceDenialResponseData,
                                                      int processId, string fedSource, string fileCycle,
                                                      string flatFileName, List<ApplicationEventData> validEvents,
                                                      List<ApplicationEventDetailData> validEventDetails,
                                                      ref List<string> errors)
        {
            try
            {
                string cutOffDaysValue = Repositories.ProcessParameterTable.GetValueForParameter(processId, "evnt_cutoff");
                int cutOffDays = int.Parse(cutOffDaysValue);

                foreach (var item in licenceDenialResponseData)
                    VerifyReceivedDataForErrors(item, flatFileName, ref validEvents); // <-- is this really needed?

                var dataToSave = licenceDenialResponseData.Where(m => (m.RqstStat_Cd != 5) || !flatFileName.ToUpper().StartsWith("PA3SL")).ToList();
                APIs.LicenceDenialResponseAPIBroker.InsertBulkData(dataToSave);

                foreach (var item in licenceDenialResponseData)
                    UpdateLicenceEventTables(item, newState: 2, flatFileName);

                // ByVal dRow As Common.LicenseResponseData.LicRspRow, ByVal appLiStCode As Integer, ByVal fileName As String

                //foreach (var item in fileLicenceDenialSummary)
                //{
                //    MarkLicenceDenialEventsAsProcessed(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd, flatFileName, 2, ref activeLicenceDenialEvents, ref activeLicenceDenialEventDetails);
                //}
                //CloseOrInactivateTraceEventDetails(cutOffDays, ref activeLicenceDenialEventDetails);
                //SendTRACEDataToTrcRsp(licenceDenialResponses);
                //CloseNETPTraceEvents();
                /*
                .SendDataToLicenseSuspension(fInfo.Name)
                .UpdateLicenseEventTables()
                .SendEventLicenseDataToFOAEA()
                .SendLicenseDataToFOAEA(fInfo.Name)
                .SendEventLicenseDataToFOAEA()
                .UpdateCycle()
                 */
            }
            catch (Exception e)
            {
                errors.Add("Error sending licence denial responses to FOAEA: " + e.Message);
            }
        }

        private void VerifyReceivedDataForErrors(LicenceDenialResponseData item, string flatFileName,
                                                 ref List<ApplicationEventData> validEvents)
        {
            var licenceEventForAppl = validEvents.Where(m => (m.Appl_EnfSrv_Cd == item.Appl_EnfSrv_Cd) &&
                                                             (m.Appl_CtrlCd == item.Appl_CtrlCd)).FirstOrDefault();
            //string responseType = string.Empty;

            if (licenceEventForAppl is not null)
            {
                //if (licenceEventForAppl.Event_Reas_Cd == EventCode.C50780_APPLICATION_ACCEPTED)
                //    responseType = "L01";
                //else if (licenceEventForAppl.Event_Reas_Cd == EventCode.C50781_L03_ACCEPTED)
                //    responseType = "L03";

                var licenceDenialAppl = APIs.LicenceDenialApplicationAPIBroker.GetApplication(item.Appl_EnfSrv_Cd, item.Appl_CtrlCd);

                // CR 932 remove code 05 from Passport Canada  
                if ((item.RqstStat_Cd == 5) && flatFileName.ToUpper().StartsWith("PA3SL"))
                {
                    string errorMsg = $"{item.Appl_EnfSrv_Cd}-{item.Appl_CtrlCd} code 05 not loaded Data In Foaea. File Name: {flatFileName}";
                    Repositories.ErrorTrackingDB.MessageBrokerError("MessageBrokerService", "File Broker Service Error",
                                                                    new Exception(errorMsg), displayExceptionError: true);
                }
            }

        }

        private void UpdateLicenceEventTables(LicenceDenialResponseData item, int newState, string flatFileName)
        {
            throw new NotImplementedException();
        }

        /*
    Public Sub UpdateEventLicense(ByVal dRow As Common.LicenseResponseData.LicRspRow, ByVal appLiStCode As Integer, ByVal fileName As String)

        Dim eventID As Integer
        '[FileNm:' + S2.Prcs_FileName + ']' + '[RecPos:' + CONVERT(varchar , S2.Prcs_RecPos) + ']' + '[ErrDes:' + S2.Prcs_RecErr + ']' + '[(EnfSrv:' + S2.dat_Appl_EnfSrv_Cd + ')' + '(CtrlCd:' + S2.dat_Appl_CtrlCd + ')]'
        Dim eventReason As String
        eventReason = "[FileNm:" & fileName & "]" & _
                        "[RecPos:" & dRow.LicRsp_SeqNr.ToString & "]" & _
                        "[ErrDes:000000MSGBRO]" & _
                        "[(EnfSrv:" & dRow.Appl_EnfSrv_Cd.ToString & ")" & _
                        "(CtrlCd:" & dRow.Appl_CtrlCd.ToString & ")]" & _
                        "[RqStat:" & dRow.RqstStat_Cd.ToString & "]"

        Dim evntLICENSERow As Common.EventLicense.EvntLicenseRow()
        evntLICENSERow = LICENSEEvents.EvntLicense.Select("Appl_EnfSrv_Cd='" & dRow("Appl_EnfSrv_Cd").ToString.Trim & "'" & " and Appl_CtrlCd='" & dRow("Appl_CtrlCd").ToString.Trim & "'")

        If evntLICENSERow.Count > 0 Then
            eventID = evntLICENSERow(0).Event_Id
            'evntLICENSERow(0).ActvSt_Cd = "P"
            'evntLICENSERow(0).Event_Compl_Dte = Date.Now
            'LICENSEEvents.EvntLicense.AcceptChanges()
        End If

        Dim evntLICENSEdtlRow As Common.EventLicenseDetail.EvntLicense_dtlRow()
        evntLICENSEdtlRow = LICENSEEventsDetail.EvntLicense_dtl.Select("Event_Id='" & eventID.ToString & "'")

        If evntLICENSEdtlRow.Count > 0 Then

            'If evntLICENSERow(0).Event_Reas_Cd = 50780 And evntLICENSERow(0).ActvSt_Cd <> "C" And dRow.RqstStat_Cd <> 8 Then
            '    'evntLICENSEdtlRow(0).Event_Reas_Cd = evntLICENSERow(0).Event_Reas_Cd
            '    evntLICENSEdtlRow(0).Event_Reas_Text = eventReason
            '    evntLICENSEdtlRow(0).Event_Effctv_Dte = Date.Now
            '    evntLICENSEdtlRow(0).AppLiSt_Cd = appLiStCode
            '    'evntTRACEdtlRow(0)("ActvSt_Cd") = "P"
            '    'Set the event to completed.
            '    evntLICENSEdtlRow(0).ActvSt_Cd = "P"

            'ElseIf evntLICENSERow(0).Event_Reas_Cd = 50781 And evntLICENSERow(0).ActvSt_Cd <> "C" And (dRow.RqstStat_Cd = 8 Or dRow.RqstStat_Cd = 1) Then
            '    'evntLICENSEdtlRow(0).Event_Reas_Cd = evntLICENSERow(0).Event_Reas_Cd
            '    evntLICENSEdtlRow(0).Event_Reas_Text = eventReason
            '    evntLICENSEdtlRow(0).Event_Effctv_Dte = Date.Now
            '    evntLICENSEdtlRow(0).AppLiSt_Cd = appLiStCode
            '    'evntTRACEdtlRow(0)("ActvSt_Cd") = "P"
            '    'Set the event to completed.
            '    evntLICENSEdtlRow(0).ActvSt_Cd = "P"
            'End If
            Select Case evntLICENSERow(0).Event_Reas_Cd
                Case 50780
                    If evntLICENSERow(0).ActvSt_Cd <> "C" And dRow.RqstStat_Cd <> 8 Then
                        'evntLICENSEdtlRow(0).Event_Reas_Cd = evntLICENSERow(0).Event_Reas_Cd
                        evntLICENSEdtlRow(0).Event_Reas_Text = eventReason
                        evntLICENSEdtlRow(0).Event_Effctv_Dte = Date.Now
                        evntLICENSEdtlRow(0).AppLiSt_Cd = appLiStCode
                        'evntTRACEdtlRow(0)("ActvSt_Cd") = "P"
                        'Set the event to completed.
                        evntLICENSEdtlRow(0).ActvSt_Cd = "P"
                    End If
                Case 50781
                    If evntLICENSERow(0).ActvSt_Cd <> "C" Then
                        If evntLICENSEdtlRow(0).ActvSt_Cd <> "C" Then
                            Select Case dRow.RqstStat_Cd
                                Case 1, 4, 8
                                    'evntLICENSEdtlRow(0).Event_Reas_Cd = evntLICENSERow(0).Event_Reas_Cd
                                    evntLICENSEdtlRow(0).Event_Reas_Text = eventReason
                                    evntLICENSEdtlRow(0).Event_Effctv_Dte = Date.Now
                                    evntLICENSEdtlRow(0).AppLiSt_Cd = appLiStCode
                                    'evntTRACEdtlRow(0)("ActvSt_Cd") = "P"
                                    'Set the event to completed.
                                    evntLICENSEdtlRow(0).ActvSt_Cd = "P"
                                Case Else
                                    ' Future use to filter the response code coming back from passport / licensing 
                                    'Dim messageText As String = ""
                                    'messageText = messageText & "L03 Request Status " & "0" & dRow.RqstStat_Cd.ToString.Trim & " received from " & dRow.EnfSrv_Cd & vbCrLf & vbLf
                                    'messageText = messageText & "L01 application " & dRow.Appl_EnfSrv_Cd & " " & dRow.Appl_CtrlCd & vbCrLf & vbLf
                                    'messageText = messageText & "L03 Request Status message: " & LiceneseDenialRequestStatus.Get("0" & dRow.RqstStat_Cd.ToString.Trim)
                                    'SendMailMessage("L03 Request Status incopmlete", "FLAS-IT-SO@justice.gc.ca", messageText)
                                    evntLICENSEdtlRow(0).Event_Reas_Text = eventReason
                                    evntLICENSEdtlRow(0).Event_Effctv_Dte = Date.Now
                                    evntLICENSEdtlRow(0).AppLiSt_Cd = appLiStCode
                                    evntLICENSEdtlRow(0).ActvSt_Cd = "P"
                            End Select
                        End If
                    End If
            End Select


            LICENSEEventsDetail.EvntLicense_dtl.AcceptChanges()
            LICENSEEvents.EvntLicense.AcceptChanges()
        End If

    End Sub
        
        */
    }
}
