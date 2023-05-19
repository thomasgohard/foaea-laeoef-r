using System;

namespace FOAEA3.Model
{
    public class DivertFundData
    {
        public string Dbtr_Id { get; set; }
        public string SummFAFR_FA_Pym_Id { get; set; }
        public DateTime? SummFAFR_FA_Payable_Dte { get; set; }
        public decimal? SummDF_DivertedDbtrAmt_Money { get; set; }
        public string EnfSrv_Loc_Cd { get; set; }
        public string EnfSrv_SubLoc_Cd { get; set; }
        public decimal? SummFAFR_AvailDbtrAmt_Money { get; set; }
    }
}
