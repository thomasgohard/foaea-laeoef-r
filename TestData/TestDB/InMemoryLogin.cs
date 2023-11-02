using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    class InMemoryLogin : ILoginRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public InMemoryLogin()
        {

        }

        public Task<bool> IsLoginExpired(string subjectName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckPreviousPasswords(int subjectId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetAllowedAccess(string username)
        {
            throw new NotImplementedException();
        }

        public Task AcceptNewTermsOfReferernce(string username)
        {
            throw new NotImplementedException();
        }

        public Task SetPassword(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays)
        {
            throw new NotImplementedException();
        }

        public Task PostConfirmationCode(int subjectId, string confirmationCode)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailByConfirmationCode(string confirmationCode)
        {
            throw new NotImplementedException();
        }

        public Task PostPassword(string confirmationCode, string password, string salt, string initial)
        {
            throw new NotImplementedException();
        }
    }
}
