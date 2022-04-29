using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialManager : ApplicationManager
    {
        public LicenceDenialApplicationData LicenceDenialApplication { get; }
        private bool AffidavitExists() => !String.IsNullOrEmpty(LicenceDenialApplication.Appl_Crdtr_FrstNme);

        public LicenceDenialManager(LicenceDenialApplicationData licenceDenial, IRepositories repositories, CustomConfig config) : base(licenceDenial, repositories, config)
        {
            LicenceDenialApplication = licenceDenial;

            // add Licence Denial specific state changes
            StateEngine.ValidStateChange.Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7, new List<ApplicationState> {
                            ApplicationState.SIN_CONFIRMED_4,
                            ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7,
                            ApplicationState.APPLICATION_REJECTED_9,
                            ApplicationState.APPLICATION_ACCEPTED_10,
                            ApplicationState.MANUALLY_TERMINATED_14
                        });

            //            StateEngine.ValidStateChange[ApplicationState.PENDING_ACCEPTANCE_SWEARING_6].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
            StateEngine.ValidStateChange[ApplicationState.SIN_CONFIRMED_4].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
        }

        public List<LicenceDenialOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords, string activeState, string recipientCode, bool isXML)
        {
            var licenceDenialDB = Repositories.LicenceDenialRepository;
            var data = licenceDenialDB.GetProvincialOutgoingData(maxRecords, activeState, recipientCode, isXML);
            return data;
        }
        /*
                 public List<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords,
                                                                             string activeState,
                                                                             string recipientCode,
                                                                             bool isXML = true)
        {
            var tracingDB = Repositories.TracingRepository;
            var data = tracingDB.GetProvincialOutgoingData(maxRecords, activeState, recipientCode, isXML);
            return data;
        }
         */

        public LicenceDenialManager(IRepositories repositories, CustomConfig config) : this(new LicenceDenialApplicationData(), repositories, config)
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
                var data = licenceDenialDB.GetLicenceDenialData(enfService, appl_L01_CtrlCd: controlCode);

                if (data != null)
                    LicenceDenialApplication.Merge(data);
            }

            return isSuccess;
        }

        public override bool CreateApplication()
        {
            if (!IsValidCategory("L01"))
                return false;

            bool success = base.CreateApplication();

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(Repositories, LicenceDenialApplication);
                failedSubmitterManager.AddToFailedSubmitAudit(FailedSubmitActivityAreaType.L01);
            }

            Repositories.LicenceDenialRepository.CreateLicenceDenialData(LicenceDenialApplication);

            return success;
        }

        public List<LicenceSuspensionHistoryData> GetLicenceSuspensionHistory()
        {
            return Repositories.LicenceDenialRepository.GetLicenceSuspensionHistory(LicenceDenialApplication.Appl_EnfSrv_Cd, LicenceDenialApplication.Appl_CtrlCd);
        }

        public void CancelApplication()
        {
            LicenceDenialApplication.Appl_LastUpdate_Dte = DateTime.Now;
            LicenceDenialApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;

            SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            MakeUpperCase();
            UpdateApplicationNoValidation();

            Repositories.LicenceDenialRepository.UpdateLicenceDenialData(LicenceDenialApplication);

            EventManager.AddEvent(EventCode.C50843_APPLICATION_CANCELLED);

            EventManager.SaveEvents();
        }

        public bool ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            if (!LoadApplication(appl_EnfSrv_Cd, appl_CtrlCd))
            {
                LicenceDenialApplication.Messages.AddError(SystemMessage.APPLICATION_NOT_FOUND);
                return false;
            }

            if (!IsValidCategory("L01"))
                return false;

            if (LicenceDenialApplication.AppLiSt_Cd.NotIn(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12))
            {
                LicenceDenialApplication.Messages.AddError("Invalid State for the current application.  Valid states allowed are 10 and 12.");
                return false;
            }

            LicenceDenialApplication.Appl_LastUpdate_Dte = DateTime.Now;
            LicenceDenialApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;

            SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

            UpdateApplicationNoValidation();

            Repositories.LicenceDenialRepository.UpdateLicenceDenialData(LicenceDenialApplication);

            EventManager.SaveEvents();

            return true;

            /*
             
    Public Sub LicenseResponse(ByVal applEnfSrvCd As String, ByVal applCtrCd As String, ByVal lastUpdateUser As String)
        IsInsert = False

        ClearEventQueue(_EventQueueData)

        'Get an existing LicSusp and LicRsp application.
        With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
            _currentLO1Application = .GetLO1Appl(applEnfSrvCd, applCtrCd)
        End With

        _newLO1Application = _currentLO1Application.Copy

        'Check if the dataset received has the appropriate application category code.
        If (ValidateApplicationCategory(_newLO1Application.Appl.Item(0).AppCtgy_Cd, L01_CATEGORY_CODE) = False) Then
            Throw New Exception("Wrong Application Category Code!")
        End If

        'Check if the current state is set to 10 or 12  If not throw an exception. 
        If ((_currentLO1Application.Appl.Item(0).AppLiSt_Cd <> 10) AndAlso (_currentLO1Application.Appl.Item(0).AppLiSt_Cd <> 12)) Then
            Throw New Exception("Invalid State for the current application.  Valid states allowed are 10 and 12.")
        Else

            'If _currentLO1Application.Appl.Item(0).AppLiSt_Cd <> 14 Then
            'Get an existing LO1 LicSusp application.
            With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
                _LO1LicSuspData = .GetLO1LicSusp(_currentLO1Application.Appl.Item(0).Appl_EnfSrv_Cd, _currentLO1Application.Appl.Item(0).Appl_CtrlCd)
            End With

            'Setting the last update fields.
            _newLO1Application.Appl.Item(0).Appl_LastUpdate_Usr = lastUpdateUser
            _newLO1Application.Appl.Item(0).Appl_LastUpdate_Dte = Date.Now()

            ' Move to State 12 - Partially serviced
            StateTransitionManager(12)

            With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
                .UpdateLO1Appl(_newLO1Application)

            End With

            With New Justice.FOAEA.MidTier.DataAccess.LicenceDenial(_connectionString)
                .UpdateLO1LicSusp(_LO1LicSuspData)
            End With

        End If

        SaveEventQueue(_EventQueueData)
    End Sub             
             */
        }

        public List<ApplicationEventData> GetRequestedLICINLicenceDenialEvents(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                               string appl_CtrlCd)
        {
            return EventManager.GetRequestedLICINLicenceDenialEvents(enfSrv_Cd, appl_EnfSrv_Cd, appl_CtrlCd);
        }
        
        public List<ApplicationEventDetailData> GetRequestedLICINLicenceDenialEventDetails(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                               string appl_CtrlCd)
        {
            return EventDetailManager.GetRequestedLICINLicenceDenialEventDetails(enfSrv_Cd, appl_EnfSrv_Cd, appl_CtrlCd);
        }

        //public DataList<LicenceDenialResponseData> GetLicenceDenialResults(bool checkCycle = false)
        //{
        //    return Repositories.LicenceDenialResponseRepository.GetLicenceDenialResponseForApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, checkCycle);
        //}

        public List<LicenceDenialOutgoingFederalData> GetFederalOutgoingData(int maxRecords,
                                                                      string activeState,
                                                                      ApplicationState lifeState,
                                                                      string enfServiceCode)
        {
            var licenceDenialDB = Repositories.LicenceDenialRepository;
            return licenceDenialDB.GetFederalOutgoingData(maxRecords, activeState, lifeState, enfServiceCode);
        }

        public void CreateResponseData(List<LicenceDenialResponseData> responseData)
        {
            var responsesDB = Repositories.LicenceDenialResponseRepository;
            responsesDB.InsertBulkData(responseData);
        }

        public void MarkResponsesAsViewed(string enfService)
        {
            var responsesDB = Repositories.LicenceDenialResponseRepository;
            responsesDB.MarkResponsesAsViewed(enfService);
        }

        public override void ProcessBringForwards(ApplicationEventData bfEvent)
        {
            bool closeEvent = false;

            TimeSpan diff;
            if (LicenceDenialApplication.Appl_LastUpdate_Dte.HasValue)
                diff = LicenceDenialApplication.Appl_LastUpdate_Dte.Value - DateTime.Now;
            else
                diff = TimeSpan.Zero;

            if ((LicenceDenialApplication.ActvSt_Cd != "A") &&
                ((!bfEvent.Event_Reas_Cd.HasValue) || (
                 (bfEvent.Event_Reas_Cd.NotIn(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                              EventCode.C50680_CHANGE_OR_SUPPLY_ADDITIONAL_DEBTOR_INFORMATION_SEE_SIN_VERIFICATION_RESULTS_PAGE_IN_FOAEA_FOR_SPECIFIC_DETAILS,
                                              EventCode.C50600_INVALID_APPLICATION)))) &&
                ((diff.Equals(TimeSpan.Zero)) || (Math.Abs(diff.TotalHours) > 24)))
            {
                bfEvent.AppLiSt_Cd = ApplicationState.MANUALLY_TERMINATED_14;
                bfEvent.ActvSt_Cd = "I";

                EventManager.SaveEvent(bfEvent);

                closeEvent = false;
            }
            else
            {
                if (bfEvent.Event_Reas_Cd.HasValue)
                    switch (bfEvent.Event_Reas_Cd)
                    {
                        case EventCode.C50528_BF_10_DAYS_FROM_RECEIPT_OF_APPLICATION:
                            if (LicenceDenialApplication.AppLiSt_Cd >= ApplicationState.APPLICATION_REJECTED_9)
                            {
                                // no action required
                            }
                            else
                            {
                                var DaysElapsed = (DateTime.Now - LicenceDenialApplication.Appl_Lgl_Dte.Value).TotalDays;
                                if (LicenceDenialApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5) &&
                                   (Math.Abs(DaysElapsed) > 40))
                                {
                                    // Reject the application according to the Application category code
                                    // .RejectT01(app.Appl.Item(0).Appl_EnfSrv_Cd, app.Appl.Item(0).Appl_CtrlCd, Appl_LastUpdate_Subm)
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else if ((LicenceDenialApplication.AppLiSt_Cd < ApplicationState.APPLICATION_REJECTED_9)
                                    && (DaysElapsed > 90))
                                {
                                    // Reject the application according to the Application category code
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else
                                {
                                    if (LicenceDenialApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5))
                                    {
                                        //Event_Reas_Cd = 50902 'Awaiting an action on this application
                                        //dteDateForNextBF = DateAdd(DateInterval.Day, 10, dteDateForNextBF)
                                    }
                                    else if (LicenceDenialApplication.AppLiSt_Cd.In(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6, ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7))
                                    {
                                        //Select Case Current_LifeState
                                        //    Case 6
                                        //        Event_Reas_Cd = 51042 'Requires legal authorization
                                        //    Case 7
                                        //        Event_Reas_Cd = 54004 'Awaiting affidavit to be entered
                                        //End Select
                                        //dteDateForNextBF = DateAdd(DateInterval.Day, 20, dteDateForNextBF)
                                    }
                                    else
                                    {
                                        //Event_Reas_Cd = 50902 'Awaiting an action on this application
                                        //dteDateForNextBF = DateAdd(DateInterval.Day, 10, dteDateForNextBF)
                                        //.CreateEvent("EvntAm", Subm_SubmCd, Appl_EnfSrv_Cd, Appl_CtrlCd, Now, Subm_Recpt_SubmCd, #12:00:00 AM#, 51100, "", "N", #12:00:00 AM#, "A", AppList_Cd, "")
                                    }
                                    //.CreateEvent("EvntSubm", Subm_SubmCd, Appl_EnfSrv_Cd, Appl_CtrlCd, Now, Subm_Recpt_SubmCd, #12:00:00 AM#, Event_Reas_Cd, "", "N", #12:00:00 AM#, "A", AppList_Cd, Appl_LastUpdate_Subm)
                                    //.CreateEvent("EvntBF", Subm_SubmCd, Appl_EnfSrv_Cd, Appl_CtrlCd, Now, Subm_Recpt_SubmCd, #12:00:00 AM#, 50528, "", "N", dteDateForNextBF, "A", AppList_Cd, "")

                                }
                            }
                            break;

                        case EventCode.C54001_BF_EVENT:
                        case EventCode.C54002_TELEPHONE_EVENT:
                            EventManager.AddEvent(bfEvent.Event_Reas_Cd.Value);
                            break;

                        default:
                            EventManager.AddEvent(EventCode.C54003_UNKNOWN_EVNTBF, queue: EventQueue.EventSYS);
                            break;
                    }
            }

            if (closeEvent)
            {
                bfEvent.AppLiSt_Cd = LicenceDenialApplication.AppLiSt_Cd;
                bfEvent.ActvSt_Cd = "C";

                EventManager.SaveEvent(bfEvent);
            }

            EventManager.SaveEvents();
        }

        public List<LicenceDenialToApplData> GetLicenceDenialToApplData(string federalSource)
        {
            var licenceDenialDB = Repositories.LicenceDenialRepository;
            return licenceDenialDB.GetLicenceDenialToApplData(federalSource);
        }
    }
}
