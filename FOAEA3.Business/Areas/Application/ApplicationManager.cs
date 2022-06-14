using FOAEA3.Resources.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class ApplicationManager
    {
        private readonly ApplicationData Application;

        public IRepositories Repositories { get; }
        public ApplicationEventManager EventManager { get; }
        public ApplicationEventDetailManager EventDetailManager { get; }

        protected ApplicationValidation Validation { get; }
        protected readonly CustomConfig config;

        protected ApplicationStateEngine StateEngine { get; }

        protected string Appl_EnfSrv_Cd => Application.Appl_EnfSrv_Cd.Trim();

        protected string Appl_CtrlCd => Application.Appl_CtrlCd.Trim();

        public ApplicationManager(ApplicationData applicationData, IRepositories repositories, CustomConfig config,
                                  ApplicationValidation applicationValidation = null)
        {
            this.config = config;
            Application = applicationData;
            EventManager = new ApplicationEventManager(Application, repositories);
            EventDetailManager = new ApplicationEventDetailManager(Application, repositories);
            Repositories = repositories;
            if (applicationValidation is null)
                Validation = new ApplicationValidation(Application, EventManager, Repositories, config);
            else
            {
                Validation = applicationValidation;
                Validation.EventManager = EventManager;
            }

            StateEngine = new ApplicationStateEngine(
                process_00_InitialState: Process_00_InitialState,
                process_01_InvalidApplication: Process_01_InvalidApplication,
                process_02_AwaitingValidation: Process_02_AwaitingValidation,
                process_03_SinConfirmationPending: Process_03_SinConfirmationPending,
                process_04_SinConfirmed: Process_04_SinConfirmed,
                process_05_SinNotConfirmed: Process_05_SinNotConfirmed,
                process_06_PendingAcceptanceSwearing: Process_06_PendingAcceptanceSwearing,
                process_07_ValidAffidavitNotReceived: Process_07_ValidAffidavitNotReceived,
                process_09_ApplicationRejected: Process_09_ApplicationRejected,
                process_10_ApplicationAccepted: Process_10_ApplicationAccepted,
                process_11_ApplicationReinstated: Process_11_ApplicationReinstated,
                process_12_PartiallyServiced: Process_12_PartiallyServiced,
                process_13_FullyServiced: Process_13_FullyServiced,
                process_14_ManuallyTerminated: Process_14_ManuallyTerminated,
                process_15_Expired: Process_15_Expired,
                process_17_FinancialTermsVaried: Process_17_FinancialTermsVaried,
                process_19_AwaitingDocumentsForVariation: Process_19_AwaitingDocumentsForVariation,
                process_35_ApplicationSuspended: Process_35_ApplicationSuspended,
                process_91_InvalidVariationSource: Process_91_InvalidVariationSource,
                process_92_InvalidVariationFinTerms: Process_92_InvalidVariationFinTerms,
                process_93_ValidFinancialVariation: Process_93_ValidFinancialVariation,
                invalidStateChange: InvalidStateChange
            );
        }

        public void SetNewStateTo(ApplicationState newState)
        {
            StateEngine.SetNewStateTo(Application.AppLiSt_Cd, newState);
        }

        public ApplicationState GetState()
        {
            return Application.AppLiSt_Cd;
        }

        public string GetCategory()
        {
            return Application.AppCtgy_Cd;
        }

        protected bool IsValidCategory(string category)
        {
            if (Application.AppCtgy_Cd != category)
            {
                Application.Messages.AddError($"Invalid category type ({Application.AppCtgy_Cd}). Expected {category}.");
                return false;
            }
            else
                return true;
        }

        public virtual bool LoadApplication(string enfService, string controlCode)
        {
            bool isSuccess = false;

            ApplicationData data = Repositories.ApplicationRepository.GetApplication(enfService, controlCode);

            if (data != null)
            {
                isSuccess = true;
                Application.Merge(data);
            }

            return isSuccess;
        }

        public bool ApplicationExists()
        {
            if (!String.IsNullOrEmpty(Appl_CtrlCd))
            {
                return Repositories.ApplicationRepository.ApplicationExists(Appl_EnfSrv_Cd, Appl_CtrlCd);
            }
            else
                return false;
        }

        public virtual bool CreateApplication()
        {
            if (ApplicationExists())
            {
                Application.Messages.AddError(ErrorResource.CANT_CREATE_APPLICATION_ALREADY_EXISTS);
                return false;
            }

            // clean data
            TrimTrailingSpaces();
            MakeUpperCase();

            Application.Appl_Create_Dte = DateTime.Now;
            Application.Appl_Create_Usr = Repositories.CurrentSubmitter;
            Application.Appl_LastUpdate_Dte = DateTime.Now;
            Application.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;

            // generate control code if not entered
            if (String.IsNullOrEmpty(Appl_CtrlCd))
            {
                Application.Appl_CtrlCd = Repositories.ApplicationRepository.GenerateApplicationControlCode(Appl_EnfSrv_Cd);
                Validation.IsSystemGeneratedControlCode = true;
            }

            // add reminder
            EventManager.AddBFEvent(EventCode.C50528_BF_10_DAYS_FROM_RECEIPT_OF_APPLICATION);

            // validate data and decide on what state to bring the application to
            Process_00_InitialState();

            if (Application.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                EventManager.Events.Clear();
                return false;
            }

            bool isSuccess;
            try
            {
                // save the application to the database
                isSuccess = Repositories.ApplicationRepository.CreateApplication(Application);

                if (isSuccess)
                {
                    // update submitter code with last control code used
                    UpdateSubmitterDefaultControlCode();

                    // save the events for the newly created application
                    EventManager.SaveEvents();

                    // update messages for display in UI
                    if (Application.Medium_Cd != "FTP") Application.Messages.AddInformation($"{LanguageResource.APPLICATION_REFERENCE_NUMBER}: {Appl_EnfSrv_Cd}-{Application.Subm_SubmCd}-{Appl_CtrlCd}");
                    if (Application.Medium_Cd != "FTP") Application.Messages.AddInformation(ReferenceData.Instance().ApplicationLifeStates[Application.AppLiSt_Cd].Description);

                    Repositories.SubmitterRepository.SubmitterMessageDelete(Application.Subm_SubmCd);
                }

            }
            catch (Exception e)
            {

                Application.Messages.AddError(e.Message);
                isSuccess = false;

            }

            if ((!isSuccess) && (Validation.IsSystemGeneratedControlCode))
                Application.Appl_CtrlCd = string.Empty; // reset to empty since it could not be saved

            return isSuccess;
        }

        public virtual void UpdateApplicationNoValidation()
        {
            Application.Appl_LastUpdate_Dte = DateTime.Now;
            Application.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;

            Repositories.ApplicationRepository.UpdateApplication(Application);
            Repositories.SubmitterRepository.SubmitterMessageDelete(Application.Appl_LastUpdate_Usr);

            EventManager.SaveEvents();
        }

        public virtual void UpdateApplication()
        {

            if (!ApplicationExists())
            {
                Application.Messages.AddError(ErrorResource.CANT_UPDATE_APPLICATION_DOES_NOT_EXISTS);
                return;
            }

            Application.Appl_LastUpdate_Dte = DateTime.Now;
            Application.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;

            bool isCancelled = Application.ActvSt_Cd == "X";
            bool isReset = Application.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5);

            if (isReset && isCancelled)
            {
                Application.Messages.AddError(EventCode.C50527_CHANGES_NOT_ALLOWED_ON_CANCELLED_APPLICATION);
                return;
            }


            // load old data from database
            var current = new ApplicationManager(new ApplicationData(), Repositories, config);
            current.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd);

            if (isReset) // reset
            {
                // clean data
                TrimTrailingSpaces();
                MakeUpperCase();

                Process_00_InitialState();
            }
            else // regular update
            {
                Validation.ValidateAndRevertNonUpdateFields(current.Application);

                // update reaason text with message

                string reasonText = BuildApplicationReasonText(BuildReferenceNumberChangeReasonText(Application, current.Application),
                                                               BuildAddressChangeReasonText(Application, current.Application),
                                                               BuildCommentsChangeReasonText(Application, current.Application));
                if (!string.IsNullOrEmpty(reasonText))
                    EventManager.AddEvent(EventCode.C51020_APPLICATION_UPDATED, reasonText);

            }

            if (Application.Messages.ContainsMessagesOfType(MessageType.Error))
                return;

            try
            {
                // save the application to the database
                Repositories.ApplicationRepository.UpdateApplication(Application);

                // update submitter code with last control code used
                UpdateSubmitterDefaultControlCode();

                // save the events for the newly created application
                EventManager.SaveEvents();

                // update messages for display in UI
                Application.Messages.AddInformation($"{LanguageResource.APPLICATION_REFERENCE_NUMBER}: {Application.Appl_EnfSrv_Cd}-{Application.Subm_SubmCd}-{Application.Appl_CtrlCd}");
                Application.Messages.AddInformation(ReferenceData.Instance().ApplicationLifeStates[Application.AppLiSt_Cd].Description);

            }
            catch (Exception e)
            {
                Application.Messages.AddError(e.Message);
            }

        }

        public void TransferApplication(string applSelectedSubmitter, string applSelectedRecipient)
        {

            Application.Subm_SubmCd = applSelectedSubmitter;
            Application.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;
            if (Application.Medium_Cd == "ONL")
                Application.Subm_Recpt_SubmCd = applSelectedRecipient;

            var caseManagementData = new CaseManagementData
            {
                Appl_CtrlCd = Application.Appl_CtrlCd,
                Appl_EnfSrv_Cd = Application.Appl_EnfSrv_Cd,
                CaseMgmnt_Dte = DateTime.Now,
                Subm_Recpt_SubmCd = applSelectedRecipient,
                Subm_SubmCd = applSelectedSubmitter
            };
            Repositories.CaseManagementRepository.CreateCaseManagement(caseManagementData);

            EventManager.AddEvent(EventCode.C51202_APPLICATION_HAS_BEEN_TRANSFERED);

            Application.Messages.AddInformation(EventCode.C51202_APPLICATION_HAS_BEEN_TRANSFERED);

            UpdateApplication();

        }

        public List<StatsOutgoingProvincialData> GetProvincialStatsOutgoingData(int maxRecords,
                                                                     string activeState,
                                                                     string recipientCode,
                                                                     bool isXML = true)
        {
            var applicationDB = Repositories.ApplicationRepository;
            var data = applicationDB.GetStatsProvincialOutgoingData(maxRecords, activeState, recipientCode, isXML);
            return data;
        }

        public virtual void ProcessBringForwards(ApplicationEventData bfEvent)
        {
            throw new NotImplementedException("ProcessBringForwards has not been implemented!");
        }

        private static string BuildApplicationReasonText(string referenceNumberText, string addressText, string commentText)
        {

            var reasonText = new StringBuilder();

            if (!string.IsNullOrEmpty(referenceNumberText))
                reasonText.Append(referenceNumberText.Trim() + ", ");
            if (!string.IsNullOrEmpty(addressText))
                reasonText.Append(addressText.Trim() + ", ");
            if (!string.IsNullOrEmpty(commentText))
                reasonText.Append(commentText.Trim());

            return reasonText.ToString().TrimEnd(',', ' ');

        }

        protected void UpdateSubmitterDefaultControlCode()
        {
            Repositories.ApplicationRepository.UpdateSubmitterDefaultControlCode(Application.Subm_SubmCd, Application.Appl_CtrlCd);
        }

        protected void TrimTrailingSpaces()
        {
            Application.Appl_Source_RfrNr = Application.Appl_Source_RfrNr?.Trim();
            Application.Appl_Dbtr_SurNme = Application.Appl_Dbtr_SurNme?.Trim();
            Application.Appl_Dbtr_FrstNme = Application.Appl_Dbtr_FrstNme?.Trim();
            Application.Appl_Dbtr_MddleNme = Application.Appl_Dbtr_MddleNme?.Trim();
            Application.Appl_Dbtr_Entrd_SIN = Application.Appl_Dbtr_Entrd_SIN?.Trim();
            Application.Appl_Dbtr_Parent_SurNme = Application.Appl_Dbtr_Parent_SurNme?.Trim();
            Application.Appl_CommSubm_Text = Application.Appl_CommSubm_Text?.Trim();
        }

        public void MakeUpperCase()
        {
            Application.Appl_EnfSrv_Cd = Application.Appl_EnfSrv_Cd?.ToUpper();
            Application.Appl_CtrlCd = Application.Appl_CtrlCd?.ToUpper();
            Application.Subm_SubmCd = Application.Subm_SubmCd?.ToUpper();
            Application.Subm_Recpt_SubmCd = Application.Subm_Recpt_SubmCd?.ToUpper();
            Application.Appl_CommSubm_Text = Application.Appl_CommSubm_Text?.ToUpper();
            Application.Appl_Group_Batch_Cd = Application.Appl_Group_Batch_Cd?.ToUpper();
            Application.Appl_Source_RfrNr = Application.Appl_Source_RfrNr?.ToUpper();
            Application.Subm_Affdvt_SubmCd = Application.Subm_Affdvt_SubmCd?.ToUpper();
            Application.Appl_Affdvt_DocTypCd = Application.Appl_Affdvt_DocTypCd?.ToUpper();
            Application.Appl_Crdtr_FrstNme = Application.Appl_Crdtr_FrstNme?.ToUpper();
            Application.Appl_Crdtr_MddleNme = Application.Appl_Crdtr_MddleNme?.ToUpper();
            Application.Appl_Crdtr_SurNme = Application.Appl_Crdtr_SurNme?.ToUpper();
            Application.Appl_Dbtr_FrstNme = Application.Appl_Dbtr_FrstNme?.ToUpper();
            Application.Appl_Dbtr_MddleNme = Application.Appl_Dbtr_MddleNme?.ToUpper();
            Application.Appl_Dbtr_SurNme = Application.Appl_Dbtr_SurNme?.ToUpper();
            Application.Appl_Dbtr_Parent_SurNme = Application.Appl_Dbtr_Parent_SurNme?.ToUpper();
            Application.Appl_Dbtr_LngCd = Application.Appl_Dbtr_LngCd?.ToUpper();
            Application.Appl_Dbtr_Gendr_Cd = Application.Appl_Dbtr_Gendr_Cd?.ToUpper();
            Application.Appl_Dbtr_Addr_Ln = Application.Appl_Dbtr_Addr_Ln?.ToUpper();
            Application.Appl_Dbtr_Addr_Ln1 = Application.Appl_Dbtr_Addr_Ln1?.ToUpper();
            Application.Appl_Dbtr_Addr_CityNme = Application.Appl_Dbtr_Addr_CityNme?.ToUpper();
            Application.Appl_Dbtr_Addr_PrvCd = Application.Appl_Dbtr_Addr_PrvCd?.ToUpper();
            Application.Appl_Dbtr_Addr_CtryCd = Application.Appl_Dbtr_Addr_CtryCd?.ToUpper();
            Application.Appl_Dbtr_Addr_PCd = Application.Appl_Dbtr_Addr_PCd?.ToUpper();
            Application.Medium_Cd = Application.Medium_Cd?.ToUpper();
            Application.AppCtgy_Cd = Application.AppCtgy_Cd?.ToUpper();
            Application.AppReas_Cd = Application.AppReas_Cd?.ToUpper();
            Application.ActvSt_Cd = Application.ActvSt_Cd?.ToUpper();
        }

        public string GetSINResultsEventText()
        {
            string result = string.Empty;

            var data = Repositories.SINResultRepository.GetSINResults(Application.Appl_EnfSrv_Cd, Application.Appl_CtrlCd);
            var sinData = data.Items.FirstOrDefault();

            if (sinData != null)
            {
                result = $"{sinData.SVR_DOB_TolCd}{sinData.SVR_GvnNme_TolCd}{sinData.SVR_MddlNme_TolCd}" +
                         $"{sinData.SVR_SurNme_TolCd}{sinData.SVR_MotherNme_TolCd}{sinData.SVR_Gendr_TolCd}";
                result += ((sinData.ValStat_Cd == 0) || (sinData.ValStat_Cd == 10)) ? "Y" : "N";
            }

            return result;
        }

        private static string BuildCommentsChangeReasonText(ApplicationData newAppl, ApplicationData currentAppl)
        {
            if (currentAppl.Appl_CommSubm_Text?.Trim() != newAppl.Appl_CommSubm_Text?.Trim())
            {
                if (currentAppl.Appl_EnfSrv_Cd == "QC01")
                    return "Commentaire";
                else
                    return "Comments";
            }
            else
                return "";
        }

        private static string BuildAddressChangeReasonText(ApplicationData newAppl, ApplicationData currentAppl)
        {
            if ((newAppl.Appl_Dbtr_Addr_Ln?.Trim() != currentAppl.Appl_Dbtr_Addr_Ln?.Trim()) ||
                (newAppl.Appl_Dbtr_Addr_Ln1?.Trim() != currentAppl.Appl_Dbtr_Addr_Ln1?.Trim()) ||
                (newAppl.Appl_Dbtr_Addr_CityNme?.Trim() != currentAppl.Appl_Dbtr_Addr_CityNme?.Trim()) ||
                (newAppl.Appl_Dbtr_Addr_CtryCd?.Trim() != currentAppl.Appl_Dbtr_Addr_CtryCd?.Trim()) ||
                (newAppl.Appl_Dbtr_Addr_PrvCd?.Trim() != currentAppl.Appl_Dbtr_Addr_PrvCd?.Trim()) ||
                (newAppl.Appl_Dbtr_Addr_PCd?.Trim() != currentAppl.Appl_Dbtr_Addr_PCd?.Trim()))
            {
                if (currentAppl.Appl_EnfSrv_Cd == "QC01")
                    return "Adresse du débiteur";
                else
                    return "Debtor Address";
            }
            else
                return "";
        }

        private static string BuildReferenceNumberChangeReasonText(ApplicationData newAppl, ApplicationData currentAppl)
        {
            if (currentAppl.Appl_Source_RfrNr != newAppl.Appl_Source_RfrNr)
            {
                if (currentAppl.Appl_EnfSrv_Cd.Trim() == "QC01")
                    return "Numéro référence de l'autorité provinciale";
                else
                    return "Enforcement Service Reference Number";
            }
            else
                return "";
        }

        public static void AddSystemError(IRepositories repositories, MessageDataList messages, string recipients, string errorMessage)
        {
            messages.AddSystemError(errorMessage);

            string subject = "System Error";
            string message = $"<b>Error: </b>{errorMessage}";
            repositories.NotificationRepository.SendEmail(subject, recipients, message);
        }

    }
}
