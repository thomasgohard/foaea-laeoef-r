using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILoginsAPIBroker
    {
        Task<TokenData> LoginAsync(LoginData2 loginData);
        Task<string> LoginVerificationAsync(LoginData2 loginData, string token);
        Task<string> LogoutAsync(LoginData2 loginData, string token);
    }
}
