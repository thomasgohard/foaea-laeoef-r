using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ILicenceDenialRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<LicenceDenialApplicationData> GetLicenceDenialData(string appl_EnfSrv_Cd, string appl_L01_CtrlCd = null, string appl_L03_CtrlCd = null);
        Task<List<LicenceSuspensionHistoryData>> GetLicenceSuspensionHistory(string appl_EnfSrv_Cd, string appl_CtrlCd);

        Task CreateLicenceDenialData(LicenceDenialApplicationData data);
        Task UpdateLicenceDenialData(LicenceDenialApplicationData data);

        Task<bool> CloseSameDayLicenceEvent(string appl_EnfSrv_Cd, string appl_L01_CtrlCd, string appl_L03_CtrlCd);

        Task<List<LicenceDenialOutgoingFederalData>> GetFederalOutgoingData(int maxRecords, string activeState,
                                                                            ApplicationState lifeState, string enfServiceCode);

        Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplData(string fedSource);

        Task<List<LicenceDenialOutgoingProvincialData>> GetProvincialOutgoingData(int maxRecords, string activeState,
                                                                                  string recipientCode, bool isXML = true);

        Task<List<SingleStringColumnData>> GetActiveLO1ApplsForDebtor(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
