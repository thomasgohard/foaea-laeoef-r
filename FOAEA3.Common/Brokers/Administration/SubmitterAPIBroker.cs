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

        public async Task<SubmitterData> GetSubmitter(string submitterCode)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}";
            return await ApiHelper.GetData<SubmitterData>(apiCall, token: Token);
        }

        public async Task<List<ApplicationModificationActivitySummaryData>> GetRecentActivity(string submitterCode, int days = 0)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}/RecentActivity?days={days}";
            return await ApiHelper.GetData<List<ApplicationModificationActivitySummaryData>>(apiCall, token: Token);
        }

        public async Task<List<ApplicationModificationActivitySummaryData>> GetAllAtState(string submitterCode, ApplicationState state)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}/ApplicationsAtState/{(int) state}";
            return await ApiHelper.GetData<List<ApplicationModificationActivitySummaryData>>(apiCall, token: Token);
        }

        public async Task<List<ApplicationModificationActivitySummaryData>> GetAllWithEvent(string submitterCode, EventCode eventReasonCode)
        {
            string apiCall = $"api/v1/Submitters/{submitterCode}/ApplicationsWithEvent/{(int) eventReasonCode}";
            return await ApiHelper.GetData<List<ApplicationModificationActivitySummaryData>>(apiCall, token: Token);
        }

        public async Task<List<string>> GetSubmitterCodesForOffice(string service, string office)
        {
            string apiCall = $"api/v1/Submitters/office/{office}?service={service}";
            return await ApiHelper.GetData<List<string>>(apiCall, token: Token);
        }

    }
}
