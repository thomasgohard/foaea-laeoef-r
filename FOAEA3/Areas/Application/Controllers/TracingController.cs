using FOAEA3.API.broker;
using FOAEA3.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using FOAEA3.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace FOAEA3.Areas.Application.Controllers
{
    [Area("Application")]
    public class TracingController : Controller
    {
        private const string SESSION_TRACING_ERRORS = "SessionTracingErrors";

        public IActionResult Create(TracingApplicationData enteredData = null)
        {
            TracingApplicationData tracingData;
            if ((enteredData is null) || (enteredData.Appl_EnfSrv_Cd is null))
                tracingData = new TracingApplicationData()
                {
                    // set default values for current user -- for testing only
                    Appl_EnfSrv_Cd = SessionData.CurrentEnforcementServiceCode,
                    Subm_SubmCd = SessionData.CurrentSubmitter,
                    Subm_Recpt_SubmCd = SessionData.CurrentSubmitter,
                    Medium_Cd = "ONL"
                };
            else
                tracingData = enteredData;

            string courtValue = GetCourtName(tracingData.Appl_EnfSrv_Cd);

            var submittersAPI = new SubmittersAPI();

            var model = new TracingDataEntryViewModel
            {
                Tracing = tracingData,
                Declarant = submittersAPI.GetDeclarant(tracingData.Subm_SubmCd),
                CourtValue = courtValue,
                CanEditCore = true,
                CanEditCommentsAndRef = true,
                IsCreate = true
            };

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TracingDataEntryViewModel data)
        {
            var tracingsAPI = new TracingsAPI();
            var tracingApplication = tracingsAPI.CreateApplication(data.Tracing);

            if (!tracingApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                var messages = tracingApplication.Messages;

                messages.AddURL(description: "Tracing Application T01", url: Request.Path);

                HttpContext.Session.Set<MessageDataList>(SESSION_TRACING_ERRORS, messages);

                return RedirectToAction("Edit", new { id = tracingApplication.Appl_EnfSrv_Cd + "-" + tracingApplication.Appl_CtrlCd });
            }
            else
            {
                var messages = tracingApplication.Messages;
                if (messages?.Count > 0)
                    HttpContext.Session.Set<MessageDataList>(SESSION_TRACING_ERRORS, messages);

                return Create(tracingApplication);
            }

        }

        public IActionResult AffidavitIndex()
        {
            var tracingsAPI = new TracingsAPI();
            var tracingApplicationsWaitingForAffidavits = tracingsAPI.GetApplicationsWaitingForAffidavit();

            return View(tracingApplicationsWaitingForAffidavits);
        }

        public IActionResult AffidavitCreate(string id)
        {
            var applKey = new ApplKey(id);

            var tracingsAPI = new TracingsAPI();
            var tracingApplication = tracingsAPI.GetApplication(applKey.EnfSrv, applKey.CtrlCd);

            var submittersAPI = new SubmittersAPI();
            var submitterProfileAPI = new SubmitterProfilesAPI();
            var familyProvisionAPI = new FamilyProvisionsAPI();
            var infoBanksAPI = new InfoBanksAPI();

            var submProfile = submitterProfileAPI.GetSubmitterProfile(SessionData.CurrentSubmitter);

            var tracingAffidavitDataEntryViewModel = new TracingAffidavitDataEntryViewModel
            {
                Province = GetProvinceName(tracingApplication.Appl_EnfSrv_Cd),
                Creditor = $"{tracingApplication.Appl_Crdtr_FrstNme?.Trim()} {tracingApplication.Appl_Crdtr_MddleNme?.Trim()} {tracingApplication.Appl_Crdtr_SurNme?.Trim()}",
                Debtor = $"{tracingApplication.Appl_Dbtr_FrstNme?.Trim()} {tracingApplication.Appl_Dbtr_MddleNme?.Trim()} {tracingApplication.Appl_Dbtr_SurNme?.Trim()}",
                Declarant = submittersAPI.GetDeclarant(tracingApplication.Subm_SubmCd),
                Tracing = tracingApplication,
                CurrentDate = DateTime.Now.ToString(DateTimeHelper.YYYY_MM_DD),
                FamilyProvisions = new SelectList(familyProvisionAPI.GetFamilyProvisions(), nameof(FamilyProvisionData.FamPro_Cd), nameof(FamilyProvisionData.Description)),
                InfoBanks = new SelectList(infoBanksAPI.GetInfoActiveBanksForProvince(submProfile.Prv_Cd), nameof(InfoBankData.InfoBank_Cd), nameof(InfoBankData.Description)),
                Commissionners = new SelectList(submittersAPI.GetCommissionersForLocation(submProfile.EnfOff_City_LocCd), nameof(CommissionerData.Subm_SubmCd), nameof(CommissionerData.Subm_Name))
            };

            return View(tracingAffidavitDataEntryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AffidavitCreate(TracingAffidavitDataEntryViewModel data)
        {
            // save affidavit

            (string firstName, string middleName, string lastName) = ExtractCreditorName(data.Creditor);

            var tracingsAPI = new TracingsAPI();
            var tracingApplication = data.Tracing;

            tracingApplication.Appl_Crdtr_FrstNme = firstName;
            tracingApplication.Appl_Crdtr_MddleNme = middleName;
            tracingApplication.Appl_Crdtr_SurNme = lastName;

            tracingsAPI.UpdateApplication(tracingApplication);

            return Edit($"{data.Tracing.Appl_EnfSrv_Cd}-{data.Tracing.Appl_CtrlCd}");
        }

        private static (string firstName, string middleName, string lastName) ExtractCreditorName(string creditorInfo)
        {
            string firstName = string.Empty;
            string middleName = string.Empty;
            string lastName = string.Empty;

            var creditorData = creditorInfo.Trim().ShrinkInternalSpaces().Split(" ");

            if (creditorData is { Length: > 0 })
            {
                firstName = creditorData[0];
            }

            if (creditorData is { Length: > 1 })
            {
                if (creditorData.Length == 2)
                    lastName = creditorData[1];
                else
                {
                    lastName = creditorData[^1];
                    middleName = string.Join(" ", creditorData[1..^1]);
                }
            }

            return (firstName, middleName, lastName);
        }

        public IActionResult Edit(string id)
        {
            // extract keys from id
            var applKey = new ApplKey(id);

            // load existing tracing application
            var tracingsAPI = new TracingsAPI();
            var tracingApplication = tracingsAPI.GetApplication(applKey.EnfSrv, applKey.CtrlCd);

            var errors = HttpContext.Session.Get<MessageDataList>(SESSION_TRACING_ERRORS);

            HttpContext.Session.Remove(SESSION_TRACING_ERRORS);
            if (errors != null)
            {
                tracingApplication.Messages.Merge(errors);
            }

            bool canEditCore;
            bool canEditCommentsAndRef;
            switch (tracingApplication.AppLiSt_Cd)
            {
                case ApplicationState.SIN_CONFIRMATION_PENDING_3:
                case ApplicationState.APPLICATION_REJECTED_9:
                case ApplicationState.APPLICATION_REINSTATED_11:
                case ApplicationState.MANUALLY_TERMINATED_14:
                    canEditCore = false;
                    canEditCommentsAndRef = false;
                    break;

                case ApplicationState.PENDING_ACCEPTANCE_SWEARING_6:
                case ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7:
                case ApplicationState.APPLICATION_ACCEPTED_10:
                case ApplicationState.PARTIALLY_SERVICED_12:
                case ApplicationState.EXPIRED_15:
                    canEditCore = false;
                    canEditCommentsAndRef = true;
                    break;

                default:
                    canEditCore = true;
                    canEditCommentsAndRef = true;
                    break;
            }

            string courtValue = GetCourtName(tracingApplication.Appl_EnfSrv_Cd);

            var submittersAPI = new SubmittersAPI();

            var model = new TracingDataEntryViewModel
            {
                Tracing = tracingApplication,
                Declarant = submittersAPI.GetDeclarant(tracingApplication.Subm_SubmCd),
                CourtValue = courtValue,
                CanEditCore = canEditCore,
                CanEditCommentsAndRef = canEditCommentsAndRef,
                IsCreate = false
            };

            // display/edit tracing application
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TracingDataEntryViewModel data)
        {
            var tracingsAPI = new TracingsAPI();

            // reload these values from original in database so as not to lose those values when we update

            TracingApplicationData existingTracing = tracingsAPI.GetApplication(data.Tracing.Appl_EnfSrv_Cd, data.Tracing.Appl_CtrlCd);
            data.Tracing.Subm_SubmCd = existingTracing.Subm_SubmCd;
            data.Tracing.Subm_Recpt_SubmCd = existingTracing.Subm_Recpt_SubmCd;
            data.Tracing.Appl_Lgl_Dte = existingTracing.Appl_Lgl_Dte;
            data.Tracing.Appl_Rcptfrm_Dte = existingTracing.Appl_Rcptfrm_Dte;
            data.Tracing.Appl_Group_Batch_Cd = existingTracing.Appl_Group_Batch_Cd;
            data.Tracing.Subm_Affdvt_SubmCd = existingTracing.Subm_Affdvt_SubmCd;
            data.Tracing.Appl_RecvAffdvt_Dte = existingTracing.Appl_RecvAffdvt_Dte;
            data.Tracing.Appl_JusticeNr = existingTracing.Appl_JusticeNr;
            data.Tracing.Appl_Crdtr_FrstNme = existingTracing.Appl_Crdtr_FrstNme;
            data.Tracing.Appl_Crdtr_MddleNme = existingTracing.Appl_Crdtr_MddleNme;
            data.Tracing.Appl_Crdtr_SurNme = existingTracing.Appl_Crdtr_SurNme;
            data.Tracing.Appl_Crdtr_Brth_Dte = existingTracing.Appl_Crdtr_Brth_Dte;
            data.Tracing.Appl_Dbtr_LngCd = existingTracing.Appl_Dbtr_LngCd;
            data.Tracing.Appl_Dbtr_Cnfrmd_SIN = existingTracing.Appl_Dbtr_Cnfrmd_SIN;
            data.Tracing.Appl_Dbtr_RtrndBySrc_SIN = existingTracing.Appl_Dbtr_RtrndBySrc_SIN;
            data.Tracing.Appl_Dbtr_Addr_CityNme = existingTracing.Appl_Dbtr_Addr_CityNme;
            data.Tracing.Appl_Dbtr_Addr_CtryCd = existingTracing.Appl_Dbtr_Addr_CtryCd;
            data.Tracing.Appl_Dbtr_Addr_Ln = existingTracing.Appl_Dbtr_Addr_Ln;
            data.Tracing.Appl_Dbtr_Addr_Ln1 = existingTracing.Appl_Dbtr_Addr_Ln1;
            data.Tracing.Appl_Dbtr_Addr_PCd = existingTracing.Appl_Dbtr_Addr_PCd;
            data.Tracing.Appl_Dbtr_Addr_PrvCd = existingTracing.Appl_Dbtr_Addr_PrvCd;
            data.Tracing.Medium_Cd = existingTracing.Medium_Cd;
            data.Tracing.Appl_Reactv_Dte = existingTracing.Appl_Reactv_Dte;
            data.Tracing.Appl_SIN_Cnfrmd_Ind = existingTracing.Appl_SIN_Cnfrmd_Ind;
            data.Tracing.AppCtgy_Cd = existingTracing.AppCtgy_Cd;
            data.Tracing.Appl_Create_Dte = existingTracing.Appl_Create_Dte;
            data.Tracing.Appl_Create_Usr = existingTracing.Appl_Create_Usr;
            data.Tracing.Appl_LastUpdate_Dte = existingTracing.Appl_LastUpdate_Dte;
            data.Tracing.Appl_LastUpdate_Usr = existingTracing.Appl_LastUpdate_Usr;
            data.Tracing.ActvSt_Cd = existingTracing.ActvSt_Cd;
            data.Tracing.AppLiSt_Cd = existingTracing.AppLiSt_Cd;
            data.Tracing.Appl_WFID = existingTracing.Appl_WFID;

            if (!data.CanEditCore)
            {
                data.Tracing.Appl_Dbtr_SurNme = existingTracing.Appl_Dbtr_SurNme;
                data.Tracing.Appl_Dbtr_FrstNme = existingTracing.Appl_Dbtr_FrstNme;
                data.Tracing.Appl_Dbtr_MddleNme = existingTracing.Appl_Dbtr_MddleNme;
                data.Tracing.Appl_Dbtr_Brth_Dte = existingTracing.Appl_Dbtr_Brth_Dte;
                data.Tracing.Appl_Dbtr_Entrd_SIN = existingTracing.Appl_Dbtr_Entrd_SIN;
                data.Tracing.Appl_Dbtr_Parent_SurNme = existingTracing.Appl_Dbtr_Parent_SurNme;
                data.Tracing.AppReas_Cd = existingTracing.AppReas_Cd;
            }

            if (!data.CanEditCommentsAndRef)
            {
                data.Tracing.Appl_CommSubm_Text = existingTracing.Appl_CommSubm_Text;
                data.Tracing.Appl_Source_RfrNr = existingTracing.Appl_Source_RfrNr;
            }

            var updatedData = tracingsAPI.UpdateApplication(data.Tracing);

            var messages = updatedData.Messages;
            if (messages?.Count > 0)
                HttpContext.Session.Set<MessageDataList>(SESSION_TRACING_ERRORS, messages);

            return RedirectToAction("Edit", new { id = updatedData.Appl_EnfSrv_Cd + "-" + updatedData.Appl_CtrlCd });

        }

        private static string GetCourtName(string enfServiceCode)
        {
            string courtValue;
            if (enfServiceCode != null)
            {
                var enfSrvAPI = new EnforcementsServiceAPI();
                var enfSrvData = enfSrvAPI.GetEnforcementService(enfServiceCode);

                courtValue = LanguageHelper.IsEnglish() ? enfSrvData.EnfSrv_Nme : enfSrvData.EnfSrv_Nme_F;
            }
            else
                courtValue = string.Empty;
            return courtValue;
        }

        private static string GetProvinceName(string enfServiceCode)
        {
            string province;

            if (enfServiceCode is not null)
            {
                var enfSrvAPI = new EnforcementsServiceAPI();
                var enfSrvData = enfSrvAPI.GetEnforcementService(enfServiceCode);

                var provAPI = new ProvincesAPI();
                var provinces = provAPI.GetProvinces();

                province = provinces.First(m => m.PrvCd == enfSrvData.EnfSrv_Addr_PrvCd).Description;
            }
            else
                province = string.Empty;

            return province;
        }

        public IActionResult TraceResults(string id)
        {
            var applAPI = new TracingsAPI();

            var applKey = new ApplKey(id);

            var model = applAPI.GetTraceResults(applKey.EnfSrv, applKey.CtrlCd);

            return View(model);
        }
    }
}