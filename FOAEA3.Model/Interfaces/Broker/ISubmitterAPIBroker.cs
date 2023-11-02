using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ISubmitterAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<SubmitterData> GetSubmitter(string submitterCode);
        Task<List<ApplicationModificationActivitySummaryData>> GetRecentActivity(string submitterCode, int days = 0);
        Task<List<ApplicationModificationActivitySummaryData>> GetAllAtState(string submitterCode, ApplicationState state);
        Task<List<ApplicationModificationActivitySummaryData>> GetAllWithEvent(string submitterCode, EventCode eventReasonCode);
        Task<List<string>> GetSubmitterCodesForOffice(string service, string office);
    }
}
