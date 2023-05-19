using System;

namespace FOAEA3.Model
{
    public class SummDF_Data
    {
        public int SummDF_Id { get; set; }
        public int SummFAFR_Id { get; set; }
        public string Batch_Id { get; set; }
        public DateTime IntFinH_Dte { get; set; }
        public DateTime SummDF_Divert_Dte { get; set; }
        public int? SummDF_CurrPerCnt { get; set; }
        public decimal? SummDF_ProRatePerc { get; set; }
        public byte? SummDF_MultiSumm_Ind { get; set; }
        public decimal? SummDF_LmpSumDueAmt_Money { get; set; }
        public decimal? SummDF_PerPymDueAmt_Money { get; set; }
        public decimal? SummDF_DivertedDbtrAmt_Money { get; set; }
        public decimal? SummDF_DivOrigDbtrAmt_Money { get; set; }
        public decimal? SummDF_DivertedAmt_Money { get; set; }
        public decimal? SummDF_DivOrigAmt_Money { get; set; }
        public decimal? SummDF_HldbAmt_Money { get; set; }
        public decimal? SummDF_FeeAmt_Money { get; set; }
        public decimal? SummDF_LmpSumAmt_Money { get; set; }
        public decimal? SummDF_PerPymAmt_Money { get; set; }
        public decimal? SummDF_MiscAmt_Money { get; set; }
    }
}
