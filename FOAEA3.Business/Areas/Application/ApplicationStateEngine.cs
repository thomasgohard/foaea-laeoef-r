using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;

namespace FOAEA3.Business.Areas.Application
{
    internal class ApplicationStateEngine
    {
        public Action Process_00_InitialState { get; set; }
        public Action Process_01_InvalidApplication { get; set; }
        public Action Process_02_AwaitingValidation { get; set; }
        public Action Process_03_SinConfirmationPending { get; set; }
        public Action Process_04_SinConfirmed { get; set; }
        public Action Process_05_SinNotConfirmed { get; set; }
        public Action Process_06_PendingAcceptanceSwearing { get; set; }
        public Action Process_07_ValidAffidavitNotReceived { get; set; }
        public Action Process_09_ApplicationRejected { get; set; }
        public Action Process_10_ApplicationAccepted { get; set; }
        public Action Process_11_ApplicationReinstated { get; set; }
        public Action Process_12_PartiallyServiced { get; set; }
        public Action Process_13_FullyServiced { get; set; }
        public Action Process_14_ManuallyTerminated { get; set; }
        public Action Process_15_Expired { get; set; }
        public Action Process_17_FinancialTermsVaried { get; set; }
        public Action Process_19_AwaitingDocumentsForVariation { get; set; }
        public Action Process_35_ApplicationSuspended { get; set; }
        public Action Process_91_InvalidVariationSource { get; set; }
        public Action Process_92_InvalidVariationFinTerms { get; set; }
        public Action Process_93_ValidFinancialVariation { get; set; }
        public Action<ApplicationState, ApplicationState> InvalidStateChange { get; set; }

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
