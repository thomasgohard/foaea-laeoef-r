using FileBroker.Model;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FileBroker.Common.Helpers
{
    public class FileBrokerSystemAccess
    {
        public FileBrokerLoginData LoginData { get; }

        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIs { get; }

        public string CurrentToken { get; set; }
        public string CurrentRefreshToken { get; set; }

        public FileBrokerSystemAccess(IAPIBrokerHelper apiBrokerHelper, ApiConfig apiFilesConfig, string userName, string userPassword)
        {
            LoginData = new FileBrokerLoginData
            {
                UserName = userName,
                Password = userPassword
            };

            ApiFilesConfig = apiFilesConfig;
            APIs = apiBrokerHelper;
            APIs.GetRefreshedToken = RefreshTokenAsync;
        }

        public async Task SystemLoginAsync()
        {
            string api = "api/v1/Tokens";
            var token = await APIs.PostDataAsync<TokenData, FileBrokerLoginData>(api,
                                                              LoginData, ApiFilesConfig.FileBrokerAccountRootAPI);

            CurrentToken = token.Token;
            CurrentRefreshToken = token.RefreshToken;
        }

        public async Task SystemLogoutAsync()
        {
            var refreshData = new TokenRefreshData
            {
                Token = CurrentToken,
                RefreshToken = CurrentRefreshToken
            };

            string api = "api/v1/Tokens/ExpireToken";
            var token = await APIs.PostDataAsync<TokenData, TokenRefreshData>(api,
                                                              refreshData, ApiFilesConfig.FileBrokerAccountRootAPI);
        }

        private async Task<string> RefreshTokenAsync()
        {
            var refreshData = new TokenRefreshData
            {
                Token = CurrentToken,
                RefreshToken = CurrentRefreshToken
            };

            string api = "api/v1/Tokens/Refresh";
            var result = await APIs.PostDataAsync<TokenData, TokenRefreshData>(api,
                                                              refreshData, ApiFilesConfig.FileBrokerAccountRootAPI);

            CurrentToken = result.Token;
            CurrentRefreshToken = result.RefreshToken;

            return CurrentToken;
        }
    }
}
