using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<LicenceDenialApplicationData> CreateLicenceDenialApplication(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> UpdateLicenceDenialApplication(LicenceDenialApplicationData licenceDenialData);
        Task<LicenceDenialApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        Task<List<LicenceDenialOutgoingFederalData>> GetOutgoingFederalLicenceDenialRequests(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode);

        Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplData(string fedSource);

        Task<LicenceDenialApplicationData> ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<List<LicenceDenialOutgoingProvincialData>> GetOutgoingProvincialLicenceDenialData(int maxRecords, string activeState,
                                                                                         string recipientCode);
        Task<LicenceDenialApplicationData> TransferLicenceDenialApplication(LicenceDenialApplicationData licenceDenialApplication,
                                                                           string newRecipientSubmitter,
                                                                           string newIssuingSubmitter);

    }
}
