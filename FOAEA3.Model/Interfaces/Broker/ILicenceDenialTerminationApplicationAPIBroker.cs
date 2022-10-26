using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialTerminationApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<LicenceDenialApplicationData> CreateLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> CancelLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> UpdateLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<LicenceDenialApplicationData> TransferLicenceDenialTerminationApplicationAsync(LicenceDenialApplicationData licenceDenialApplication,
                                                                           string newRecipientSubmitter,
                                                                           string newIssuingSubmitter);
    }
}
