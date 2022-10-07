using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILoginsAPIBroker
    {
        Task<TokenData> LoginAsync(FoaeaLoginData loginData);
        Task<string> LoginVerificationAsync(FoaeaLoginData loginData);
        Task<string> LogoutAsync(FoaeaLoginData loginData);
    }
}
