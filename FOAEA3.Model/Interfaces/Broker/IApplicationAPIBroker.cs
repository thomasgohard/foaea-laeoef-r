using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<ApplicationData> GetApplicationAsync(string appl_EnfSrvCd, string appl_CtrlCd);
        Task<ApplicationData> SinConfirmationAsync(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData);

        Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusDataAsync(int maxRecords, string activeState,
                                                                          string recipientCode);

        Task<ApplicationData> ValidateCoreValuesAsync(ApplicationData application);
    }
}
