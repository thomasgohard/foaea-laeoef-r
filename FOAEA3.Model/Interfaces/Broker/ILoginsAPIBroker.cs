using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILoginsAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<TokenData> SubjectLoginAsync(FoaeaLoginData loginData);
        Task<List<string>> GetAvailableSubmittersAsync();
        Task<TokenData> SelectSubmitterAsync(string submitter);
        Task<TokenData> AcceptTerms();
        Task<TokenData> LoginAsync(FoaeaLoginData loginData);
        Task<string> LoginVerificationAsync(FoaeaLoginData loginData);
        Task<string> LogoutAsync(FoaeaLoginData loginData);
        Task<TokenData> RefreshTokenAsync(string oldToken, string oldRefreshToken);
    }
}
