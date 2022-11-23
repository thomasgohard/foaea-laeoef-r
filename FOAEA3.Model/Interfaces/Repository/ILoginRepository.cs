using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ILoginRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> IsLoginExpiredAsync(string subjectName);
        Task<bool> CheckPreviousPasswordsAsync(int subjectId, string newPassword);
        Task<bool> GetAllowedAccessAsync(string username);
        Task AcceptNewTermsOfReferernceAsync(string username);
        Task SetPasswordAsync(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays);
    }
}