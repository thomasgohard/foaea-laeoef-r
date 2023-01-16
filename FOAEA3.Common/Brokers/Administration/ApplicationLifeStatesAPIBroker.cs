using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{

    public class ApplicationLifeStatesAPIBroker : IApplicationLifeStatesAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ApplicationLifeStatesAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<ApplicationLifeStateData>> GetApplicationLifeStatesAsync()
        {
            string apiCall = $"api/v1/ApplicationLifeStates";
            var result = await ApiHelper.GetDataAsync<DataList<ApplicationLifeStateData>>(apiCall, token: Token);
            return result.Items;
        }
    }
}
