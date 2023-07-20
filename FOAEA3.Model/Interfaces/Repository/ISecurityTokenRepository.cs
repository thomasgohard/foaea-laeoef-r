using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISecurityTokenRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task Create(SecurityTokenData securityToken);
        Task<SecurityTokenData> GetTokenData(string token);
        Task MarkTokenAsExpired(string token);
    }
}