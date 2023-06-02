using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class TracingManager : ApplicationManager
    {
        protected override async Task Process_04_SinConfirmed()
        {
            await base.Process_04_SinConfirmed();

            if (TracingValidation.IsC78() || await AffidavitExistsAsync())
                await SetNewStateTo(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);
            else
                await SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
        }

        protected override async Task Process_06_PendingAcceptanceSwearing()
        {
            await base.Process_06_PendingAcceptanceSwearing();

            await Validation.AddDuplicateSINWarningEventsAsync();

            if (TracingValidation.IsC78())
            {
                await SetNewStateTo(ApplicationState.APPLICATION_ACCEPTED_10);
            }
            else
            {
                string signAuthority = await DB.SubmitterTable.GetSignAuthorityForSubmitterAsync(TracingApplication.Subm_SubmCd);

                EventManager.AddEvent(EventCode.C51042_REQUIRES_LEGAL_AUTHORIZATION, appState: ApplicationState.PENDING_ACCEPTANCE_SWEARING_6,
                                      recipientSubm: signAuthority);

                if (TracingApplication.Medium_Cd == "FTP")
                    EventManager.AddEvent(EventCode.C51042_REQUIRES_LEGAL_AUTHORIZATION, appState: ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);

                // FOAEA users can bypass swearing and go directly to state 10 
                if (TracingApplication.Subm_Recpt_SubmCd.IsInternalUser() &&
                    !String.IsNullOrEmpty(TracingApplication.Subm_Affdvt_SubmCd) &&
                    TracingApplication.Appl_RecvAffdvt_Dte.HasValue)
                {
                    await SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
                }
            }
        }

        protected override async Task Process_07_ValidAffidavitNotReceived()
        {
            if (String.IsNullOrEmpty(TracingApplication.Subm_Affdvt_SubmCd) ||
               (!TracingApplication.Appl_RecvAffdvt_Dte.HasValue))
            {
                await base.Process_07_ValidAffidavitNotReceived();
                EventManager.AddEvent(EventCode.C54004_AWAITING_AFFIDAVIT_TO_BE_ENTERED);
            }
            else
            {
                await Process_10_ApplicationAccepted();
            }
        }

        protected override async Task Process_10_ApplicationAccepted()
        {
            await Validation.AddDuplicateCreditorWarningEventsAsync();

            TracingApplication.Messages.AddInformation(EventCode.C50780_APPLICATION_ACCEPTED);

            EventManager.AddTraceEvent(EventCode.C50780_APPLICATION_ACCEPTED, appState: ApplicationState.APPLICATION_ACCEPTED_10);

            EventManager.AddSubmEvent(EventCode.C50780_APPLICATION_ACCEPTED, appState: ApplicationState.APPLICATION_ACCEPTED_10);

            EventManager.AddSubmEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                      appState: ApplicationState.APPLICATION_REINSTATED_11);

            EventManager.AddBFEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                    appState: ApplicationState.APPLICATION_REINSTATED_11,
                                    effectiveDateTime: DateTime.Now.AddMonths(3));

            TracingApplication.Messages.AddInformation(EventCode.C50780_APPLICATION_ACCEPTED);

            TracingApplication.Appl_Reactv_Dte = DateTime.Now;

            await base.Process_10_ApplicationAccepted();
        }

        protected override async Task Process_11_ApplicationReinstated()
        {
            if (TracingApplication.Appl_RecvAffdvt_Dte is null)
            {
                await AddSystemErrorAsync(DB, TracingApplication.Messages, Config.Recipients.SystemErrorRecipients,
                               $"Appl_RecvAffdvt_Dte is null for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}. Cannot process state 11 (Reinstate).");
                return;
            }

            DateTime quarterDate;
            int eventTraceCount = await EventManager.GetTraceEventCountAsync(Appl_EnfSrv_Cd, Appl_CtrlCd,
                                                                             TracingApplication.Appl_RecvAffdvt_Dte.Value.AddDays(-1),
                                                                             BFEventReasonCode,
                                                                             BFEvent_Id);

            switch (eventTraceCount)
            {
                case 0:
                    await AddSystemErrorAsync(DB, TracingApplication.Messages, Config.Recipients.SystemErrorRecipients,
                                   $"Reinstate requested for T01 ({Appl_EnfSrv_Cd}-{Appl_CtrlCd}) with no previous trace requests");
                    return;
                case 1:
                case 2:
                    quarterDate = ReinstateEffectiveDate.AddQuarter(1);
                    break;
                case 3:
                    quarterDate = ReinstateEffectiveDate.AddQuarter(1).AddDays(-14);
                    break;
                case 4:
                    quarterDate = ReinstateEffectiveDate;
                    break;
                default:
                    quarterDate = ReinstateEffectiveDate.AddDays(14);
                    break;
            }

            await base.Process_11_ApplicationReinstated();

            TracingApplication.ActvSt_Cd = "A";
            TracingApplication.Appl_Reactv_Dte = ReinstateEffectiveDate;

            if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > DateTime.Now)
            {
                if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > quarterDate)
                {
                    if (eventTraceCount <= 3)
                        await SetTracingForReinstateAsync(quarterDate, quarterDate, EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING);
                    else if (eventTraceCount == 4)
                        await SetTracingForReinstateAsync(quarterDate, TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1), EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING);
                    else if (eventTraceCount == 5)
                    {
                        TracingApplication.AppLiSt_Cd = ApplicationState.PARTIALLY_SERVICED_12;
                        EventManager.AddBFEvent(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                                appState: ApplicationState.APPLICATION_REINSTATED_11,
                                                effectiveDateTime: TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1));
                    }
                    else
                    {
                        await DB.NotificationService.SendEmailAsync("Trace Cycle requests exceed yearly limit",
                                                                      Config.Recipients.EmailRecipients, Appl_EnfSrv_Cd + " " + Appl_CtrlCd);
                        await SetNewStateTo(ApplicationState.EXPIRED_15);
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
                                                    effectiveDateTime: TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1));
                        }
                        else
                            await SetNewStateTo(ApplicationState.EXPIRED_15);
                    }
                    else
                    {
                        if (TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1) > DateTime.Now)
                            await SetTracingForReinstateAsync(quarterDate, TracingApplication.Appl_RecvAffdvt_Dte.Value.AddYears(1).AddDays(1), EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING);
                        else
                            await SetNewStateTo(ApplicationState.EXPIRED_15);
                    }
                }
            }
            else
                await SetNewStateTo(ApplicationState.EXPIRED_15);
        }

        protected override async Task Process_12_PartiallyServiced()
        {
            await base.Process_12_PartiallyServiced();

            EventManager.AddEvent(EventCode.C50821_FIRST_TRACE_RESULT_RECEIVED);
        }

        protected override async Task Process_14_ManuallyTerminated()
        {
            await base.Process_14_ManuallyTerminated();

            await EventManager.DeleteBFEvent(TracingApplication.Subm_SubmCd, TracingApplication.Appl_CtrlCd);
        }

        protected override async Task Process_15_Expired()
        {
            await base.Process_15_Expired();

            EventManager.AddEvent(EventCode.C50823_LAST_TRACE_RESULT);
        }
    }
}
