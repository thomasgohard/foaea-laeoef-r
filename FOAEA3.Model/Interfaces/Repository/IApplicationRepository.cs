using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        bool ApplicationExists(string appl_EnfSrv_Cd, string appl_CtrlCd);
        bool CreateApplication(ApplicationData application);
        void UpdateApplication(ApplicationData application);
        string GenerateApplicationControlCode(string appl_EnfSrv_Cd);
        ApplicationData GetApplication(string appl_EnfSrv_Cd, string appl_CtrlCd);
        bool GetApplLocalConfirmedSINExists(string enteredSIN, string debtorSurname, DateTime? debtorBirthDate, string submCd, string ctrlCd, string debtorFirstName = "");
        List<ApplicationConfirmedSINData> GetConfirmedSINOtherEnforcementOfficeExists(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN);
        (string errorSameEnfOFf, string errorDiffEnfOff) GetConfirmedSINRecords(string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN);
        bool GetConfirmedSINSameEnforcementOfficeExists(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN, string categoryCode);
        DataList<ApplicationData> GetRequestedSINApplDataForFile(string enfSrv_Cd, string fileName);
        List<ApplicationData> GetDailyApplCountBySIN(string appl_Dbtr_Entrd_SIN, string appl_EnfSrv_Cd, string appl_CtrlCd, string appCtgy_Cd, string appl_Source_RfrNr);
        List<ApplicationData> GetSameCreditorForAppCtgy(string appl_CtrlCd, string subm_SubmCd, string appl_Dbtr_Entrd_SIN, byte appl_SIN_Cnfrmd_Ind, string actvSt_Cd, string appCtgy_Cd);
        void UpdateSubmitterDefaultControlCode(string subm_SubmCd, string appl_CtrlCd);
        List<StatsOutgoingProvincialData> GetStatsProvincialOutgoingData(int maxRecords, string activeState, string recipientCode,
                                                                         bool isXML = true);
    }
}