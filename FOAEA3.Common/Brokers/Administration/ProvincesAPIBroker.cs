using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class ProvincesAPIBroker : IProvincesAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ProvincesAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<ProvinceData>> GetProvincesAsync()
        {
            string apiCall = $"api/v1/Provinces";
            var result = await ApiHelper.GetDataAsync<DataList<ProvinceData>>(apiCall, token: Token);
            return result.Items;
        }

    }
}
