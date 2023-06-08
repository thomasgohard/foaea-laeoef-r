using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialEventAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<List<ApplicationEventData>> GetRequestedLICINEvents(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<List<ApplicationEventDetailData>> GetRequestedLICINEventDetails(string enfSrvCd, string appl_EnfSrv_Cd, string appl_CtrlCd);

    }
}
