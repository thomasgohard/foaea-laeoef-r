using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class GendersAPIBroker : IGendersAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public GendersAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<GenderData>> GetGendersAsync()
        {
            string apiCall = $"api/v1/Genders";
            var result = await ApiHelper.GetDataAsync<DataList<GenderData>>(apiCall, token: Token);
            return result.Items;
        }
    }
}
