using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class LoginsAPIBroker : ILoginsAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LoginsAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/logins/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/logins/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1);
        }

        public async Task<List<Claim>> LoginAsync(LoginData2 loginData)
        {
            string apiCall = "api/v1/logins/testLogin";
            var data = await ApiHelper.PostDataAsync<List<Claim>, LoginData2>(apiCall, loginData);
            return data;
        }

        public async Task<string> LogoutAsync(LoginData2 loginData)
        {
            string apiCall = "api/v1/logins/testLogout";
            _ = await ApiHelper.PostDataAsync<List<string>, LoginData2>(apiCall, loginData);

            return string.Empty;
        }

    }
}
