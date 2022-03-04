using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using System;
using System.Linq;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class InterceptionManager : ApplicationManager
    {
        protected override void Process_02_AwaitingValidation()
        {
            var exGratias = Repositories.InterceptionRepository.GetExGratias();

            string appEnteredSIN = InterceptionApplication.Appl_Dbtr_Entrd_SIN;
            string appRefNumber = InterceptionApplication.Appl_Source_RfrNr;
            bool exGratiaSinBlocked = exGratias.Any(m => (!string.IsNullOrEmpty(m.SIN.Trim())) &&
                                                         (m.SIN == appEnteredSIN) && m.BlockSIN);
            bool exGratiaRefNrBlocked = exGratias.Any(m => (!string.IsNullOrEmpty(m.Source_Ref_Nr.Trim())) &&
                                                           (m.Source_Ref_Nr == appRefNumber) && m.BlockREF);

            if (exGratiaSinBlocked || exGratiaRefNrBlocked)
            {
                InterceptionApplication.AppLiSt_Cd = ApplicationState.AWAITING_VALIDATION_2;

                EventManager.AddEvent(EventCode.C50600_INVALID_APPLICATION,
                                      eventReasonText: "Awaiting Action by FOAEA / En attente d’une action par AEOEF");

                var dbNotification = Repositories.NotificationRepository;

                string subject = "Application blocked due to ExGratia process";
                string body = "Application has been blocked because of: ";

                if (exGratiaRefNrBlocked) body += $"\n\tRef #: {appRefNumber}";
                if (exGratiaSinBlocked) body += $"\n\tSIN : {appEnteredSIN}";

                body += $"\n\n{Appl_EnfSrv_Cd}-{Appl_CtrlCd}";

                dbNotification.SendEmail(subject, config.ExGratiaRecipients, body);
            }
            else
            {

                if (String.IsNullOrEmpty(InterceptionApplication.Appl_Dbtr_Entrd_SIN))
                    InterceptionValidation.CheckCreditorSurname();

                base.Process_02_AwaitingValidation();

            }
        }

        protected override void Process_04_SinConfirmed()
        {
            base.Process_04_SinConfirmed();

            InterceptionApplication.Appl_Affdvt_DocTypCd = I01_AFFITDAVIT_DOCUMENT_CODE;
            SetNewStateTo(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);
        }

        protected override void Process_06_PendingAcceptanceSwearing()
        {
            base.Process_06_PendingAcceptanceSwearing();

            Validation.AddDuplicateSINWarningEvents();

            var submitterDB = Repositories.SubmitterRepository;
            string signAuthority = submitterDB.GetSignAuthorityForSubmitter(InterceptionApplication.Subm_SubmCd);

            EventManager.AddEvent(EventCode.C50701_WAITING_ACCEPTANCE_OF_GARNISHEE_SUMMONS_AT_FOAEA, recipientSubm: signAuthority);
        }

        protected override void Process_07_ValidAffidavitNotReceived()
        {
            var expectedNextState = ApplicationState.PENDING_ACCEPTANCE_SWEARING_6;

            if (string.IsNullOrEmpty(InterceptionApplication.Subm_Affdvt_SubmCd) || (!InterceptionApplication.Appl_RecvAffdvt_Dte.HasValue))
                EventManager.AddEvent(EventCode.C51019_REJECTED_AFFIDAVIT);  // ?? this never happens since this is not called except when accepting...?
            else
                expectedNextState = ApplicationState.APPLICATION_ACCEPTED_10;

            SendDebtorLetter();

            SetNewStateTo(expectedNextState);
        }

        protected override void Process_09_ApplicationRejected()
        {
            //base.Process_09_ApplicationRejected();
            InterceptionApplication.AppLiSt_Cd = ApplicationState.APPLICATION_REJECTED_9;
            InterceptionApplication.ActvSt_Cd = "J";

            if (!AcceptedWithin30Days.HasValue)
                AcceptedWithin30Days = true;

            if (!AcceptedWithin30Days.Value)
            {
                return;
            }
            else if (InterceptionApplication.AppLiSt_Cd == ApplicationState.PENDING_ACCEPTANCE_SWEARING_6)
            {
                if (ESDReceived)
                    EventManager.AddEvent(EventCode.C50761_APPLICATION_REJECTED_AS_SUMMONS_RECEIVED_MORE_THAN_30_DAYS_FROM_ITS_ISSUANCE);
            }
            else if (InterceptionApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5))
            {
                EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME, queue: EventQueue.EventAM);
            }
            else
                EventManager.AddEvent(EventCode.C50591_REJECTED_APPLICATION);
        }

        protected override void Process_10_ApplicationAccepted()
        {
            base.Process_10_ApplicationAccepted();

            var interceptionDB = Repositories.InterceptionRepository;

            string justiceID = interceptionDB.GetApplicationJusticeNumber(InterceptionApplication.Appl_Dbtr_Cnfrmd_SIN,
                                                                                               Appl_EnfSrv_Cd, Appl_CtrlCd);
            string debtorID;
            string justiceSuffix;
            EventCode eventBFNreasonCode;

            if (string.IsNullOrEmpty(justiceID))
            {
                eventBFNreasonCode = EventCode.C56001_NEW_BFN_FOR_NEW_DEBTOR;
                debtorID = GenerateDebtorID(InterceptionApplication.Appl_Dbtr_SurNme);
                justiceSuffix = "A";
            }
            else
            {
                eventBFNreasonCode = EventCode.C56002_NEW_BFN_FOR_EXISTING_DEBTOR;
                debtorID = GetDebtorID(justiceID);
                ProcessSummSmryBFN(debtorID, ref eventBFNreasonCode);
                nextJusticeID_callCount = 0;
                justiceSuffix = NextJusticeID(justiceID);
            }

            ChangeStateForFinancialTerms(oldState: "P", newState: "A", 10);

            bool isESDsite = IsESD_MEP(Appl_EnfSrv_Cd);
            DateTime startDate = interceptionDB.GetGarnisheeSummonsReceiptDate(Appl_EnfSrv_Cd, Appl_CtrlCd, isESDsite);

            CreateSummonsSummary(debtorID, justiceSuffix, startDate);

            if (!string.IsNullOrEmpty(InterceptionApplication.IntFinH.IntFinH_DefHldbAmn_Period))
            {
                var fixedAmountDB = RepositoriesFinance.SummonsSummaryFixedAmountRepository;
                var fixedAmountData = fixedAmountDB.GetSummonsSummaryFixedAmount(Appl_EnfSrv_Cd, Appl_CtrlCd);

                if (fixedAmountData is null)
                {
                    // create fixed amount data
                    fixedAmountDB.CreateSummonsSummaryFixedAmount(Appl_EnfSrv_Cd, Appl_CtrlCd, startDate);
                }
                else
                {
                    // TODO when would this already exists?
                    fixedAmountData.SummSmry_LastFixedAmountCalc_Dte = DateTime.Now;
                    fixedAmountData.SummSmry_FixedAmount_Recalc_Dte = startDate;

                    fixedAmountDB.UpdateSummonsSummaryFixedAmount(fixedAmountData);
                }
            }

            DateTime endDate = startDate.AddYears(5).AddDays(-1);
            string reasonText = $"{debtorID}{justiceSuffix}{startDate.Year}{startDate.DayOfYear:000}" +
                                $"{endDate.Year}{endDate.DayOfYear:000}";

            justiceID = debtorID + justiceSuffix;

            InterceptionApplication.Appl_JusticeNr = justiceID;

            if (eventBFNreasonCode != EventCode.UNDEFINED)
                EventManager.AddEvent(eventBFNreasonCode, queue: EventQueue.EventBFN, activeState:"A");

            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED, eventReasonText: reasonText, activeState: "I");

            NotifyMatchingActiveApplications(EventCode.C50934_AN_APPLICATION_HAS_BEEN_ACCEPTED_FOR_THE_SAME_DEBTOR___CREDITOR_FROM_ANOTHER_JURISDICTION);
        }

        protected override void Process_12_PartiallyServiced()
        {
            var currentState = InterceptionApplication.AppLiSt_Cd;

            if (currentState == ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)
            {
                if (VariationAction == VariationDocumentAction.AcceptVariationDocument)
                {
                    base.Process_12_PartiallyServiced();

                    EventManager.AddEvent(EventCode.C51111_VARIATION_ACCEPTED);
                    var interceptionDB = Repositories.InterceptionRepository;
                    if (interceptionDB.IsVariationIncrease(Appl_EnfSrv_Cd, Appl_CtrlCd))
                        EventManager.AddEvent(EventCode.C51113_VARIATION_ACCEPTED_WITH_AN_ARREARS_VALUE_SIGNIFICANTLY_GREATER_THAN_THE_PREVIOUS_ARREARS);
                }
                else
                {
                    var summonsSummaryData = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(Appl_EnfSrv_Cd, Appl_CtrlCd).FirstOrDefault();

                    var recalcDate = summonsSummaryData?.SummSmry_Recalc_Dte;
                    if ((recalcDate.HasValue) && (recalcDate.Value.Year == 3000) && (recalcDate.Value.Month == 1) && (recalcDate.Value.Day == 1))
                        InterceptionApplication.AppLiSt_Cd = ApplicationState.APPLICATION_SUSPENDED_35;

                    EventManager.AddEvent(EventCode.C51110_VARIATION_REJECTED);
                }
            }
            else if (currentState == ApplicationState.APPLICATION_ACCEPTED_10)
            {
                base.Process_12_PartiallyServiced();

                EventManager.AddEvent(EventCode.C50826_MONEY_HAS_BEEN_RECEIVED);
            }

        }

        protected override void Process_13_FullyServiced()
        {
            base.Process_13_FullyServiced();

            StopBlockFunds(ApplicationState.FULLY_SERVICED_13);

            InterceptionApplication.ActvSt_Cd = "C";

            EventManager.AddEvent(EventCode.C50850_APPLICATION_FINANCIALLY_SATISFIED);
        }

        protected override void Process_14_ManuallyTerminated()
        {
            var previousState = InterceptionApplication.AppLiSt_Cd;

            StopBlockFunds(ApplicationState.MANUALLY_TERMINATED_14);

            InterceptionApplication.ActvSt_Cd = "X";
            InterceptionApplication.AppLiSt_Cd = ApplicationState.MANUALLY_TERMINATED_14;

            if (previousState > ApplicationState.APPLICATION_REJECTED_9)
                EventManager.AddEvent(EventCode.C50843_APPLICATION_CANCELLED, activeState: "I");
            else
                EventManager.AddEvent(EventCode.C50843_APPLICATION_CANCELLED, activeState: "A");

        }

        protected override void Process_15_Expired()
        {
            InterceptionApplication.AppLiSt_Cd = ApplicationState.EXPIRED_15;
            
            EventManager.AddEvent(EventCode.C50860_APPLICATION_COMPLETED, activeState: "I");
         
            StopBlockFunds(ApplicationState.EXPIRED_15);

            InterceptionApplication.ActvSt_Cd = "C";
        }

        protected override void Process_17_FinancialTermsVaried()
        {
            var currentApplicationManager = new InterceptionManager(Repositories, RepositoriesFinance, config);
            currentApplicationManager.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd);

            var currentApplInfo = currentApplicationManager.InterceptionApplication;

            switch (currentApplInfo.AppLiSt_Cd)
            {
                case ApplicationState.APPLICATION_ACCEPTED_10:
                case ApplicationState.PARTIALLY_SERVICED_12:
                case ApplicationState.APPLICATION_SUSPENDED_35:
                    
                    if (!InterceptionValidation.ValidVariationDefaultHoldbacks())
                        SetNewStateTo(ApplicationState.INVALID_VARIATION_FINTERMS_92);

                    else if (!InterceptionValidation.ValidVariationSourceSpecificHoldbacks())
                        SetNewStateTo(ApplicationState.INVALID_VARIATION_SOURCE_91);

                    else
                        SetNewStateTo(ApplicationState.VALID_FINANCIAL_VARIATION_93);

                    break;

                default:

                    // should not try to vary application at any other state
                    EventManager.AddEvent(EventCode.C55006_APPLICATION_NOT_IN_EFFECT);
                    return;
            }
        }

        protected override void Process_19_AwaitingDocumentsForVariation()
        {
            base.Process_19_AwaitingDocumentsForVariation();

            EventManager.AddBFEvent(EventCode.C50896_AWAITING_DOCUMENTS_FOR_VARIATION, effectiveTimestamp: DateTime.Now.AddDays(5));
        }

        protected override void Process_35_ApplicationSuspended()
        {

            if (InterceptionApplication.AppLiSt_Cd.In(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12,
                                                      ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19))
                EventManager.AddEvent(EventCode.C51115_APPLICATION_SUSPENDED);
            else
                EventManager.AddEvent(EventCode.C55006_APPLICATION_NOT_IN_EFFECT);

            base.Process_35_ApplicationSuspended();
        }

        protected override void Process_91_InvalidVariationSource()
        {
            EventManager.AddEvent(EventCode.C55001_INVALID_SOURCE_HOLDBACK);
            EventManager.AddEvent(EventCode.C55000_INVALID_VARIATION);
        }

        protected override void Process_92_InvalidVariationFinTerms()
        {
            EventManager.AddEvent(EventCode.C55002_INVALID_FINANCIAL_TERMS);
            EventManager.AddEvent(EventCode.C55000_INVALID_VARIATION);
        }

        protected override void Process_93_ValidFinancialVariation()
        {
            EventManager.AddEvent(EventCode.C50896_AWAITING_DOCUMENTS_FOR_VARIATION);

            SetNewStateTo(ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19);
        }

    }
}
