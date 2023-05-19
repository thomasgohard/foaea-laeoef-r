using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class ActiveStatusesAPIBroker : IActiveStatusesAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ActiveStatusesAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<ActiveStatusData>> GetActiveStatusesAsync()
        {
            string apiCall = $"api/v1/ActiveStatuses";
            var result = await ApiHelper.GetDataAsync<DataList<ActiveStatusData>>(apiCall, token: Token);
            return result.Items;
        }
    }
}
