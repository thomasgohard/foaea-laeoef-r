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
                    {"appl_enfsrvcd", appl_EnfSrv_Cd},
                    {"appl_ctrlcd", appl_CtrlCd }
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

        public List<InterceptionFinancialHoldbackData> GetAllInterceptionFinancialTerms(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            var data = MainDB.GetDataFromStoredProc<InterceptionFinancialHoldbackData>("DefaultHoldbackGetHoldback", parameters, FillIntFinHDataFromReader);

            return data;
        }

        public void CreateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = SetIntFinHParameters(intFinH);

            _ = MainDB.ExecProc("IntFinH_Insert", parameters);
        }

        public void UpdateInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = SetIntFinHParameters(intFinH);

            _ = MainDB.ExecProc("IntFinH_Update", parameters);
        }

        public void DeleteInterceptionFinancialTerms(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", intFinH.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", intFinH.Appl_CtrlCd},
                {"IntFinH_Dte", intFinH.IntFinH_Dte}
            };

            _ = MainDB.ExecProc("IntFinH_Delete", parameters);
        }

        private static Dictionary<string, object> SetIntFinHParameters(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", intFinH.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", intFinH.Appl_CtrlCd},
                {"IntFinH_Dte", intFinH.IntFinH_Dte},
                {"HldbCtg_Cd", intFinH.HldbCtg_Cd},
                {"IntFinH_LmpSum_Money", intFinH.IntFinH_LmpSum_Money},
                {"IntFinH_TtlAmn_Money", intFinH.IntFinH_TtlAmn_Money},
                {"IntFinH_LiStCd", intFinH.IntFinH_LiStCd},
                {"ActvSt_Cd", intFinH.ActvSt_Cd}
            };

            if (intFinH.IntFinH_RcvtAffdvt_Dte.HasValue)
                parameters.Add("IntFinH_RcvtAffdvt_Dte", intFinH.IntFinH_RcvtAffdvt_Dte.Value);

            if (!string.IsNullOrEmpty(intFinH.IntFinH_Affdvt_SubmCd))
                parameters.Add("IntFinH_Affdvt_SubmCd", intFinH.IntFinH_Affdvt_SubmCd);

            if (!string.IsNullOrEmpty(intFinH.PymPr_Cd))
                parameters.Add("PymPr_Cd", intFinH.PymPr_Cd);

            if (!string.IsNullOrEmpty(intFinH.HldbTyp_Cd))
                parameters.Add("HldbTyp_Cd", intFinH.HldbTyp_Cd);

            if (intFinH.IntFinH_DefHldbAmn_Money.HasValue)
                parameters.Add("IntFinH_DefHldbAmn_Money", intFinH.IntFinH_DefHldbAmn_Money.Value);

            if (intFinH.IntFinH_DefHldbPrcnt.HasValue)
                parameters.Add("IntFinH_DefHldbPrcnt", intFinH.IntFinH_DefHldbPrcnt.Value);

            if (intFinH.IntFinH_CmlPrPym_Ind.HasValue)
                parameters.Add("IntFinH_CmlPrPym_Ind", intFinH.IntFinH_CmlPrPym_Ind.Value);

            if (intFinH.IntFinH_MxmTtl_Money.HasValue)
                parameters.Add("IntFinH_MxmTtl_Money", intFinH.IntFinH_MxmTtl_Money.Value);

            if (intFinH.IntFinH_PerPym_Money.HasValue)
                parameters.Add("IntFinH_PerPym_Money", intFinH.IntFinH_PerPym_Money.Value);

            if (intFinH.IntFinH_VarIss_Dte.HasValue)
                parameters.Add("IntFinH_VarIss_Dte", intFinH.IntFinH_VarIss_Dte.Value);

            if (!string.IsNullOrEmpty(intFinH.IntFinH_CreateUsr))
                parameters.Add("IntFinH_CreateUsr", intFinH.IntFinH_CreateUsr);

            if (!string.IsNullOrEmpty(intFinH.IntFinH_DefHldbAmn_Period))
                parameters.Add("IntFinH_DefHldbAmn_Period", intFinH.IntFinH_DefHldbAmn_Period);

            return parameters;
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

        public List<HoldbackConditionData> GetAllHoldbackConditions(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            var data = MainDB.GetDataFromStoredProc<HoldbackConditionData>("GetHldbCndForAppl", parameters, FillHldbCndDataFromReader);
            return data;
        }

        public void CreateHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {
            foreach (var holdbackCondition in holdbackConditions)
                CreateHoldbackCondition(holdbackCondition);
        }

        private void CreateHoldbackCondition(HoldbackConditionData holdbackCondition)
        {
            var parameters = SetHoldbackConditionParameters(holdbackCondition);

            _ = MainDB.ExecProc("HldbCnd_Insert", parameters);
        }

        public void UpdateHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {
            foreach (var holdbackCondition in holdbackConditions)
                UpdateHoldbackCondition(holdbackCondition);
        }

        private void UpdateHoldbackCondition(HoldbackConditionData holdbackCondition)
        {
            var parameters = SetHoldbackConditionParameters(holdbackCondition);

            _ = MainDB.ExecProc("HldbCnd_Update", parameters);
        }

        public void DeleteHoldbackConditions(List<HoldbackConditionData> holdbackConditions)
        {
            foreach (var holdbackCondition in holdbackConditions)
                DeleteHoldbackCondition(holdbackCondition);
        }

        private void DeleteHoldbackCondition(HoldbackConditionData holdbackCondition)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", holdbackCondition.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", holdbackCondition.Appl_CtrlCd},
                {"IntFinH_Dte", holdbackCondition.IntFinH_Dte},
                {"EnfSrv_Cd", holdbackCondition.EnfSrv_Cd}
            };

            _ = MainDB.ExecProc("HldbCnd_Delete", parameters);
        }

        private static Dictionary<string, object> SetHoldbackConditionParameters(HoldbackConditionData holdbackCondition)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", holdbackCondition.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", holdbackCondition.Appl_CtrlCd},
                {"IntFinH_Dte", holdbackCondition.IntFinH_Dte},
                {"EnfSrv_Cd", holdbackCondition.EnfSrv_Cd},
                {"HldbCtg_Cd", holdbackCondition.HldbCtg_Cd},
                {"HldbCnd_LiStCd", holdbackCondition.HldbCnd_LiStCd},
                {"ActvSt_Cd", holdbackCondition.ActvSt_Cd}
            };

            if (holdbackCondition.HldbCnd_MxmPerChq_Money.HasValue)
                parameters.Add("HldbCnd_MxmPerChq_Money", holdbackCondition.HldbCnd_MxmPerChq_Money.Value);

            if (holdbackCondition.HldbCnd_SrcHldbAmn_Money.HasValue)
                parameters.Add("HldbCnd_SrcHldbAmn_Money", holdbackCondition.HldbCnd_SrcHldbAmn_Money.Value);

            if (holdbackCondition.HldbCnd_SrcHldbPrcnt.HasValue)
                parameters.Add("HldbCnd_SrcHldbPrcnt", holdbackCondition.HldbCnd_SrcHldbPrcnt.Value);

            return parameters;
        }

        public List<InterceptionApplicationData> GetSameCreditorForI01(string appl_CtrlCd, string submCd, string enteredSIN,
                                                                       byte confirmedSIN, string activeState)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_CtrlCd", appl_CtrlCd},
                {"Subm_SubmCd", submCd},
                {"Appl_Dbtr_Entrd_SIN", enteredSIN ?? " "},
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
                {"SIN", confirmedSIN},
                {"removeSIN", removeSIN }
            };

            var returnParameters = new Dictionary<string, string>
            {
                {"message", "S120"}
            };

            var data = MainDB.GetDataFromStoredProcViaReturnParameters("MessageBrokerDeleteEISOOUTHistoryBySIN", parameters, returnParameters);

            return data["message"] as string;
        }

        public List<ProcessEISOOUTHistoryData> GetEISOHistoryBySIN(string confirmedSIN)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmedSIN", confirmedSIN}
            };

            var data = MainDB.GetDataFromStoredProc<ProcessEISOOUTHistoryData>("Prcs_EISOOUT_History_SelectForSIN", parameters, FillEISOOUTFromReader);

            return data;
        }

        private void FillEISOOUTFromReader(IDBHelperReader rdr, ProcessEISOOUTHistoryData data)
        {
            data.TRANS_TYPE_CD = rdr["TRANS_TYPE_CD"] as string;
            data.ACCT_NBR = rdr["ACCT_NBR"] as string;
            data.TRANS_AMT = rdr["TRANS_AMT"] as string;
            data.RQST_RFND_EID = rdr["RQST_RFND_EID"] as string;
            data.OUTPUT_DEST_CD = rdr["OUTPUT_DEST_CD"] as string;
            data.XREF_ACCT_NBR = rdr["XREF_ACCT_NBR"] as string;
            data.FOA_DELETE_IND = rdr["FOA_DELETE_IND"] as string;
            data.FOA_RECOUP_PRCNT = rdr["FOA_RECOUP_PRCNT"] as string;
            data.BLANK_AREA = rdr["BLANK_AREA"] as string;
            data.PAYMENT_RECEIVED = rdr["PAYMENT_RECEIVED"] as int?; // can be null 
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

        public void UpdateESDrequired(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                {"Appl_CtrlCd", appl_CtrlCd },
                {"bResetUpdate", resetUpdate }
            };

            if (esdReceivedDate.HasValue)
                parameters.Add("dESDReceivedDate", esdReceivedDate.Value);
            else
                parameters.Add("dESDReceivedDate", null);

            MainDB.ExecProc("UpdateESDRequired", parameters);

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
            return MainDB.GetAllData<PaymentPeriodData>("PymPr", FillPaymentPeriodFromReader);
        }

        public List<HoldbackTypeData> GetHoldbackTypes()
        {
            return MainDB.GetAllData<HoldbackTypeData>("HldbTyp", FillHolbackTypeFromReader);
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
