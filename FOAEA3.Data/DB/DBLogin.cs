using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBLogin : DBbase, ILoginRepository
    {
        private const int PREVIOUS_PASSWORDS_HISTORY = 5;
        public DBLogin(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<bool> IsLoginExpiredAsync(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Username", subjectName}
            };

            DateTime? expiryDate = (await MainDB.GetDataFromStoredProcAsync<SubjectData>("SecurityMembershipGetPasswordExpiry", parameters, FillSubjectDataFromReader))
                                    .ElementAt(0).PasswordExpiryDate;
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

        public async Task<bool> CheckPreviousPasswordsAsync(int subjectId, string newPassword)
        {
            var parameters = new Dictionary<string, object>
            {
                {"subjectId", subjectId.ToString()}
            };

            List<SubjectData> previousPasswords = await MainDB.GetDataFromStoredProcAsync<SubjectData>("SecurityMembershipGetPreviousPasswords", parameters, FillSubjectDataFromReader);

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

        public async Task<bool> GetAllowedAccessAsync(string username)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Username", username}
                };

            List<SubjectData> accessList = await MainDB.GetDataFromStoredProcAsync<SubjectData>("SecurityMembershipGetAllowedAccess", parameters, FillSubjectDataFromReader);

            if (accessList.Count > 0)
            {
                return accessList.ElementAt(0).AllowedAccess;
            }
            else
            {
                // This is when no record has been found in the Subject table 
                // for the specified Username
                return false;
            }

        }

        public async Task SetPasswordAsync(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays)
        {

            var parameters = new Dictionary<string, object>
            {
                {"Username", username},
                {"Password", password},
                {"PasswordFormat", passwordFormat.ToString()},
                {"PasswordSalt", passwordSalt},
                {"ExpireIn", passwordExpireDays.ToString()}
            };


            _ = await MainDB.ExecProcAsync("SecurityMembershipSetPassword", parameters);

        }

        public async Task<bool> VerifyConfirmationCodeAsync(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };

            int result = await MainDB.GetDataFromStoredProcAsync<int>("PasswordResetVerifyConfirmationCode", parameters);
            return (result == 1);
        }

        public async Task<bool> VerifyPasswordResetFlagSetAsync(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            int result = await MainDB.GetDataFromStoredProcAsync<int>("PasswordResetVerifyPasswordResetFlagSet", parameters);
            return (result == 1);
        }

        public async Task<bool> VerifyPasswordResetAsync(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            int result = await MainDB.GetDataFromStoredProcAsync<int>("PasswordResetVerifyPasswordReset", parameters);
            return (result == 1);
        }

        public async Task PostConfirmationCodeAsync(int subjectId, string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode },
                {"SubjectID", subjectId.ToString() }

            };
            _ = await MainDB.ExecProcAsync("PasswordResetPostConfirmationCode", parameters);
        }

        public async Task PostPasswordAsync(string confirmationCode, string password, string salt, string initial)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode },
                {"Password", password },
                {"PasswordSalt", salt },
                {"initial", initial }
            };
            _ = await MainDB.ExecProcAsync("PasswordResetPostPassword", parameters);
        }

        public async Task PostConfirmationReceivedAsync(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };
            _ = await MainDB.ExecProcAsync("PasswordResetPostConfirmationReceived", parameters);
        }

        public async Task<string> GetEmailByConfirmationCodeAsync(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };

            return await MainDB.GetDataFromStoredProcAsync<string>("PasswordResetGetEmailByConfirmationCode", parameters);

        }

        public async Task ProcessResetAsync(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmationCode", confirmationCode }
            };
            _ = await MainDB.ExecProcAsync("PasswordResetProcessReset", parameters);
        }

        public async Task<bool> VerifyPasswordSentAsync(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            int result = await MainDB.GetDataFromStoredProcAsync<int>("PasswordResetVerifyPasswordSent", parameters);
            return (result == 1);
        }

        public async Task AcceptNewTermsOfReferernceAsync(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SubjectName", subjectName }
            };

            _ = await MainDB.ExecProcAsync("SubjuctUpdateTermOfRefFlag", parameters);

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
