using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class LoginsAPIBroker : ILoginsAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public LoginsAPIBroker(IAPIBrokerHelper apiHelper, string currentToken)
        {
            ApiHelper = apiHelper;
            Token = currentToken;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/logins/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/logins/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<TokenData> LoginAsync(LoginData2 loginData)
        {
            string apiCall = "api/v1/logins/testLogin";
            var data = await ApiHelper.PostDataAsync<TokenData, LoginData2>(apiCall, loginData);
            return data;
        }

        public async Task<string> LoginVerificationAsync(LoginData2 loginData)
        {
            string apiCall = "api/v1/logins/testVerify";
            var data = await ApiHelper.PostDataGetStringAsync<LoginData2>(apiCall, loginData, token: Token);
            return data;
        }


        public async Task<string> LogoutAsync(LoginData2 loginData)
        {
            string apiCall = "api/v1/logins/testLogout";
            _ = await ApiHelper.PostDataAsync<List<string>, LoginData2>(apiCall, loginData, token: Token);

            return string.Empty;
        }

        public async Task<TokenData> RefreshTokenAsync(string oldToken, string oldRefreshToken)
        {
            var refreshData = new TokenRefreshData
            {
                Token = oldToken,
                RefreshToken = oldRefreshToken              
            };

            string apiCall = "api/v1/logins/TestRefreshToken";
            var data = await ApiHelper.PostDataAsync<TokenData, TokenRefreshData>(apiCall, refreshData, 
                                                                                  token: oldToken);

            return data;
        }

    }
}
