using FileBroker.Common.Brokers;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FileBroker.Common
{
    public static class TokenHelper
    {
        public static async Task<string> GetFoaeaApiTokenAsync(APIBrokerHelper applicationApiHelper,
                                                               IConfiguration config)
        {
            string token = string.Empty;
            var loginAPIs = new LoginsAPIBroker(applicationApiHelper, token);

            var loginData = new FoaeaLoginData
            {
                UserName = config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                Password = config["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues(),
                Submitter = config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
            };

            var result = await loginAPIs.LoginAsync(loginData);

            return result.Token;
        }

        public static async Task<string> GetFileBrokerApiTokenAsync(APIBrokerHelper fb_accountApiHelper,
                                                                    IConfiguration config)
        {
            string token = string.Empty;
            var accountAPIs = new AccountAPIBroker(fb_accountApiHelper, token);

            string userName = config["FILE_BROKER:userName"].ReplaceVariablesWithEnvironmentValues();
            string userPassword = config["FILE_BROKER:userPassword"].ReplaceVariablesWithEnvironmentValues();

            var loginData = new FileBrokerLoginData
            {
                UserName = userName,
                Password = userPassword
            };

            var tokenData = await accountAPIs.CreateTokenAsync(loginData);

            return tokenData.Token;
        }

    }
}
