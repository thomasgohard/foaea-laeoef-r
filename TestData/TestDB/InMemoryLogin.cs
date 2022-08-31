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

        public async Task<bool> IsLoginExpiredAsync(string subjectName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> CheckPreviousPasswordsAsync(int subjectId, string newPassword)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> GetAllowedAccessAsync(string username)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task AcceptNewTermsOfReferernceAsync(string username)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task SetPasswordAsync(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
