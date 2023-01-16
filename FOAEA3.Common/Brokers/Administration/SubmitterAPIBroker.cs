using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class SubmitterAPIBroker : ISubmitterAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public SubmitterAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<SubmitterData> GetSubmitterAsync(string submitterCode)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}";
            return await ApiHelper.GetDataAsync<SubmitterData>(apiCall, token: Token);
        }
    }
}
