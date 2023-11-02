using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialTerminationApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<LicenceDenialApplicationData> CreateLicenceDenialTerminationApplication(LicenceDenialApplicationData appData,
                                                                                     string controlCodeForL01);
        Task<LicenceDenialApplicationData> CancelLicenceDenialTerminationApplication(LicenceDenialApplicationData appData);
        Task<LicenceDenialApplicationData> UpdateLicenceDenialTerminationApplication(LicenceDenialApplicationData appData);
        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<LicenceDenialApplicationData> TransferLicenceDenialTerminationApplication(LicenceDenialApplicationData appData,
                                                                                       string newRecipientSubmitter,
                                                                                       string newIssuingSubmitter);
    }
}
