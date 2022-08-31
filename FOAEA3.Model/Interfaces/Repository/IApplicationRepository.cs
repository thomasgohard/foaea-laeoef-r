using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> ApplicationExistsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<bool> CreateApplicationAsync(ApplicationData application);
        Task UpdateApplicationAsync(ApplicationData application);
        Task<string> GenerateApplicationControlCodeAsync(string appl_EnfSrv_Cd);
        Task<ApplicationData> GetApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<List<ApplicationData>> GetApplicationsForAutomationAsync(string appl_EnfSrv_Cd, string medium_Cd,
                                                           ApplicationState appLiSt_Cd, string appCtgy_Cd,
                                                           string actvSt_Cd);
        Task<bool> GetApplLocalConfirmedSINExistsAsync(string enteredSIN, string debtorSurname, DateTime? debtorBirthDate, string submCd, string ctrlCd, string debtorFirstName = "");
        Task<List<ApplicationConfirmedSINData>> GetConfirmedSINOtherEnforcementOfficeExistsAsync(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN);
        Task<(string errorSameEnfOFf, string errorDiffEnfOff)> GetConfirmedSINRecordsAsync(string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN);
        Task<bool> GetConfirmedSINSameEnforcementOfficeExistsAsync(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN, string categoryCode);
        Task<DataList<ApplicationData>> GetRequestedSINApplDataForFileAsync(string enfSrv_Cd, string fileName);
        Task<List<ApplicationData>> GetDailyApplCountBySINAsync(string appl_Dbtr_Entrd_SIN, string appl_EnfSrv_Cd, string appl_CtrlCd, string appCtgy_Cd, string appl_Source_RfrNr);
        Task<List<ApplicationData>> GetSameCreditorForAppCtgyAsync(string appl_CtrlCd, string subm_SubmCd, string appl_Dbtr_Entrd_SIN, byte appl_SIN_Cnfrmd_Ind, string actvSt_Cd, string appCtgy_Cd);
        Task UpdateSubmitterDefaultControlCodeAsync(string subm_SubmCd, string appl_CtrlCd);
        Task<List<StatsOutgoingProvincialData>> GetStatsProvincialOutgoingDataAsync(int maxRecords, string activeState, string recipientCode,
                                                                         bool isXML = true);
    }
}