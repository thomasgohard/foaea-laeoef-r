using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialTerminationApplicationAPIBroker
    {
        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
