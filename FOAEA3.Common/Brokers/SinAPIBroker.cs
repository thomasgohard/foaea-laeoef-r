using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class SinAPIBroker : ISinAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public SinAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task InsertBulkData(List<SINResultData> resultData)
        {
            _ = await ApiHelper.PostData<SINResultData, List<SINResultData>>("api/v1/applicationFederalSins/bulk",
                                                                             resultData, token: Token);
        }

        public async Task<List<SINOutgoingFederalData>> GetOutgoingFederalSins(int maxRecords, string activeState,
                                                                   int lifeState, string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalSins";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                             $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return await ApiHelper.GetData<List<SINOutgoingFederalData>>(apiCall, token: Token);
        }

    }
}
