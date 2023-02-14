using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class ApplicationReasonsAPIBroker : IApplicationReasonsAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ApplicationReasonsAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<ApplicationReasonData>> GetApplicationReasonsAsync()
        {
            string apiCall = $"api/v1/ApplicationReasons";
            var result = await ApiHelper.GetDataAsync<DataList<ApplicationReasonData>>(apiCall, token: Token);
            return result.Items;
        }
    }
}
