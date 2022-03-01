using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    internal class DBGarnSummary : DBbase, IGarnSummaryRepository
    {
        public DBGarnSummary(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<GarnSummaryData> GetGarnSummary(string appl_EnfSrv_Cd, string enfOfficeCode, int fiscalMonth, int fiscalYear)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"EnfOff_Cd", enfOfficeCode },
                    {"FiscMonth", fiscalMonth},
                    {"FiscYear", fiscalYear}
                };

            var data = MainDB.GetDataFromStoredProc<GarnSummaryData>("GetGarnSmry", parameters, FillGarnSummaryDataFromReader);

            return data;
        }

        public void CreateGarnSummary(GarnSummaryData garnSummaryData)
        {
            var parameters = SetGarnSummaryParameters(garnSummaryData);

            MainDB.ExecProc("GarnSmry_Insert", parameters);
        }

        public void UpdateGarnSummary(GarnSummaryData garnSummaryData)
        {
            var parameters = SetGarnSummaryParameters(garnSummaryData);

            MainDB.ExecProc("GarnSmry_Update", parameters);
        }

        private static Dictionary<string, object> SetGarnSummaryParameters(GarnSummaryData garnSummaryData)
        {
            return new Dictionary<string, object> {
                {"EnfSrv_Cd", garnSummaryData.EnfSrv_Cd},
                {"EnfOff_Cd", garnSummaryData.EnfOff_Cd},
                {"AcctYear", garnSummaryData.AcctYear},
                {"AcctMonth", garnSummaryData.AcctMonth},
                {"ttl_ActiveSummons_Count", garnSummaryData.Ttl_ActiveSummons_Count},
                {"mth_ActiveSummons_Count", garnSummaryData.Mth_ActiveSummons_Count},
                {"mth_ActionedSummons_Count", garnSummaryData.Mth_ActionedSummons_Count},
                {"mth_LumpSumActive_Amount", garnSummaryData.Mth_LumpSumActive_Amount},
                {"mth_PeriodicDiverted_Amount", garnSummaryData.Mth_PeriodicDiverted_Amount},
                {"mth_PeriodicActive_Amount", garnSummaryData.Mth_PeriodicActive_Amount},
                {"mth_FeesActive_Amount", garnSummaryData.Mth_FeesActive_Amount},
                {"mth_FeesDiverted_Amount", garnSummaryData.Mth_FeesDiverted_Amount},
                {"mth_FeesOwed_Amount", garnSummaryData.Mth_FeesOwed_Amount},
                {"mth_FeesRemitted_Amount", garnSummaryData.Mth_FeesRemitted_Amount},
                {"mth_FeesCollected_Amount", garnSummaryData.Mth_FeesCollected_Amount},
                {"mth_FeesDisbursed_Amount", garnSummaryData.Mth_FeesDisbursed_Amount},
                {"mth_Uncollected_Amount", garnSummaryData.Mth_Uncollected_Amount},
                {"mth_FeesSatisfied_Count", garnSummaryData.Mth_FeesSatisfied_Count},
                {"mth_FeesUnsatisfied_Count", garnSummaryData.Mth_FeesUnsatisfied_Count},
                {"mth_Garnisheed_Amount", garnSummaryData.Mth_Garnisheed_Amount},
                {"mth_DivertActions_Count", garnSummaryData.Mth_DivertActions_Count},
                {"mth_Variation1_Count", garnSummaryData.Mth_Variation1_Count},
                {"mth_Variation2_Count", garnSummaryData.Mth_Variation2_Count},
                {"mth_Variation3_Count", garnSummaryData.Mth_Variation3_Count},
                {"mth_Variations_Count", garnSummaryData.Mth_Variations_Count},
                {"mth_SummonsReceived_Count", garnSummaryData.Mth_SummonsReceived_Count},
                {"mth_SummonsCancelled_Count", garnSummaryData.Mth_SummonsCancelled_Count},
                {"mth_SummonsRejected_Count", garnSummaryData.Mth_SummonsRejected_Count},
                {"mth_SummonsSatisfied_Count", garnSummaryData.Mth_SummonsSatisfied_Count},
                {"mth_SummonsExpired_Count", garnSummaryData.Mth_SummonsExpired_Count},
                {"mth_SummonsSuspended_Count", garnSummaryData.Mth_SummonsSuspended_Count},
                {"mth_SummonsArchived_Count", garnSummaryData.Mth_SummonsArchived_Count},
                {"mth_SummonsSIN_Count", garnSummaryData.Mth_SummonsSIN_Count},
                {"mth_Action_Count", garnSummaryData.Mth_Action_Count},
                {"mth_FAAvailable_Amount", garnSummaryData.Mth_FAAvailable_Amount},
                {"mth_FA_Count", garnSummaryData.Mth_FA_Count},
                {"mth_CRAction_Count", garnSummaryData.Mth_CRAction_Count},
                {"mth_CRFee_Amount", garnSummaryData.Mth_CRFee_Amount},
                {"mth_CRPaid_Amount", garnSummaryData.Mth_CRPaid_Amount},
                {"mth_LumpSumDiverted_Amount", garnSummaryData.Mth_LumpSumDiverted_Amount}
            };
        }

        private void FillGarnSummaryDataFromReader(IDBHelperReader rdr, GarnSummaryData data)
        {
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.EnfOff_Cd = rdr["EnfOff_Cd"] as string;
            data.AcctYear = (int)rdr["AcctYear"];
            data.AcctMonth = (int)rdr["AcctMonth"];
            data.Ttl_ActiveSummons_Count = rdr["ttl_ActiveSummons_Count"] as int?; // can be null 
            data.Mth_ActiveSummons_Count = rdr["mth_ActiveSummons_Count"] as int?; // can be null 
            data.Mth_ActionedSummons_Count = rdr["mth_ActionedSummons_Count"] as int?; // can be null 
            data.Mth_LumpSumActive_Amount = rdr["mth_LumpSumActive_Amount"] as decimal?; // can be null 
            data.Mth_PeriodicDiverted_Amount = rdr["mth_PeriodicDiverted_Amount"] as decimal?; // can be null 
            data.Mth_PeriodicActive_Amount = rdr["mth_PeriodicActive_Amount"] as decimal?; // can be null 
            data.Mth_FeesActive_Amount = rdr["mth_FeesActive_Amount"] as decimal?; // can be null 
            data.Mth_FeesDiverted_Amount = rdr["mth_FeesDiverted_Amount"] as decimal?; // can be null 
            data.Mth_FeesOwed_Amount = rdr["mth_FeesOwed_Amount"] as decimal?; // can be null 
            data.Mth_FeesRemitted_Amount = rdr["mth_FeesRemitted_Amount"] as decimal?; // can be null 
            data.Mth_FeesCollected_Amount = rdr["mth_FeesCollected_Amount"] as decimal?; // can be null 
            data.Mth_FeesDisbursed_Amount = rdr["mth_FeesDisbursed_Amount"] as decimal?; // can be null 
            data.Mth_Uncollected_Amount = rdr["mth_Uncollected_Amount"] as decimal?; // can be null 
            data.Mth_FeesSatisfied_Count = rdr["mth_FeesSatisfied_Count"] as int?; // can be null 
            data.Mth_FeesUnsatisfied_Count = rdr["mth_FeesUnsatisfied_Count"] as int?; // can be null 
            data.Mth_Garnisheed_Amount = rdr["mth_Garnisheed_Amount"] as decimal?; // can be null 
            data.Mth_DivertActions_Count = rdr["mth_DivertActions_Count"] as int?; // can be null 
            data.Mth_Variation1_Count = rdr["mth_Variation1_Count"] as int?; // can be null 
            data.Mth_Variation2_Count = rdr["mth_Variation2_Count"] as int?; // can be null 
            data.Mth_Variation3_Count = rdr["mth_Variation3_Count"] as int?; // can be null 
            data.Mth_Variations_Count = rdr["mth_Variations_Count"] as int?; // can be null 
            data.Mth_SummonsReceived_Count = rdr["mth_SummonsReceived_Count"] as int?; // can be null 
            data.Mth_SummonsCancelled_Count = rdr["mth_SummonsCancelled_Count"] as int?; // can be null 
            data.Mth_SummonsRejected_Count = rdr["mth_SummonsRejected_Count"] as int?; // can be null 
            data.Mth_SummonsSatisfied_Count = rdr["mth_SummonsSatisfied_Count"] as int?; // can be null 
            data.Mth_SummonsExpired_Count = rdr["mth_SummonsExpired_Count"] as int?; // can be null 
            data.Mth_SummonsSuspended_Count = rdr["mth_SummonsSuspended_Count"] as int?; // can be null 
            data.Mth_SummonsArchived_Count = rdr["mth_SummonsArchived_Count"] as int?; // can be null 
            data.Mth_SummonsSIN_Count = rdr["mth_SummonsSIN_Count"] as int?; // can be null 
            data.Mth_Action_Count = rdr["mth_Action_Count"] as int?; // can be null 
            data.Mth_FAAvailable_Amount = rdr["mth_FAAvailable_Amount"] as decimal?; // can be null 
            data.Mth_FA_Count = rdr["mth_FA_Count"] as int?; // can be null 
            data.Mth_CRAction_Count = rdr["mth_CRAction_Count"] as int?; // can be null 
            data.Mth_CRFee_Amount = rdr["mth_CRFee_Amount"] as decimal?; // can be null 
            data.Mth_CRPaid_Amount = rdr["mth_CRPaid_Amount"] as decimal?; // can be null 
            data.Mth_LumpSumDiverted_Amount = rdr["mth_LumpSumDiverted_Amount"] as decimal?; // can be null 
        }

    }
}
