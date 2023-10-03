using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<ApplicationData> GetApplication(string appl_EnfSrvCd, string appl_CtrlCd);
        Task<ApplicationData> SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData);
        Task<List<ApplicationData>> GetApplicationsForSin(string confirmedSIN);
        Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusData(int maxRecords, string activeState,
                                                                          string recipientCode);

        Task<ApplicationData> ValidateCoreValues(ApplicationData application);
    }
}
