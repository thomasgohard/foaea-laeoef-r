using FOAEA3.API.broker;
using FOAEA3.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using FOAEA3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FOAEA3.Areas.Application.Controllers
{
    [Area("Application")]
    public class LicenceDenialController : Controller
    {

        private const string SESSION_LICENCING_ERRORS = "SessionLicencingErrors";

        public IActionResult Create(LicenceDenialApplicationData enteredData = null)
        {
            LicenceDenialApplicationData licenceDenialData;
            if ((enteredData is null) || (enteredData.Appl_EnfSrv_Cd is null))
                licenceDenialData = new LicenceDenialApplicationData()
                {
                    // set default values for current user -- for testing only
                    Appl_EnfSrv_Cd = SessionData.CurrentEnforcementServiceCode,
                    Subm_SubmCd = SessionData.CurrentSubmitter
                };
            else
                licenceDenialData = enteredData;

            string courtValue = GetCourtName(licenceDenialData.Appl_EnfSrv_Cd);

            var submittersAPI = new SubmittersAPI();

            var model = new LicenceDenialDataEntryViewModel
            {
                LicenceDenial = licenceDenialData,
                Declarant = submittersAPI.GetDeclarant(licenceDenialData.Subm_SubmCd),
                CourtValue = courtValue,
                CanEditCore = true,
                CanEditCommentsAndRef = true,
                IsCreate = true
            };

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LicenceDenialDataEntryViewModel data)
        {
            var licenceDenialsAPI = new LicenceDenialsAPI();
            LicenceDenialApplicationData licencingApplication = licenceDenialsAPI.CreateApplication(data.LicenceDenial);

            if (!licencingApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                return Create(data.LicenceDenial);
            }
            else
            {
                var messages = data.LicenceDenial.Messages;
                if (messages?.Count > 0)
                    HttpContext.Session.Set<MessageDataList>(SESSION_LICENCING_ERRORS, messages);

                return RedirectToAction("Edit", new { id = data.LicenceDenial.Appl_EnfSrv_Cd + "-" + data.LicenceDenial.Appl_CtrlCd });
            }

        }

        public IActionResult Edit(string id)
        {
            // extract keys from id
            var applKey = new ApplKey(id);

            // load existing licence denial application
            var licenceDenialsAPI = new LicenceDenialsAPI();
            var licenceDenialApplication = licenceDenialsAPI.GetApplication(applKey.EnfSrv, applKey.CtrlCd);

            DateTime? terminationDate = null;

            if (!string.IsNullOrEmpty(licenceDenialApplication.LicSusp_Appl_CtrlCd))
            {
                // there was an L03 termination application, so get it's created date
                var applicationAPI = new ApplicationsAPI();
                var terminationAppl = applicationAPI.GetApplication(applKey.EnfSrv, applKey.CtrlCd);
                terminationDate = terminationAppl.Appl_Create_Dte;
            }

            /*
             
                      With New BusinessBroker.ApplicationSystem()
                        Dim app As Justice.FOAEA.Common.ApplicationData = .GetApplication(EnforcementServiceCode, Me.lkL03ControlCode.Text)
                        If app.Appl.Rows.Count > 0 Then
                            'Dim test As Date = DateUtils.FormatDate(app.Appl.Rows(0).Item("Appl_Create_Dte"), DateUtils.DateFormats.SQL_DATE)
                            'Dim r As Justice.FOAEA.Common.ApplicationData.ApplRow
                            'r = app.Appl.Rows(0)
                            ltTerminationDate.Text = DateUtils.FormatDate(app.Appl.Rows(0).Item("Appl_Create_Dte"), DateUtils.DateFormats.YYYY_MM_DD)

                        End If
                    End With

             */

            MessageDataList errors = HttpContext.Session.Get<MessageDataList>(SESSION_LICENCING_ERRORS);

            HttpContext.Session.Remove(SESSION_LICENCING_ERRORS);
            if (errors != null)
            {
                licenceDenialApplication.Messages.Merge(errors);
            }

            bool canEditCore;
            bool canEditCommentsAndRef;
            switch (licenceDenialApplication.AppLiSt_Cd)
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

            string courtValue = GetCourtName(licenceDenialApplication.Appl_EnfSrv_Cd);

            var submittersAPI = new SubmittersAPI();

            var model = new LicenceDenialDataEntryViewModel
            {
                LicenceDenial = licenceDenialApplication,
                Declarant = submittersAPI.GetDeclarant(licenceDenialApplication.Subm_SubmCd),
                CourtValue = courtValue,
                CanEditCore = canEditCore,
                CanEditCommentsAndRef = canEditCommentsAndRef,
                IsCreate = false,
                TerminationDate = terminationDate
            };

            // display/edit licence denial application
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(LicenceDenialDataEntryViewModel data)
        {
            var licenceDenialsAPI = new LicenceDenialsAPI();

            // reload these values from original in database so as not to lose those values when we update

            LicenceDenialApplicationData existingLicenceDenial = licenceDenialsAPI.GetApplication(data.LicenceDenial.Appl_EnfSrv_Cd, data.LicenceDenial.Appl_CtrlCd);
            data.LicenceDenial.Subm_SubmCd = existingLicenceDenial.Subm_SubmCd;
            data.LicenceDenial.Subm_Recpt_SubmCd = existingLicenceDenial.Subm_Recpt_SubmCd;
            data.LicenceDenial.Appl_Lgl_Dte = existingLicenceDenial.Appl_Lgl_Dte;
            data.LicenceDenial.Appl_Rcptfrm_Dte = existingLicenceDenial.Appl_Rcptfrm_Dte;
            data.LicenceDenial.Appl_Group_Batch_Cd = existingLicenceDenial.Appl_Group_Batch_Cd;
            data.LicenceDenial.Subm_Affdvt_SubmCd = existingLicenceDenial.Subm_Affdvt_SubmCd;
            data.LicenceDenial.Appl_RecvAffdvt_Dte = existingLicenceDenial.Appl_RecvAffdvt_Dte;
            data.LicenceDenial.Appl_JusticeNr = existingLicenceDenial.Appl_JusticeNr;
            data.LicenceDenial.Appl_Crdtr_FrstNme = existingLicenceDenial.Appl_Crdtr_FrstNme;
            data.LicenceDenial.Appl_Crdtr_MddleNme = existingLicenceDenial.Appl_Crdtr_MddleNme;
            data.LicenceDenial.Appl_Crdtr_SurNme = existingLicenceDenial.Appl_Crdtr_SurNme;
            data.LicenceDenial.Appl_Crdtr_Brth_Dte = existingLicenceDenial.Appl_Crdtr_Brth_Dte;
            data.LicenceDenial.Appl_Dbtr_LngCd = existingLicenceDenial.Appl_Dbtr_LngCd;
            data.LicenceDenial.Appl_Dbtr_Cnfrmd_SIN = existingLicenceDenial.Appl_Dbtr_Cnfrmd_SIN;
            data.LicenceDenial.Appl_Dbtr_RtrndBySrc_SIN = existingLicenceDenial.Appl_Dbtr_RtrndBySrc_SIN;
            data.LicenceDenial.Appl_Dbtr_Addr_CityNme = existingLicenceDenial.Appl_Dbtr_Addr_CityNme;
            data.LicenceDenial.Appl_Dbtr_Addr_CtryCd = existingLicenceDenial.Appl_Dbtr_Addr_CtryCd;
            data.LicenceDenial.Appl_Dbtr_Addr_Ln = existingLicenceDenial.Appl_Dbtr_Addr_Ln;
            data.LicenceDenial.Appl_Dbtr_Addr_Ln1 = existingLicenceDenial.Appl_Dbtr_Addr_Ln1;
            data.LicenceDenial.Appl_Dbtr_Addr_PCd = existingLicenceDenial.Appl_Dbtr_Addr_PCd;
            data.LicenceDenial.Appl_Dbtr_Addr_PrvCd = existingLicenceDenial.Appl_Dbtr_Addr_PrvCd;
            data.LicenceDenial.Medium_Cd = existingLicenceDenial.Medium_Cd;
            data.LicenceDenial.Appl_Reactv_Dte = existingLicenceDenial.Appl_Reactv_Dte;
            data.LicenceDenial.Appl_SIN_Cnfrmd_Ind = existingLicenceDenial.Appl_SIN_Cnfrmd_Ind;
            data.LicenceDenial.AppCtgy_Cd = existingLicenceDenial.AppCtgy_Cd;
            data.LicenceDenial.Appl_Create_Dte = existingLicenceDenial.Appl_Create_Dte;
            data.LicenceDenial.Appl_Create_Usr = existingLicenceDenial.Appl_Create_Usr;
            data.LicenceDenial.Appl_LastUpdate_Dte = existingLicenceDenial.Appl_LastUpdate_Dte;
            data.LicenceDenial.Appl_LastUpdate_Usr = existingLicenceDenial.Appl_LastUpdate_Usr;
            data.LicenceDenial.ActvSt_Cd = existingLicenceDenial.ActvSt_Cd;
            data.LicenceDenial.AppLiSt_Cd = existingLicenceDenial.AppLiSt_Cd;
            data.LicenceDenial.Appl_WFID = existingLicenceDenial.Appl_WFID;

            if (!data.CanEditCore)
            {
                data.LicenceDenial.Appl_Dbtr_SurNme = existingLicenceDenial.Appl_Dbtr_SurNme;
                data.LicenceDenial.Appl_Dbtr_FrstNme = existingLicenceDenial.Appl_Dbtr_FrstNme;
                data.LicenceDenial.Appl_Dbtr_MddleNme = existingLicenceDenial.Appl_Dbtr_MddleNme;
                data.LicenceDenial.Appl_Dbtr_Brth_Dte = existingLicenceDenial.Appl_Dbtr_Brth_Dte;
                data.LicenceDenial.Appl_Dbtr_Entrd_SIN = existingLicenceDenial.Appl_Dbtr_Entrd_SIN;
                data.LicenceDenial.Appl_Dbtr_Parent_SurNme = existingLicenceDenial.Appl_Dbtr_Parent_SurNme;
                data.LicenceDenial.AppReas_Cd = existingLicenceDenial.AppReas_Cd;
            }

            if (!data.CanEditCommentsAndRef)
            {
                data.LicenceDenial.Appl_CommSubm_Text = existingLicenceDenial.Appl_CommSubm_Text;
                data.LicenceDenial.Appl_Source_RfrNr = existingLicenceDenial.Appl_Source_RfrNr;
            }

            LicenceDenialApplicationData updatedData = licenceDenialsAPI.UpdateApplication(data.LicenceDenial);

            var messages = updatedData.Messages;
            if (messages?.Count > 0)
                HttpContext.Session.Set<MessageDataList>(SESSION_LICENCING_ERRORS, messages);

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

        public IActionResult TraceResults(string id)
        {
            var applAPI = new LicenceDenialsAPI();

            var applKey = new ApplKey(id);

            var model = applAPI.GetTraceResults(applKey.EnfSrv, applKey.CtrlCd);

            return View(model);
        }
    }
}