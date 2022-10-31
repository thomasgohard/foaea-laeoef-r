using DBHelper;
using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialTerminationManager : ApplicationManager
    {
        public LicenceDenialApplicationData LicenceDenialTerminationApplication { get; }

        public LicenceDenialTerminationManager(LicenceDenialApplicationData licenceDenialTermination, IRepositories repositories, CustomConfig config) :
            base(licenceDenialTermination, repositories, config)
        {
            LicenceDenialTerminationApplication = licenceDenialTermination;
        }

        public LicenceDenialTerminationManager(IRepositories repositories, CustomConfig config) :
            this(new LicenceDenialApplicationData(), repositories, config)
        {

        }

        public override async Task<bool> LoadApplicationAsync(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = await base.LoadApplicationAsync(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from LicSusp table 
                var licenceDenialDB = DB.LicenceDenialTable;
                var data = await licenceDenialDB.GetLicenceDenialDataAsync(enfService, appl_L03_CtrlCd: controlCode);

                if (data != null)
                    LicenceDenialTerminationApplication.Merge(data);
            }

            return isSuccess;
        }

        public async Task<bool> CreateApplicationAsync(string controlCodeForL01, DateTime requestDate)
        {
            if (!IsValidCategory("L03"))
                return false;

            // bool success = base.CreateApplication();
            bool success = await CreateLicenceDenialTerminationAsync(controlCodeForL01, requestDate);

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(DB, LicenceDenialTerminationApplication);
                await failedSubmitterManager.AddToFailedSubmitAuditAsync(FailedSubmitActivityAreaType.L03);
            }

            return success;
        }

        private async Task<bool> CreateLicenceDenialTerminationAsync(string controlCodeForL01, DateTime requestDate)
        {
            bool result = await PrepareLicencDenialTermination(controlCodeForL01, requestDate);

            return result;

            /*


            if (String.IsNullOrEmpty(Appl_CtrlCd))
            {
                LicenceDenialTerminationApplication.Appl_CtrlCd = await DB.ApplicationTable.GenerateApplicationControlCodeAsync(Appl_EnfSrv_Cd);
                Validation.IsSystemGeneratedControlCode = true;
            }

            TrimTrailingSpaces();
            MakeUpperCase();

            await DB.ApplicationTable.CreateApplicationAsync(LicenceDenialTerminationApplication);

            if (LicenceDenialTerminationApplication.AppLiSt_Cd != ApplicationState.INVALID_APPLICATION_1)
            {
                await licenceDenialManager.CancelApplicationAsync();
            }

            switch (LicenceDenialTerminationApplication.AppLiSt_Cd)
            {
                case ApplicationState.APPLICATION_ACCEPTED_10:
                    EventManager.AddEvent(EventCode.C50781_L03_ACCEPTED, queue: EventQueue.EventLicence);
                    EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED);
                    break;

                case ApplicationState.EXPIRED_15:
                    EventManager.AddEvent(EventCode.C50860_APPLICATION_COMPLETED);
                    break;

                default:
                    break;
            }

            await DB.LicenceDenialTable.CloseSameDayLicenceEventAsync(LicenceDenialTerminationApplication.Appl_EnfSrv_Cd,
                                                                          originalL01.Appl_CtrlCd,
                                                                          LicenceDenialTerminationApplication.Appl_CtrlCd);

            await EventManager.SaveEventsAsync();

            if (DB.CurrentSubmitter != "MSGBRO")
            {
                if (LicenceDenialTerminationApplication.AppLiSt_Cd.NotIn(ApplicationState.INVALID_APPLICATION_1,
                                                                         ApplicationState.APPLICATION_REJECTED_9))
                {
                    LicenceDenialTerminationApplication.Messages.AddInformation(EventCode.C50843_APPLICATION_CANCELLED);
                }
            }


            return true;

             */

            /*
Dim returnBoolean = True
        Dim L03applicationData As New Common.ApplicationData

        ClearEventQueue(_EventQueueData)

        ' State set for L03 here
        L03applicationData = PrepareL03(L01appl_EnfSrv_Cd, L01appl_CtrlCd, L03appl_EnfSrv_Cd, L03appl_CtrlCd, appl_Source_RfrNr, Subm_SubmCd, Subm_Recpt_SubmCd, declareDate, appReas_Cd, appl_CommSubm_Text, terminationRequestDate, lastUpdateUser)


        With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
            MakeApplicationUpperCase(L03applicationData)
            .InsertLicenseDenialAppl(L03applicationData)
        End With

        If L03applicationData.Appl.Item(0).AppLiSt_Cd <> 1 Then
            With New Business.LicenseDenialSystem(_connectionString)
                ' Set the last update user and date for the L01 to be canceled
                _newLO1Application.Appl.Item(0).Appl_LastUpdate_Usr = lastUpdateUser
                _newLO1Application.Appl.Item(0).Appl_LastUpdate_Dte = Date.Now
                _newLO1Application.Appl.Item(0).ActvSt_Cd = "X"

                'Regulation changes
                If L03LastAddressLn.Trim = "" Then
                    _LO1LicSuspData.LicSusp.Item(0).SetLicSusp_Dbtr_LastAddr_LnNull()
                Else
                    _LO1LicSuspData.LicSusp.Item(0).LicSusp_Dbtr_LastAddr_Ln = L03LastAddressLn.Trim
                End If
                If L03LastAddressLn1.Trim = "" Then
                    _LO1LicSuspData.LicSusp.Item(0).SetLicSusp_Dbtr_LastAddr_Ln1Null()
                Else
                    _LO1LicSuspData.LicSusp.Item(0).LicSusp_Dbtr_LastAddr_Ln1 = L03LastAddressLn1.Trim
                End If
                If L03LastAddressCity.Trim = "" Then
                    _LO1LicSuspData.LicSusp.Item(0).SetLicSusp_Dbtr_LastAddr_CityNmeNull()
                Else
                    _LO1LicSuspData.LicSusp.Item(0).LicSusp_Dbtr_LastAddr_CityNme = L03LastAddressCity.Trim
                End If
                If L03LastAddressProvince.Trim = "" Then
                    _LO1LicSuspData.LicSusp.Item(0).SetLicSusp_Dbtr_LastAddr_PrvCdNull()
                Else
                    _LO1LicSuspData.LicSusp.Item(0).LicSusp_Dbtr_LastAddr_PrvCd = L03LastAddressProvince.Trim
                End If
                If L03LastAddressCountry.Trim = "" Then
                    _LO1LicSuspData.LicSusp.Item(0).SetLicSusp_Dbtr_LastAddr_CtryCdNull()
                Else
                    _LO1LicSuspData.LicSusp.Item(0).LicSusp_Dbtr_LastAddr_CtryCd = L03LastAddressCountry.Trim
                End If
                If L03LastAddressPostalCd.Trim = "" Then
                    _LO1LicSuspData.LicSusp.Item(0).SetLicSusp_Dbtr_LastAddr_PCdNull()
                Else
                    _LO1LicSuspData.LicSusp.Item(0).LicSusp_Dbtr_LastAddr_PCd = L03LastAddressPostalCd.Trim
                End If

                ValidatePostalCodeL03(_LO1LicSuspData, Subm_SubmCd, L03appl_EnfSrv_Cd, L03appl_CtrlCd, Subm_Recpt_SubmCd, selectEnglish)

                ' Cancel the L01 application, updating it to state 14 and also status to cancelled, 
                ' and stating its corresponding L03 control code
                CancelL01(_newLO1Application, _LO1LicSuspData, L03appl_CtrlCd, lastUpdateUser)
            End With
        End If

        '
        ' MV 16Oct2014: CR 631 - advise mep if other active L01 for debtor
        '
        Dim msg As String = ""
        With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
            Dim dr As DataRow
            For Each dr In .GetActiveLO1ApplsForDebtor(L01appl_EnfSrv_Cd, L01appl_CtrlCd).Tables(0).Rows
                msg += dr(0) + " "
            Next
        End With

        If (msg.Trim.Length > 0) Then
            QueueEvent(_EventQueueData, CreateEventEnum.CreateSubmEvent, 50936, _newLO1Application, msg) 'other active L01 found
        End If

        'CHo 15Sep2010: Fix for defect# Foa284
        'Create proper events according to end State of L03
        Select Case L03applicationData.Appl.Item(0).AppLiSt_Cd
            Case 15   ' Completed
                ' Eric F.: we should not create any EvntLicense record with error code 50860 (it's only in EvntSubm)
                'QueueEvent(_EventQueueData, CreateEventEnum.CreateLicenseEvent, 50860, _newLO1Application, ) 'Application Completed
                QueueEvent(_EventQueueData, CreateEventEnum.CreateSubmEvent, 50860, L03applicationData, ) 'Application Completed
            Case 10  ' Accepted
                QueueEvent(_EventQueueData, CreateEventEnum.CreateLicenseEvent, 50781, _newLO1Application, ) 'L03 Accepted
                QueueEvent(_EventQueueData, CreateEventEnum.CreateSubmEvent, 50780, L03applicationData, ) 'Application Accepted
            Case 1 ' Invalid Application info
                ' 
                ' MJVTODO:  Automatically reject L03 per CR539
                '
                With New ApplicationSystem(_connectionString)
                    With L03applicationData.Appl.Item(0)
                        .AppLiSt_Cd = 9
                        .ActvSt_Cd = "X"
                    End With
                    .UpdateApplication(L03applicationData)
                End With
                QueueEvent(_EventQueueData, CreateEventEnum.CreateSubmEvent, 51020, L03applicationData, ) 'Update only allowed to Enf.Srv.Ref., Debtor address and Comments, other changes ignored

            Case Else
                ' No event created
        End Select

        SaveEventQueue(_EventQueueData)

        'close same day event
        CloseSameDayLicenseEvent(L01appl_EnfSrv_Cd, L01appl_CtrlCd, L03appl_CtrlCd)


        ' We do not display a message if the Last Update User is the Message Broker (MSGBRO)
        If lastUpdateUser.Trim.ToUpper <> "MSGBRO" Then
            'DisplayMsg(_newLO1Application, 50843) 'Application cancelled
            If (L03applicationData.Appl.Item(0).AppLiSt_Cd = 1) Or (L03applicationData.Appl.Item(0).AppLiSt_Cd = 9) Then
                DisplayMsg(lastUpdateUser, Subm_SubmCd, L03applicationData.Appl.Item(0).Appl_EnfSrv_Cd, L03applicationData.Appl.Item(0).Appl_CtrlCd, L03applicationData.Appl.Item(0).AppLiSt_Cd, 50843, L03applicationData.Appl.Item(0).Appl_EnfSrv_Cd, L03applicationData.Appl.Item(0).Subm_SubmCd)
            Else
                DisplayMsg(lastUpdateUser, Subm_SubmCd, _newLO1Application.Appl.Item(0).Appl_EnfSrv_Cd, _newLO1Application.Appl.Item(0).Appl_CtrlCd, _newLO1Application.Appl.Item(0).AppLiSt_Cd, 50843, _newLO1Application.Appl.Item(0).Appl_EnfSrv_Cd, _newLO1Application.Appl.Item(0).Subm_SubmCd)
            End If
        End If

        Return returnBoolean    
*/
            /*            
Private Function PrepareL03(ByVal L01appl_EnfSrv_Cd As String, ByVal L01appl_CtrlCd As String, ByRef L03appl_EnfSrv_Cd As String, ByRef L03appl_CtrlCd As String, ByVal appl_Source_RfrNr As String,
                                ByVal Subm_SubmCd As String, ByVal Subm_Recpt_SubmCd As String,
                                ByVal declareDate As Date, ByVal appReas_Cd As String, ByVal appl_CommSubm_Text As String, ByVal terminationRequestDate As Date,
                                ByVal lastUpdateUser As String) As Common.ApplicationData

        Dim L03applicationData As New Common.ApplicationData
        Dim row As Justice.FOAEA.Common.ApplicationData.ApplRow

        ' Fetch the L01 information
        With New DataAccess.LicenceDenial(_connectionString)
            _newLO1Application = .GetLO1Appl(L01appl_EnfSrv_Cd, L01appl_CtrlCd)
            _LO1LicSuspData = .GetLO1LicSusp(L01appl_EnfSrv_Cd, L01appl_CtrlCd)
        End With

        'Check if we found the corresponding L01 (in APPL and LIC_SUSP tables)
        If _newLO1Application.Appl.Rows.Count = 0 OrElse _LO1LicSuspData.LicSusp.Rows.Count = 0 Then
            Throw New Exception("Cannot create an L03 for an L01 that doesn't exist.")
        Else
            'CR512 - DO NOT allow an L03 to be insert if the L01 has been cancelled or rejected
            Dim currenL01State As Integer

            row = _newLO1Application.Appl.Rows(0)
            currenL01State = row.AppLiSt_Cd
            If currenL01State = 9 Or currenL01State = 14 Then
                Throw New Exception("Cannot create an L03 for an L01 (" & L01appl_CtrlCd & ") that has been cancelled or rejected.")
            End If

        End If

        'Check if the dataset received has the appropriate application category code.
        If (ValidateApplicationCategory(_newLO1Application.Appl.Item(0).AppCtgy_Cd, L01_CATEGORY_CODE) = False) Then
            Throw New Exception("Wrong Application Category Code for the provided L01!")
        End If

        'CHo 16Sep2010 fix for defect# foa274: terminationRequestDate was not being recorded 
        _LO1LicSuspData.LicSusp.Item(0).LicSusp_TermRequestDte = terminationRequestDate

        row = L03applicationData.Appl.NewApplRow

        ' CH 20Sep2010: fix for defect# Foa288...
        ' Special Business rule to always force the Enforcement Service Code and 
        ' Submitter Code to be the same as the L01
        row.Appl_EnfSrv_Cd = _newLO1Application.Appl.Item(0).Appl_EnfSrv_Cd
        'row.Subm_SubmCd = _newLO1Application.Appl.Item(0).Subm_SubmCd
        row.Subm_SubmCd = Subm_SubmCd
        ' Also see CtrlCd generation at the bottom of this function
        If L03appl_CtrlCd.Trim <> String.Empty Then
            'Check if the primary key already exists (so not to overwrite existing record)
            With New MidTier.Business.ApplicationSystem(_connectionString)
                If .RecordExists(L03appl_EnfSrv_Cd, L03appl_CtrlCd) Then
                    ' Throw an exception to the calling app
                    Throw New Exception("Application already exists in database.")
                End If
            End With
        End If

        ' Check if the current submitter and the L01 original submitter are different
        If _newLO1Application.Appl.Item(0).Subm_SubmCd.ToUpper.Trim <> Subm_SubmCd.ToUpper.Trim Then
            ' If they are different, keep track of the current submitter in the 
            ' L03 comments since it was replaced by the L01 submitter automatically
            If L01appl_EnfSrv_Cd.ToUpper.Trim <> L03appl_EnfSrv_Cd.ToUpper.Trim Then
                appl_CommSubm_Text = "*** LO3 submitted by " & Subm_SubmCd.ToUpper & " *** " & vbCrLf & appl_CommSubm_Text
            End If
        End If
        row.Appl_Dbtr_SurNme = _newLO1Application.Appl.Item(0).Appl_Dbtr_SurNme
        row.Appl_Dbtr_FrstNme = _newLO1Application.Appl.Item(0).Appl_Dbtr_FrstNme
        row.Appl_Dbtr_MddleNme = _newLO1Application.Appl.Item(0).Appl_Dbtr_MddleNme
        row.Appl_Dbtr_Brth_Dte = _newLO1Application.Appl.Item(0).Appl_Dbtr_Brth_Dte
        row.Appl_Dbtr_Gendr_Cd = _newLO1Application.Appl.Item(0).Appl_Dbtr_Gendr_Cd
        row.Appl_Dbtr_Entrd_SIN = _newLO1Application.Appl.Item(0).Appl_Dbtr_Entrd_SIN
        row.Appl_Dbtr_Cnfrmd_SIN = _newLO1Application.Appl.Item(0).Appl_Dbtr_Cnfrmd_SIN

        'Set L03 State depending on L01 State
        Select Case _newLO1Application.Appl.Item(0).AppLiSt_Cd
            Case 1, 3, 5, 6, 7, 9
                row.AppLiSt_Cd = 15 'State Completed
                row.ActvSt_Cd = Common.GlobalTypes.BaseEvent.StatusCode.COMPLETED 'Status Completed (C)
            Case 14
                row.AppLiSt_Cd = 1 ' State Invalid Application Info since the L01 has already been Canceled earlier
                row.ActvSt_Cd = Common.GlobalTypes.BaseEvent.StatusCode.ACTIVE  'Status Active (A)
            Case Else 'Is = 10, 12
                row.AppLiSt_Cd = 10 'State Accepted 
                row.ActvSt_Cd = Common.GlobalTypes.BaseEvent.StatusCode.ACTIVE  'Status Active (A)
        End Select

        'Set to specific values:
        row.AppCtgy_Cd = L03_CATEGORY_CODE
        row.Appl_SIN_Cnfrmd_Ind = 0
        row.Appl_Dbtr_LngCd = "E"

        'Left blank (set to nulls):
        row.SetAppl_Dbtr_Addr_CityNmeNull()
        row.SetAppl_Dbtr_Addr_CtryCdNull()
        row.SetAppl_Dbtr_Addr_LnNull()
        row.SetAppl_Dbtr_Addr_Ln1Null()
        row.SetAppl_Dbtr_Addr_PCdNull()
        row.SetAppl_Dbtr_Addr_PrvCdNull()
        row.SetAppl_RecvAffdvt_DteNull()
        row.SetAppl_Reactv_DteNull()
        row.SetSubm_Affdvt_SubmCdNull()
        row.SetAppl_JusticeNrNull()
        row.SetAppl_Group_Batch_CdNull()
        row.SetAppl_Affdvt_DocTypCdNull()
        row.SetAppl_Dbtr_RtrndBySrc_SINNull()
        ''''''row.SetAppl_Dbtr_Cnfrmd_SINNull()

        ' Values provided by the Front-End
        row.Appl_LastUpdate_Usr = lastUpdateUser
        row.Appl_LastUpdate_Dte = Now
        row.Appl_Create_Usr = Subm_SubmCd
        row.Appl_Create_Dte = Now
        row.Subm_Recpt_SubmCd = Subm_Recpt_SubmCd
        row.Appl_Source_RfrNr = appl_Source_RfrNr
        row.Appl_CommSubm_Text = appl_CommSubm_Text
        'If declareDate <> String.Empty Then 
        row.Appl_Lgl_Dte = declareDate

        '' New feature implemented to allow to Create L03 from 
        '' FTP with the Message Broker (MSGBRO) as the Last Update User
        'If lastUpdateUser.Trim.ToUpper = "MSGBRO" Then
        '    row.Medium_Cd = "FTP"
        '    row.Appl_Rcptfrm_Dte = New Date(Now.Year, Now.Month, Now.Day)
        'Else
        '    'Check if we can remove this boolean and replace it with Subm_Recpt_SubmCd.trim.startswith("FO")
        '    If Left(Subm_Recpt_SubmCd.Trim.ToUpper, 2) = "FO" Then
        '        row.Medium_Cd = "PAP"
        '        row.Appl_Rcptfrm_Dte = declareDate
        '    Else
        '        row.Medium_Cd = "ONL"
        '        row.Appl_Rcptfrm_Dte = New Date(Now.Year, Now.Month, Now.Day)
        '    End If
        'End If

        'Check if we can remove this boolean and replace it with Subm_Recpt_SubmCd.trim.startswith("FO")

        If Left(Subm_Recpt_SubmCd.Trim.ToUpper, 2) = "FO" Then
            row.Medium_Cd = "PAP"
            row.Appl_Rcptfrm_Dte = declareDate
        Else
            row.Medium_Cd = _newLO1Application.Appl.Item(0).Medium_Cd
            row.Appl_Rcptfrm_Dte = New Date(Now.Year, Now.Month, Now.Day)
        End If


        row.AppReas_Cd = appReas_Cd

        ' Apply the proper L03 ControlCode
        If L03appl_CtrlCd.Trim = String.Empty Then
            row.Appl_CtrlCd = row.Subm_SubmCd
            ' Auto-generate the Control Code (so we set it to the value of the submitter)
            L03applicationData.Appl.AddApplRow(row)
            ' Get new L03 application code
            GetApplSequenceNo(L03applicationData)
            ' Return new L03 control code into param
            L03appl_CtrlCd = L03applicationData.Appl.Item(0).Appl_CtrlCd
        Else
            ' Assigned from Front-end
            row.Appl_CtrlCd = L03appl_CtrlCd.Trim
            L03applicationData.Appl.AddApplRow(row)
        End If

        Return L03applicationData
    End Function
             */
        }
        private async Task<bool> PrepareLicencDenialTermination(string controlCodeForL01, DateTime requestDate)
        {
            // get the L01 information

            var licenceDenialManager = new LicenceDenialManager(DB, config);
            if (!await licenceDenialManager.LoadApplicationAsync(LicenceDenialTerminationApplication.Appl_EnfSrv_Cd, controlCodeForL01))
            {
                LicenceDenialTerminationApplication.Messages.AddError("Cannot create an L03 for an L01 that doesn't exist.");
                return false;
            }

            var originalL01 = licenceDenialManager.LicenceDenialApplication;

            if (originalL01.AppLiSt_Cd.In(ApplicationState.APPLICATION_REJECTED_9, ApplicationState.MANUALLY_TERMINATED_14))
            {
                LicenceDenialTerminationApplication.Messages.AddError($"Cannot create an L03 for an L01 ({originalL01.Appl_CtrlCd}) that has been cancelled or rejected.");
                return false;
            }

            if (originalL01.AppCtgy_Cd != "L01")
            {
                LicenceDenialTerminationApplication.Messages.AddError($"Invalid category type ({originalL01.AppCtgy_Cd}). Expected L01.");
                return false;
            }

            if (!string.IsNullOrEmpty(LicenceDenialTerminationApplication.Appl_CtrlCd) && (await ApplicationExistsAsync()))
            {
                LicenceDenialTerminationApplication.Messages.AddError("Application already exists in database.");
                return false;
            }

            originalL01.LicSusp_TermRequestDte = requestDate;

            SetL03ValuesBasedOnL01(originalL01, requestDate);

            return true;
        }

        public async Task<bool> ProcessLicenceDenialTerminationResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            if (!await LoadApplicationAsync(appl_EnfSrv_Cd, appl_CtrlCd))
            {
                LicenceDenialTerminationApplication.Messages.AddError(SystemMessage.APPLICATION_NOT_FOUND);
                return false;
            }

            if (!IsValidCategory("L03"))
                return false;

            if (LicenceDenialTerminationApplication.AppLiSt_Cd.NotIn(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12))
            {
                LicenceDenialTerminationApplication.Messages.AddError("Invalid State for the current application.  Valid states allowed are 10 and 12.");
                return false;
            }

            var lastResponse = await DB.LicenceDenialResponseTable.GetLastResponseDataAsync(appl_EnfSrv_Cd, appl_CtrlCd);

            LicenceDenialTerminationApplication.Appl_LastUpdate_Dte = DateTime.Now;
            LicenceDenialTerminationApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;

            if (LicenceDenialTerminationApplication.AppLiSt_Cd == ApplicationState.APPLICATION_ACCEPTED_10)
            {
                LicenceDenialTerminationApplication.AppLiSt_Cd = ApplicationState.PARTIALLY_SERVICED_12;
            }
            else if (LicenceDenialTerminationApplication.AppLiSt_Cd == ApplicationState.PARTIALLY_SERVICED_12)
            {
                LicenceDenialTerminationApplication.AppLiSt_Cd = ApplicationState.EXPIRED_15;
                LicenceDenialTerminationApplication.ActvSt_Cd = "C";
            }

            await UpdateApplicationNoValidationAsync();

            await DB.LicenceDenialTable.UpdateLicenceDenialDataAsync(LicenceDenialTerminationApplication);

            EventManager.AddEvent(EventCode.C50828_LICENSE_RESPONSE_RECEIVED, lastResponse.EnfSrv_Cd);

            await EventManager.SaveEventsAsync();

            return (LicenceDenialTerminationApplication.AppLiSt_Cd == ApplicationState.EXPIRED_15);

        }

        public async Task<bool> CancelApplication()
        {
            if (!IsValidCategory("I01"))
                return false;

            string appl_CommSubm_Text = LicenceDenialTerminationApplication.Appl_CommSubm_Text;

            // var newAppl_Source_RfrNr = LicenceDenialTerminationApplication.Appl_Source_RfrNr;
            var newAppl_Dbtr_Addr_Ln = LicenceDenialTerminationApplication.Appl_Dbtr_Addr_Ln;
            var newAppl_Dbtr_Addr_Ln1 = LicenceDenialTerminationApplication.Appl_Dbtr_Addr_Ln1;
            var newAppl_Dbtr_Addr_CityNme = LicenceDenialTerminationApplication.Appl_Dbtr_Addr_CityNme;
            var newAppl_Dbtr_Addr_PrvCd = LicenceDenialTerminationApplication.Appl_Dbtr_Addr_PrvCd;
            var newAppl_Dbtr_Addr_CtryCd = LicenceDenialTerminationApplication.Appl_Dbtr_Addr_CtryCd;
            var newAppl_Dbtr_Addr_PCd = LicenceDenialTerminationApplication.Appl_Dbtr_Addr_PCd;

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd))
            {
                LicenceDenialTerminationApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            LicenceDenialTerminationApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            LicenceDenialTerminationApplication.Appl_LastUpdate_Dte = DateTime.Now;

            LicenceDenialTerminationApplication.Appl_CommSubm_Text = appl_CommSubm_Text ?? LicenceDenialTerminationApplication.Appl_CommSubm_Text;

            LicenceDenialTerminationApplication.Appl_Dbtr_Addr_Ln = newAppl_Dbtr_Addr_Ln;
            LicenceDenialTerminationApplication.Appl_Dbtr_Addr_Ln1 = newAppl_Dbtr_Addr_Ln1;
            LicenceDenialTerminationApplication.Appl_Dbtr_Addr_CityNme = newAppl_Dbtr_Addr_CityNme;
            LicenceDenialTerminationApplication.Appl_Dbtr_Addr_PrvCd = newAppl_Dbtr_Addr_PrvCd;
            LicenceDenialTerminationApplication.Appl_Dbtr_Addr_CtryCd = newAppl_Dbtr_Addr_CtryCd;
            LicenceDenialTerminationApplication.Appl_Dbtr_Addr_PCd = newAppl_Dbtr_Addr_PCd;

            if (LicenceDenialTerminationApplication.ActvSt_Cd != "A")
            {
                EventManager.AddEvent(EventCode.C50841_CAN_ONLY_CANCEL_AN_ACTIVE_APPLICATION, activeState: "C");
                await EventManager.SaveEventsAsync();
                return false;
            }

            await SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            await UpdateApplicationNoValidationAsync();

            await EventManager.SaveEventsAsync();

            if (LicenceDenialTerminationApplication.Medium_Cd != "FTP")
                LicenceDenialTerminationApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        public override async Task UpdateApplicationAsync()
        {
            var current = new LicenceDenialTerminationManager(DB, config)
            {
                CurrentUser = this.CurrentUser
            };
            await current.LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

            // keep these stored values
            LicenceDenialTerminationApplication.Appl_Create_Dte = current.LicenceDenialTerminationApplication.Appl_Create_Dte;
            LicenceDenialTerminationApplication.Appl_Create_Usr = current.LicenceDenialTerminationApplication.Appl_Create_Usr;

            await base.UpdateApplicationAsync();
        }

        private void SetL03ValuesBasedOnL01(LicenceDenialApplicationData originalL01, DateTime requestDate)
        {
            var newL03 = LicenceDenialTerminationApplication;

            newL03.Appl_EnfSrv_Cd = originalL01.Appl_EnfSrv_Cd;
            newL03.Subm_SubmCd = originalL01.Subm_SubmCd;

            newL03.LicSusp_TermRequestDte = requestDate;

            string appl_CommSubm_Text = LicenceDenialTerminationApplication.Appl_CommSubm_Text;
            if ((DB.CurrentSubmitter != originalL01.Subm_SubmCd) &&
                (newL03.Appl_EnfSrv_Cd.Trim() != originalL01.Appl_EnfSrv_Cd.Trim()))
            {
                appl_CommSubm_Text = $"*** LO3 submitted by {DB.CurrentSubmitter} ***\n" + appl_CommSubm_Text;
            }

            newL03.Appl_Dbtr_SurNme = originalL01.Appl_Dbtr_SurNme;
            newL03.Appl_Dbtr_FrstNme = originalL01.Appl_Dbtr_FrstNme;
            newL03.Appl_Dbtr_MddleNme = originalL01.Appl_Dbtr_MddleNme;
            newL03.Appl_Dbtr_Brth_Dte = originalL01.Appl_Dbtr_Brth_Dte;
            newL03.Appl_Dbtr_Gendr_Cd = originalL01.Appl_Dbtr_Gendr_Cd;
            newL03.Appl_Dbtr_Entrd_SIN = originalL01.Appl_Dbtr_Entrd_SIN;
            newL03.Appl_Dbtr_Cnfrmd_SIN = originalL01.Appl_Dbtr_Cnfrmd_SIN;

            switch (originalL01.AppLiSt_Cd)
            {
                case ApplicationState.INVALID_APPLICATION_1:
                case ApplicationState.SIN_CONFIRMATION_PENDING_3:
                case ApplicationState.SIN_NOT_CONFIRMED_5:
                case ApplicationState.PENDING_ACCEPTANCE_SWEARING_6:
                case ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7:
                case ApplicationState.APPLICATION_REJECTED_9:
                    newL03.AppLiSt_Cd = ApplicationState.EXPIRED_15;
                    newL03.ActvSt_Cd = "C";
                    break;
                case ApplicationState.MANUALLY_TERMINATED_14:
                    newL03.AppLiSt_Cd = ApplicationState.INVALID_APPLICATION_1;
                    newL03.ActvSt_Cd = "A";
                    break;
                case ApplicationState.APPLICATION_ACCEPTED_10:
                case ApplicationState.PARTIALLY_SERVICED_12:
                    newL03.AppLiSt_Cd = ApplicationState.APPLICATION_ACCEPTED_10;
                    newL03.ActvSt_Cd = "A";
                    break;
            }

            newL03.Appl_SIN_Cnfrmd_Ind = 0;
            newL03.Appl_Dbtr_LngCd = "E";

            newL03.Appl_Dbtr_Addr_CityNme = null;
            newL03.Appl_Dbtr_Addr_CtryCd = null;
            newL03.Appl_Dbtr_Addr_Ln = null;
            newL03.Appl_Dbtr_Addr_Ln1 = null;
            newL03.Appl_Dbtr_Addr_PCd = null;
            newL03.Appl_Dbtr_Addr_PrvCd = null;
            newL03.Appl_RecvAffdvt_Dte = null;
            newL03.Appl_Reactv_Dte = null;
            newL03.Subm_Affdvt_SubmCd = null;
            newL03.Appl_JusticeNr = null;
            newL03.Appl_Group_Batch_Cd = null;
            newL03.Appl_Affdvt_DocTypCd = null;
            newL03.Appl_Dbtr_RtrndBySrc_SIN = null;

            newL03.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            newL03.Appl_LastUpdate_Dte = DateTime.Now;

            newL03.Appl_Create_Usr = DB.CurrentSubmitter;
            newL03.Appl_Create_Dte = DateTime.Now;

            newL03.Appl_CommSubm_Text = appl_CommSubm_Text;

            if (DB.CurrentSubmitter.IsInternalUser())
            {
                newL03.Medium_Cd = "PAP";
            }
            else
            {
                newL03.Medium_Cd = originalL01.Medium_Cd;
                newL03.Appl_Rcptfrm_Dte = DateTime.Now.Date;
            }

        }

    }
}
