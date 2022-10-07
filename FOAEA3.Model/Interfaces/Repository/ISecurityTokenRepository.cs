using FOAEA3.Model;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
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