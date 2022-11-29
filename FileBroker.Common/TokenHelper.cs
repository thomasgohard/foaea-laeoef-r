using FileBroker.Common.Brokers;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using System.Threading.Tasks;

namespace FileBroker.Common
{
    public static class TokenHelper
    {
        public static async Task<string> GetFoaeaApiTokenAsync(APIBrokerHelper applicationApiHelper,
                                                               IFileBrokerConfigurationHelper config)
        {
            string token = string.Empty;
            var loginAPIs = new LoginsAPIBroker(applicationApiHelper, token);

            var result = await loginAPIs.LoginAsync(config.FoaeaLogin);

            return result.Token;
        }

        public static async Task<string> GetFileBrokerApiTokenAsync(APIBrokerHelper fb_accountApiHelper,
                                                                    IFileBrokerConfigurationHelper config)
        {
            string token = string.Empty;
            var accountAPIs = new AccountAPIBroker(fb_accountApiHelper, token);

            var tokenData = await accountAPIs.CreateTokenAsync(config.FileBrokerLogin);

            return tokenData.Token;
        }

    }
}
