using FOAEA3.API.broker;
using FOAEA3.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using FOAEA3.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FOAEA3.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class SubmitterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(string provCD, string offCd, string srvCd)
        {
            SubmitterData submitterData = new SubmitterData()
            {
                EnfOff_City_LocCd = offCd ?? string.Empty,
                EnfSrv_Cd = srvCd ?? string.Empty,
            };

            var model = new SubmitterDataEntryViewModel()
            {
                Submitter = submitterData,
                Province = provCD
            };

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            SubmitterData submitterData = ExtractDataFromCollection(collection);
            bool readOnlyAccess = FormHelper.GetValueFromCollection(collection, "ReadOnly_Ind", ((submitterData.Subm_Class == "RO") || (submitterData.Subm_Class == "R1")));

            var submittersAPI = new SubmittersAPI();
            submitterData = submittersAPI.CreateSubmitter(submitterData, collection["suffixCode"], readOnlyAccess);

            submitterData.Subm_Create_Usr = SessionData.CurrentSubmitter;

            var model = new SubmitterDataEntryViewModel
            {
                Submitter = submitterData,
                Province = submitterData.EnfSrv_Cd.Substring(0, 2)
            };

            if (submitterData.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                return View("Create", model); // not saved due to errors, stay on this page
            }
            else
            {
                model.Submitter.Messages.AddInformation(Resources.LanguageResource.DATA_UPDATE_SUCCESS);
                model.Submitter.Messages.AddInformation(submitterData.Subm_SubmCd);

                return View("Edit", model); // successfully created, so go to edit page for this submitter
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNewFromCurrent(IFormCollection collection)
        {
            SubmitterData model = ExtractDataFromCollection(collection);

            string provCd = collection["selectProvinces"];
            if (string.IsNullOrEmpty(provCd)) // for disabled select box, then a hidden input contains the province code
                provCd = collection["provinceCode"];

            string offCd = model.EnfOff_City_LocCd;
            string srvCd = model.EnfSrv_Cd;

            return Create(provCd, offCd, srvCd);
        }

        public IActionResult Edit(string submCd, string provCd)
        {
            var submittersAPI = new SubmittersAPI();

            SubmitterData submitterData = submittersAPI.GetSubmitter(submCd);

            var model = new SubmitterDataEntryViewModel
            {
                Submitter = submitterData,
                Province = provCd
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IFormCollection collection)
        {
            SubmitterData submitterData = ExtractDataFromCollection(collection);

            var submittersAPI = new SubmittersAPI();
            submitterData = submittersAPI.UpdateSubmitter(submitterData);

            if (!submitterData.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                submitterData.Messages.AddInformation("The database was updated successfully.");
                submitterData.Messages.AddInformation(submitterData.Subm_SubmCd);
            }

            var model = new SubmitterDataEntryViewModel
            {
                Submitter = submitterData,
                Province = submitterData.EnfSrv_Cd?.Substring(0, 2)
            };

            return View(model);
        }

        public JsonResult GetSubmittersForProvince(string provCd, string enfOffKey, bool onlyActive)
        {
            var submittersAPI = new SubmittersAPI();

            List<SubmitterData> data;

            if ((enfOffKey == "-1") || (enfOffKey == "** All **"))
            {
                data = submittersAPI.GetSubmittersForProvince(provCd, onlyActive);
            }
            else
            {
                string[] tokens = enfOffKey.Split("-");
                string enfSrvCode = tokens[0];
                string enfOffCode = tokens[1];

                data = submittersAPI.GetSubmittersForProvinceAndOffice(provCd, enfOffCode, enfSrvCode, onlyActive);
            }
            
           // var sortedData = data.OrderBy(m => m.ActvSt_Cd + m.Subm_SurNme);

            return Json(data);
        }

        public JsonResult GetProvinces()
        {
            try
            {
                var provinces = SubmitterHelper.GetProvinces(LanguageHelper.IsEnglish());

                return Json(from province in provinces
                            select new
                            {
                                province.prvCd,
                                province.prvTxt
                            });

            }
            catch (Exception e)
            {
                return new JsonResult(e.Message)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

        }

        public JsonResult GetEnfOffForProvince(string provCd)
        {
            var enfOffices = SubmitterHelper.GetEnfOffForProvince(provCd);

            return Json(from enfOff in enfOffices
                        select new
                        {
                            enfSrv_Cd = enfOff.enfSrvCd,
                            enfOff_City_LocCd = enfOff.cityLoc,
                            enfOff_Addr_CityNme = enfOff.cityName,
                            enfOff_Nme = enfOff.offName,
                            actvSt_Cd = enfOff.activeCode
                        });
        }

        public JsonResult GetEnfOffForService(string enfServiceCd)
        {
            var enfOffices = SubmitterHelper.GetEnfOffForService(enfServiceCd);

            return Json(from enfOff in enfOffices
                        select new
                        {
                            enfOff.enfSrv_Cd,
                            enfOff.enfOff_City_LocCd
                        });
        }

        public JsonResult GetEnfOffInfo(string enfOffCd, string enfSrvCd)
        {
            var (enfOff_AbbrCd, enfOff_Addr_CityNme) = SubmitterHelper.GetEnfOffInfo(enfOffCd, enfSrvCd);

            return Json(new { enfOff_AbbrCd, enfOff_Addr_CityNme });
        }

        public JsonResult GetEnfServicesForProvince(string provCd)
        {
            var enfServices = SubmitterHelper.GetEnfServicesForProvince(provCd, LanguageHelper.IsEnglish());

            return Json(from enfService in enfServices
                        select new
                        {
                            enfService.enfSrv_Cd,
                            enfService.enfSrv_Nme
                        });
        }

        public JsonResult GetSubjectsForSubmitter(string submCd)
        {
            var subjects = SubmitterHelper.GetSubjectsForSubmitter(submCd);

            return Json(from subject in subjects
                        select new
                        {
                            subject.subjectName,
                            subject.allowedAccess
                        });
        }

        private SubmitterData ExtractDataFromCollection(IFormCollection collection)
        {

            string submitterCode = collection["Subm_SubmCd"];

            var submittersAPI = new SubmittersAPI();

            SubmitterData data = submittersAPI.GetSubmitter(submitterCode);

            data.Subm_SubmCd = collection["Subm_SubmCd"];
            data.ActvSt_Cd = collection["ActvSt_Cd"];
            data.Subm_Title = collection["Subm_Title"];
            data.Subm_FrstNme = collection["Subm_FrstNme"];
            data.Subm_MddleNme = collection["Subm_MddleNme"];
            data.Subm_SurNme = collection["Subm_SurNme"];
            data.Subm_Assg_Email = collection["Subm_Assg_Email"];

            data.Subm_Tel_AreaC = collection["Subm_Tel_AreaC"];
            data.Subm_TelNr = collection["Subm_TelNr"];
            data.Subm_TelEx = collection["Subm_TelEx"];
            data.Subm_Fax_AreaC = collection["Subm_Fax_AreaC"];
            data.Subm_FaxNr = collection["Subm_FaxNr"];

            data.Subm_Trcn_AccsPrvCd = FormHelper.GetValueFromCollection(collection, "Subm_Trcn_AccsPrvCd", data.Subm_Trcn_AccsPrvCd);
            data.Subm_Intrc_AccsPrvCd = FormHelper.GetValueFromCollection(collection, "Subm_Intrc_AccsPrvCd", data.Subm_Intrc_AccsPrvCd);
            data.Subm_Lic_AccsPrvCd = FormHelper.GetValueFromCollection(collection, "Subm_Lic_AccsPrvCd", data.Subm_Lic_AccsPrvCd);
            data.Subm_LglSgnAuth_Ind = FormHelper.GetValueFromCollection(collection, "Subm_LglSgnAuth_Ind", data.Subm_LglSgnAuth_Ind);
            data.Subm_Fin_Ind = FormHelper.GetValueFromCollection(collection, "Subm_Fin_Ind", data.Subm_Fin_Ind);

            data.Lng_Cd = collection.Keys.Contains("Lng_Cd") ? (string)collection["Lng_Cd"] : data.Lng_Cd;
            data.Subm_Comments = collection["Subm_Comments"];

            data.EnfSrv_Cd = collection.Keys.Contains("selectEnfServices") ? (string)collection["selectEnfServices"] : data.EnfSrv_Cd;
            data.EnfOff_City_LocCd = collection.Keys.Contains("selectEnfOff") ? (string)collection["selectEnfOff"] : data.EnfOff_City_LocCd;

            // only one option should be true

            bool originalSubm_EnfSrvAuth_Ind = data.Subm_EnfSrvAuth_Ind;
            bool originalSubm_EnfOffAuth_Ind = data.Subm_EnfOffAuth_Ind;
            bool originalSubm_SysMgr_Ind = data.Subm_SysMgr_Ind;
            bool originalSubm_AppMgr_Ind = data.Subm_AppMgr_Ind;
            bool originalSubm_CourtUsr_Ind = data.Subm_CourtUsr_Ind;

            data.Subm_EnfSrvAuth_Ind = false;
            data.Subm_EnfOffAuth_Ind = false;
            data.Subm_SysMgr_Ind = false;
            data.Subm_AppMgr_Ind = false;
            data.Subm_CourtUsr_Ind = false;

            string valOptionRole = collection["optRole"];

            switch (valOptionRole)
            {
                case "Subm_EnfSrvAuth_Ind":
                    data.Subm_EnfSrvAuth_Ind = true;
                    break;
                case "Subm_EnfOffAuth_Ind":
                    data.Subm_EnfOffAuth_Ind = true;
                    break;
                case "Subm_SysMgr_Ind":
                    data.Subm_SysMgr_Ind = true;
                    break;
                case "Subm_AppMgr_Ind":
                    data.Subm_AppMgr_Ind = true;
                    break;
                case "Subm_CourtUsr_Ind":
                    data.Subm_CourtUsr_Ind = true;
                    break;
                case "LimitedFinancialUser_Ind":
                    break;
                default:
                    data.Subm_EnfSrvAuth_Ind = originalSubm_EnfSrvAuth_Ind;
                    data.Subm_EnfOffAuth_Ind = originalSubm_EnfOffAuth_Ind;
                    data.Subm_SysMgr_Ind = originalSubm_SysMgr_Ind;
                    data.Subm_AppMgr_Ind = originalSubm_AppMgr_Ind;
                    data.Subm_CourtUsr_Ind = originalSubm_CourtUsr_Ind;
                    break;
            }

            _ = FormHelper.GetValueFromCollection(collection, "ReadOnly_Ind", ((data.Subm_Class == "RO") || (data.Subm_Class == "R1")));

            return data;
        }

    }
}