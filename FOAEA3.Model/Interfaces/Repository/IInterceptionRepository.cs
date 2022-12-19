using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IInterceptionRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<InterceptionApplicationData>> FindMatchingActiveApplicationsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                        string confirmedSIN, string creditorFirstName,
                                                                        string creditorSurname);

        Task<bool> IsFeeCumulativeForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<bool> IsVariationIncreaseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);

        Task<InterceptionFinancialHoldbackData> GetInterceptionFinancialTermsAsync(string enfService, string controlCode, string activeState = "A");
        Task<List<InterceptionFinancialHoldbackData>> GetAllInterceptionFinancialTermsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH);
        Task UpdateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH);
        Task DeleteInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH);

        Task<List<HoldbackConditionData>> GetHoldbackConditionsAsync(string enfService, string controlCode, DateTime intFinH_Date, string activeState = "A");
        Task<List<HoldbackConditionData>> GetAllHoldbackConditionsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions);
        Task UpdateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions);
        Task DeleteHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions);
        Task DeleteHoldbackConditionAsync(HoldbackConditionData holdbackCondition);

        Task<List<InterceptionApplicationData>> GetSameCreditorForI01Async(string appl_CtrlCd, string submCd, string enteredSIN, byte confirmedSIN,
                                                                 string activeState);
        Task<List<ExGratiaListData>> GetExGratiasAsync();
        Task<string> GetApplicationJusticeNumberAsync(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<string> GetDebtorIdAsync(string first3Char);
        Task<string> GetDebtorIdByConfirmedSin(string sin, string category);
        Task<bool> CheckDebtorIdExists(string debtorId);
        Task<bool> IsAlreadyUsedJusticeNumberAsync(string justiceNumber);
        Task<DateTime> GetGarnisheeSummonsReceiptDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD);
        Task<int> GetTotalActiveSummonsAsync(string appl_EnfSrv_Cd, string enfOfficeCode);
        Task<string> EISOHistoryDeleteBySINAsync(string confirmedSIN, bool removeSIN);
        Task<List<ProcessEISOOUTHistoryData>> GetEISOHistoryBySINAsync(string confirmedSIN);
        Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications();
        Task<List<EIoutgoingFederalData>> GetEIoutgoingData(string enfSrv);
        Task<DateTime> GetDateLastUIBatchLoaded();
        Task<List<ElectronicSummonsDocumentRequiredData>> GetESDrequiredAsync();
        Task<List<ApplicationData>> GetApplicationsForRejectAsync();
        Task<List<ApplicationData>> GetTerminatedI01Async();
        Task<ApplicationData> GetAutoAcceptGarnisheeOverrideDataAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);

        Task<(bool, DateTime)> IsNewESDreceivedAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired);
        Task InsertESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired, DateTime? esdReceivedDate = null);
        Task UpdateESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false);
        Task<ElectronicSummonsDocumentZipData> GetESDasync(string fileName);
        Task<ElectronicSummonsDocumentZipData> CreateESDasync(int processId, string fileName, DateTime dateReceived);

        Task InsertBalanceSnapshotAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount,
                                   BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null);

        Task<List<PaymentPeriodData>> GetPaymentPeriodsAsync();
        Task<List<HoldbackTypeData>> GetHoldbackTypesAsync();
        Task<ElectronicSummonsDocumentPdfData> CreateESDPDFasync(ElectronicSummonsDocumentPdfData newPDFentry);
        Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<bool> IsSinBlockedAsync(string appl_Dbtr_Entrd_SIN);
        Task<bool> IsRefNumberBlockedAsync(string appl_Source_RfrNr);

        Task MessageBrokerCRAReconciliationAsync();
        Task FTBatchNotification_CheckFTTransactionsAddedAsync();
    }
}
