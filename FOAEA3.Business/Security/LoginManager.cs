using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    internal class LoginManager
    {
        private readonly IRepositories DB;
        private const int PASSWORD_EXPIRY_DAYS = 180;
        private const int PASSWORD_FORMAT = 1;

        public LoginManager(IRepositories repositories)
        {
            DB = repositories;
        }
        public static bool ValidateLogins = true;

        public async Task<bool> LoginIsAccountExpiredAsync(string subjectName)
        {
            return await DB.LoginTable.IsLoginExpiredAsync(subjectName);
        }

        public async Task<bool> CheckPreviousPasswordsAsync(string subjectName, string newPassword)
        {
            SubjectData subject = await GetSubjectAsync(subjectName);
            return await DB.LoginTable.CheckPreviousPasswordsAsync(subject.SubjectId, newPassword);

        }
        public async Task<bool> GetAllowedAccessAsync(string username)
        {
            return await DB.LoginTable.GetAllowedAccessAsync(username);
        }

        public async Task AcceptNewTermsOfReferernceAsync(string username)
        {
            await DB.LoginTable.AcceptNewTermsOfReferernceAsync(username);
        }

        public async Task<bool> SetPasswordAsync(string username, string password, string passwordSalt)
        {
            SubjectData subject = await GetSubjectAsync(username);
            if (subject.SubjectName == null)
            {
                return false;
            }
            else
            {
                await DB.LoginTable.SetPasswordAsync(username, password, PASSWORD_FORMAT, passwordSalt, PASSWORD_EXPIRY_DAYS);
                return true;
            }

        }

        public async Task SendEmailAsync(string subject, string recipient, string body, int isHTML = 1)
        {
            var dbNotification = new DBNotification(DB.MainDB);
            await dbNotification.SendEmailAsync(subject, recipient, body, isHTML);
        }

        public async Task<SubjectData> GetSubjectAsync(string subjectName)
        {
            return await DB.SubjectTable.GetSubjectAsync(subjectName);
        }

        public async Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode)
        {
            return await DB.SubjectTable.GetSubjectByConfirmationCodeAsync(confirmationCode);
        }

        public async Task PostConfirmationCodeAsync(int subjectId, string confirmationCode)
        {
            await DB.LoginTable.PostConfirmationCodeAsync(subjectId, confirmationCode);
        }

        public async Task<string> GetEmailByConfirmationCodeAsync(string confirmationCode)
        {
            return await DB.LoginTable.GetEmailByConfirmationCodeAsync(confirmationCode);
        }

        public async Task PostPasswordAsync(string confirmationCode, string password, string salt, string initial)
        {
            await DB.LoginTable.PostPasswordAsync(confirmationCode, password, salt, initial);
        }
    }
}
