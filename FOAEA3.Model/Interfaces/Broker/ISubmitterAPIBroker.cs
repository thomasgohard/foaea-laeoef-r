using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ISubmitterAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<SubmitterData> GetSubmitterAsync(string submitterCode);
        Task<List<ApplicationModificationActivitySummaryData>> GetRecentActivity(string submitterCode, int days = 0);
        Task<List<ApplicationModificationActivitySummaryData>> GetAllAtState(string submitterCode, ApplicationState state);
    }
}
