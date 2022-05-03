using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;

namespace FOAEA3.Business.Areas.Application
{
    internal class ApplicationStateEngine
    {
        private readonly Action Process_00_InitialState ;
        private readonly Action Process_01_InvalidApplication ;
        private readonly Action Process_02_AwaitingValidation ;
        private readonly Action Process_03_SinConfirmationPending ;
        private readonly Action Process_04_SinConfirmed ;
        private readonly Action Process_05_SinNotConfirmed ;
        private readonly Action Process_06_PendingAcceptanceSwearing ;
        private readonly Action Process_07_ValidAffidavitNotReceived ;
        private readonly Action Process_09_ApplicationRejected ;
        private readonly Action Process_10_ApplicationAccepted ;
        private readonly Action Process_11_ApplicationReinstated ;
        private readonly Action Process_12_PartiallyServiced ;
        private readonly Action Process_13_FullyServiced ;
        private readonly Action Process_14_ManuallyTerminated ;
        private readonly Action Process_15_Expired ;
        private readonly Action Process_17_FinancialTermsVaried ;
        private readonly Action Process_19_AwaitingDocumentsForVariation ;
        private readonly Action Process_35_ApplicationSuspended ;
        private readonly Action Process_91_InvalidVariationSource ;
        private readonly Action Process_92_InvalidVariationFinTerms ;
        private readonly Action Process_93_ValidFinancialVariation ;
        private readonly Action<ApplicationState, ApplicationState> InvalidStateChange ;

        public ApplicationStateEngine(
                Action process_00_InitialState,
                Action process_01_InvalidApplication,
                Action process_02_AwaitingValidation,
                Action process_03_SinConfirmationPending,
                Action process_04_SinConfirmed,
                Action process_05_SinNotConfirmed,
                Action process_06_PendingAcceptanceSwearing,
                Action process_07_ValidAffidavitNotReceived,
                Action process_09_ApplicationRejected,
                Action process_10_ApplicationAccepted,
                Action process_11_ApplicationReinstated,
                Action process_12_PartiallyServiced,
                Action process_13_FullyServiced,
                Action process_14_ManuallyTerminated,
                Action process_15_Expired,
                Action process_17_FinancialTermsVaried,
                Action process_19_AwaitingDocumentsForVariation,
                Action process_35_ApplicationSuspended,
                Action process_91_InvalidVariationSource,
                Action process_92_InvalidVariationFinTerms,
                Action process_93_ValidFinancialVariation,
                Action<ApplicationState, ApplicationState> invalidStateChange
            )
        {
            Process_00_InitialState = process_00_InitialState;
            Process_01_InvalidApplication = process_01_InvalidApplication;
            Process_02_AwaitingValidation = process_02_AwaitingValidation;
            Process_03_SinConfirmationPending = process_03_SinConfirmationPending;
            Process_04_SinConfirmed = process_04_SinConfirmed;
            Process_05_SinNotConfirmed = process_05_SinNotConfirmed;
            Process_06_PendingAcceptanceSwearing = process_06_PendingAcceptanceSwearing;
            Process_07_ValidAffidavitNotReceived = process_07_ValidAffidavitNotReceived;
            Process_09_ApplicationRejected = process_09_ApplicationRejected;
            Process_10_ApplicationAccepted = process_10_ApplicationAccepted;
            Process_11_ApplicationReinstated = process_11_ApplicationReinstated;
            Process_12_PartiallyServiced = process_12_PartiallyServiced;
            Process_13_FullyServiced = process_13_FullyServiced;
            Process_14_ManuallyTerminated = process_14_ManuallyTerminated;
            Process_15_Expired = process_15_Expired;
            Process_17_FinancialTermsVaried = process_17_FinancialTermsVaried;
            Process_19_AwaitingDocumentsForVariation = process_19_AwaitingDocumentsForVariation;
            Process_35_ApplicationSuspended = process_35_ApplicationSuspended;
            Process_91_InvalidVariationSource = process_91_InvalidVariationSource;
            Process_92_InvalidVariationFinTerms = process_92_InvalidVariationFinTerms;
            Process_93_ValidFinancialVariation = process_93_ValidFinancialVariation;
            InvalidStateChange = invalidStateChange;
        }

