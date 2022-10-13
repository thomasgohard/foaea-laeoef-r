using FluentAssertions;
using FOAEA3.Model;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FOAEA3.API.Tests
{
    public class LoginsControllerTests : IClassFixture<FoaeaApi>
    {
        readonly HttpClient _client;

        public LoginsControllerTests(FoaeaApi app)
        {
            _client = app.CreateClient();
           
        }

        [Fact]
        public async Task TestLogin()
        {
            var userLoginInfo = new FoaeaLoginData
            {
                //UserName = config["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                //Password = config["FOAEA:password"].ReplaceVariablesWithEnvironmentValues(),
                //Submitter = config["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
            };

            string keyData = JsonConvert.SerializeObject(userLoginInfo);
            var content = new StringContent(keyData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/v1/logins/TestLogin", content);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);            
        }

        [Fact]
        public async Task TestVersionNoLogin()
        {
            var response = await _client.GetAsync("/api/v1/logins/Version");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);            
        }

    }
}
