using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FOAEA3.API.Tests
{
    public static class TestHelper
    {
        public enum TEST_USER
        {
            Ontario = 1
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.test.json")
                            .AddEnvironmentVariables()
                            .Build();
            return config;
        }

        public static FoaeaLoginData GetSystemUser(IConfiguration config)
        {
            return new FoaeaLoginData
            {
                UserName = config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                Password = config["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues(),
                Submitter = config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
            };
        }

        public static FoaeaLoginData GetTestUser(IConfiguration config, TEST_USER thisUser)
        {
            int userNumber = (int)thisUser;

            return new FoaeaLoginData
            {
                UserName = config[$"TEST_USER{userNumber}:userName"].ReplaceVariablesWithEnvironmentValues(),
                Password = config[$"TEST_USER{userNumber}:userPassword"].ReplaceVariablesWithEnvironmentValues(),
                Submitter = config[$"TEST_USER{userNumber}:submitter"].ReplaceVariablesWithEnvironmentValues()
            };
        }

        public static async Task<TokenData> LoginToFoaea(FoaeaLoginData userLoginInfo, HttpClient client)
        {
            string keyData = JsonConvert.SerializeObject(userLoginInfo);
            var content = new StringContent(keyData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/logins/SingleStepLogin", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tokenData = JsonConvert.DeserializeObject<TokenData>(responseContent);
                return tokenData;
            }
            else
                return null;
        }
    }
}
