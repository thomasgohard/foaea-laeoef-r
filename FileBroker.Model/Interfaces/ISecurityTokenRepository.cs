using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface ISecurityTokenRepository
    {
        Task CreateAsync(SecurityTokenData securityToken);
        Task<SecurityTokenData> GetTokenDataAsync(string token);
        Task MarkTokenAsExpired(string token);
    }
}
