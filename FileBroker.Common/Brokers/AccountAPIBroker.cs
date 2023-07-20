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
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public AccountAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/Tokens/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/Tokens/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<TokenData> CreateToken(FileBrokerModel.FileBrokerLoginData loginData)
        {
            string apiCall = $"api/v1/Tokens";
            return await ApiHelper.PostData<TokenData, FileBrokerModel.FileBrokerLoginData>(apiCall, loginData, token: Token);
        }
    }
}
