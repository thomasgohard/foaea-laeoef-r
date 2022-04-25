using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationAPIBroker
    {
        ApplicationData GetApplication(string appl_EnfSrvCd, string appl_CtrlCd);
        ApplicationData SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData);

        List<StatsOutgoingProvincialData> GetOutgoingProvincialStatusData(int maxRecords, string activeState,
                                                                          string recipientCode);
    }
}
