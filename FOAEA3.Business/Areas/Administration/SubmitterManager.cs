using FOAEA3.Resources.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Exceptions;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using FOAEA3.Business.Areas.Application;

namespace FOAEA3.Business.Areas.Administration
{
    internal class SubmitterManager
    {
        private readonly IRepositories Repositories;

        internal SubmitterManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        internal SubmitterData GetSubmitter(string submCd)
        {
            return (Repositories.SubmitterRepository.GetSubmitter(submCode: submCd).FirstOrDefault());
        }

        internal string GetSignAuthorityForSubmitter(string submCd)
        {
            return Repositories.SubmitterRepository.GetSignAuthorityForSubmitter(submCd);
        }

        internal List<SubmitterData> GetSubmittersForProvince(string provCd, bool onlyActive)
        {
            List<SubmitterData> submitterData = null;
            submitterData = Repositories.SubmitterRepository.GetSubmitter(prov: provCd);

            return onlyActive ? submitterData.FindAll(m => m.ActvSt_Cd == "A") : submitterData;

        }

        internal List<SubmitterData> GetSubmittersForProvinceAndOffice(string provCd, string enfOff, string enfSrv, bool onlyActive)
        {
            List<SubmitterData> submitterData = null;
            submitterData = Repositories.SubmitterRepository.GetSubmitter(prov: provCd, enfOffCode: enfOff, enfServCode: enfSrv);

            return onlyActive ? submitterData.FindAll(m => m.ActvSt_Cd == "A") : submitterData;
        }

        internal SubmitterData CreateSubmitter(SubmitterData submitterData, string suffixCode, bool readOnlyAccess)
        {
            if (submitterData.Subm_Create_Usr is null)
                submitterData.Subm_Create_Usr = Repositories.CurrentSubmitter;

            if (string.IsNullOrEmpty(submitterData.Subm_SubmCd))
                submitterData.Subm_SubmCd = GenerateSubmitterCode(submitterData, suffixCode);
            submitterData.Subm_Class = CalculateSubm_Class(submitterData, readOnlyAccess);

            submitterData.Messages.Clear();
            Validate(submitterData);

            if (!submitterData.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                Repositories.SubmitterRepository.CreateSubmitter(submitterData);
            }
            else
                submitterData.Subm_SubmCd = string.Empty;

            return submitterData;
        }

        internal SubmitterData UpdateSubmitter(SubmitterData submitterData, bool readOnly)
        {
            submitterData.Subm_Class = CalculateSubm_Class(submitterData, readOnly);

            submitterData.Messages.Clear();
            Validate(submitterData);

            if (!submitterData.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                Repositories.SubmitterRepository.UpdateSubmitter(submitterData);
            }

            return submitterData;
        }
        internal DateTime UpdateSubmitterLastLogin(string submCd)
        {
            return Repositories.SubmitterRepository.UpdateSubmitterLastLogin(submCd);
        }

        internal List<CommissionerData> GetCommissioners(string enfOffLocCode, string currentSubmitter)
        {
            return Repositories.SubmitterRepository.GetCommissioners(enfOffLocCode, currentSubmitter);
        }

