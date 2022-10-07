using FileBroker.Model.Interfaces.Broker;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Threading.Tasks;
using FileBrokerModel = FileBroker.Model;

namespace FileBroker.Common.Brokers
{
    public class AccountAPIBroker : IAccountAPIBroker, IVersionSupport
    {
        private IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public AccountAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/Tokens/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/Tokens/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<TokenData> CreateTokenAsync(FileBrokerModel.FileBrokerLoginData loginData)
        {
            string apiCall = $"api/v1/Tokens";
            return await ApiHelper.PostDataAsync<TokenData, FileBrokerModel.FileBrokerLoginData>(apiCall, loginData, token: Token);
        }
    }
}
