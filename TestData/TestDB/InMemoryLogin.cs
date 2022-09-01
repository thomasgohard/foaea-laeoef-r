using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    class InMemoryLogin: ILoginRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public InMemoryLogin()
        {

        }

        public  Task<bool> IsLoginExpiredAsync(string subjectName)
        {
            throw new NotImplementedException();
        }

        public  Task<bool> CheckPreviousPasswordsAsync(int subjectId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public  Task<bool> GetAllowedAccessAsync(string username)
        {
            throw new NotImplementedException();
        }

        public  Task AcceptNewTermsOfReferernceAsync(string username)
        {
            throw new NotImplementedException();
        }

        public  Task SetPasswordAsync(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays)
        {
            throw new NotImplementedException();
        }
    }
}