        private static void Validate(SubmitterData newSubmitter)
        {

            // verify any missing mandatory fields
            if (string.IsNullOrEmpty(newSubmitter.Subm_FrstNme)) newSubmitter.Messages.AddError(ErrorResource.ERROR_MISSING_SUBM_FIRSTNAME, "Subm_FrstNme");
            if (string.IsNullOrEmpty(newSubmitter.Subm_SurNme)) newSubmitter.Messages.AddError(ErrorResource.ERROR_MISSING_SUBM_SURNAME, "Subm_SurNme");
            if (string.IsNullOrEmpty(newSubmitter.Lng_Cd)) newSubmitter.Messages.AddError(ErrorResource.ERROR_LANGUAGE_MISSING, "Lng_Cd");
            if (!string.IsNullOrEmpty(newSubmitter.Subm_Comments) && newSubmitter.Subm_Comments.Trim().Length > 255) newSubmitter.Messages.AddError(ErrorResource.ERROR_COMMENTS_TOO_LONG, "Subm_Comments");
            if (!newSubmitter.Subm_EnfSrvAuth_Ind && !newSubmitter.Subm_EnfOffAuth_Ind && !newSubmitter.Subm_SysMgr_Ind
                && !newSubmitter.Subm_CourtUsr_Ind && !newSubmitter.Subm_AppMgr_Ind)
            {
                newSubmitter.Messages.AddError(ErrorResource.ERROR_MISSING_ROLE, "optButtons");
            }

            // area code must be 999 in format
            // phone must be 999-9999 in format

            // mandatory, must be numeric if entered
            if (string.IsNullOrEmpty(newSubmitter.Subm_Tel_AreaC)) newSubmitter.Messages.AddError(ErrorResource.ERROR_MISSING_PHONE_NUMBER, "Subm_Tel_AreaC");
            else if (newSubmitter.Subm_Tel_AreaC.Trim().Length != 3) newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_PHONE, "Subm_Tel_AreaC");
            if (string.IsNullOrEmpty(newSubmitter.Subm_TelNr)) newSubmitter.Messages.AddError(ErrorResource.ERROR_MISSING_PHONE_NUMBER, "Subm_TelNr");
            else if (!ValidationHelper.IsValidPhoneNumber(newSubmitter.Subm_TelNr)) newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_PHONE, "Subm_TelNr");

