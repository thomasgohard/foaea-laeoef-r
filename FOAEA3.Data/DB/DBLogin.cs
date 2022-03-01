using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Data.DB
{
    public class DBLogin : DBbase, ILoginRepository
    {
        private const int PREVIOUS_PASSWORDS_HISTORY = 5;
        public DBLogin(IDBTools mainDB) : base(mainDB)
        {

        }

        public bool IsLoginExpired(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Username", subjectName}
            };

            DateTime? expiryDate = MainDB.GetDataFromStoredProc<SubjectData>("SecurityMembershipGetPasswordExpiry", parameters, FillSubjectDataFromReader).ElementAt(0).PasswordExpiryDate;
            if (expiryDate.HasValue)
            {
                if (expiryDate.Value.Date > DateTime.Now.Date)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public bool CheckPreviousPasswords(int subjectId, string newPassword)
        {
            var parameters = new Dictionary<string, object>
            {
                {"subjectId", subjectId.ToString()}
            };

            List<SubjectData> previousPasswords = MainDB.GetDataFromStoredProc<SubjectData>("SecurityMembershipGetPreviousPasswords", parameters, FillSubjectDataFromReader);

            int x = 1;
            foreach (SubjectData previousPassword in previousPasswords)
            {
                // -- ensure we only check the required amount of passwords in history

                if (x > PREVIOUS_PASSWORDS_HISTORY)
                {
                    break;
                }

                if (newPassword.Equals(previousPassword.Password))
                {
                    return true;
                }
                x += 1;
            }

            return false;

        }

        public void GetAllowedAccess(string username, ref bool IsAllowed)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Username", username}
                };

            List<SubjectData> accessList = MainDB.GetDataFromStoredProc<SubjectData>("SecurityMembershipGetAllowedAccess", parameters, FillSubjectDataFromReader);



            if (accessList.Count > 0)
            {
                IsAllowed = accessList.ElementAt(0).AllowedAccess;
            }
            else
            {
                // This is when no record has been found in the Subject table 
                // for the specified Username
                IsAllowed = false;
            }

        }

        public void SetPassword(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays)
        {

            var parameters = new Dictionary<string, object>
            {
                {"Username", username},
                {"Password", password},
                {"PasswordFormat", passwordFormat.ToString()},
                {"PasswordSalt", passwordSalt},
                {"ExpireIn", passwordExpireDays.ToString()}
            };


            _ = MainDB.ExecProc("SecurityMembershipSetPassword", parameters);

        }

        public bool VerifyConfirmationCode(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };

            int result = MainDB.GetDataFromStoredProc<int>("PasswordResetVerifyConfirmationCode", parameters);
            return (result == 1);
        }

        public bool VerifyPasswordResetFlagSet(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            int result = MainDB.GetDataFromStoredProc<int>("PasswordResetVerifyPasswordResetFlagSet", parameters);
            return (result == 1);
        }

        public bool VerifyPasswordReset(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            int result = MainDB.GetDataFromStoredProc<int>("PasswordResetVerifyPasswordReset", parameters);
            return (result == 1);
        }

        public void PostConfirmationCode(int subjectId, string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode },
                {"SubjectID", subjectId.ToString() }

            };
            _ = MainDB.ExecProc("PasswordResetPostConfirmationCode", parameters);
        }

        public void PostPassword(string confirmationCode, string password, string salt, string initial)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode },
                {"Password", password },
                {"PasswordSalt", salt },
                {"initial", initial }
            };
            _ = MainDB.ExecProc("PasswordResetPostPassword", parameters);
        }

        public void PostConfirmationReceived(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };
            _ = MainDB.ExecProc("PasswordResetPostConfirmationReceived", parameters);
        }

        public string GetEmailByConfirmationCode(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };

            return MainDB.GetDataFromStoredProc<string>("PasswordResetGetEmailByConfirmationCode", parameters);

        }

        public void ProcessReset(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };
            _ = MainDB.ExecProc("PasswordResetProcessReset", parameters);
        }

        public bool VerifyPasswordSent(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            int result = MainDB.GetDataFromStoredProc<int>("PasswordResetVerifyPasswordSent", parameters);
            return (result == 1);
        }

        public void AcceptNewTermsOfReferernce(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            _ = MainDB.ExecProc("SubjuctUpdateTermOfRefFlag", parameters);

        }

        private void FillSubjectDataFromReader(IDBHelperReader rdr, SubjectData data)
        {
            data.SubjectId = (int)(rdr["SubjectId"]);
            data.SubjectName = (string)(rdr["SubjectName"]);
            data.defaultRoleName = (string)(rdr["defaultRoleName"]);
            data.defaultRoleId = (int)(rdr["defaultRoleId"]);
            data.IsAccessingUsingRole = (bool)(rdr["IsAccessingUsingRole"]);
            data.roleChangeDate = (DateTime)(rdr["roleChangeDate"]);
            data.currentRoleName = (string)(rdr["currentRoleName"]);
            data.AllowedAccess = (bool)(rdr["AllowedAccess"]);
            data.IsTrusted = (bool)(rdr["IsTrusted"]);
            data.PhoneNumber = (string)(rdr["PhoneNumber"]);
            data.EMailAddress = (string)(rdr["EMailAddress"]);
            data.OrganizationId = (int)(rdr["OrganizationId"]);
            data.OrganizationName = (string)(rdr["OrganizationName"]);
            data.EntrustName = (string)(rdr["EntrustName"]);
            data.Password = (string)(rdr["Password"]);
            data.PasswordFormat = (int?)(rdr["PasswordFormat"]);
            data.PasswordSalt = (string)(rdr["PasswordSalt"]);
            data.PasswordExpiryDate = (DateTime?)(rdr["PasswordExpiryDate"]);
            data.IsAccountLocked = (bool?)(rdr["IsAccountLocked"]);
            data.UnsuccessfulLoginAttempts = (int)(rdr["UnsuccessfulLoginAttempts"]);
            data.ConfirmationCode = (string)(rdr["ConfirmationCode"]);
            data.ConfirmationSent = (DateTime?)(rdr["ConfirmationSent"]);
            data.ConfirmationReceived = (bool?)(rdr["ConfirmationReceived"]);
            data.PasswordSent = (DateTime?)(rdr["PasswordSent"]);
            data.PasswordReset = (bool?)(rdr["PasswordReset"]);
            data.PKIExpiration = (DateTime?)(rdr["PKIExpiration"]);
            data.HasAcceptedNewTermsOfRef = (bool)(rdr["HasAcceptedNewTermsOfRef"]);
        }
        
    }
}
