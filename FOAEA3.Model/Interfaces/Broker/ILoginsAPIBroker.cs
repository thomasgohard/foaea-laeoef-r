using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILoginsAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<TokenData> SubjectLogin(FoaeaLoginData loginData);
        Task<List<string>> GetAvailableSubmitters();
        Task<TokenData> SelectSubmitter(string submitter);
        Task<TokenData> AcceptTerms();
        Task<TokenData> Login(FoaeaLoginData loginData);
        Task<string> LoginVerification(FoaeaLoginData loginData);
        Task<string> Logout(FoaeaLoginData loginData);
        Task<TokenData> RefreshToken(string oldToken, string oldRefreshToken);
    }
}
