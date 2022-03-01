namespace FOAEA3.Model
{
    public class GarnSummaryData
    {
        public string EnfSrv_Cd { get; set; }
        public string EnfOff_Cd { get; set; }
        public int AcctYear { get; set; }
        public int AcctMonth { get; set; }
        public int? Ttl_ActiveSummons_Count { get; set; }
        public int? Mth_ActiveSummons_Count { get; set; }
        public int? Mth_ActionedSummons_Count { get; set; }
        public decimal? Mth_LumpSumActive_Amount { get; set; }
        public decimal? Mth_PeriodicDiverted_Amount { get; set; }
        public decimal? Mth_PeriodicActive_Amount { get; set; }
        public decimal? Mth_FeesActive_Amount { get; set; }
        public decimal? Mth_FeesDiverted_Amount { get; set; }
        public decimal? Mth_FeesOwed_Amount { get; set; }
        public decimal? Mth_FeesRemitted_Amount { get; set; }
        public decimal? Mth_FeesCollected_Amount { get; set; }
        public decimal? Mth_FeesDisbursed_Amount { get; set; }
        public decimal? Mth_Uncollected_Amount { get; set; }
        public int? Mth_FeesSatisfied_Count { get; set; }
        public int? Mth_FeesUnsatisfied_Count { get; set; }
        public decimal? Mth_Garnisheed_Amount { get; set; }
        public int? Mth_DivertActions_Count { get; set; }
        public int? Mth_Variation1_Count { get; set; }
        public int? Mth_Variation2_Count { get; set; }
        public int? Mth_Variation3_Count { get; set; }
        public int? Mth_Variations_Count { get; set; }
        public int? Mth_SummonsReceived_Count { get; set; }
        public int? Mth_SummonsCancelled_Count { get; set; }
        public int? Mth_SummonsRejected_Count { get; set; }
        public int? Mth_SummonsSatisfied_Count { get; set; }
        public int? Mth_SummonsExpired_Count { get; set; }
        public int? Mth_SummonsSuspended_Count { get; set; }
        public int? Mth_SummonsArchived_Count { get; set; }
        public int? Mth_SummonsSIN_Count { get; set; }
        public int? Mth_Action_Count { get; set; }
        public decimal? Mth_FAAvailable_Amount { get; set; }
        public int? Mth_FA_Count { get; set; }
        public int? Mth_CRAction_Count { get; set; }
        public decimal? Mth_CRFee_Amount { get; set; }
        public decimal? Mth_CRPaid_Amount { get; set; }
        public decimal? Mth_LumpSumDiverted_Amount { get; set; }
    }
}
