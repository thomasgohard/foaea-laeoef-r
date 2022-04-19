using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Data.Base;
using FOAEA3.Business.Security;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using System;

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

        public override bool LoadApplication(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = base.LoadApplication(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from LicSusp table 
                var licenceDenialDB = Repositories.LicenceDenialRepository;
                var data = licenceDenialDB.GetLicenceDenialData(enfService, appl_L03_CtrlCd: controlCode);

                if (data != null)
                    LicenceDenialTerminationApplication.Merge(data);
            }

            return isSuccess;
        }

        public bool CreateApplication(string controlCodeForL01, DateTime requestDate)
        {
            if (!IsValidCategory("L03"))
                return false;

            // bool success = base.CreateApplication();
            bool success = CreateLicenceDenialTermination(controlCodeForL01, requestDate);

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(Repositories, LicenceDenialTerminationApplication);
                failedSubmitterManager.AddToFailedSubmitAudit(FailedSubmitActivityAreaType.L03);
            }

            return success;
        }

        private bool CreateLicenceDenialTermination(string controlCodeForL01, DateTime requestDate)
        {
            SetNewStateTo(ApplicationState.INVALID_APPLICATION_1);

            var licenceDenialManager = new LicenceDenialManager(Repositories, config);
            if (!licenceDenialManager.LoadApplication(LicenceDenialTerminationApplication.Appl_EnfSrv_Cd, controlCodeForL01))
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

            if (!string.IsNullOrEmpty(LicenceDenialTerminationApplication.Appl_CtrlCd) && (ApplicationExists()))
            {
                LicenceDenialTerminationApplication.Messages.AddError("Application already exists in database.");
                return false;
            }

            SetL03ValuesBasedOnL01(originalL01, requestDate);

            if (String.IsNullOrEmpty(Appl_CtrlCd))
            {
                LicenceDenialTerminationApplication.Appl_CtrlCd = Repositories.ApplicationRepository.GenerateApplicationControlCode(Appl_EnfSrv_Cd);
                Validation.IsSystemGeneratedControlCode = true;
            }

            TrimTrailingSpaces();
            MakeUpperCase();

            Repositories.ApplicationRepository.CreateApplication(LicenceDenialTerminationApplication);

            if (LicenceDenialTerminationApplication.AppLiSt_Cd != ApplicationState.INVALID_APPLICATION_1)
            {
                licenceDenialManager.CancelApplication();
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

            Repositories.LicenceDenialRepository.CloseSameDayLicenceEvent(LicenceDenialTerminationApplication.Appl_EnfSrv_Cd,
                                                                          originalL01.Appl_CtrlCd,
                                                                          LicenceDenialTerminationApplication.Appl_CtrlCd);

            EventManager.SaveEvents();

            if (Repositories.CurrentSubmitter != "MSGBRO")
            {
                if (LicenceDenialTerminationApplication.AppLiSt_Cd.NotIn(ApplicationState.INVALID_APPLICATION_1,
                                                                         ApplicationState.APPLICATION_REJECTED_9))
                {
                    LicenceDenialTerminationApplication.Messages.AddInformation(EventCode.C50843_APPLICATION_CANCELLED);
                }
            }


            return true;
        }

        public bool ProcessLicenceDenialTerminationResponse(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            if (!LoadApplication(appl_EnfSrv_Cd, appl_CtrlCd))
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

            LicenceDenialTerminationApplication.Appl_LastUpdate_Dte = DateTime.Now;
            LicenceDenialTerminationApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;

           

            UpdateApplicationNoValidation();

            Repositories.LicenceDenialRepository.UpdateLicenceDenialData(LicenceDenialTerminationApplication);

            EventManager.SaveEvents();

            return true;

            /*
             
    Public Function ProcessL03(ByVal applicationEnforcementServiceCode As String,
                   ByVal applicationControlCode As String,
                   ByVal lastUpdateUser As String) As Boolean

        Dim returnBoolean As Boolean = True
        'IsInsert = False

        ' CR769 - Event 50828 should be generated in an L03. the event should be generated no matter what returned code is
        'Get the request status code and the source.
        Dim LO1LicRspData As Justice.FOAEA.Common.LicenseResponseData

        'Get an existing LO3 application
        With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
            _currentLO3Application = .GetLO3FromL01(applicationEnforcementServiceCode, applicationControlCode)
            'Get an existing LicRsp application.
            LO1LicRspData = .GetLastLicRsp(applicationEnforcementServiceCode, applicationControlCode)
        End With

        _newLO3Application = _currentLO3Application.Copy

        _newLO3Application.Appl.Item(0).Appl_LastUpdate_Usr = lastUpdateUser
        _newLO3Application.Appl.Item(0).Appl_LastUpdate_Dte = Date.Now

        If _currentLO3Application.Appl.Item(0).AppLiSt_Cd = 10 Then
            _newLO3Application.Appl.Item(0).AppLiSt_Cd = 12
        Else
            If _currentLO3Application.Appl.Item(0).AppLiSt_Cd = 12 Then
                _newLO3Application.Appl.Item(0).ActvSt_Cd = "C"
                _newLO3Application.Appl.Item(0).AppLiSt_Cd = 15
            End If

        End If

        With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
            MakeApplicationUpperCase(_newLO3Application)
            .UpdateLO1Appl(_newLO3Application)
        End With

        ClearEventQueue(_EventQueueData)
        ' CR769 - Event 50828 should be generated in an L03. the event should be generated no matter what returned code is

        Dim rqstStatCd As Short = LO1LicRspData.LicRsp.Item(0).RqstStat_Cd
        Dim source As String = LO1LicRspData.LicRsp.Item(0).EnfSrv_Cd

        QueueEvent(_EventQueueData, CreateEventEnum.CreateSubmEvent, 50828, _newLO3Application, source) '-- L03
        SaveEventQueue(_EventQueueData)

        If _newLO3Application.Appl.Item(0).AppLiSt_Cd = 15 Then
            Return True
        Else
            Return False
        End If

    End Function             
             */

        }

        private void SetL03ValuesBasedOnL01(LicenceDenialApplicationData originalL01, DateTime requestDate)
        {
            var newL03 = LicenceDenialTerminationApplication;

            newL03.Appl_EnfSrv_Cd = originalL01.Appl_EnfSrv_Cd;
            newL03.Subm_SubmCd = originalL01.Subm_SubmCd;

            newL03.LicSusp_TermRequestDte = requestDate;

            string appl_CommSubm_Text = LicenceDenialTerminationApplication.Appl_CommSubm_Text;
            if ((Repositories.CurrentSubmitter != originalL01.Subm_SubmCd) &&
                (newL03.Appl_EnfSrv_Cd.Trim() != originalL01.Appl_EnfSrv_Cd.Trim()))
            {
                appl_CommSubm_Text = $"*** LO3 submitted by {Repositories.CurrentSubmitter} ***\n" + appl_CommSubm_Text;
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
                    newL03.AppLiSt_Cd = ApplicationState.EXPIRED_15;
                    newL03.ActvSt_Cd = "C";
                    break;
                case ApplicationState.APPLICATION_ACCEPTED_10:
                case ApplicationState.PARTIALLY_SERVICED_12:
                    newL03.AppLiSt_Cd = ApplicationState.APPLICATION_ACCEPTED_10;
                    newL03.ActvSt_Cd = "A";
                    break;
            }

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

            newL03.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;
            newL03.Appl_LastUpdate_Dte = DateTime.Now;

            newL03.Appl_Create_Usr = Repositories.CurrentSubmitter;
            newL03.Appl_Create_Dte = DateTime.Now;
            
            newL03.Appl_CommSubm_Text = appl_CommSubm_Text;

            if (UserHelper.IsInternalUser(Repositories.CurrentSubmitter))
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
