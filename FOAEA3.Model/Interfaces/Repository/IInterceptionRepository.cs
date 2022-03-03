using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IInterceptionRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<InterceptionApplicationData> FindMatchingActiveApplications(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                        string confirmedSIN, string creditorFirstName,
                                                                        string creditorSurname);

        /*
         	            @Appl_CtrlCd char(6),
	            @Appl_EnfSrv_Cd char(6),
	            @Appl_Dbtr_Cnfrmd_SIN char(9),
	            @Appl_Crdtr_FrstNme char(25),
	            @Appl_Crdtr_SurNme char(25)*/

        bool IsFeeCumulativeForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd);
        bool IsVariationIncrease(string appl_EnfSrv_Cd, string appl_CtrlCd);
        
        InterceptionFinancialHoldbackData GetInterceptionFinancialTerms(string enfService, string controlCode, string activeState = "A");
        void CreateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH);
        void UpdateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH);
        
        List<HoldbackConditionData> GetHoldbackConditions(string enfService, string controlCode, DateTime intFinH_Date, string activeState = "A");
        void CreateHoldbackConditions(List<HoldbackConditionData> holdbackConditions);
        void UpdateHoldbackConditions(List<HoldbackConditionData> holdbackConditions);

        List<InterceptionApplicationData> GetSameCreditorForI01(string appl_CtrlCd, string submCd, string enteredSIN, byte confirmedSIN,
                                                                 string activeState);
        List<ExGratiaListData> GetExGratias();
        string GetApplicationJusticeNumber(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd);
        string GetDebtorID(string first3Char);
        bool IsAlreadyUsedJusticeNumber(string justiceNumber);
        DateTime GetGarnisheeSummonsReceiptDate(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD);
        int GetTotalActiveSummons(string appl_EnfSrv_Cd, string enfOfficeCode);
        string EISOHistoryDeleteBySIN(string confirmedSIN, bool removeSIN);
        void InsertESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired, DateTime? esdReceivedDate = null);
        void InsertBalanceSnapshot(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount,
                                   BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null);

        List<PaymentPeriodData> GetPaymentPeriods();
        List<HoldbackTypeData> GetHoldbackTypes();        
    }
}
