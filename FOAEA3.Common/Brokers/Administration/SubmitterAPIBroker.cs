using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Administration
{
    public class SubmitterAPIBroker : ISubmitterAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public SubmitterAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<SubmitterData> GetSubmitterAsync(string submitterCode)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}";
            return await ApiHelper.GetDataAsync<SubmitterData>(apiCall, token: Token);
        }

        public async Task<List<ApplicationModificationActivitySummaryData>> GetRecentActivity(string submitterCode, int days = 0)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}/RecentActivity?days={days}";
            return await ApiHelper.GetDataAsync<List<ApplicationModificationActivitySummaryData>>(apiCall, token: Token);
        }

        public async Task<List<ApplicationModificationActivitySummaryData>> GetAllAtState(string submitterCode, ApplicationState state)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}/ApplicationsAtState/{(int)state}";
            return await ApiHelper.GetDataAsync<List<ApplicationModificationActivitySummaryData>>(apiCall, token: Token);
        }

        public async Task<List<string>> GetSubmitterCodesForOffice(string service, string office)
        {
            string apiCall = $"api/v1/Submitters/office/{office}?service={service}";
            return await ApiHelper.GetDataAsync<List<string>>(apiCall, token: Token);
        }

    }
}
