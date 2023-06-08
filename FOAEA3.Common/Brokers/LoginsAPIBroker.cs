using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class LoginsAPIBroker : ILoginsAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public const string SYSTEM_SUBJECT = "System_Support";
        public const string SYSTEM_SUBMITTER = "MSGBRO";

        public LoginsAPIBroker(IAPIBrokerHelper apiHelper, string currentToken = null)
        {
            ApiHelper = apiHelper;
            Token = currentToken;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/logins/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/logins/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<TokenData> SubjectLogin(FoaeaLoginData loginData)
        {
            string apiCall = "api/v1/logins/SubjectLogin";
            var data = await ApiHelper.PostData<TokenData, FoaeaLoginData>(apiCall, loginData);
            return data;
        }

        public async Task<List<string>> GetAvailableSubmitters()
        {
            string apiCall = $"api/v1/logins/Submitters";
            var data = await ApiHelper.GetData<List<string>>(apiCall, token: Token);
            return data;
        }

        public async Task<TokenData> SelectSubmitter(string submitter)
        {
            string apiCall = $"api/v1/logins/SelectSubmitter?submitter={submitter}";
            var data = await ApiHelper.PutData<TokenData, FoaeaLoginData>(apiCall, new FoaeaLoginData(), token: Token);
            return data;
        }

         public async Task<TokenData> AcceptTerms()
        {
            string apiCall = $"api/v1/logins/AcceptTerms";
            var data = await ApiHelper.PutData<TokenData, FoaeaLoginData>(apiCall, new FoaeaLoginData(), token: Token);
            return data;
        }

        public async Task<TokenData> Login(FoaeaLoginData loginData)
        {
            string apiCall = "api/v1/logins/SingleStepLogin";
            var data = await ApiHelper.PostData<TokenData, FoaeaLoginData>(apiCall, loginData);
            return data;
        }

        public async Task<string> LoginVerification(FoaeaLoginData loginData)
        {
            string apiCall = "api/v1/logins/testVerify";
            var data = await ApiHelper.PostDataGetString<FoaeaLoginData>(apiCall, loginData, token: Token);
            return data;
        }


        public async Task<string> Logout(FoaeaLoginData loginData)
        {
            string apiCall = "api/v1/logins/testLogout";
            _ = await ApiHelper.PostData<List<string>, FoaeaLoginData>(apiCall, loginData, token: Token);

            return string.Empty;
        }

        public async Task<TokenData> RefreshToken(string oldToken, string oldRefreshToken)
        {
            var refreshData = new TokenRefreshData
            {
                Token = oldToken,
                RefreshToken = oldRefreshToken
            };

            string apiCall = "api/v1/logins/TestRefreshToken";
            var data = await ApiHelper.PostData<TokenData, TokenRefreshData>(apiCall, refreshData,
                                                                                  token: oldToken);

            return data;
        }

    }
}
