using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    internal class LoginManager
    {
        private readonly IRepositories Repositories;
        private const int PASSWORD_EXPIRY_DAYS = 180;
        private const int PASSWORD_FORMAT = 1;

        public LoginManager(IRepositories repositories)
        {
            Repositories = repositories;
        }
        public static bool ValidateLogins = true;

        public async Task<bool> LoginIsAccountExpiredAsync(string subjectName)
        {
            return await Repositories.LoginRepository.IsLoginExpiredAsync(subjectName);
        }

        public async Task<bool> CheckPreviousPasswordsAsync(string subjectName, string newPassword)
        {
            SubjectData subject = await GetSubjectAsync(subjectName);
            return await Repositories.LoginRepository.CheckPreviousPasswordsAsync(subject.SubjectId, newPassword);

        }
        public async Task<bool> GetAllowedAccessAsync(string username)
        {
            return await Repositories.LoginRepository.GetAllowedAccessAsync(username);
        }

        public async Task AcceptNewTermsOfReferernceAsync(string username)
        {
            await Repositories.LoginRepository.AcceptNewTermsOfReferernceAsync(username);
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
                await Repositories.LoginRepository.SetPasswordAsync(username, password, PASSWORD_FORMAT, passwordSalt, PASSWORD_EXPIRY_DAYS);
                return true;
            }

        }

        public async Task SendEmailAsync(string subject, string recipient, string body, int isHTML = 1)
        {
            var dbNotification = new DBNotification(Repositories.MainDB);
            await dbNotification.SendEmailAsync(subject, recipient, body, isHTML);
        }

        public async Task<SubjectData> GetSubjectAsync(string subjectName)
        {
            //int subjectID = GetSubjectLoginCredentials(subjectName).SubjectId;
            return (await Repositories.SubjectRepository.GetSubjectAsync(subjectName));
        }

        public async Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode)
        {
            //int subjectID = GetSubjectLoginCredentials(subjectName).SubjectId;
            return (await Repositories.SubjectRepository.GetSubjectByConfirmationCodeAsync(confirmationCode));
        }

    }
}
