using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Business.Security
{
    internal class LoginManager
    {
        private readonly IRepositories Repositories;
        private const int PASSWORD_EXPIRY_DAYS = 180;
        private const int PASSWORD_FORMAT = 1;

        internal LoginManager(IRepositories repositories)
        {
            Repositories = repositories;
        }
        public static bool ValidateLogins = true;

        //private SubjectData GetSubjectLoginCredentials(string subjectName)
        //{
        //    return (Repositories.LoginRepository.GetSubject(subjectName));
        //}

        internal bool LoginIsAccountExpired(string subjectName)
        {
            return Repositories.LoginRepository.IsLoginExpired(subjectName);
        }
        //internal bool ValidateCredentials(string subjectName, string password)
        //{
        //    SubjectData subject = GetSubject(subjectName);
        //    return PasswordHelper.IsValidPassword(password, subject.PasswordSalt, subject.Password);
        //}
        internal bool CheckPreviousPasswords(string subjectName, string newPassword)
        {
            SubjectData subject = GetSubject(subjectName);
            return Repositories.LoginRepository.CheckPreviousPasswords(subject.SubjectId, newPassword);

        }
        internal void GetAllowedAccess(string username, ref bool IsAllowed)
        {
            Repositories.LoginRepository.GetAllowedAccess(username, ref IsAllowed);
        }
        internal void AcceptNewTermsOfReferernce(string username)
        {
            Repositories.LoginRepository.AcceptNewTermsOfReferernce(username);
        }

        internal bool SetPassword(string username, string password, string passwordSalt)
        {
            SubjectData subject = GetSubject(username);
            if (subject.SubjectName == null)
            {
                return false;
            }
            else
            {
                Repositories.LoginRepository.SetPassword(username, password, PASSWORD_FORMAT, passwordSalt, PASSWORD_EXPIRY_DAYS);
                return true;
            }

        }

        internal void SendEmail(string subject, string recipient, string body, int isHTML = 1)
        {
            var dbNotification = new DBNotification(Repositories.MainDB);
            dbNotification.SendEmail(subject, recipient, body, isHTML);
        }

        internal SubjectData GetSubject(string subjectName)
        {
            //int subjectID = GetSubjectLoginCredentials(subjectName).SubjectId;
            return (Repositories.SubjectRepository.GetSubject(subjectName));
        }

        internal SubjectData GetSubjectByConfirmationCode(string confirmationCode)
        {
            //int subjectID = GetSubjectLoginCredentials(subjectName).SubjectId;
            return (Repositories.SubjectRepository.GetSubjectByConfirmationCode(confirmationCode));
        }

    }
}
