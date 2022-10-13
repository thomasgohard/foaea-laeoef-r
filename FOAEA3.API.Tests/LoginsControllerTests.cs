using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FOAEA3.API.Tests
{
    public class LoginsControllerTests
    {
        private readonly FoaeaApi _app;
        readonly IConfiguration _config;

        public LoginsControllerTests()
        {
            _app = new FoaeaApi();
            _config = InitConfiguration();
        }

        [Fact]
        public async Task TestVersionNotLoggedIn()
        {
            HttpClient client = _app.CreateClient();

            try
            {
                var response = await client.GetStringAsync("/api/v1/logins/Version");

                Assert.Fail("Should have generated an exception");
            }
            catch (Exception e)
            {
                Assert.Contains("401 (Unauthorized)", e.Message, StringComparison.InvariantCultureIgnoreCase);
            }

        }

        [Fact]
        public async Task TestVersionLoggedIn()
        {
            var client = _app.CreateClient();

            FoaeaLoginData userLoginInfo = GetDefaultSystemUser();

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetStringAsync("/api/v1/logins/Version");

            Assert.StartsWith("Logins API Version", response, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task TestLogin()
        {
            var client = _app.CreateClient();

            var userLoginInfo = GetDefaultSystemUser();

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            Assert.NotNull(tokenData);
            Assert.True(tokenData.TokenExpiration > DateTime.Now);
            Assert.True(tokenData.RefreshTokenExpiration > DateTime.Now);
        }

        [Fact]
        public async Task TestVerify()
        {
            var client = _app.CreateClient();

            var userLoginInfo = GetDefaultSystemUser();

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);

            var response = await client.PostAsync("/api/v1/logins/TestVerify", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.StartsWith($"Logged in user: {userLoginInfo.UserName}", responseContent, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task TestRefreshToken()
        {
            var client = _app.CreateClient();

            var userLoginInfo = GetDefaultSystemUser();

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);

            var refreshData = new TokenRefreshData
            {
                Token = tokenData.Token,
                RefreshToken = tokenData.RefreshToken
            };

            string keyData = JsonConvert.SerializeObject(refreshData);
            var content = new StringContent(keyData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/logins/TestRefreshToken", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var refreshedTokenData = JsonConvert.DeserializeObject<TokenData>(responseContent);
                Assert.NotNull(refreshedTokenData);
                Assert.NotEqual(tokenData.Token, refreshedTokenData.Token);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + refreshedTokenData.Token);

                var responseVerify = await client.PostAsync("/api/v1/logins/TestVerify", null);
                var responseVerifyContent = await responseVerify.Content.ReadAsStringAsync();

                Assert.StartsWith($"Logged in user: {userLoginInfo.UserName}", responseVerifyContent, StringComparison.InvariantCultureIgnoreCase);
            }
            else
                Assert.Fail($"Status code return was not 200 OK ({response.StatusCode})");
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.test.json")
                            .AddEnvironmentVariables()
                            .Build();
            return config;
        }

        private FoaeaLoginData GetDefaultSystemUser()
        {
            return new FoaeaLoginData
            {
                UserName = _config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                Password = _config["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues(),
                Submitter = _config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
            };
        }

        private static async Task<TokenData> LoginToFoaeaAsync(FoaeaLoginData userLoginInfo, HttpClient client)
        {
            string keyData = JsonConvert.SerializeObject(userLoginInfo);
            var content = new StringContent(keyData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/logins/TestLogin", content);
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
