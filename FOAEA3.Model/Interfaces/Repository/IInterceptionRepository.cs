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

        Task<List<InterceptionApplicationData>> FindMatchingActiveApplications(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                        string confirmedSIN, string creditorFirstName,
                                                                        string creditorSurname);

        Task<bool> IsFeeCumulativeForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<bool> IsVariationIncrease(string appl_EnfSrv_Cd, string appl_CtrlCd);

        Task<InterceptionFinancialHoldbackData> GetInterceptionFinancialTerms(string enfService, string controlCode, string activeState = "A");
        Task<List<InterceptionFinancialHoldbackData>> GetAllInterceptionFinancialTerms(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH);
        Task UpdateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH);
        Task DeleteInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH);

        Task<List<HoldbackConditionData>> GetHoldbackConditions(string enfService, string controlCode, DateTime intFinH_Date, string activeState = "A");
        Task<List<HoldbackConditionData>> GetAllHoldbackConditions(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateHoldbackConditions(List<HoldbackConditionData> holdbackConditions);
        Task UpdateHoldbackConditions(List<HoldbackConditionData> holdbackConditions);
        Task DeleteHoldbackConditions(List<HoldbackConditionData> holdbackConditions);
        Task DeleteHoldbackCondition(HoldbackConditionData holdbackCondition);

        Task<List<InterceptionApplicationData>> GetSameCreditorForI01(string appl_CtrlCd, string submCd, string enteredSIN, byte confirmedSIN,
                                                                      string activeState);
        Task<List<ExGratiaListData>> GetExGratias();
        Task<string> GetApplicationJusticeNumber(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<string> GetDebtorId(string first3Char);
        Task<string> GetDebtorIdByConfirmedSin(string sin, string category);
        Task<bool> CheckDebtorIdExists(string debtorId);
        Task<bool> IsAlreadyUsedJusticeNumber(string justiceNumber);
        Task<DateTime> GetGarnisheeSummonsReceiptDate(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD);
        Task<int> GetTotalActiveSummons(string appl_EnfSrv_Cd, string enfOfficeCode);
        Task<string> EISOHistoryDeleteBySIN(string confirmedSIN, bool removeSIN);
        Task<List<ProcessEISOOUTHistoryData>> GetEISOHistoryBySIN(string confirmedSIN);
        Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications();
        Task<List<EIoutgoingFederalData>> GetEIoutgoingData(string enfSrv);
        Task<DateTime> GetDateLastUIBatchLoaded();
        Task<List<ElectronicSummonsDocumentRequiredData>> GetESDrequired();
        Task<List<ApplicationData>> GetApplicationsForReject();
        Task<List<ApplicationData>> GetTerminatedI01();
        Task<ApplicationData> GetAutoAcceptGarnisheeOverrideData(string appl_EnfSrv_Cd, string appl_CtrlCd);

        Task<(bool, DateTime)> IsNewESDreceived(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired);
        Task InsertESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired, DateTime? esdReceivedDate = null);
        Task UpdateESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false);
        Task<ElectronicSummonsDocumentZipData> GetESD(string fileName);
        Task<ElectronicSummonsDocumentZipData> CreateESD(int processId, string fileName, DateTime dateReceived);

        Task InsertBalanceSnapshot(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount,
                                   BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null);

        Task<List<PaymentPeriodData>> GetPaymentPeriods();
        Task<List<HoldbackTypeData>> GetHoldbackTypes();
        Task<ElectronicSummonsDocumentPdfData> CreateESDPDF(ElectronicSummonsDocumentPdfData newPDFentry);
        Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task<bool> IsSinBlocked(string appl_Dbtr_Entrd_SIN);
        Task<bool> IsRefNumberBlocked(string appl_Source_RfrNr);

        Task MessageBrokerCRAReconciliation();
        Task FTBatchNotification_CheckFTTransactionsAdded();
    }
}
