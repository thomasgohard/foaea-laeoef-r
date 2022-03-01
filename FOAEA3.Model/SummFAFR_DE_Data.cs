using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class SummFAFR_DE_Data
    {
        public int SummFAFR_Id { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Batch_Id { get; set; }
        public byte? SummFAFR_OrigFA_Ind { get; set; }
        public short? SummFAFRLiSt_Cd { get; set; }
        public int? SummFAFR_Reas_Cd { get; set; }
        public string EnfSrv_Src_Cd { get; set; }
        public string EnfSrv_Loc_Cd { get; set; }
        public string EnfSrv_SubLoc_Cd { get; set; }
        public string SummFAFR_FA_Pym_Id { get; set; }
        public byte? SummFAFR_MultiSumm_Ind { get; set; }
        public DateTime? SummFAFR_Post_Dte { get; set; }
        public DateTime? SummFAFR_FA_Payable_Dte { get; set; }
        public decimal? SummFAFR_AvailDbtrAmt_Money { get; set; }
        public decimal? SummFAFR_AvailAmt_Money { get; set; }
        public string SummFAFR_Comments { get; set; }
        public string Dbtr_Id { get; set; }

        // calculated
        public string SummFAFR_OrigFA_IndDesc
        {
            get
            {
                if (SummFAFR_OrigFA_Ind == 1)
                    return "FA";
                else
                    return "FR";
            }
        }
    }
}
