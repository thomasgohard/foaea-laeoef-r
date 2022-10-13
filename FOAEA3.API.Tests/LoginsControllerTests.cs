using FluentAssertions;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FOAEA3.API.Tests
{
    public class LoginsControllerTests : IClassFixture<FoaeaApi>
    {
        readonly HttpClient _client;
        readonly IConfiguration _config;

        public LoginsControllerTests(FoaeaApi app)
        {
            _client = app.CreateClient();
            _config = InitConfiguration();
        }

        [Fact]
        public async Task TestLogin()
        {
            var userLoginInfo = new FoaeaLoginData
            {
                UserName = _config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                Password = _config["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues(),
                Submitter = _config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
            };

            string keyData = JsonConvert.SerializeObject(userLoginInfo);
            var content = new StringContent(keyData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/v1/logins/TestLogin", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tokenData = JsonConvert.DeserializeObject<TokenData>(responseContent);
            if (tokenData is not null)
            {
                Assert.True(tokenData.TokenExpiration > DateTime.Now);
                Assert.True(tokenData.RefreshTokenExpiration > DateTime.Now);
            }
            else
                Assert.Fail("Missing tokendata expirations or invalid expiration dates");

        }

        [Fact]
        public async Task TestVersionNoLogin()
        {
            var response = await _client.GetAsync("/api/v1/logins/Version");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);            
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }
    }
}
