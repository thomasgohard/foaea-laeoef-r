using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class FoaEventsAPIBroker : IFoaEventsAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public FoaEventsAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<FoaEventDataDictionary> GetFoaEvents()
        {
            string apiCall = $"api/v1/FoaEvents";
            var result = await ApiHelper.GetData<FoaEventDataDictionary>(apiCall, token: Token);
            return result;
        }
    }
}
