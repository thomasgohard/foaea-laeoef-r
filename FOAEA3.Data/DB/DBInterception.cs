using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Data.DB
{
    internal class DBInterception : DBbase, IInterceptionRepository
    {

        public DBInterception(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<InterceptionApplicationData> FindMatchingActiveApplications(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                        string confirmedSIN, string creditorFirstName,
                                                                        string creditorSurname)
        {

            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"Appl_Dbtr_Cnfrmd_SIN", confirmedSIN },
                    {"Appl_Crdtr_FrstNme", creditorFirstName },
                    {"Appl_Crdtr_SurNme", creditorSurname }
                };

            var data = MainDB.GetDataFromStoredProc<InterceptionApplicationData>("GetMatchingActiveApplications", parameters, DBApplication.FillApplicationDataFromReader);

            return data;
        }

        public bool IsFeeCumulativeForApplication(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrvCd", appl_EnfSrv_Cd},
                    {"CtrlCd", appl_CtrlCd }
                };

            int result = MainDB.GetDataFromStoredProc<int>("IsFeeCumulativeForApplication", parameters);

            return (result == 1);
        }

        public bool IsVariationIncrease(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrvCd", appl_EnfSrv_Cd},
                    {"CtrlCd", appl_CtrlCd }
                };

            int result = MainDB.GetDataFromStoredProc<int>("ApplVerifyVariationIncrease", parameters);

            return (result == 1);
        }

        public InterceptionFinancialHoldbackData GetInterceptionFinancialTerms(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                               string activeState = "A")
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"ActvSt_Cd", activeState }
                };

            var data = MainDB.GetDataFromStoredProc<InterceptionFinancialHoldbackData>("GetIntFinHtoActivateDeactivate", parameters, FillIntFinHDataFromReader);

            return data.SingleOrDefault();
        }

        public void CreateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {

        }
        public void UpdateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {

        }

        public List<HoldbackConditionData> GetHoldbackConditions(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime intFinH_Date, 
                                                                 string activeState = "A")
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"IntFinH_Dte", intFinH_Date }
                };

            var data = MainDB.GetDataFromStoredProc<HoldbackConditionData>("GetHldbCndToActivateDeactivate", parameters, FillHldbCndDataFromReader);

            return data.Where(m => m.ActvSt_Cd == activeState).ToList();
        }

        public void CreateHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {

        }

        public void UpdateHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {

        }

        public List<InterceptionApplicationData> GetSameCreditorForI01(string appl_CtrlCd, string submCd, string enteredSIN, string confirmedSIN,
                                                                       string activeState)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_CtrlCd", appl_CtrlCd},
                {"Subm_SubmCd", submCd},
                {"Appl_Dbtr_Entrd_SIN", enteredSIN},
                {"Appl_SIN_Cnfrmd_Ind", confirmedSIN},
                {"ActvSt_Cd", activeState}
            };

            var data = MainDB.GetDataFromStoredProc<InterceptionApplicationData>("GetSameCreditorForI01", parameters, DBApplication.FillApplicationDataFromReader);

            return data;
        }

        public string GetApplicationJusticeNumber(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"Appl_Dbtr_Cnfrmd_SIN", confirmedSIN}
                };

            return MainDB.GetDataFromProcSingleValue<string>("GetApplJusticeNr", parameters);
        }

        public string GetDebtorID(string first3Char)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"First3Char", first3Char}
                };

            return MainDB.GetDataFromProcSingleValue<string>("GetSummSmryDebtorID", parameters);
        }

        public bool IsAlreadyUsedJusticeNumber(string justiceNumber)
        {
            var parameters = new Dictionary<string, object>
            {
                {"JusticeNr", justiceNumber}
            };

            var data = MainDB.GetDataFromStoredProc<InterceptionApplicationData>("ApplGetApplByJusticeNR", parameters, DBApplication.FillApplicationDataFromReader);

            if (data.Count > 0)
            {
                if (data[0].Appl_JusticeNr.Trim().ToUpper() == justiceNumber.Trim().ToUpper())
                    return true;                
            }

            return false;
        }

        public DateTime GetGarnisheeSummonsReceiptDate(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"isESD", isESD}
                };

            return MainDB.GetDataFromProcSingleValue<DateTime>("GetGarnisheeSummonsReceiptDate", parameters);
        }

        public int GetTotalActiveSummons(string appl_EnfSrv_Cd, string enfOfficeCode)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"EnfOff_Cd", enfOfficeCode }
                };

            return MainDB.GetDataFromProcSingleValue<int>("GetTotalActiveSummons", parameters);
        }
               
        public string EISOHistoryDeleteBySIN(string confirmedSIN, bool removeSIN)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@SIN", confirmedSIN},
                {"@removeSIN", removeSIN }
            };

            var returnParameters = new Dictionary<string, string>
            {
                {"message", "S120"}
            };

            var data = MainDB.GetDataFromStoredProcViaReturnParameters("MessageBrokerDeleteEISOOUTHistoryBySIN", parameters, returnParameters);

            return data["message"] as string;
        }

        public void InsertESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired,
                                      DateTime? esdReceivedDate = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                {"Appl_CtrlCd", appl_CtrlCd },
                {"ESDRequired", originalESDrequired }
            };

            if (esdReceivedDate.HasValue)
                parameters.Add("ESDReceivedDate", esdReceivedDate.Value);

            MainDB.ExecProc("InsertESDRequired", parameters);

        }

        public void InsertBalanceSnapshot(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount, 
                                          BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                {"Appl_CtrlCd", appl_CtrlCd },
                {"Appl_TotalAmnt", totalAmount},
                {"change_type", (int) changeType}
            };

            if (summFAFR_id.HasValue)
                parameters.Add("SummFAFR_Id", summFAFR_id.Value);

            if (intFinH_Date.HasValue)
                parameters.Add("IntFinH_Dte", intFinH_Date.Value);

            MainDB.ExecProc("InsertBalanceSnapshot", parameters);

        }

        public List<ExGratiaListData> GetExGratias()
        {
            return MainDB.GetAllData<ExGratiaListData>("GetExGratiaList", FillExGratiaListFromReader);
        }

        public List<PaymentPeriodData> GetPaymentPeriods()
        {
            return MainDB.GetAllData<PaymentPeriodData>("PymPr_Select", FillPaymentPeriodFromReader);
        }

        public List<HoldbackTypeData> GetHoldbackTypes()
        {
            return MainDB.GetAllData<HoldbackTypeData>("HldbTyp_Select", FillHolbackTypeFromReader);
        }

        private void FillIntFinHDataFromReader(IDBHelperReader rdr, InterceptionFinancialHoldbackData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.IntFinH_Dte = (DateTime)rdr["IntFinH_Dte"];
            data.IntFinH_RcvtAffdvt_Dte = rdr["IntFinH_RcvtAffdvt_Dte"] as DateTime?; // can be null 
            data.IntFinH_Affdvt_SubmCd = rdr["IntFinH_Affdvt_SubmCd"] as string; // can be null 
            data.PymPr_Cd = rdr["PymPr_Cd"] as string; // can be null 
            if (rdr.ColumnExists("IntFinH_NextRecalcDate_Cd"))
                data.IntFinH_NextRecalcDate_Cd = rdr["IntFinH_NextRecalcDate_Cd"] as int?; // can be null 
            data.HldbTyp_Cd = rdr["HldbTyp_Cd"] as string; // can be null 
            data.IntFinH_DefHldbAmn_Money = rdr["IntFinH_DefHldbAmn_Money"] as decimal?; // can be null 
            data.IntFinH_DefHldbPrcnt = rdr["IntFinH_DefHldbPrcnt"] as int?; // can be null 
            data.HldbCtg_Cd = rdr["HldbCtg_Cd"] as string;
            data.IntFinH_CmlPrPym_Ind = rdr["IntFinH_CmlPrPym_Ind"] as byte?; // can be null 
            data.IntFinH_MxmTtl_Money = rdr["IntFinH_MxmTtl_Money"] as decimal?; // can be null 
            data.IntFinH_PerPym_Money = rdr["IntFinH_PerPym_Money"] as decimal?; // can be null 
            data.IntFinH_LmpSum_Money = (decimal)rdr["IntFinH_LmpSum_Money"];
            data.IntFinH_TtlAmn_Money = (decimal)rdr["IntFinH_TtlAmn_Money"];
            data.IntFinH_VarIss_Dte = rdr["IntFinH_VarIss_Dte"] as DateTime?; // can be null 
            data.IntFinH_CreateUsr = rdr["IntFinH_CreateUsr"] as string; // can be null 
            data.IntFinH_LiStCd = (short)rdr["IntFinH_LiStCd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.IntFinH_DefHldbAmn_Period = rdr["IntFinH_DefHldbAmn_Period"] as string; // can be null 
        }

        private void FillHldbCndDataFromReader(IDBHelperReader rdr, HoldbackConditionData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.IntFinH_Dte = (DateTime)rdr["IntFinH_Dte"];
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.HldbCnd_MxmPerChq_Money = rdr["HldbCnd_MxmPerChq_Money"] as decimal?; // can be null 
            data.HldbCnd_SrcHldbAmn_Money = rdr["HldbCnd_SrcHldbAmn_Money"] as decimal?; // can be null 
            data.HldbCnd_SrcHldbPrcnt = rdr["HldbCnd_SrcHldbPrcnt"] as int?; // can be null 
            data.HldbCtg_Cd = rdr["HldbCtg_Cd"] as string;
            data.HldbCnd_LiStCd = (short)rdr["HldbCnd_LiStCd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }

        private void FillExGratiaListFromReader(IDBHelperReader rdr, ExGratiaListData data)
        {
            data.ID = (int)rdr["ID"];
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Source_Ref_Nr = rdr["Source_Ref_Nr"] as string;
            data.DateAdded = (DateTime)rdr["DateAdded"];
            data.Surname = rdr["Surname"] as string;
            data.SIN = rdr["SIN"] as string;
            data.BlockSIN = (bool)rdr["BlockSIN"];
            data.BlockREF = (bool)rdr["BlockREF"];
        }

        private void FillPaymentPeriodFromReader(IDBHelperReader rdr, PaymentPeriodData data)
        {
            data.PymPr_Cd = rdr["PymPr_Cd"] as string;
            data.PymPr_Txt_E = rdr["PymPr_Txt_E"] as string;
            data.PymPr_Txt_F = rdr["PymPr_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }

        private void FillHolbackTypeFromReader(IDBHelperReader rdr, HoldbackTypeData data)
        {
            data.HldbTyp_Cd = rdr["HldbTyp_Cd"] as string;
            data.HldbTyp_Txt_E = rdr["HldbTyp_Txt_E"] as string; // can be null 
            data.HldbTyp_Txt_F = rdr["HldbTyp_Txt_F"] as string; // can be null 
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }


    }
}