        public Dictionary<ApplicationState, List<ApplicationState>> ValidStateChange = new()
        {
            {
                ApplicationState.INITIAL_STATE_0,
                new List<ApplicationState> {
                        ApplicationState.INVALID_APPLICATION_1,
                        ApplicationState.AWAITING_VALIDATION_2
                    }
            },
            {
                ApplicationState.INVALID_APPLICATION_1,
                new List<ApplicationState> {
                        ApplicationState.INITIAL_STATE_0,
                        ApplicationState.APPLICATION_REJECTED_9,
                        ApplicationState.MANUALLY_TERMINATED_14
                    }
            },
            {
                ApplicationState.AWAITING_VALIDATION_2,
                new List<ApplicationState> {
                        ApplicationState.INVALID_APPLICATION_1,
                        ApplicationState.SIN_CONFIRMATION_PENDING_3,
                        ApplicationState.SIN_CONFIRMED_4
                    }
            },
            {
                ApplicationState.SIN_CONFIRMATION_PENDING_3,
                new List<ApplicationState> {
                        ApplicationState.SIN_CONFIRMATION_PENDING_3,
                        ApplicationState.SIN_CONFIRMED_4,
                        ApplicationState.SIN_NOT_CONFIRMED_5,
                        ApplicationState.APPLICATION_REJECTED_9,
                        ApplicationState.MANUALLY_TERMINATED_14
                    }
            },
            {
                ApplicationState.SIN_CONFIRMED_4,
                new List<ApplicationState> {
                        ApplicationState.PENDING_ACCEPTANCE_SWEARING_6
                    }
            },
            {
                ApplicationState.SIN_NOT_CONFIRMED_5,
                new List<ApplicationState> {
                        ApplicationState.INITIAL_STATE_0,
                        ApplicationState.INVALID_APPLICATION_1,
                        ApplicationState.SIN_CONFIRMED_4,
                        ApplicationState.SIN_NOT_CONFIRMED_5,
                        ApplicationState.APPLICATION_REJECTED_9,
                        ApplicationState.MANUALLY_TERMINATED_14
                    }
            },
            {
                ApplicationState.PENDING_ACCEPTANCE_SWEARING_6,
                new List<ApplicationState> {
                        ApplicationState.INVALID_APPLICATION_1,
                        ApplicationState.PENDING_ACCEPTANCE_SWEARING_6,
                        ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7,
                        ApplicationState.APPLICATION_REJECTED_9,
                        ApplicationState.APPLICATION_ACCEPTED_10,
                        ApplicationState.MANUALLY_TERMINATED_14
                    }
            },
            {
                ApplicationState.APPLICATION_ACCEPTED_10,
                new List<ApplicationState> {
                        ApplicationState.APPLICATION_ACCEPTED_10,
                        ApplicationState.PARTIALLY_SERVICED_12,
                        ApplicationState.FULLY_SERVICED_13,
                        ApplicationState.MANUALLY_TERMINATED_14,
                        ApplicationState.EXPIRED_15
                    }
            },
            {
                ApplicationState.PARTIALLY_SERVICED_12,
                new List<ApplicationState> {
                        ApplicationState.PARTIALLY_SERVICED_12,
                        ApplicationState.FULLY_SERVICED_13,
                        ApplicationState.MANUALLY_TERMINATED_14,
                        ApplicationState.EXPIRED_15
                    }
            }
        };

        private Dictionary<ApplicationState, Action> ApplicationStateAction =>
                   new()
                   {
                       { ApplicationState.INITIAL_STATE_0, Process_00_InitialState },
                       { ApplicationState.INVALID_APPLICATION_1, Process_01_InvalidApplication },
                       { ApplicationState.AWAITING_VALIDATION_2, Process_02_AwaitingValidation },
                       { ApplicationState.SIN_CONFIRMATION_PENDING_3, Process_03_SinConfirmationPending },
                       { ApplicationState.SIN_CONFIRMED_4, Process_04_SinConfirmed },
                       { ApplicationState.SIN_NOT_CONFIRMED_5, Process_05_SinNotConfirmed },
                       { ApplicationState.PENDING_ACCEPTANCE_SWEARING_6, Process_06_PendingAcceptanceSwearing },
                       { ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7, Process_07_ValidAffidavitNotReceived },
                       { ApplicationState.APPLICATION_REJECTED_9, Process_09_ApplicationRejected },
                       { ApplicationState.APPLICATION_ACCEPTED_10, Process_10_ApplicationAccepted },
                       { ApplicationState.APPLICATION_REINSTATED_11, Process_11_ApplicationReinstated },
                       { ApplicationState.PARTIALLY_SERVICED_12, Process_12_PartiallyServiced },
                       { ApplicationState.FULLY_SERVICED_13, Process_13_FullyServiced },
                       { ApplicationState.MANUALLY_TERMINATED_14, Process_14_ManuallyTerminated },
                       { ApplicationState.EXPIRED_15, Process_15_Expired },
                       { ApplicationState.FINANCIAL_TERMS_VARIED_17, Process_17_FinancialTermsVaried },
                       { ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19, Process_19_AwaitingDocumentsForVariation },
                       { ApplicationState.APPLICATION_SUSPENDED_35, Process_35_ApplicationSuspended },
                       { ApplicationState.INVALID_VARIATION_SOURCE_91, Process_91_InvalidVariationSource },
                       { ApplicationState.INVALID_VARIATION_FINTERMS_92, Process_92_InvalidVariationFinTerms },
                       { ApplicationState.VALID_FINANCIAL_VARIATION_93, Process_93_ValidFinancialVariation }
                   };

        public bool IsValidStateChange(ApplicationState oldState, ApplicationState newState)
        {
            if (ValidStateChange.TryGetValue(oldState, out List<ApplicationState> validChoices))
                return validChoices.Contains(newState);
            else
                return false;
        }

        public void SetNewStateTo(ApplicationState oldState, ApplicationState newState)
        {
            if (IsValidStateChange(oldState, newState))
            {
                Action stateAction = ApplicationStateAction[newState];
                stateAction();
            }
            else
                InvalidStateChange(oldState, newState);
        }
    }
}
