using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISecurityTokenRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task CreateAsync(SecurityTokenData securityToken);
        Task<SecurityTokenData> GetTokenDataAsync(string token);
        Task MarkTokenAsExpired(string token);
    }
}