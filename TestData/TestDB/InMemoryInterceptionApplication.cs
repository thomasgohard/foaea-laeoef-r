using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryInterception : IInterceptionRepository
    {
        public string CurrentSubmitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task CreateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task CreateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task DeleteHoldbackConditionAsync(HoldbackConditionData holdbackCondition)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task DeleteHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task DeleteInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<string> EISOHistoryDeleteBySINAsync(string confirmedSIN, bool removeSIN)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<InterceptionApplicationData>> FindMatchingActiveApplicationsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, string confirmedSIN, string creditorFirstName, string creditorSurname)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<HoldbackConditionData>> GetAllHoldbackConditionsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<InterceptionFinancialHoldbackData>> GetAllInterceptionFinancialTermsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<string> GetApplicationJusticeNumberAsync(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<string> GetDebtorIDAsync(string first3Char)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ProcessEISOOUTHistoryData>> GetEISOHistoryBySINAsync(string confirmedSIN)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ExGratiaListData>> GetExGratiasAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<DateTime> GetGarnisheeSummonsReceiptDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<HoldbackConditionData>> GetHoldbackConditionsAsync(string enfService, string controlCode, DateTime intFinH_Date, string activeState = "A")
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<HoldbackTypeData>> GetHoldbackTypesAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionFinancialHoldbackData> GetInterceptionFinancialTermsAsync(string enfService, string controlCode, string activeState = "A")
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<PaymentPeriodData>> GetPaymentPeriodsAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<InterceptionApplicationData>> GetSameCreditorForI01Async(string appl_CtrlCd, string submCd, string enteredSIN, byte confirmedSIN, string activeState)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<int> GetTotalActiveSummonsAsync(string appl_EnfSrv_Cd, string enfOfficeCode)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task InsertBalanceSnapshotAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount, BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task InsertESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired, DateTime? esdReceivedDate = null)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> IsAlreadyUsedJusticeNumberAsync(string justiceNumber)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> IsFeeCumulativeForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            return await Task.Run(() =>
            {
                return false;
            });
        }

        public async Task<bool> IsVariationIncreaseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
