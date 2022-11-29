using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialTerminationApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<LicenceDenialApplicationData> CreateLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData appData,
                                                                                          string controlCodeForL01);
        Task<LicenceDenialApplicationData> CancelLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData appData);
        Task<LicenceDenialApplicationData> UpdateLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData appData);
        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<LicenceDenialApplicationData> TransferLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData appData,
                                                                           string newRecipientSubmitter,
                                                                           string newIssuingSubmitter);
    }
}
