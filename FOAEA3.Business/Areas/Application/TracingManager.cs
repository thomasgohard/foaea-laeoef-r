using FOAEA3.Resources.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using FOAEA3.Business.Security;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class TracingManager : ApplicationManager
    {
        public TracingApplicationData TracingApplication { get; }
        private DateTime ReinstateEffectiveDate { get; set; }
        private EventCode BFEventReasonCode { get; set; }
        private int BFEvent_Id { get; set; }

        public TracingManager(TracingApplicationData tracing, IRepositories repositories, CustomConfig config) :
            base(tracing, repositories, config, new TracingValidation(tracing, repositories, config))
        {

            TracingApplication = tracing;

            // add Tracing specific valid state changes
            StateEngine.ValidStateChange.Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7, new List<ApplicationState> {
                                             ApplicationState.SIN_CONFIRMED_4,
                                             ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7,
                                             ApplicationState.APPLICATION_REJECTED_9,
                                             ApplicationState.APPLICATION_ACCEPTED_10,
                                             ApplicationState.MANUALLY_TERMINATED_14
                                            });
            StateEngine.ValidStateChange[ApplicationState.APPLICATION_ACCEPTED_10].Add(ApplicationState.APPLICATION_REINSTATED_11);
            StateEngine.ValidStateChange[ApplicationState.PARTIALLY_SERVICED_12].Add(ApplicationState.APPLICATION_REINSTATED_11);
//            StateEngine.ValidStateChange[ApplicationState.PENDING_ACCEPTANCE_SWEARING_6].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
            StateEngine.ValidStateChange[ApplicationState.SIN_CONFIRMED_4].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

        }

        public TracingManager(IRepositories repositories, CustomConfig config) :
            this(new TracingApplicationData(), repositories, config)
        {
        }

        public override bool LoadApplication(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = base.LoadApplication(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from Trace table, if available
                TracingApplicationData data = Repositories.TracingRepository.GetTracingData(enfService, controlCode);

                if (data != null)
                    TracingApplication.Merge(data);
            }

            if (!isSuccess)
            {
                // add to audit
            }

            return isSuccess;
        }

        public override bool CreateApplication()
        {
            if (!IsValidCategory("T01"))
                return false;

            bool success = base.CreateApplication();

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(Repositories, TracingApplication);
                failedSubmitterManager.AddToFailedSubmitAudit(FailedSubmitActivityAreaType.T01);
            }

            // TODO: check and write Tracing specific info, if any were included

            return success;

        }

        public override void UpdateApplication()
        {
            // TODO: check and update Tracing specific info, if any were included
            if (!string.IsNullOrEmpty(TracingApplication.FamPro_Cd))
            {
                // affidavit data has been passed -- need to either create it or update it
                if (!AffidavitExists())
                {
                    ValidateAndProcessNewAffidavit();

                    // ignore all changes other than Creditor

                    string newAppl_Crdtr_FrstNme = TracingApplication.Appl_Crdtr_FrstNme;
                    string newAppl_Crdtr_MddleNme = TracingApplication.Appl_Crdtr_MddleNme;
                    string newAppl_Crdtr_SurNme = TracingApplication.Appl_Crdtr_SurNme;

                    LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd);

                    TracingApplication.Appl_Crdtr_FrstNme = newAppl_Crdtr_FrstNme;
                    TracingApplication.Appl_Crdtr_MddleNme = newAppl_Crdtr_MddleNme;
                    TracingApplication.Appl_Crdtr_SurNme = newAppl_Crdtr_SurNme;

                    MakeUpperCase();

                    base.UpdateApplicationNoValidation();
                }
                else
                {
                    Repositories.TracingRepository.UpdateTracingData(TracingApplication);

                    MakeUpperCase();

                    base.UpdateApplication();
                }

            }

        }

        public bool ReinstateApplication(string enfService, string controlCode, string lastUpdateUser,
                                         DateTime eventEffectiveDate, EventCode bfEventCode, int bfEventId)
        {
            ReinstateEffectiveDate = eventEffectiveDate;
            BFEventReasonCode = bfEventCode;
            BFEvent_Id = bfEventId;

            LoadApplication(enfService, controlCode);

            //TracingApplication.Events.Clear();

            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;

            if (!IsValidCategory("T01"))
                return false;

            TracingApplication.Appl_Reactv_Dte = ReinstateEffectiveDate;

            int traceCycle = TracingApplication.Trace_Cycl_Qty;

            SetNewStateTo(ApplicationState.APPLICATION_REINSTATED_11);

            TracingApplication.Trace_Cycl_Qty = traceCycle + 1;
            TracingApplication.Trace_LiSt_Cd = 80;
            TracingApplication.Trace_LstCyclStr_Dte = DateTime.Now;

            MakeUpperCase();

            UpdateApplication();

            return true;
        }

        public bool RejectApplication(string enfService, string controlCode, string lastUpdateUser)
        {
            bool result = true;

            LoadApplication(enfService, controlCode);

            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;

            if (!IsValidCategory("T01"))
                return false;

            SetNewStateTo(ApplicationState.APPLICATION_REJECTED_9);

            UpdateApplication();

            return result;
        }

        public bool CertifyAffidavit(string lastUpdateUser)
        {
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;
            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;

            if (TracingApplication.AppLiSt_Cd != ApplicationState.PENDING_ACCEPTANCE_SWEARING_6)
            {
                TracingApplication.Messages.AddError($"CertifyL02 is not available for state {TracingApplication.AppLiSt_Cd}.  It should be from state 6 (legal authorization pending) only.");
                return false;
            }

            if (!UserHelper.IsInternalUser(lastUpdateUser))
            {
                TracingApplication.Subm_Affdvt_SubmCd = lastUpdateUser;
                TracingApplication.Appl_RecvAffdvt_Dte = DateTime.Now;

                SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

                MakeUpperCase();
                UpdateApplicationNoValidation();

                EventManager.SaveEvents();

                TracingApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);
            }
            else
            {
                // TODO: send message back that this is not allowed?
            }

            return true;

        }

        public void RejectAffidavit(string lastUpdateUser)
        {
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;
            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;

            SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

            MakeUpperCase();
            UpdateApplicationNoValidation();

            EventManager.AddEvent(EventCode.C51019_REJECTED_AFFIDAVIT);

            EventManager.SaveEvents();

            TracingApplication.Messages.AddInformation(EventCode.C50763_AFFIDAVIT_REJECTED_BY_FOAEA);
        }

        public void FullyServiceApplication(string enfSrvCode)
        {
            if (TracingApplication.AppLiSt_Cd == ApplicationState.MANUALLY_TERMINATED_14)
            {
                var traceResponseDB = Repositories.TraceResponseRepository;
                traceResponseDB.DeleteCancelledApplicationTraceResponseData(Appl_EnfSrv_Cd, Appl_CtrlCd, enfSrvCode);
            }
            else
            {
                SetNewStateTo(ApplicationState.FULLY_SERVICED_13);

                MakeUpperCase();
                UpdateApplication();
            }
        }

        public void PartiallyServiceApplication(string enfSrvCode)
        {
            if (TracingApplication.AppLiSt_Cd == ApplicationState.MANUALLY_TERMINATED_14)
            {
                var traceResponseDB = Repositories.TraceResponseRepository;
                traceResponseDB.DeleteCancelledApplicationTraceResponseData(Appl_EnfSrv_Cd, Appl_CtrlCd, enfSrvCode);
            }
            else
            {
                SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

                MakeUpperCase();
                UpdateApplication();
            }
        }

        public void TerminateApplication(string enfSrvCode)
        {

            var traceResponseDB = Repositories.TraceResponseRepository;
            traceResponseDB.DeleteCancelledApplicationTraceResponseData(Appl_EnfSrv_Cd, Appl_CtrlCd, enfSrvCode);

            SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            MakeUpperCase();
            UpdateApplication();

        }

        public List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle)
        {
            var tracingDB = Repositories.TracingRepository;
            return tracingDB.GetTraceCycleQuantityData(enfSrv_Cd, cycle);
        }

        public List<TraceToApplData> GetTraceToApplData()
        {
            var tracingDB = Repositories.TracingRepository;
            return tracingDB.GetTraceToApplData();
        }

        private void SetTracingForReinstate(DateTime traceDate, DateTime bfDate, EventCode eventReasonCode)
        {

            TracingApplication.AppLiSt_Cd = ApplicationState.APPLICATION_ACCEPTED_10;
            TracingApplication.Messages.AddInformation(EventCode.C50780_APPLICATION_ACCEPTED);

            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED, queue: EventQueue.EventTrace);
            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED);

            EventManager.AddEvent(eventReasonCode, appState: ApplicationState.APPLICATION_REINSTATED_11, effectiveDateTime: traceDate);
            EventManager.AddBFEvent(eventReasonCode, appState: ApplicationState.APPLICATION_REINSTATED_11, effectiveTimestamp: bfDate);

            Validation.AddDuplicateCreditorWarningEvents();

        }

        private void ValidateAndProcessNewAffidavit()
        {

            if (!UserHelper.IsInternalUser(TracingApplication.Appl_LastUpdate_Usr))
            {
                TracingApplication.Subm_Affdvt_SubmCd = null;
            }

            if (TracingApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_CONFIRMATION_PENDING_3,
                                                 ApplicationState.SIN_NOT_CONFIRMED_5, ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7))
            {
                TracingApplication.Trace_LiSt_Cd = 80;
                Repositories.TracingRepository.CreateTracingData(TracingApplication);

                if (TracingApplication.AppLiSt_Cd == ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7)
                {
                    Process_04_SinConfirmed();
                }
            }

        }

        public override void ProcessBringForwards(ApplicationEventData bfEvent)
        {
            bool closeEvent = false;

            TimeSpan diff = TimeSpan.Zero;
            if (TracingApplication.Appl_LastUpdate_Dte.HasValue)
                diff = TracingApplication.Appl_LastUpdate_Dte.Value - DateTime.Now;

            if ((TracingApplication.ActvSt_Cd != "A") &&
                ((!bfEvent.Event_Reas_Cd.HasValue) || (
                 (bfEvent.Event_Reas_Cd.NotIn(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                              EventCode.C50680_CHANGE_OR_SUPPLY_ADDITIONAL_DEBTOR_INFORMATION_SEE_SIN_VERIFICATION_RESULTS_PAGE_IN_FOAEA_FOR_SPECIFIC_DETAILS,
                                              EventCode.C50600_INVALID_APPLICATION)))) &&
                ((diff.Equals(TimeSpan.Zero)) || (Math.Abs(diff.TotalHours) > 24)))
            {
                EventManager.SaveEvent(bfEvent, ApplicationState.MANUALLY_TERMINATED_14, "I");
                closeEvent = false;
            }
            else
            {
                if (bfEvent.Event_Reas_Cd.HasValue)
                {
                    var currentLifeState = TracingApplication.AppLiSt_Cd;

                    switch (bfEvent.Event_Reas_Cd)
                    {
                        case EventCode.C50528_BF_10_DAYS_FROM_RECEIPT_OF_APPLICATION:
                            if (currentLifeState < ApplicationState.APPLICATION_REJECTED_9)
                            {
                                var DaysElapsed = Math.Abs((DateTime.Now - TracingApplication.Appl_Lgl_Dte.Value).TotalDays);

                                if (currentLifeState.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5) &&
                                   (DaysElapsed > 40))
                                {
                                    RejectApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, Repositories.CurrentSubmitter);
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else if (DaysElapsed > 90)
                                {
                                    RejectApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, Repositories.CurrentSubmitter);
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else
                                {
                                    AddNewBFevent();
                                }
                            }
                            break;

                        case EventCode.C50804_SCHEDULED_TO_BE_REINSTATED:
                            if (currentLifeState == ApplicationState.APPLICATION_REINSTATED_11)
                            {
                                ReinstateApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, Repositories.CurrentSubmitter,
                                                     bfEvent.Event_Effctv_Dte, bfEvent.Event_Reas_Cd.Value, bfEvent.Event_Id);
                                SetNewStateTo(ApplicationState.APPLICATION_REINSTATED_11);
                            }
                            break;

                        case EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING:
                            if (currentLifeState.In(ApplicationState.PARTIALLY_SERVICED_12, ApplicationState.EXPIRED_15))
                            {
                                // TODO: if it is at state 15, how can it re-instate?
                                ReinstateApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, Repositories.CurrentSubmitter,
                                                     bfEvent.Event_Effctv_Dte, bfEvent.Event_Reas_Cd.Value, bfEvent.Event_Id);
                                SetNewStateTo(ApplicationState.APPLICATION_REINSTATED_11);
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
            }

            if (closeEvent)
            {
                EventManager.SaveEvent(bfEvent, TracingApplication.AppLiSt_Cd, "C");
            }

            EventManager.SaveEvents();
        }

        private void AddNewBFevent()
        {
            DateTime nextBFdate = DateTime.Now;
            EventCode eventReasonCode = EventCode.UNDEFINED;

            if (TracingApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5))
            {
                eventReasonCode = EventCode.C50902_AWAITING_AN_ACTION_ON_THIS_APPLICATION;
                nextBFdate = nextBFdate.AddDays(10);
            }
            else if (TracingApplication.AppLiSt_Cd.In(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6, ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7))
            {
                switch (TracingApplication.AppLiSt_Cd)
                {
                    case ApplicationState.PENDING_ACCEPTANCE_SWEARING_6:
                        eventReasonCode = EventCode.C51042_REQUIRES_LEGAL_AUTHORIZATION;
                        break;
                    case ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7:
                        eventReasonCode = EventCode.C54004_AWAITING_AFFIDAVIT_TO_BE_ENTERED;
                        break;
                }
                nextBFdate = nextBFdate.AddDays(20);
            }
            else
            {
                eventReasonCode = EventCode.C50902_AWAITING_AN_ACTION_ON_THIS_APPLICATION;
                nextBFdate = nextBFdate.AddDays(10);

                EventManager.AddEvent(EventCode.C51100_STATE_OUT_OF_SYNC, queue: EventQueue.EventAM);
            }

            EventManager.AddEvent(eventReasonCode);
            EventManager.AddBFEvent(EventCode.C50520_INVALID_STATUS_CODE, effectiveTimestamp: nextBFdate);
        }

        public DataList<TracingApplicationData> GetApplicationsWaitingForAffidavit()
        {
            return Repositories.TracingRepository.GetApplicationsWaitingForAffidavit();
        }

        private bool AffidavitExists()
        {
            return Repositories.TracingRepository.TracingDataExists(Appl_EnfSrv_Cd, Appl_CtrlCd);
        }

        public DataList<TraceResponseData> GetTraceResults(bool checkCycle = false)
        {
            return Repositories.TraceResponseRepository.GetTraceResponseForApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, checkCycle);
        }

        public List<ApplicationEventData> GetRequestedTRCINTracingEvents(string enfSrv_Cd, string cycle)
        {
            return EventManager.GetRequestedTRCINTracingEvents(enfSrv_Cd, cycle);
        }

        public List<ApplicationEventDetailData> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle)
        {
            return EventDetailManager.GetActiveTracingEventDetails(enfSrv_Cd, cycle);
        }

        public void ApplySINconfirmation()
        {
            SetNewStateTo(ApplicationState.SIN_CONFIRMED_4);

            var sinManager = new ApplicationSINManager(TracingApplication, this);
            sinManager.UpdateSINChangeHistory();

            foreach (var eventItem in EventManager.Events)
                eventItem.Subm_Update_SubmCd = "SYSTEM";

            UpdateApplicationNoValidation();

        }

        public void ApplySINfailed()
        {
            SetNewStateTo(ApplicationState.SIN_NOT_CONFIRMED_5);

            foreach (var eventItem in EventManager.Events)
                eventItem.Subm_Update_SubmCd = "SYSTEM";

            UpdateApplicationNoValidation();
        }

        public void CreateResponseData(List<TraceResponseData> responseData)
        {
            var responsesDB = Repositories.TraceResponseRepository;
            responsesDB.InsertBulkData(responseData);
        }

        public void MarkResponsesAsViewed(string enfService)
        {
            var responsesDB = Repositories.TraceResponseRepository;
            responsesDB.MarkResponsesAsViewed(enfService);
        }

        public List<TracingOutgoingFederalData> GetFederalOutgoingData(int maxRecords,
                                                                       string activeState,
                                                                       ApplicationState lifeState,
                                                                       string enfServiceCode)
        {
            var tracingDB = Repositories.TracingRepository;
            return tracingDB.GetFederalOutgoingData(maxRecords, activeState, lifeState, enfServiceCode);
        }

        public List<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords,
                                                                             string activeState,
                                                                             string recipientCode,
                                                                             bool isXML = true)
        {
            var tracingDB = Repositories.TracingRepository;
            var data = tracingDB.GetProvincialOutgoingData(maxRecords, activeState, recipientCode, isXML);
            return data;
        }
    }

}
