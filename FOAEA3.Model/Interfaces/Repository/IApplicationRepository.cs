using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IApplicationRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> ApplicationExists(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<bool> CreateApplication(ApplicationData application);
        Task UpdateApplication(ApplicationData application);
        Task<string> GenerateApplicationControlCode(string appl_EnfSrv_Cd);
        Task<ApplicationData> GetApplication(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<List<ApplicationData>> GetApplicationsForAutomation(string appl_EnfSrv_Cd, string medium_Cd,
                                                           ApplicationState appLiSt_Cd, string appCtgy_Cd,
                                                           string actvSt_Cd);
        Task<List<ApplicationData>> GetApplicationsForCategoryAndLifeState(string category, ApplicationState lifeState);
        Task<List<ApplicationData>> GetApplicationsForSin(string confirmedSIN);
        Task<List<ApplicationModificationActivitySummaryData>> GetApplicationRecentActivityForSubmitter(string submCd, int days = 0);
        Task<List<ApplicationModificationActivitySummaryData>> GetApplicationAtStateForSubmitter(string submCd, ApplicationState state);
        Task<List<ApplicationModificationActivitySummaryData>> GetApplicationWithEventForSubmitter(string submCd, int eventReasonCode);
        Task<bool> GetApplLocalConfirmedSINExists(string enteredSIN, string debtorSurname, DateTime? debtorBirthDate, string submCd, string ctrlCd, string debtorFirstName = "");
        Task<List<ApplicationConfirmedSINData>> GetConfirmedSINOtherEnforcementOfficeExists(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN);
        Task<(string errorSameEnfOFf, string errorDiffEnfOff)> GetConfirmedSINRecords(string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN);
        Task<bool> GetConfirmedSINSameEnforcementOfficeExists(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN, string categoryCode);
        Task<List<ConfirmedSinData>> GetConfirmedSinByDebtorId(string debtorId, bool isActiveOnly);
        Task<DataList<ApplicationData>> GetRequestedSINApplDataForFile(string enfSrv_Cd, string fileName);
        Task<List<ApplicationData>> GetDailyApplCountBySIN(string appl_Dbtr_Entrd_SIN, string appl_EnfSrv_Cd, string appl_CtrlCd, string appCtgy_Cd, string appl_Source_RfrNr);
        Task<List<ApplicationData>> GetSameCreditorForAppCtgy(string appl_CtrlCd, string subm_SubmCd, string appl_Dbtr_Entrd_SIN, byte appl_SIN_Cnfrmd_Ind, string actvSt_Cd, string appCtgy_Cd);
        Task UpdateSubmitterDefaultControlCode(string subm_SubmCd, string appl_CtrlCd);
        Task<List<StatsOutgoingProvincialData>> GetStatsProvincialOutgoingData(int maxRecords, string activeState, string recipientCode,
                                                                               bool isXML = true);
    }
}