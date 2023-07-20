using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class SubmitterProfileAPIBroker : ISubmitterProfileAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public SubmitterProfileAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<SubmitterProfileData> GetSubmitterProfile(string submitterCode)
        {
            string apiCall = $"api/v1/SubmitterProfiles/{submitterCode}";
            return await ApiHelper.GetData<SubmitterProfileData>(apiCall, token: Token);
        }
    }
}
