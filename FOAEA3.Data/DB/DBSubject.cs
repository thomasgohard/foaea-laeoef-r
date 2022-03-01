using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FOAEA3.Data.DB
{
    public class DBSubject : DBbase, ISubjectRepository
    {
        public DBSubject(IDBTools mainDB) : base(mainDB)
        {

        }

        public SubjectData GetSubject(string subjectName)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"SubjectName", subjectName}
                };

            var data = MainDB.GetDataFromStoredProc<SubjectData>("Subject_SelectBySubjectName", parameters, FillSubjectDataFromReader).FirstOrDefault();

            return data;
        }

        public List<SubjectData> GetSubjectsForSubmitter(string submCd)
        {
            var parameters = new Dictionary<string, object>
            {
                { "RoleName", submCd }
            };

            return MainDB.GetDataFromStoredProc<SubjectData>("SubmGetSubjects", parameters, FillSubmitterSubjectDataFromReader);
        }

        public SubjectData GetSubjectByConfirmationCode(string confirmationCode)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"ConfirmationCode", confirmationCode}
                };
            return MainDB.GetDataFromStoredProc<SubjectData>("Subject_SelectByConfirmationCode", parameters, FillSubjectDataFromReader).ElementAt(0);
            //return MainDB.GetDataForKey<SubjectData, string>("Subject", subjectName, "SubjectName", FillSubjectDataFromReader);
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
