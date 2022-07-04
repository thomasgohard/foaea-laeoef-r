using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Exceptions;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Structs;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FOAEA3.Business.Areas.Application
{
    internal class ApplicationValidation
    {
        protected IRepositories Repositories { get; }
        private ApplicationData Application { get; }

        protected readonly CustomConfig config;

        public ApplicationEventManager EventManager { get; set; }

        public bool IsSystemGeneratedControlCode { get; set; }

        public ApplicationValidation(ApplicationData application, ApplicationEventManager eventManager,
                                     IRepositories repositories, CustomConfig config)
        {
            this.config = config;
            Application = application;
            Repositories = repositories;
            EventManager = eventManager;
        }

        public ApplicationValidation(ApplicationData application, IRepositories repositories, CustomConfig config)
        {
            this.config = config;
            Application = application;
            Repositories = repositories;
        }

        public virtual void VerifyPresets()
        {
            if (!String.IsNullOrEmpty(Application.Subm_Affdvt_SubmCd))
                EventManager.AddEvent(EventCode.C50509_CANNOT_PRESIGN_WITH_A_LEGAL_SIGNATURE);

            if (Application.Appl_RecvAffdvt_Dte.HasValue)
                EventManager.AddEvent(EventCode.C50510_CANNOT_PRERECEIVE_A_LEGAL_DOCUMENT);

            if (!String.IsNullOrEmpty(Application.Appl_JusticeNr))
                EventManager.AddEvent(EventCode.C50512_CANNOT_PREASSIGN_OR_CHANGE_A_JUSTICE_NUMBER);
        }

        public virtual void ValidateControlCode()
        {
            bool isValid;
            bool isUpdate = false;
            string errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(Application.Appl_CtrlCd))
            {
                var existingApp = new ApplicationManager(new ApplicationData(), Repositories, config);
                if (existingApp.LoadApplication(Application.Appl_EnfSrv_Cd, Application.Appl_CtrlCd))
                {
                    // update
                    isUpdate = true;
                }
            }
            if (!isUpdate) // create
            {
                string ctrlCd = Application.Appl_CtrlCd;

                isValid = true;

                if (ctrlCd?.Length > 0)
                {
                    if ((!IsSystemGeneratedControlCode) && ("defghijksvDEFGHIJKSV".Contains(ctrlCd.Substring(0, 1))))
                    {
                        isValid = false;
                        errorMessage = Resources.ErrorResource.INVALID_CONTROL_CODE;
                    }
                    else if (ctrlCd.Length > 6)
                    {
                        isValid = false;
                        errorMessage = Resources.ErrorResource.MAX_CONTROL_CODE_LENGTH;
                    }
                    else if (!ctrlCd.IsAlphanumeric())
                    {
                        isValid = false;
                        errorMessage = Resources.ErrorResource.REQUIRED_ALPHANUMERIC_CONTROL_CODE;
                    }
                    else
                        isValid = true;
                }
            }
            else
                isValid = true;

            if (!isValid)
            {
                EventManager.AddEvent(EventCode.C50503_INVALID_CONTROL_CODE);
                Application.Messages.AddError(errorMessage);
            }
        }

        public virtual bool IsValidComment()
        {
            if (Application.Appl_CommSubm_Text?.Length > 500)
            {
                return false;
            }
            else
                return true;
        }

        public virtual bool IsValidDebtorAge()
        {
            var debtor = new PersonData
            {
                Birthdate = Application.Appl_Dbtr_Brth_Dte ?? DateTime.Now
            };

            return (debtor.GetAge() >= 15);
        }

        public virtual void ValidateDebtorSurname()
        {
            bool isValid;
            string errorMessage = string.Empty;

            string debtorSurname = Application.Appl_Dbtr_SurNme;
            if (string.IsNullOrEmpty(debtorSurname))
            {
                isValid = false;
                errorMessage = Resources.ErrorResource.MISSING_DEBTOR_SURNAME;
            }
            else
            {
                isValid = true;

                if (debtorSurname.Trim().ToUpper() == "XXXX")
                {
                    isValid = false;
                    errorMessage = Resources.ErrorResource.DEBTOR_SURNAME_CANNOT_BE_XXXX;
                }

                if (debtorSurname.Length < 3)
                {
                    isValid = false;
                    errorMessage = Resources.ErrorResource.DEBTOR_SURNAME_LENGTH_3;
                }
                else if (debtorSurname.IndexOf(" ", 0, 3) != -1)
                {
                    isValid = false;
                    errorMessage = Resources.ErrorResource.DEBTOR_SURNAME_CANNOT_CONTAIN_SPACE;
                }

                if (!Regex.IsMatch(debtorSurname, @"^[a-zA-Z\-'. ]*$")) // debtor surname must be alpha but can contain "-" and "" 
                {
                    isValid = false;
                    errorMessage = Resources.ErrorResource.INVALID_DEBTOR_SURNAME;
                }

            }

            if (!isValid)
                Application.Messages.AddError(errorMessage);

        }

        public virtual bool IsValidMandatoryData()
        {
            bool isSuccess = true;

            isSuccess = IsValidMandatory(isSuccess, Application.Subm_SubmCd, Resources.ErrorResource.MISSING_SUBMITTER);
            isSuccess = IsValidMandatory(isSuccess, Application.Appl_CtrlCd, Resources.ErrorResource.MISSING_CONTROL_CODE);
            isSuccess = IsValidMandatory(isSuccess, Application.Appl_EnfSrv_Cd, Resources.ErrorResource.MISSING_ENFSRV_CODE);
            isSuccess = IsValidMandatory(isSuccess, Application.Appl_Dbtr_FrstNme, Resources.ErrorResource.MISSING_DEBTOR_FIRST_NAME);
            isSuccess = IsValidMandatory(isSuccess, Application.Appl_Dbtr_SurNme, Resources.ErrorResource.MISSING_DEBTOR_SURNAME);
            isSuccess = IsValidMandatory(isSuccess, Application.Subm_Recpt_SubmCd, Resources.ErrorResource.MISSING_RECIPIENT_SUBMITTER);
            isSuccess = IsValidMandatory(isSuccess, Application.Medium_Cd, Resources.ErrorResource.MISSING_MEDIUM_CODE);
            isSuccess = IsValidMandatory(isSuccess, Application.Appl_Dbtr_Gendr_Cd, Resources.ErrorResource.MISSING_GENDER);
            isSuccess = IsValidMandatory(isSuccess, Application.AppCtgy_Cd, Resources.ErrorResource.MISSING_CATEGORY_CODE);
            isSuccess = IsValidMandatory(isSuccess, Application.Appl_Dbtr_Brth_Dte, Resources.ErrorResource.MISSING_DEBTOR_DOB);

            if ((!string.IsNullOrEmpty(Application.Medium_Cd)) && (Application.Medium_Cd.ToUpper() == "FTP"))
                isSuccess = IsValidMandatory(isSuccess, Application.Appl_Group_Batch_Cd, Resources.ErrorResource.MISSING_GROUP_BATCH_CODE);

            return isSuccess;
        }

        protected bool IsValidMandatory(bool isValid, string value, string error)
        {
            if (string.IsNullOrEmpty(value))
            {
                Application.Messages.AddError(error);
                return false;
            }

            return isValid;
        }

        protected bool IsValidMandatory(bool isValid, DateTime? value, string error)
        {
            if (value == null)
            {
                Application.Messages.AddError(error);
                return false;
            }

            return isValid;
        }

        public virtual bool IsValidPostalCode(out string reasonText)
        {
            // --- error messages for postal code: are stored in a DB table - PostalCodeErrorMessages ---
            // 1-Postal code does not exist (if the postal code in question is not in the Canada Post file at all)
            // 2-Postal code does not match the Province/Territory (if the Province associated to the postal code is not the same in the application VS the Canada Post file)
            // 3-Postal code does not match City (if the City associated to the postal code is not the same in the application VS the two found in the Canada Post file)
            // More than one of these can occur for the same triggering of Event 50772 (really just 2 and 3).

            string postalCode = Application.Appl_Dbtr_Addr_PCd;
            string provinceCode = Application.Appl_Dbtr_Addr_PrvCd;
            string cityName = Application.Appl_Dbtr_Addr_CityNme;
            reasonText = string.Empty;

            if (postalCode == "-")
            {
                if (Application.Medium_Cd != "FTP") Application.Messages.AddError("Invalid Single Dash Postal Code");
                return false;
            }

            if (Application.Appl_Dbtr_Addr_CtryCd == "CAN")
            {
                if (!ValidationHelper.IsValidPostalCode(postalCode))
                {
                    if (Application.Medium_Cd != "FTP") Application.Messages.AddError("Invalid Postal Code Format");
                    reasonText = "1";
                    return false;
                }

                var postalCodeDB = Repositories.PostalCodeRepository;
                if (!postalCodeDB.ValidatePostalCode(postalCode.Replace(" ", ""), provinceCode, cityName,
                                                     out string validProvCode, out PostalCodeFlag validFlags))
                {
                    if (Application.Medium_Cd != "FTP") Application.Messages.AddError("Invalid Postal Code for Province/City");
                    reasonText = "1";
                    return false;
                }

                if (string.IsNullOrEmpty(provinceCode) || provinceCode.In("77", "88", "99"))
                {
                    Application.Appl_Dbtr_Addr_PrvCd = validProvCode;
                    Application.Messages.AddWarning("Replaced Invalid Province with Postal Code province");
                    validFlags = new PostalCodeFlag
                    {
                        IsPostalCodeValid = validFlags.IsPostalCodeValid,
                        IsProvinceValid = true,
                        IsCityNameValid = validFlags.IsCityNameValid
                    };
                    reasonText = "1";
                }

                if (!validFlags.IsPostalCodeValid)
                    reasonText = "1";
                else
                {
                    if (!validFlags.IsProvinceValid)
                        reasonText = "2";

                    if (!validFlags.IsCityNameValid)
                        reasonText = "3";

                    if (!validFlags.IsProvinceValid && !validFlags.IsCityNameValid)
                        reasonText = "2,3";
                }

                if (!validFlags.IsProvinceValid && validFlags.IsCityNameValid && (Application.Medium_Cd != "FTP"))
                    Application.Messages.AddWarning("Postal Code failed lookup province");

                else if (validFlags.IsProvinceValid && !validFlags.IsCityNameValid && (Application.Medium_Cd != "FTP"))
                    Application.Messages.AddWarning("Postal Code failed lookup city");

                else if (!validFlags.IsProvinceValid && !validFlags.IsCityNameValid && (Application.Medium_Cd != "FTP"))
                    Application.Messages.AddWarning("Postal Code failed lookup city and province");

                if (!string.IsNullOrEmpty(reasonText))
                    return false;

                return true;
            }
            else
            {
                var isValid = Application.Appl_Dbtr_Addr_CtryCd switch
                {
                    "USA" or "US" => ValidateInternationalPostalCode(postalCode, @"^\d{5}(?:[-\s]\d{4})?$", EventCode.C50774_INVALID_ZIP_CODE_FORMAT_FOR_COUNTRY_CODE_USA),
                    "GBR" => ValidateInternationalPostalCode(postalCode, @"^[A-Za-z]{1,2}[0-9]?[0-9A-Za-z]\s?[0-9][A-Za-z]{2}$", EventCode.C50775_INVALID_POSTAL_CODE_FORMAT_FOR_THE_CHOSEN_COUNTRY),
                    "AUS" => ValidateInternationalPostalCode(postalCode, @"^\d{4}$", EventCode.C50775_INVALID_POSTAL_CODE_FORMAT_FOR_THE_CHOSEN_COUNTRY),
                    "CHN" => ValidateInternationalPostalCode(postalCode, @"^\d{6}$", EventCode.C50775_INVALID_POSTAL_CODE_FORMAT_FOR_THE_CHOSEN_COUNTRY),
                    "DEU" or "MEX" or "FRA" => ValidateInternationalPostalCode(postalCode, @"^\d{5}$", EventCode.C50775_INVALID_POSTAL_CODE_FORMAT_FOR_THE_CHOSEN_COUNTRY),
                    "POL" => ValidateInternationalPostalCode(postalCode, @"^\d{2}-\d{3}$", EventCode.C50775_INVALID_POSTAL_CODE_FORMAT_FOR_THE_CHOSEN_COUNTRY),
                    _ => true, // no validation yet for other countries
                };

                if ((!isValid) && (Application.Medium_Cd != "FTP"))
                    Application.Messages.AddWarning("Invalid international postal code");

                return true;
            }
        }

        private bool ValidateInternationalPostalCode(string postalCode, string regEx, EventCode eventCode)
        {
            if (!ValidationHelper.IsBaseRegexValid(postalCode, regEx))
            {
                EventManager.AddEvent(eventCode);
                return false;
            }
            else
                return true;
        }

        public virtual bool IsValidGender()
        {
            return ReferenceData.Instance().Genders.ContainsKey(Application.Appl_Dbtr_Gendr_Cd);
        }

        public bool IsSINLocallyConfirmed()
        {
            bool isConfirmed = false;

            bool isTempSIN = (Application.Appl_Dbtr_Entrd_SIN.Substring(0, 1) == "9");
            bool doesLocalSINExists;

            if (Application.AppCtgy_Cd == "I01")
                doesLocalSINExists = Repositories.ApplicationRepository.GetApplLocalConfirmedSINExists(Application.Appl_Dbtr_Entrd_SIN, Application.Appl_Dbtr_SurNme, Application.Appl_Dbtr_Brth_Dte, Application.Subm_SubmCd, Application.Appl_CtrlCd);
            else
                doesLocalSINExists = Repositories.ApplicationRepository.GetApplLocalConfirmedSINExists(Application.Appl_Dbtr_Entrd_SIN, Application.Appl_Dbtr_SurNme, Application.Appl_Dbtr_Brth_Dte, Application.Subm_SubmCd, Application.Appl_CtrlCd, Application.Appl_Dbtr_FrstNme);

            if (!isTempSIN && doesLocalSINExists)
            {
                Application.Appl_Dbtr_Cnfrmd_SIN = Application.Appl_Dbtr_Entrd_SIN;
                Application.Appl_SIN_Cnfrmd_Ind = 1;
                isConfirmed = true;
            }

            return isConfirmed;

        }

        public bool IsSameDayApplication()
        {
            bool isSameDayApplication = false;

            if (!String.IsNullOrEmpty(Application.Appl_Dbtr_Entrd_SIN))
            {
                var applications = Repositories.ApplicationRepository.GetDailyApplCountBySIN(
                                                                            Application.Appl_Dbtr_Entrd_SIN,
                                                                            Application.Appl_EnfSrv_Cd,
                                                                            Application.Appl_CtrlCd,
                                                                            Application.AppCtgy_Cd,
                                                                            Application.Appl_Source_RfrNr);
                if (applications.Count > 0)
                    isSameDayApplication = true;
            }

            return isSameDayApplication;

        }

        public void AddDuplicateSINWarningEvents()
        {
            (string errorSameEnfOFf, string errorDiffEnfOff) = Repositories.ApplicationRepository.GetConfirmedSINRecords(Application.Subm_SubmCd, Application.Appl_CtrlCd, Application.Appl_Dbtr_Cnfrmd_SIN);
            if (!String.IsNullOrEmpty(errorSameEnfOFf) &&
                Repositories.ApplicationRepository.GetConfirmedSINSameEnforcementOfficeExists(Application.Appl_EnfSrv_Cd, Application.Subm_SubmCd, Application.Appl_CtrlCd, Application.Appl_Dbtr_Cnfrmd_SIN, Application.AppCtgy_Cd))
            {
                EventManager.AddEvent(EventCode.C50931_MORE_THAN_ONE_ACTIVE_APPL_FOR_THIS_DEBTOR_WITHIN_YOUR_JURISDIC, errorSameEnfOFf);
            }

            if (!String.IsNullOrEmpty(errorDiffEnfOff))
            {
                List<ApplicationConfirmedSINData> applsInOtherJurisdictions = Repositories.ApplicationRepository.GetConfirmedSINOtherEnforcementOfficeExists(Application.Appl_EnfSrv_Cd, Application.Subm_SubmCd, Application.Appl_CtrlCd, Application.Appl_Dbtr_Cnfrmd_SIN);

                if (applsInOtherJurisdictions.Count > 0)
                {
                    EventManager.AddEvent(EventCode.C50930_THIS_DEBTOR_IS_ACTIVE_IN_ANOTHER_JURISDICTION_CONTACT_THE_JURISDICTION_CONCERNED, errorDiffEnfOff);

                    string errorMessage = Application.Appl_EnfSrv_Cd.Trim() + "-" + Application.Subm_SubmCd.Trim() + "-" + Application.Appl_CtrlCd.Trim();
                    foreach (ApplicationConfirmedSINData confirmedAppl in applsInOtherJurisdictions)
                    {
                        // add event to each of the applications found
                        EventManager.Events.Add(new ApplicationEventData
                        {
                            Appl_CtrlCd = confirmedAppl.Appl_CtrlCd,
                            Appl_EnfSrv_Cd = confirmedAppl.Appl_EnfSrv_Cd,
                            Subm_SubmCd = confirmedAppl.Subm_SubmCd,
                            Event_Reas_Cd = EventCode.C50930_THIS_DEBTOR_IS_ACTIVE_IN_ANOTHER_JURISDICTION_CONTACT_THE_JURISDICTION_CONCERNED,
                            Event_Reas_Text = errorMessage
                        });

                    }

                }
            }
        }

        public void AddDuplicateCreditorWarningEvents()
        {

            List<ApplicationData> applicationList = Repositories.ApplicationRepository.GetSameCreditorForAppCtgy(Application.Appl_CtrlCd, Application.Subm_SubmCd,
                                                                                     Application.Appl_Dbtr_Entrd_SIN,
                                                                                     Application.Appl_SIN_Cnfrmd_Ind,
                                                                                     Application.ActvSt_Cd,
                                                                                     Application.AppCtgy_Cd);

            if (applicationList.Count > 0)
            {
                var allMatchErrorMessage = new StringBuilder();
                string originalApplicationErrorMessage = $"{Application.Appl_EnfSrv_Cd.Trim()}-{Application.Subm_SubmCd.Trim()}-{Application.Appl_CtrlCd.Trim()}";

                foreach (var applicationFound in applicationList)
                {
                    if ((!String.IsNullOrEmpty(applicationFound.Appl_Crdtr_SurNme)) &&
                        (applicationFound.Appl_Crdtr_SurNme.ToUpper() == Application.Appl_Crdtr_SurNme.ToUpper()))
                    {
                        allMatchErrorMessage.Append($"{applicationFound.Appl_EnfSrv_Cd.Trim()}-{applicationFound.Subm_SubmCd.Trim()}-{applicationFound.Appl_CtrlCd.Trim()} ");

                        var eventData = new ApplicationEventData
                        {
                            Event_Reas_Cd = EventCode.C50932_THERE_EXISTS_ONE_OR_MORE_ACTIVE_APPLICATIONS_OF_THIS_TYPE_FOR_THE_SAME_DEBTOR___CREDITOR,
                            Event_Reas_Text = originalApplicationErrorMessage,
                            Appl_EnfSrv_Cd = applicationFound.Appl_EnfSrv_Cd,
                            Appl_CtrlCd = applicationFound.Appl_CtrlCd
                        };

                        EventManager.Events.Add(eventData);
                    }
                }

                EventManager.AddEvent(EventCode.C50932_THERE_EXISTS_ONE_OR_MORE_ACTIVE_APPLICATIONS_OF_THIS_TYPE_FOR_THE_SAME_DEBTOR___CREDITOR,
                                     allMatchErrorMessage.ToString().Trim());

            }

        }

        public void ValidateAndRevertNonUpdateFields(ApplicationData current)
        {
            switch (current.AppLiSt_Cd)
            {
                case ApplicationState.INVALID_APPLICATION_1:
                case ApplicationState.SIN_NOT_CONFIRMED_5:

                    if ((Application.Appl_EnfSrv_Cd != current.Appl_EnfSrv_Cd) ||
                        (Application.Appl_CtrlCd != current.Appl_CtrlCd) ||
                        (Application.AppCtgy_Cd == current.AppCtgy_Cd)
                        )
                    {
                        EventManager.AddEvent(EventCode.C50621_ALLOWED_UPDATES_ACCEPTED_OTHERS_IGNORED_CONTACT_FOAEA_FOR_CLARIFICATION);
                        if (Application.Medium_Cd != "FTP") Application.Messages.AddWarning(EventCode.C50621_ALLOWED_UPDATES_ACCEPTED_OTHERS_IGNORED_CONTACT_FOAEA_FOR_CLARIFICATION);

                        // revert

                        Application.Appl_EnfSrv_Cd = current.Appl_EnfSrv_Cd;
                        Application.Appl_CtrlCd = current.Appl_CtrlCd;
                        Application.AppCtgy_Cd = current.AppCtgy_Cd;
                    }

                    break;

                case ApplicationState.SIN_CONFIRMATION_PENDING_3:
                case ApplicationState.APPLICATION_REJECTED_9:

                    // nothing can be changed

                    if(Application.Medium_Cd != "FTP") Application.Messages.AddError(EventCode.C50500_INVALID_UPDATE);

                    break;

                case ApplicationState.PENDING_ACCEPTANCE_SWEARING_6:
                case ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7:
                case ApplicationState.APPLICATION_ACCEPTED_10:
                case ApplicationState.PARTIALLY_SERVICED_12:

                    if (ValidateAndRevertCommonFields(current))
                        EventManager.AddEvent(EventCode.C50622_ALLOWED_UPDATES_ACCEPTED_OTHERS_IGNORED_CONTACT_FOAEA_FOR_CLARIFICATION);

                    break;

                case ApplicationState.FULLY_SERVICED_13:
                case ApplicationState.MANUALLY_TERMINATED_14:
                case ApplicationState.EXPIRED_15:

                    bool revertedSomeFields = ValidateAndRevertCommonFields(current);

                    // additionally, cannot change life state or application state

                    if ((Application.ActvSt_Cd != current.ActvSt_Cd) ||
                        (Application.AppLiSt_Cd != current.AppLiSt_Cd))
                    {
                        revertedSomeFields = true;
                        Application.ActvSt_Cd = current.ActvSt_Cd;
                        Application.AppLiSt_Cd = current.AppLiSt_Cd;
                    }

                    if (revertedSomeFields)
                        EventManager.AddEvent(EventCode.C50622_ALLOWED_UPDATES_ACCEPTED_OTHERS_IGNORED_CONTACT_FOAEA_FOR_CLARIFICATION);

                    break;
                default:
                    throw new ApplicationManagerException($"Unknown state [{Application.AppLiSt_Cd}] for Update");
            }
        }

        private bool ValidateAndRevertCommonFields(ApplicationData current)
        {
            bool revertedSomeFields = false;

            if ((Application.Appl_EnfSrv_Cd != current.Appl_EnfSrv_Cd) ||
                (Application.Appl_CtrlCd != current.Appl_CtrlCd) ||
                (Application.Subm_SubmCd != current.Subm_SubmCd) ||
                (Application.Subm_Recpt_SubmCd != current.Subm_Recpt_SubmCd) ||
                (!Application.Appl_Lgl_Dte.AreDatesEqual(current.Appl_Lgl_Dte)) ||
                (!Application.Appl_Rcptfrm_Dte.AreDatesEqual(current.Appl_Rcptfrm_Dte)) ||
                (Application.Appl_Group_Batch_Cd != current.Appl_Group_Batch_Cd) ||
                (Application.Subm_Affdvt_SubmCd != current.Subm_Affdvt_SubmCd) ||
                (!Application.Appl_RecvAffdvt_Dte.AreDatesEqual(current.Appl_RecvAffdvt_Dte)) ||
                (Application.Appl_Affdvt_DocTypCd != current.Appl_Affdvt_DocTypCd) ||
                (Application.Appl_JusticeNr != current.Appl_JusticeNr) ||
                (Application.Appl_Crdtr_FrstNme != current.Appl_Crdtr_FrstNme) ||
                (Application.Appl_Crdtr_MddleNme != current.Appl_Crdtr_MddleNme) ||
                (Application.Appl_Crdtr_SurNme != current.Appl_Crdtr_SurNme) ||
                (Application.Appl_Dbtr_FrstNme != current.Appl_Dbtr_FrstNme) ||
                (Application.Appl_Dbtr_MddleNme != current.Appl_Dbtr_MddleNme) ||
                (Application.Appl_Dbtr_SurNme != current.Appl_Dbtr_SurNme) ||
                (Application.Appl_Dbtr_Parent_SurNme != current.Appl_Dbtr_Parent_SurNme) ||
                (!Application.Appl_Dbtr_Brth_Dte.AreDatesEqual(current.Appl_Dbtr_Brth_Dte)) ||
                (Application.Appl_Dbtr_LngCd != current.Appl_Dbtr_LngCd) ||
                (Application.Appl_Dbtr_Gendr_Cd != current.Appl_Dbtr_Gendr_Cd) ||
                (Application.Appl_Dbtr_Entrd_SIN != current.Appl_Dbtr_Entrd_SIN) ||
                (Application.Appl_Dbtr_Cnfrmd_SIN != current.Appl_Dbtr_Cnfrmd_SIN) ||
                (Application.Appl_Dbtr_RtrndBySrc_SIN != current.Appl_Dbtr_RtrndBySrc_SIN) ||
                (Application.Medium_Cd != current.Medium_Cd) ||
                (!Application.Appl_Reactv_Dte.AreDatesEqual(current.Appl_Reactv_Dte)) ||
                (Application.Appl_SIN_Cnfrmd_Ind != current.Appl_SIN_Cnfrmd_Ind) ||
                (Application.AppCtgy_Cd != current.AppCtgy_Cd) ||
                (Application.AppReas_Cd != current.AppReas_Cd) ||
                (!Application.Appl_Create_Dte.AreDatesEqual(current.Appl_Create_Dte)) ||
                (Application.Appl_Create_Usr != current.Appl_Create_Usr) ||
                (Application.Appl_WFID != current.Appl_WFID) ||
                (!Application.Appl_Crdtr_Brth_Dte.AreDatesEqual(current.Appl_Crdtr_Brth_Dte)))
            {
                // revert any values that cannot be modified

                Application.Appl_EnfSrv_Cd = current.Appl_EnfSrv_Cd;
                Application.Appl_CtrlCd = current.Appl_CtrlCd;
                Application.Subm_SubmCd = current.Subm_SubmCd;
                Application.Subm_Recpt_SubmCd = current.Subm_Recpt_SubmCd;
                Application.Appl_Lgl_Dte = current.Appl_Lgl_Dte;
                Application.Appl_Rcptfrm_Dte = current.Appl_Rcptfrm_Dte;
                Application.Appl_Group_Batch_Cd = current.Appl_Group_Batch_Cd;
                Application.Subm_Affdvt_SubmCd = current.Subm_Affdvt_SubmCd;
                Application.Appl_RecvAffdvt_Dte = current.Appl_RecvAffdvt_Dte;
                Application.Appl_Affdvt_DocTypCd = current.Appl_Affdvt_DocTypCd;
                Application.Appl_JusticeNr = current.Appl_JusticeNr;
                Application.Appl_Crdtr_FrstNme = current.Appl_Crdtr_FrstNme;
                Application.Appl_Crdtr_MddleNme = current.Appl_Crdtr_MddleNme;
                Application.Appl_Crdtr_SurNme = current.Appl_Crdtr_SurNme;
                Application.Appl_Dbtr_FrstNme = current.Appl_Dbtr_FrstNme;
                Application.Appl_Dbtr_MddleNme = current.Appl_Dbtr_MddleNme;
                Application.Appl_Dbtr_SurNme = current.Appl_Dbtr_SurNme;
                Application.Appl_Dbtr_Parent_SurNme = current.Appl_Dbtr_Parent_SurNme;
                Application.Appl_Dbtr_Brth_Dte = current.Appl_Dbtr_Brth_Dte;
                Application.Appl_Dbtr_LngCd = current.Appl_Dbtr_LngCd;
                Application.Appl_Dbtr_Gendr_Cd = current.Appl_Dbtr_Gendr_Cd;
                Application.Appl_Dbtr_Entrd_SIN = current.Appl_Dbtr_Entrd_SIN;
                Application.Appl_Dbtr_Cnfrmd_SIN = current.Appl_Dbtr_Cnfrmd_SIN;
                Application.Appl_Dbtr_RtrndBySrc_SIN = current.Appl_Dbtr_RtrndBySrc_SIN;
                Application.Medium_Cd = current.Medium_Cd;
                Application.Appl_Reactv_Dte = current.Appl_Reactv_Dte;
                Application.Appl_SIN_Cnfrmd_Ind = current.Appl_SIN_Cnfrmd_Ind;
                Application.AppCtgy_Cd = current.AppCtgy_Cd;
                Application.AppReas_Cd = current.AppReas_Cd;
                Application.Appl_Create_Dte = current.Appl_Create_Dte;
                Application.Appl_Create_Usr = current.Appl_Create_Usr;
                Application.Appl_WFID = current.Appl_WFID;
                Application.Appl_Crdtr_Brth_Dte = current.Appl_Crdtr_Brth_Dte;

                revertedSomeFields = true;
            }

            return revertedSomeFields;
        }

        public bool ValidateCodeValues()
        {
            PostalCodeValidationInBound();

            string debtorCountry = Application.Appl_Dbtr_Addr_CtryCd?.ToUpper();
            string debtorProvince = Application.Appl_Dbtr_Addr_PrvCd?.ToUpper();
            if (!string.IsNullOrEmpty(debtorCountry) &&
                !string.IsNullOrEmpty(debtorProvince) &&
                (debtorCountry != "USA"))
            {
                if (ReferenceData.Instance().Provinces.ContainsKey(debtorProvince))
                {
                    string countryForProvince = ReferenceData.Instance().Provinces[debtorProvince].PrvCtryCd;
                    if (countryForProvince == "USA")
                        Application.Messages.AddError("Code Value Error for Appl_Dbtr_Addr_PrvCd", "Appl_Dbtr_Addr_PrvCd");
                }
            }

            if (!ReferenceData.Instance().Mediums.ContainsKey(Application.Medium_Cd))
            {
                Application.Messages.AddError("Code Value Error for Medium_Cd", "Medium_Cd");
                return false;
            }

            if (!ReferenceData.Instance().Languages.ContainsKey(Application.Appl_Dbtr_LngCd))
            {
                Application.Messages.AddError("Code Value Error for Appl_Dbtr_LngCd", "Appl_Dbtr_LngCd");
                return false;
            }

            if (!ReferenceData.Instance().Genders.ContainsKey(Application.Appl_Dbtr_Gendr_Cd))
            {
                Application.Messages.AddError("Code Value Error for Appl_Dbtr_Gendr_Cd", "Appl_Dbtr_Gendr_Cd");
                return false;
            }

            var enfServices = Repositories.EnfSrvRepository.GetEnfService();
            if (enfServices != null)
            {
                var service = enfServices.FirstOrDefault(m => m.EnfSrv_Cd.Trim() == Application.Appl_EnfSrv_Cd.Trim());
                if (service is null)
                {
                    Application.Messages.AddError("Code Value Error for Appl_EnfSrv_Cd", "Appl_EnfSrv_Cd");
                    return false;
                }
            }

            if (!ReferenceData.Instance().DocumentTypes.ContainsKey(Application.Appl_Affdvt_DocTypCd))
            {
                Application.Messages.AddError("Code Value Error for Appl_Affdvt_DocTypCd", "Appl_Affdvt_DocTypCd");
                return false;
            }

            if (!ReferenceData.Instance().Countries.ContainsKey(Application.Appl_Dbtr_Addr_CtryCd))
            {
                Application.Messages.AddError("Code Value Error for Appl_Dbtr_Addr_CtryCd", "Appl_Dbtr_Addr_CtryCd");
                return false;
            }

            if (!ReferenceData.Instance().ApplicationReasons.ContainsKey(Application.AppReas_Cd.Trim()))
            {
                Application.Messages.AddError("Code Value Error for AppReas_Cd", "AppReas_Cd");
                return false;
            }

            if (!ReferenceData.Instance().ApplicationCategories.ContainsKey(Application.AppCtgy_Cd))
            {
                Application.Messages.AddError("Code Value Error for AppCtgy_Cd", "AppCtgy_Cd");
                return false;
            }

            return true;
        }

        private void PostalCodeValidationInBound()
        {
            bool isProvValid = true;

            //--------------------------------------------------------------------------------------------
            // CR526/542 - This block of original code checks import province is in prv table...
            // This is a bit redundant for postal code validation, but other things depend on it 
            // so leave it alone...
            //--------------------------------------------------------------------------------------------
            string debtorCountry = Application.Appl_Dbtr_Addr_CtryCd?.ToUpper();
            string debtorProvince = Application.Appl_Dbtr_Addr_PrvCd?.ToUpper();
            string debtorCity = Application.Appl_Dbtr_Addr_CityNme?.Trim();
            string debtorPostalCode = Application.Appl_Dbtr_Addr_PCd?.Replace(" ", "");

            if (debtorCountry is "CAN")
            {
                if (string.IsNullOrEmpty(debtorProvince))
                    isProvValid = false;
                else if (!ReferenceData.Instance().Provinces.ContainsKey(debtorProvince))
                    isProvValid = false;

                if (!isProvValid && Application.AppLiSt_Cd.In(ApplicationState.MANUALLY_TERMINATED_14,
                                                              ApplicationState.FINANCIAL_TERMS_VARIED_17,
                                                              ApplicationState.APPLICATION_SUSPENDED_35))
                {
                    Application.Appl_Dbtr_Addr_PrvCd = null;
                    if (Application.AppLiSt_Cd != ApplicationState.APPLICATION_SUSPENDED_35)
                    {
                        Application.Messages.AddError("Code Value Error for Appl_Dbtr_Addr_PrvCd", "Appl_Dbtr_Addr_PrvCd");
                        return;
                    }
                }
            }

            // CR526/542 CODE START:
            // This new block is for the revamped postal code validation (new requirements).
            // -----------------------------------------------------------------------------
            bool canValidatePostalCode = false;

            if (Application.AppLiSt_Cd.In(ApplicationState.INITIAL_STATE_0, ApplicationState.INVALID_APPLICATION_1,
                                          ApplicationState.SIN_NOT_CONFIRMED_5, ApplicationState.MANUALLY_TERMINATED_14,
                                          ApplicationState.FINANCIAL_TERMS_VARIED_17, ApplicationState.APPLICATION_SUSPENDED_35))
            {
                canValidatePostalCode = true;
            }

            if ((debtorCountry is "CAN") && (canValidatePostalCode))
            {
                var postalCodeDB = Repositories.PostalCodeRepository;
                postalCodeDB.ValidatePostalCode(debtorPostalCode, debtorProvince ?? "", debtorCity,
                                                out string validProvCode, out PostalCodeFlag validFlags);

                if (!string.IsNullOrEmpty(validProvCode))
                {
                    if (validFlags.IsPostalCodeValid)
                    {
                        if (string.IsNullOrEmpty(debtorProvince) || debtorProvince.In("99", "88", "77"))
                            Application.Appl_Dbtr_Addr_PrvCd = validProvCode;
                    }
                }
                else if (!isProvValid)
                {
                    Application.Appl_Dbtr_Addr_PrvCd = null;
                    Application.Messages.AddError("Code Value Error for Appl_Dbtr_Addr_PrvCd", "Appl_Dbtr_Addr_PrvCd");
                }
            }

        }

    }

}

