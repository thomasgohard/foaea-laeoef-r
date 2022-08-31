using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    class InMemoryApplication : IApplicationRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }
        public async Task<bool> ApplicationExistsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> CreateApplicationAsync(ApplicationData application)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<string> GenerateApplicationControlCodeAsync(string appl_EnfSrv_Cd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<ApplicationData> GetApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationData>> GetApplicationsForAutomationAsync(string appl_EnfSrv_Cd, string medium_Cd, ApplicationState appLiSt_Cd, string appCtgy_Cd, string actvSt_Cd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> GetApplLocalConfirmedSINExistsAsync(string enteredSIN, string debtorSurname, DateTime? debtorBirthDate, string submCd, string ctrlCd, string debtorFirstName = "")
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationConfirmedSINData>> GetConfirmedSINOtherEnforcementOfficeExistsAsync(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<(string errorSameEnfOFf, string errorDiffEnfOff)> GetConfirmedSINRecordsAsync(string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> GetConfirmedSINSameEnforcementOfficeExistsAsync(string appl_EnfSrv_Cd, string subm_SubmCd, string appl_CtrlCd, string appl_Dbtr_Cnfrmd_SIN, string categoryCode)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationData>> GetDailyApplCountBySINAsync(string appl_Dbtr_Entrd_SIN, string appl_EnfSrv_Cd, string appl_CtrlCd, string appCtgy_Cd, string appl_Source_RfrNr)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<DataList<ApplicationData>> GetRequestedSINApplDataForFileAsync(string enfSrv_Cd, string fileName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationData>> GetSameCreditorForAppCtgyAsync(string appl_CtrlCd, string subm_SubmCd, string appl_Dbtr_Entrd_SIN, byte appl_SIN_Cnfrmd_Ind, string actvSt_Cd, string appCtgy_Cd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<StatsOutgoingProvincialData>> GetStatsProvincialOutgoingDataAsync(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateApplicationAsync(ApplicationData application)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateSubmitterDefaultControlCodeAsync(string subm_SubmCd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
