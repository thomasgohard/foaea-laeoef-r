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

        public Task<bool> CheckDebtorIdExists(string debtorId)
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentZipData> CreateESD(int processId, string fileName, DateTime dateReceived)
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentPdfData> CreateESDPDF(ElectronicSummonsDocumentPdfData newPDFentry)
        {
            throw new NotImplementedException();
        }

        public Task CreateHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {
            throw new NotImplementedException();
        }

        public Task CreateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {
            throw new NotImplementedException();
        }

        public Task DeleteHoldbackCondition(HoldbackConditionData holdbackCondition)
        {
            throw new NotImplementedException();
        }

        public Task DeleteHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {
            throw new NotImplementedException();
        }

        public Task DeleteInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {
            throw new NotImplementedException();
        }

        public Task<string> EISOHistoryDeleteBySIN(string confirmedSIN, bool removeSIN)
        {
            throw new NotImplementedException();
        }

        public Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> FindMatchingActiveApplications(string appl_EnfSrv_Cd, string appl_CtrlCd, string confirmedSIN, string creditorFirstName, string creditorSurname)
        {
            throw new NotImplementedException();
        }

        public Task FTBatchNotification_CheckFTTransactionsAdded()
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldbackConditionData>> GetAllHoldbackConditions(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionFinancialHoldbackData>> GetAllInterceptionFinancialTerms(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetApplicationJusticeNumber(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationData>> GetApplicationsForReject()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> GetAutoAcceptGarnisheeOverrideData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> GetDateLastUIBatchLoaded()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDebtorId(string first3Char)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDebtorIdByConfirmedSin(string sin, string category)
        {
            throw new NotImplementedException();
        }

        public Task<List<EIoutgoingFederalData>> GetEIoutgoingData(string enfSrv)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessEISOOUTHistoryData>> GetEISOHistoryBySIN(string confirmedSIN)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentZipData> GetESD(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ElectronicSummonsDocumentRequiredData>> GetESDrequired()
        {
            throw new NotImplementedException();
        }

        public Task<List<ExGratiaListData>> GetExGratias()
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> GetGarnisheeSummonsReceiptDate(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD)
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldbackConditionData>> GetHoldbackConditions(string enfService, string controlCode, DateTime intFinH_Date, string activeState = "A")
        {
            throw new NotImplementedException();
        }

        public Task<List<HoldbackTypeData>> GetHoldbackTypes()
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionFinancialHoldbackData> GetInterceptionFinancialTerms(string enfService, string controlCode, string activeState = "A")
        {
            throw new NotImplementedException();
        }

        public Task<List<PaymentPeriodData>> GetPaymentPeriods()
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> GetSameCreditorForI01(string appl_CtrlCd, string submCd, string enteredSIN, byte confirmedSIN, string activeState)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationData>> GetTerminatedI01()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalActiveSummons(string appl_EnfSrv_Cd, string enfOfficeCode)
        {
            throw new NotImplementedException();
        }

        public Task InsertBalanceSnapshot(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount, BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null)
        {
            throw new NotImplementedException();
        }

        public Task InsertESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired, DateTime? esdReceivedDate = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAlreadyUsedJusticeNumber(string justiceNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsFeeCumulativeForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            return Task.FromResult(false);
        }

        public Task<(bool, DateTime)> IsNewESDreceived(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRefNumberBlocked(string appl_Source_RfrNr)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSinBlocked(string appl_Dbtr_Entrd_SIN)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsVariationIncrease(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task MessageBrokerCRAReconciliation()
        {
            throw new NotImplementedException();
        }

        public Task UpdateESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {
            throw new NotImplementedException();
        }

        public Task UpdateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {
            throw new NotImplementedException();
        }
    }
}
