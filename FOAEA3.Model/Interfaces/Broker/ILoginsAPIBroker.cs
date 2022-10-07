using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILoginsAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<TokenData> LoginAsync(FoaeaLoginData loginData);
        Task<string> LoginVerificationAsync(FoaeaLoginData loginData);
        Task<string> LogoutAsync(FoaeaLoginData loginData);
        Task<TokenData> RefreshTokenAsync(string oldToken, string oldRefreshToken);
    }
}
