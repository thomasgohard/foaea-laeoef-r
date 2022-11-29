using System;

namespace FOAEA3.Model
{
    public class HoldbackConditionData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public DateTime IntFinH_Dte { get; set; }
        public string EnfSrv_Cd { get; set; }
        public decimal? HldbCnd_MxmPerChq_Money { get; set; }
        public decimal? HldbCnd_SrcHldbAmn_Money { get; set; }
        public int? HldbCnd_SrcHldbPrcnt { get; set; }
        public string HldbCtg_Cd { get; set; }
        public short HldbCnd_LiStCd { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
