using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSubject : DBbase, ISubjectRepository
    {
        public DBSubject(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<SubjectData> GetSubject(string subjectName)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"SubjectName", subjectName}
                };

            var data = (await MainDB.GetDataFromStoredProcAsync<SubjectData>("Subject_SelectBySubjectName", parameters, FillSubjectDataFromReader))
                            .FirstOrDefault();

            return data;
        }

        public async Task<List<SubjectData>> GetSubjectsForSubmitter(string submCd)
        {
            var parameters = new Dictionary<string, object>
            {
                { "RoleName", submCd }
            };

            return await MainDB.GetDataFromStoredProcAsync<SubjectData>("SubmGetSubjects", parameters, FillSubmitterSubjectDataFromReader);
        }

        public async Task<SubjectData> GetSubjectByConfirmationCode(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"ConfirmationCode", confirmationCode}
                };
            return (await MainDB.GetDataFromStoredProcAsync<SubjectData>("Subject_SelectByConfirmationCode", parameters, FillSubjectDataFromReader))
                        .ElementAt(0);
        }

        private void FillSubmitterSubjectDataFromReader(IDBHelperReader rdr, SubjectData data)
        {
            data.SubjectId = (int)rdr["SubjectId"];
            data.SubjectName = (string)rdr["SubjectName"];
            data.OrganizationId = (int)rdr["OrganizationId"];
            data.OrganizationName = (string)rdr["OrganizationName"];
            data.AllowedAccess = ((string)rdr["ActvSt_Cd"] == "A");
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
