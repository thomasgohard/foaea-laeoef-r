using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class SubmittersAPIBroker : ISubmittersAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public SubmittersAPIBroker(IAPIBrokerHelper apiHelper, string currentToken = null)
        {
            ApiHelper = apiHelper;
            Token = currentToken;
        }

        public async Task<string> GetFOAEAOfficersEmails()
        {
            string apiCall = "api/v1/submitters/FOAEAOfficersEmails";
            return await ApiHelper.GetString(apiCall, token: Token);
        }
    }
}
