using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryInterception : IInterceptionRepository
    {
        public string CurrentSubmitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<ElectronicSummonsDocumentZipData> CreateESDasync(int processId, string fileName, DateTime dateReceived)
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentPdfData> CreateESDPDFasync(ElectronicSummonsDocumentPdfData newPDFentry)
        {
            throw new NotImplementedException();
        }

        public Task CreateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            throw new NotImplementedException();
        }

        public Task CreateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            throw new NotImplementedException();
        }

        public Task DeleteHoldbackConditionAsync(HoldbackConditionData holdbackCondition)
        {
            throw new NotImplementedException();
        }

        public Task DeleteHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            throw new NotImplementedException();
        }

        public Task DeleteInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            throw new NotImplementedException();
        }

        public Task<string> EISOHistoryDeleteBySINAsync(string confirmedSIN, bool removeSIN)
        {
            throw new NotImplementedException();
        }

        public Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> FindMatchingActiveApplicationsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, string confirmedSIN, string creditorFirstName, string creditorSurname)
        {
            throw new NotImplementedException();
        }

        public Task FTBatchNotification_CheckFTTransactionsAddedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldbackConditionData>> GetAllHoldbackConditionsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionFinancialHoldbackData>> GetAllInterceptionFinancialTermsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetApplicationJusticeNumberAsync(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationData>> GetApplicationsForRejectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> GetAutoAcceptGarnisheeOverrideDataAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> GetDateLastUIBatchLoaded()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDebtorIDAsync(string first3Char)
        {
            throw new NotImplementedException();
        }

        public Task<List<EIoutgoingFederalData>> GetEIoutgoingData(string enfSrv)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessEISOOUTHistoryData>> GetEISOHistoryBySINAsync(string confirmedSIN)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentZipData> GetESDasync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ElectronicSummonsDocumentRequiredData>> GetESDrequiredAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<ExGratiaListData>> GetExGratiasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> GetGarnisheeSummonsReceiptDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD)
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldbackConditionData>> GetHoldbackConditionsAsync(string enfService, string controlCode, DateTime intFinH_Date, string activeState = "A")
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldbackTypeData>> GetHoldbackTypesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionFinancialHoldbackData> GetInterceptionFinancialTermsAsync(string enfService, string controlCode, string activeState = "A")
        {
            throw new NotImplementedException();
        }

        public Task<List<PaymentPeriodData>> GetPaymentPeriodsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> GetSameCreditorForI01Async(string appl_CtrlCd, string submCd, string enteredSIN, byte confirmedSIN, string activeState)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationData>> GetTerminatedI01Async()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalActiveSummonsAsync(string appl_EnfSrv_Cd, string enfOfficeCode)
        {
            throw new NotImplementedException();
        }

        public Task InsertBalanceSnapshotAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount, BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null)
        {
            throw new NotImplementedException();
        }

        public Task InsertESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired, DateTime? esdReceivedDate = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAlreadyUsedJusticeNumberAsync(string justiceNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsFeeCumulativeForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            return Task.FromResult(false);
        }

        public Task<(bool, DateTime)> IsNewESDreceivedAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRefNumberBlockedAsync(string appl_Source_RfrNr)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSinBlockedAsync(string appl_Dbtr_Entrd_SIN)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsVariationIncreaseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task MessageBrokerCRAReconciliationAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            throw new NotImplementedException();
        }

        public Task UpdateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            throw new NotImplementedException();
        }
    }
}
