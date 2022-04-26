using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Model.Base;

namespace TestData.TestDB
{
    class InMemoryApplication : IApplicationRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public bool ApplicationExists(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public bool CreateApplication(ApplicationData application)
        {
            throw new NotImplementedException();
        }

        public string GenerateApplicationControlCode(string appl_EnfSrv_Cd)
        {
            throw new NotImplementedException();
        }

        public ApplicationData GetApplication(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public bool GetApplLocalConfirmedSINExists(string enteredSIN, string debtorSurname, DateTime? debtorBirthDate, string submCd, string ctrlCd, string debtorFirstName = "")
        {
            throw new NotImplementedException();
        }

        public List<ApplicationConfirmedSINData> GetConfirmedSINOtherEnforcementOfficeExists(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN)
        {
            throw new NotImplementedException();
        }

        public (string errorSameEnfOFf, string errorDiffEnfOff) GetConfirmedSINRecords(string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN)
        {
            throw new NotImplementedException();
        }

        public bool GetConfirmedSINSameEnforcementOfficeExists(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN, string categoryCode)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationData> GetDailyApplCountBySIN(string appl_Dbtr_Entrd_SIN, string appl_EnfSrv_Cd, string appl_CtrlCd, string appCtgy_Cd, string appl_Source_RfrNr)
        {
            throw new NotImplementedException();
        }

        public DataList<ApplicationData> GetRequestedSINApplDataForFile(string enfSrv_Cd, string fileName)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationData> GetSameCreditorForAppCtgy(string appl_CtrlCd, string subm_SubmCd, string appl_Dbtr_Entrd_SIN, byte appl_SIN_Cnfrmd_Ind, string actvSt_Cd, string appCtgy_Cd)
        {
            throw new NotImplementedException();
        }

        public List<StatsOutgoingProvincialData> GetStatsProvincialOutgoingData(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            throw new NotImplementedException();
        }

        public bool IsFeeCumulativeForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            return false;
        }

        public void UpdateApplication(ApplicationData application)
        {
            throw new NotImplementedException();
        }

        public void UpdateSubmitterDefaultControlCode(string subm_SubmCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }
    }
}
