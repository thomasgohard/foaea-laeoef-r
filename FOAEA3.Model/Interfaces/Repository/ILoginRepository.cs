using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ILoginRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> IsLoginExpired(string subjectName);
        Task<bool> CheckPreviousPasswords(int subjectId, string newPassword);
        Task<bool> GetAllowedAccess(string username);
        Task AcceptNewTermsOfReferernce(string username);
        Task SetPassword(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays);
        Task PostConfirmationCode(int subjectId, string confirmationCode);
        Task<string> GetEmailByConfirmationCode(string confirmationCode);
        Task PostPassword(string confirmationCode, string password, string salt, string initial);
    }
}