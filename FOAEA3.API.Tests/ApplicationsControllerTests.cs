using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using static FOAEA3.API.Tests.TestHelper;

namespace FOAEA3.API.Tests
{
    public class ApplicationsControllerTests
    {
        private readonly FoaeaApi _app;
        readonly IConfiguration _config;

        public ApplicationsControllerTests()
        {
            _app = new FoaeaApi();
            _config = InitConfiguration();
        }

        [Fact]
        public async Task TestVersion()
        {
            var client = _app.CreateClient();

            FoaeaLoginData userLoginInfo = GetSystemUser(_config);

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetStringAsync("/api/v1/applications/Version");

            Assert.StartsWith("Applications API Version", response, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task TestDB()
        {
            var client = _app.CreateClient();

            FoaeaLoginData userLoginInfo = GetSystemUser(_config);

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetStringAsync("/api/v1/applications/DB");

            string connectionString = "Server=%FOAEA_DB_SERVER%;Database=FOAEA_DEV;Integrated Security=SSPI;Trust Server Certificate=true;";
            connectionString = connectionString.ReplaceVariablesWithEnvironmentValues();
            Assert.StartsWith(connectionString, response, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public async Task TestGetNFApplicationBySystemUser()
        {
            string enfSrv = "NF01";
            string ctrlCd = "032607";

            var client = _app.CreateClient();

            FoaeaLoginData userLoginInfo = GetSystemUser(_config);

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetAsync($"/api/v1/applications/{enfSrv}-{ctrlCd}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var appData = JsonConvert.DeserializeObject<ApplicationData>(responseContent);
            Assert.NotNull(appData);
            Assert.Equal(enfSrv, appData.Appl_EnfSrv_Cd.Trim(), true);
            Assert.Equal(ctrlCd, appData.Appl_CtrlCd.Trim(), true);
        }

        [Fact]
        public async Task TestGetNFApplicationByOntarioUser()
        {
            string enfSrv = "NF01";
            string ctrlCd = "032607";

            var client = _app.CreateClient();

            var userLoginInfo = GetTestUser(_config, TEST_USER.Ontario);

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetAsync($"/api/v1/applications/{enfSrv}-{ctrlCd}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestGetONApplicationByOntarioUser()
        {
            string enfSrv = "ON01";
            string ctrlCd = "P12347";

            var client = _app.CreateClient();

            var userLoginInfo = GetTestUser(_config, TEST_USER.Ontario);

            var tokenData = await LoginToFoaeaAsync(userLoginInfo, client);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenData.Token);
            var response = await client.GetAsync($"/api/v1/applications/{enfSrv}-{ctrlCd}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var appData = JsonConvert.DeserializeObject<ApplicationData>(responseContent);
            Assert.NotNull(appData);
            Assert.Equal(enfSrv, appData.Appl_EnfSrv_Cd.Trim(), false);
            Assert.Equal(ctrlCd, appData.Appl_CtrlCd.Trim(), false);
        }

    }
}
