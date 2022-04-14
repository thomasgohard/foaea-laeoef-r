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
                    UpdateLicenceEventTables(item, newState: 2, flatFileName, validEvents, validEventDetails);

                SendLicenceDataToFOAEA(fedSource, flatFileName);
                /*
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

        private void SendLicenceDataToFOAEA(string fedSource, string fileName)
        {
            var dataToAppl = APIs.LicenceDenialApplicationAPIBroker.GetLicenceDenialToApplData(fedSource);

            foreach(var item in dataToAppl)
            {
                ProcessEventsToAppl(item, fileName);
            }
        }

        private void ProcessEventsToAppl(LicenceDenialToApplData item, string fileName)
        {
            
        }


        /*
         
        Dim eventLicensedata As Justice.FOAEA.Common.EventLicense
        Dim eventLicenseDtldata As Justice.FOAEA.Common.EventLicenseDetail

        Dim evntLicenseRow As Justice.FOAEA.Common.EventLicense.EvntLicenseRow()
        Dim evntLicenseDtlRow As Justice.FOAEA.Common.EventLicenseDetail.EvntLicense_dtlRow()

        Dim fullyProcessed As Boolean = True

        eventLicensedata = Me.DataClass.LICENSEEvents
        eventLicenseDtldata = Me.DataClass.LICENSEEventsDetail

        evntLicenseDtlRow = eventLicenseDtldata.EvntLicense_dtl.Select("Event_Id='" & LicenseDataToApplRow.Event_Id.ToString & "'")
        evntLicenseRow = eventLicensedata.EvntLicense.Select("Event_Id='" & LicenseDataToApplRow.Event_Id.ToString & "'")

        If LicenseDataToApplRow.Event_Reas_Cd = 50780 And LicenseDataToApplRow.ActvSt_Cd = "A" Then

            Dim sendFullyServed As New MidTier.Business.LicenseDenialSystem(DataClass.ConnectionStringFOAEA)

            Try
                sendFullyServed.LicenseResponse(LicenseDataToApplRow.Appl_EnfSrv_Cd, LicenseDataToApplRow.Appl_CtrlCd, "MSGBRO")
            Catch ex As Exception
                'cr 938 no record found
                If ex.Message = "There is no row at position 0." Then
                    Me.DataClass.MessageBrokerError("MessageBrokerService", "Message Broker Service Error", New SystemException(LicenseDataToApplRow.Appl_EnfSrv_Cd & "-" & LicenseDataToApplRow.Appl_CtrlCd & " no record found. File " & sFileName & " was loaded"), True)
                    Exit Sub
                Else
                    Dim newEventLog As New EventLog

                    newEventLog.Source = "Message Broker Inbound"
                    newEventLog.WriteEntry(ex.Message, System.Diagnostics.EventLogEntryType.Error)

                    newEventLog.Dispose()
                    Exit Sub
                End If
            End Try

            'CR 670 future use
        Else 'L03 update


            Dim L03processing As New MidTier.Business.LicenseDenialSystem(DataClass.ConnectionStringFOAEA)
            Try

                fullyProcessed = L03processing.ProcessL03(LicenseDataToApplRow.Appl_EnfSrv_Cd, LicenseDataToApplRow.Appl_CtrlCd, "MSGBRO")

            Catch ex As Exception
                'cr 938 no record found
                If ex.Message = "There is no row at position 0." Then
                    Me.DataClass.MessageBrokerError("MessageBrokerService", "Message Broker Service Error", New SystemException(LicenseDataToApplRow.Appl_EnfSrv_Cd & "-" & LicenseDataToApplRow.Appl_CtrlCd & " no record found. File " & sFileName & " was loaded"), True)
                    Exit Sub
                Else
                    Dim newEventLog As New EventLog

                    newEventLog.Source = "Message Broker Inbound"
                    newEventLog.WriteEntry(ex.Message, System.Diagnostics.EventLogEntryType.Error)

                    newEventLog.Dispose()
                    Exit Sub
                End If

            End Try
        End If

        'If the L03 is not fully processed, do not close the EventLivence record
        If evntLicenseRow.Count > 0 And fullyProcessed Then
            evntLicenseRow(0).ActvSt_Cd = LicenseDataToApplRow.ActvSt_Cd
            evntLicenseRow(0).Event_Compl_Dte = Date.Now
            eventLicensedata.EvntLicense.AcceptChanges()
        End If

        If evntLicenseDtlRow.Count > 0 Then
            evntLicenseDtlRow(0).ActvSt_Cd = LicenseDataToApplRow.ActvSt_Cd
            evntLicenseDtlRow(0).AppLiSt_Cd = LicenseDataToApplRow.dtl_List
            evntLicenseDtlRow(0).Event_Compl_Dte = Date.Now
            eventLicenseDtldata.EvntLicense_dtl.AcceptChanges()
        End If
    End Sub
         */

        private void VerifyReceivedDataForErrors(LicenceDenialResponseData item, string flatFileName,
                                                 ref List<ApplicationEventData> validEvents)
        {
            var eventForAppl = validEvents.Where(m => (m.Appl_EnfSrv_Cd == item.Appl_EnfSrv_Cd) &&
                                                             (m.Appl_CtrlCd == item.Appl_CtrlCd)).FirstOrDefault();
            //string responseType = string.Empty;

            if (eventForAppl is not null)
            {
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

        private void UpdateLicenceEventTables(LicenceDenialResponseData item, short newState, string fileName,
                                              List<ApplicationEventData> validEvents, List<ApplicationEventDetailData> validEventDetails)
        {
            string eventReason = $"[FileNm:{fileName}][RecPos:{item.LicRsp_SeqNr}][ErrDes:000000MSGBRO]" +
                                 $"[(EnfSrv:{item.Appl_EnfSrv_Cd})(CtrlCd:{item.Appl_CtrlCd})]" +
                                 $"[RqStat:{item.RqstStat_Cd}]";

            var eventForAppl = validEvents.Where(m => (m.Appl_EnfSrv_Cd == item.Appl_EnfSrv_Cd) &&
                                                 (m.Appl_CtrlCd == item.Appl_CtrlCd)).FirstOrDefault();
            if (eventForAppl != null)
            {
                int eventId = eventForAppl.Event_Id;

                var eventDetailForAppl = validEventDetails.Where(m => m.Event_Id == eventId).FirstOrDefault();
                if (eventDetailForAppl != null)
                {
                    switch (eventForAppl.Event_Reas_Cd)
                    {
                        case EventCode.C50780_APPLICATION_ACCEPTED: // L01
                            if ((eventForAppl.ActvSt_Cd != "C") && (item.RqstStat_Cd != 8))
                            {
                                eventDetailForAppl.Event_Reas_Text = eventReason;
                                eventDetailForAppl.Event_Effctv_Dte = DateTime.Now;
                                eventDetailForAppl.AppLiSt_Cd = newState;
                                eventDetailForAppl.ActvSt_Cd = "P";
                            }
                            break;
                        case EventCode.C50781_L03_ACCEPTED: // L03
                            if ((eventForAppl.ActvSt_Cd != "C") && (eventDetailForAppl.ActvSt_Cd != "C"))
                            {
                                eventDetailForAppl.Event_Reas_Text = eventReason;
                                eventDetailForAppl.Event_Effctv_Dte = DateTime.Now;
                                eventDetailForAppl.AppLiSt_Cd = newState;
                                eventDetailForAppl.ActvSt_Cd = "P";
                            }
                            break;
                    }

                    APIs.ApplicationEventAPIBroker.SaveEventDetail(eventDetailForAppl);
                }
            }
        }
    }
}
