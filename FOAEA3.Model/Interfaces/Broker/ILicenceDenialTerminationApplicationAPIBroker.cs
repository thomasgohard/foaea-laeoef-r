using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialTerminationApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
