using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class EnfOfficesAPIBroker : IEnfOfficesAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public EnfOfficesAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<EnfOffData>> GetEnfOfficesForEnfServiceAsync(string enfService)
        {
            string apiCall = $"api/v1/EnfOffices?enfServCode={enfService}";
            var result = await ApiHelper.GetDataAsync<List<EnfOffData>>(apiCall, token: Token);
            return result;
        }
    }
}
