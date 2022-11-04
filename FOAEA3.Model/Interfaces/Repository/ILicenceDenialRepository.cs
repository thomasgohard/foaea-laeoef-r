using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ILicenceDenialRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<LicenceDenialApplicationData> GetLicenceDenialDataAsync(string appl_EnfSrv_Cd, string appl_L01_CtrlCd = null, string appl_L03_CtrlCd = null);
        Task<List<LicenceSuspensionHistoryData>> GetLicenceSuspensionHistoryAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);

        Task CreateLicenceDenialDataAsync(LicenceDenialApplicationData data);
        Task UpdateLicenceDenialDataAsync(LicenceDenialApplicationData data);

        Task<bool> CloseSameDayLicenceEventAsync(string appl_EnfSrv_Cd, string appl_L01_CtrlCd, string appl_L03_CtrlCd);

        Task<List<LicenceDenialOutgoingFederalData>> GetFederalOutgoingDataAsync(int maxRecords,
                                                                string activeState,
                                                                ApplicationState lifeState,
                                                                string enfServiceCode);

        Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplDataAsync(string fedSource);

        Task<List<LicenceDenialOutgoingProvincialData>> GetProvincialOutgoingDataAsync(int maxRecords,
                                                                            string activeState,
                                                                            string recipientCode,
                                                                            bool isXML = true);

        Task<List<SingleStringColumnData>> GetActiveLO1ApplsForDebtor(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
