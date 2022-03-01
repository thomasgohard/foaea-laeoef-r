using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    class InMemoryLogin: ILoginRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public InMemoryLogin()
        {

        }
        public void SendEmail(string subject, string recipient, string body, int isHTML = 1)
        {
            throw new NotImplementedException();
        }
        //public SubjectData GetSubjectLoginCredentials(string subjectName)
        //{
        //    throw new NotImplementedException();
        //}
        public SubjectData GetSubject(int subjectId)
        {
            throw new NotImplementedException();
        }
        public SubjectData GetSubject(string subjectName)
        {
            throw new NotImplementedException();
        }
        public SubjectData GetSubjectByConfirmationCode(string confirmationCode)
        {
            throw new NotImplementedException();
        }
        public bool IsLoginExpired(string subjectName)
        {
            throw new NotImplementedException();
        }
        public void AcceptNewTermsOfReferernce(string subjectName)
        {
            throw new NotImplementedException();
        }
        public bool CheckPreviousPasswords(int subjectId, string newPassword)
        {
            throw new NotImplementedException();
        }
        public void GetAllowedAccess(string username, ref bool IsAllowed)
        {
            throw new NotImplementedException();
        }
        public void SetPassword(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays)
        {
            throw new NotImplementedException();
        }


    }
}