            // optional, must be numeric if entered
            if (!string.IsNullOrEmpty(newSubmitter.Subm_TelEx) && !ValidationHelper.IsValidInteger(newSubmitter.Subm_TelEx)) newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_PHONE, "Subm_TelEx");
            if (!string.IsNullOrEmpty(newSubmitter.Subm_Fax_AreaC))
            {
                if (!ValidationHelper.IsValidInteger(newSubmitter.Subm_Fax_AreaC)) newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_PHONE, "Subm_Fax_AreaC");
                else if (newSubmitter.Subm_Fax_AreaC.Trim().Length != 3) newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_PHONE, "Subm_Fax_AreaC");
            }
            if (!string.IsNullOrEmpty(newSubmitter.Subm_FaxNr) && !ValidationHelper.IsValidPhoneNumber(newSubmitter.Subm_FaxNr)) newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_PHONE, "SubSubm_FaxNrm_TelNr");

            // verify valid email value (if entered)
            if (!string.IsNullOrEmpty(newSubmitter.Subm_Assg_Email) && !ValidationHelper.IsValidEmail(newSubmitter.Subm_Assg_Email))
            {
                newSubmitter.Messages.AddError(ErrorResource.ERROR_INVALID_EMAIL, "Subm_Assg_Email");
            }

            // warn if no duties were selected
            if (!newSubmitter.Subm_Trcn_AccsPrvCd && !newSubmitter.Subm_Intrc_AccsPrvCd && !newSubmitter.Subm_Lic_AccsPrvCd
                && !newSubmitter.Subm_LglSgnAuth_Ind && !newSubmitter.Subm_Fin_Ind)
                newSubmitter.Messages.AddWarning(ErrorResource.WARNING_NO_DUTIES);

            // invalid authority
            if (!newSubmitter.Subm_EnfSrvAuth_Ind && !newSubmitter.Subm_EnfOffAuth_Ind && !newSubmitter.Subm_SysMgr_Ind &&
                !newSubmitter.Subm_CourtUsr_Ind && !newSubmitter.Subm_AppMgr_Ind && (newSubmitter.Subm_Class != "LF") &&
                newSubmitter.Subm_SubmCd != "FO0SAM")
            {
                newSubmitter.Messages.AddError(EventCode.C53506_INVALID_AUTHORITY_PROVILEGE, "optButtons");
            }

            if (newSubmitter.Subm_Fin_Ind && newSubmitter.Subm_Class != "FC" && newSubmitter.Subm_SubmCd != "FO0SAM")
            {
                newSubmitter.Messages.AddError(EventCode.C53506_INVALID_AUTHORITY_PROVILEGE, "optButtons");
            }

        }

        private string GenerateSubmitterCode(SubmitterData data, string suffixCode)
        {
            string level = CalcLevel(data).ToString();

            string enfSrvCd = data.EnfSrv_Cd;
            string prefix = enfSrvCd.Substring(0, 2);

            string middle;
            if (data.Subm_CourtUsr_Ind)
                middle = "C0";
            else
            {
                EnfOffData offData = Repositories.EnfOffRepository.GetEnfOff(enfOffCode: data.EnfOff_City_LocCd, enfServCode: data.EnfSrv_Cd).FirstOrDefault();
                if (offData is not null)
                    middle = level + offData.EnfOff_AbbrCd;
                else
                    middle = "??";
            }

            string suffix;
            if (prefix != "FO")
            {
                string maxCourtSubmitterCode = Repositories.SubmitterRepository.GetMaxSubmitterCode(prefix + middle);
                suffix = CalculateLastTwoChars(maxCourtSubmitterCode);
            }
            else
                suffix = suffixCode;

            return prefix + middle + suffix;
        }

        private static string CalculateSubm_Class(SubmitterData data, bool readOnlyAccess)
        {
            string result = data.Subm_Class;

            var srv = data.Subm_SubmCd.AsSpan().Slice(start: 0, length: 2);
            int level = CalcLevel(data);

            if (srv == "FO")
                switch (level)
                {
                    case 1: result = "FC"; break;
                    case 2: result = "FO"; break;
                    case 3: result = "SM"; break;
                }
            else if (srv == "FI")
                result = "LF";
            else
                switch (level)
                {
                    case 1: result = (readOnlyAccess) ? "R1" : "ES"; break;
                    case 2: result = (readOnlyAccess) ? "RO" : "EO"; break;
                    case 3: result = string.Empty; break;
                }

            return result;
        }

        private static string CalculateLastTwoChars(string maxSubmitterCode)
        {
            string lastTwo;

            string maxLastTwo = (string.IsNullOrEmpty(maxSubmitterCode)) ? "00" : maxSubmitterCode[^2..].ToUpper();

            if (int.TryParse(maxLastTwo, out int maxLastTwoValue))
            {
                if (maxLastTwo == "99")
                    lastTwo = "A0";
                else
                {
                    lastTwo = (maxLastTwoValue + 1).ToString();
                    if (lastTwo.Length == 1)
                        lastTwo = "0" + lastTwo;
                }
            }
            else
            {
                char firstChar = maxLastTwo[0];
                char lastChar = maxLastTwo[1];

                if (char.IsNumber(lastChar))
                {
                    if (lastChar == '9')
                        lastTwo = firstChar + "A";
                    else
                        lastTwo = firstChar + (char.GetNumericValue(lastChar) + 1).ToString();
                }
                else
                {
                    if (maxLastTwo == "ZZ")
                    {
                        throw new SubmitterException("Cannot find more digits -- max 'ZZ' has been reached!");
                    }
                    else
                    {
                        if (lastChar == 'Z')
                        {
                            firstChar = CalculateNextChar(firstChar);
                            lastTwo = firstChar + "0";
                        }
                        else
                        {
                            lastChar = CalculateNextChar(lastChar);
                            lastTwo = firstChar.ToString() + lastChar;
                        }
                    }
                }

            }

            return lastTwo;

        }

        private static char CalculateNextChar(char letter)
        {
            if (char.ToUpper(letter) == 'Z')
                return 'A';
            else
                return char.ToUpper((char)((int)letter + 1));
        }

        private static int CalcLevel(SubmitterData data)
        {
            int level;
            if (data.Subm_EnfSrvAuth_Ind)
                level = 1;
            else if (data.Subm_EnfOffAuth_Ind)
                level = 2;
            else if (data.Subm_SysMgr_Ind)
                level = 3;
            else
                level = 2;

            return level;
        }

    }
}
