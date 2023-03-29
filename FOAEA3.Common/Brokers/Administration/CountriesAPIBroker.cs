using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class CountriesAPIBroker : ICountriesAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public CountriesAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<CountryData>> GetCountriesAsync()
        {
            string apiCall = $"api/v1/Countries";
            var result = await ApiHelper.GetDataAsync<DataList<CountryData>>(apiCall, token: Token);
            return result.Items;
        }
    }
}
