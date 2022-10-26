using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<LicenceDenialApplicationData> CreateLicenceDenialApplicationAsync(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> UpdateLicenceDenialApplicationAsync(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        Task<List<LicenceDenialOutgoingFederalData>> GetOutgoingFederalLicenceDenialRequestsAsync(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode);

        Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplDataAsync(string fedSource);

        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<List<LicenceDenialOutgoingProvincialData>> GetOutgoingProvincialLicenceDenialDataAsync(int maxRecords, string activeState,
                                                                                         string recipientCode);
        Task<LicenceDenialApplicationData> TransferLicenceDenialApplicationAsync(LicenceDenialApplicationData licenceDenialApplication,
                                                                           string newRecipientSubmitter,
                                                                           string newIssuingSubmitter);

    }
}
