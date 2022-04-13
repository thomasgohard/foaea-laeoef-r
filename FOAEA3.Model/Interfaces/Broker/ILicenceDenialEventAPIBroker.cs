using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialEventAPIBroker
    {
        List<ApplicationEventData> GetRequestedLICINEvents(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd);
        List<ApplicationEventDetailData> GetRequestedLICINEventDetails(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd);

    }
}
