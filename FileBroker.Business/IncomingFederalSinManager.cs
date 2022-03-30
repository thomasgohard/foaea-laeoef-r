using FileBroker.Business.Helpers;
using FileBroker.Data;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using Incoming.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileBroker.Business
{
    public class IncomingFederalSinManager
    {

        private APIBrokerList APIs { get; }
        private RepositoryList Repositories { get; }

        public IncomingFederalSinManager(APIBrokerList apiBrokers, RepositoryList repositories)
        {
            APIs = apiBrokers;
            Repositories = repositories;
        }

        public List<string> ProcessFlatFile(string flatFileContent, string flatFileName)
        {
            var fileTableData = GetFileTableData(flatFileName);

            var sinFileData = new FedSinFileBase();

            var errors = new List<string>();

            if (!fileTableData.Active.HasValue || !fileTableData.Active.Value)
                errors.Add($"[{fileTableData.Name}] is not active.");

            if (errors.Any())
                return errors;

            Repositories.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

            string fileCycle = Path.GetExtension(flatFileName)[1..];
            try
            {
                var fileLoader = new IncomingFederalSinFileLoader(Repositories.FlatFileSpecs, fileTableData.PrcId);
                fileLoader.FillSinFileDataFromFlatFile(sinFileData, flatFileContent, ref errors);

                if (errors.Any())
                    return errors;

                ValidateHeader(sinFileData.SININ01, flatFileName, ref errors);
                ValidateFooter(sinFileData.SININ99, sinFileData.SININ02, ref errors);

                if (errors.Any())
                    return errors;

                var sinResults = ExtractSinResultsFromFileData(sinFileData, fileCycle, ref errors);

                if (errors.Any())
                    return errors;

                if ((sinResults != null) && (sinFileData.SININ02.Count > 0))
                {
                    var processCodes = Repositories.ProcessParameterTable.GetProcessCodes(fileTableData.PrcId);

                    SendSinResultsToFOAEA(sinFileData.SININ02, sinResults, fileTableData.PrcId, fileCycle, flatFileName, 
                                          processCodes.AppLiSt_Cd, ref errors);
                }

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

        private void SendSinResultsToFOAEA(List<FedSin_RecType02> sININ02, List<SINResultData> sinResults, int prcId, 
                                           string fileCycle, string flatFileName, int appLiSt_Cd, 
                                           ref List<string> errors)
        {
            //.UpdateSINEventTables()
            var requestedEvents = APIs.ApplicationEventAPIBroker.GetRequestedSINEventDataForFile(flatFileName);
            UpdateSinEventTables(requestedEvents, appLiSt_Cd);

            //.SendSINDataTOSINValResult()
            APIs.SinAPIBroker.InsertBulkData(sinResults);

             //.SendEventSINDataToFOAEA()

             //.SendSINDatatoFOAEA()

             //.SendEventSINDataToFOAEA()
        }

        private void UpdateSinEventTables(List<ApplicationEventData> requestedEvents, int appLiSt_Cd)
        {
            foreach(var sinEvent in requestedEvents)
            {

            }

            /*
             
            For Each dRow As DataRow In SinData.Tables(0).Rows
                Me.DataClass.UpdateEventSIN(dRow, Me.DataClass.AppLiSt_Cd) 
            Next

            'this will force both the EvntSin and EvntSIN_dtl tables to be updated
            Me.DataClass.UpdateEventDetail = True

             
            Public Sub UpdateEventSIN(ByVal dRow As DataRow, ByVal appLiStCode As Integer)

                Dim eventID As Integer
                Dim eventReason As String

                eventReason = "[FileNm:" & dRow("Prcs_FileName").ToString & "]" & _
                              "[ErrDes:000000MSGBRO]" & _
                               "[(EnfSrv:" & dRow("Appl_EnfSrv_Cd").ToString & ")" & _
                               "(CtrlCd:" & dRow("Appl_CtrlCd").ToString & ")]"


                Dim evntSINRow As DataRow()
                evntSINRow = SINEvents.Tables("EvntSIN").Select("Appl_EnfSrv_Cd='" & dRow("Appl_EnfSrv_Cd").ToString.Trim & "'" & " and Appl_CtrlCd='" & dRow("Appl_CtrlCd").ToString.Trim & "'")

                If evntSINRow.Count > 0 Then
                    eventID = evntSINRow(0)("Event_Id")
                    evntSINRow(0)("ActvSt_Cd") = "P"
                    evntSINRow(0)("Event_Compl_Dte") = Date.Now
                    SINEvents.Tables("EvntSIN").AcceptChanges()
                End If

                Dim evntSINdtlRow As DataRow()
                evntSINdtlRow = SINEventsDetail.Tables("EvntSIN_dtl").Select("Event_Id='" & eventID.ToString & "'")

                If evntSINdtlRow.Count > 0 Then
                    evntSINdtlRow(0)("Event_Reas_Cd") = dRow("Prcs_Reas_Cd")
                    evntSINdtlRow(0)("Event_Reas_Text") = eventReason
                    evntSINdtlRow(0)("Event_Effctv_Dte") = Date.Now
                    evntSINdtlRow(0)("AppLiSt_Cd") = appLiStCode
                    evntSINdtlRow(0)("ActvSt_Cd") = "C"
                    evntSINdtlRow(0)("Event_Compl_Dte") = Date.Now
                    SINEventsDetail.Tables("EvntSIN_dtl").AcceptChanges()
                End If

            End Sub             
             */
        }

        private List<SINResultData> ExtractSinResultsFromFileData(FedSinFileBase sinFileData, string fileCycle,
                                                                  ref List<string> errors)
        {
            var sinResults = new List<SINResultData>();

            foreach (var sinResult in sinFileData.SININ02)
            {
                sinResults.Add(
                    new SINResultData
                    {
                        Appl_EnfSrv_Cd = sinResult.dat_Appl_EnfSrvCd,
                        Appl_CtrlCd = sinResult.dat_Appl_CtrlCd,
                        SVR_TimeStamp = sinFileData.SININ01.FileDate,
                        SVR_TolCd = sinResult.dat_SVR_TolCd,
                        SVR_SIN = sinResult.dat_SVR_SIN,
                        SVR_DOB_TolCd = sinResult.dat_SVR_DOB_TolCd,
                        SVR_GvnNme_TolCd = sinResult.dat_SVR_GvnNme_TolCd,
                        SVR_MddlNme_TolCd = sinResult.dat_SVR_MddlNme_TolCd,
                        SVR_SurNme_TolCd = sinResult.dat_SVR_SurNme_TolCd,
                        SVR_MotherNme_TolCd = sinResult.dat_SVR_MotherNme_TolCd,
                        SVR_Gendr_TolCd = sinResult.dat_SVR_Gendr_TolCd,
                        ValStat_Cd = sinResult.dat_ValStat_Cd,
                        ActvSt_Cd = "C"
                    }
                );
            }

            return sinResults;
        }


        private FileTableData GetFileTableData(string flatFileName)
        {
            string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

            return Repositories.FileTable.GetFileTableDataForFileName(fileNameNoCycle);
        }

        private static void ValidateHeader(FedSin_RecType01 dataFromFile, string flatFileName, ref List<string> errors)
        {
            int cycle = FileHelper.GetCycleFromFilename(flatFileName);
            if (dataFromFile.Cycle != cycle)
            {
                errors.Add($"Cycle in file [{dataFromFile.Cycle}] does not match cycle of file [{cycle}]");
            }
        }

        private static void ValidateFooter(FedSin_RecType99 dataFromFile, List<FedSin_RecType02> dataSummary, ref List<string> errors)
        {
            if (dataFromFile.ResponseCnt != dataSummary.Count)
            {
                errors.Add("Invalid ResponseCnt in section 99");
            }
        }

    }
}
