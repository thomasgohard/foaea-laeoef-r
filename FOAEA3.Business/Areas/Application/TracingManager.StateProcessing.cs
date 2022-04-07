using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using System;
using System.Linq;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class TracingManager : ApplicationManager
    {
        protected override void Process_04_SinConfirmed()
        {
            base.Process_04_SinConfirmed();

            if (AffidavitExists())
                SetNewStateTo(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);
            else
                SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
        }

        protected override void Process_06_PendingAcceptanceSwearing()
        {
            base.Process_06_PendingAcceptanceSwearing();

            Validation.AddDuplicateSINWarningEvents();

            string signAuthority = Repositories.SubmitterRepository.GetSignAuthorityForSubmitter(TracingApplication.Subm_SubmCd);

            EventManager.AddEvent(EventCode.C51042_REQUIRES_LEGAL_AUTHORIZATION, appState: ApplicationState.PENDING_ACCEPTANCE_SWEARING_6,
                                  recipientSubm: signAuthority);

            if (TracingApplication.Medium_Cd == "FTP")
                EventManager.AddEvent(EventCode.C51042_REQUIRES_LEGAL_AUTHORIZATION, appState: ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);

            // FOAEA users can bypass swearing and go directly to state 10 
            if (UserHelper.IsInternalUser(TracingApplication.Subm_Recpt_SubmCd) &&
                !String.IsNullOrEmpty(TracingApplication.Subm_Affdvt_SubmCd) &&
                TracingApplication.Appl_RecvAffdvt_Dte.HasValue)
            {
                SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
            }
        }

        protected override void Process_07_ValidAffidavitNotReceived()
        {

            if (String.IsNullOrEmpty(TracingApplication.Subm_Affdvt_SubmCd) ||
               (!TracingApplication.Appl_RecvAffdvt_Dte.HasValue))
            {
                base.Process_07_ValidAffidavitNotReceived();
                EventManager.AddEvent(EventCode.C54004_AWAITING_AFFIDAVIT_TO_BE_ENTERED);
            }
            else
            {
                Process_10_ApplicationAccepted();
            }

        }

        protected override void Process_10_ApplicationAccepted()
        {
            Validation.AddDuplicateCreditorWarningEvents();

            TracingApplication.Messages.AddInformation(EventCode.C50780_APPLICATION_ACCEPTED);

            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED, queue: EventQueue.EventTrace);

            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED, queue: EventQueue.EventSubm, appState: ApplicationState.APPLICATION_ACCEPTED_10);

            EventManager.AddEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING, queue: EventQueue.EventSubm,
                                  appState: ApplicationState.APPLICATION_REINSTATED_11);

            EventManager.AddEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING, queue: EventQueue.EventBF,
                                  appState: ApplicationState.APPLICATION_REINSTATED_11, effectiveDateTime: DateTime.Now.AddMonths(3));

            TracingApplication.Messages.AddInformation(EventCode.C50780_APPLICATION_ACCEPTED);

            TracingApplication.Appl_Reactv_Dte = DateTime.Now;

            base.Process_10_ApplicationAccepted();
        }

        protected override void Process_11_ApplicationReinstated()
        {
            base.Process_11_ApplicationReinstated();

            DateTime quarterDate; // = DateTimeHelper.AddQuarter(ReinstateEffectiveDate, 1);
            int eventTraceCount = EventManager.GetTraceEventCount(
                                                    Appl_EnfSrv_Cd,
                                                    Appl_CtrlCd,
                                                    TracingApplication.Appl_RecvAffdvt_Dte.Value.AddDays(-1),
                                                    BFEventReasonCode,
                                                    BFEvent_Id);

            switch (eventTraceCount)
            {
                case 0:
                    Repositories.NotificationRepository.SendEmail("Reinstate requested for T01 with no previous trace requests",
                                                                  config.EmailRecipients, Appl_EnfSrv_Cd + " " + Appl_CtrlCd);
                    return;
                case 1:
                case 2:
                    quarterDate = DateTimeHelper.AddQuarter(ReinstateEffectiveDate, 1);
                    break;
                case 3:
                    quarterDate = DateTimeHelper.AddQuarter(ReinstateEffectiveDate, 1).AddDays(-14);
                    break;
                case 4:
                    quarterDate = ReinstateEffectiveDate;
                    break;
                default:
                    quarterDate = ReinstateEffectiveDate.AddDays(14);
                    break;
            }

            TracingApplication.ActvSt_Cd = "A";
            TracingApplication.Appl_Reactv_Dte = ReinstateEffectiveDate;

            if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > DateTime.Now)
            {
                if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > quarterDate)
                {
                    if (eventTraceCount <= 3)
                        SetTracingForReinstate(quarterDate, quarterDate, EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING);
                    else if (eventTraceCount == 4)
                        SetTracingForReinstate(quarterDate, TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1), EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING);
                    else if (eventTraceCount == 5)
                    {
                        TracingApplication.AppLiSt_Cd = ApplicationState.PARTIALLY_SERVICED_12;
                        EventManager.AddBFEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                                      appState: ApplicationState.APPLICATION_REINSTATED_11,
                                                      effectiveTimestamp: TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1));
                    }
                    else
                    {
                        Repositories.NotificationRepository.SendEmail("Trace Cycle requests exceed yearly limit",
                                                                      config.EmailRecipients, Appl_EnfSrv_Cd + " " + Appl_CtrlCd);
                        SetNewStateTo(ApplicationState.EXPIRED_15);
                    }
                }
                else
                {
                    if (eventTraceCount > 4)
                    {
                        if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > DateTime.Now)
                        {
                            TracingApplication.AppLiSt_Cd = ApplicationState.PARTIALLY_SERVICED_12;
                            EventManager.AddBFEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                                          appState: ApplicationState.APPLICATION_REINSTATED_11,
                                                          effectiveTimestamp: TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1));
                        }
                        else
                            SetNewStateTo(ApplicationState.EXPIRED_15);
                    }
                    else
                    {
                        if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > DateTime.Now)
                            SetTracingForReinstate(quarterDate, TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1), EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING);
                        else
                            SetNewStateTo(ApplicationState.EXPIRED_15);
                    }
                }
            }
            else
                SetNewStateTo(ApplicationState.EXPIRED_15);
        }

        protected override void Process_12_PartiallyServiced()
        {
            base.Process_12_PartiallyServiced();
            EventManager.AddEvent(EventCode.C50821_FIRST_TRACE_RESULT_RECEIVED);
        }

        protected override void Process_13_FullyServiced()
        {
            EventManager.AddEvent(EventCode.C50823_LAST_TRACE_RESULT);

            var events = EventManager.GetEventBF(TracingApplication.Subm_SubmCd, TracingApplication.Appl_CtrlCd,
                                                 EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING, "A");

            bool isValidAffidavitDate = TracingApplication.Appl_RecvAffdvt_Dte.HasValue &&
                                        (DateTime.Now > TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1));

            if (events.Any() || isValidAffidavitDate)
                SetNewStateTo(ApplicationState.EXPIRED_15);
            else
                base.Process_13_FullyServiced();

        }

        protected override void Process_14_ManuallyTerminated()
        {
            base.Process_14_ManuallyTerminated();

            EventManager.DeleteBFEvent(TracingApplication.Subm_SubmCd, TracingApplication.Appl_CtrlCd);
        }

    }
}
