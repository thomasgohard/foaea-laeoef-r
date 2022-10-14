using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static FOAEA3.API.Tests.TestHelper;

namespace FOAEA3.API.Tests
{
    public class LoginsControllerTests
    {
        private readonly FoaeaApi _app;
        readonly IConfiguration _config;

        public LoginsControllerTests()
        {
            _app = new FoaeaApi();
            _config = TestHelper.InitConfiguration();
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

            FoaeaLoginData userLoginInfo = TestHelper.GetSystemUser(_config);

            var tokenData = await TestHelper.LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetStringAsync("/api/v1/logins/Version");

            Assert.StartsWith("Logins API Version", response, StringComparison.InvariantCultureIgnoreCase);
        }


        [Fact]
        public async Task TestDB()
        {
            var client = _app.CreateClient();

            FoaeaLoginData userLoginInfo = TestHelper.GetSystemUser(_config);

            var tokenData = await TestHelper.LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetStringAsync("/api/v1/logins/DB");

            string connectionString = "Server=%FOAEA_DB_SERVER%;Database=FOAEA_DEV;Integrated Security=SSPI;Trust Server Certificate=true;";
            connectionString = connectionString.ReplaceVariablesWithEnvironmentValues();
            Assert.StartsWith(connectionString, response, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task TestSystemLogin()
        {
            var client = _app.CreateClient();

            var userLoginInfo = TestHelper.GetSystemUser(_config);

            var tokenData = await TestHelper.LoginToFoaeaAsync(userLoginInfo, client);

            Assert.NotNull(tokenData);
            Assert.True(tokenData.TokenExpiration > DateTime.Now);
            Assert.True(tokenData.RefreshTokenExpiration > DateTime.Now);
        }

        [Fact]
        public async Task TestUser1Login()
        {
            var client = _app.CreateClient();

            var userLoginInfo = TestHelper.GetTestUser(_config, TEST_USER.Ontario);

            var tokenData = await TestHelper.LoginToFoaeaAsync(userLoginInfo, client);

            Assert.NotNull(tokenData);
            Assert.True(tokenData.TokenExpiration > DateTime.Now);
            Assert.True(tokenData.RefreshTokenExpiration > DateTime.Now);
        }

        [Fact]
        public async Task TestVerify()
        {
            var client = _app.CreateClient();

            var userLoginInfo = TestHelper.GetSystemUser(_config);

            var tokenData = await TestHelper.LoginToFoaeaAsync(userLoginInfo, client);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);

            var response = await client.PostAsync("/api/v1/logins/TestVerify", null);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.StartsWith($"Logged in user: {userLoginInfo.UserName}", responseContent, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task TestRefreshToken()
        {
            var client = _app.CreateClient();

            var userLoginInfo = TestHelper.GetSystemUser(_config);

            var tokenData = await TestHelper.LoginToFoaeaAsync(userLoginInfo, client);
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

    }
}
