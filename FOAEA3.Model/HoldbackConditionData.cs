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

        public bool ValuesEqual(object obj)
        {
            if (obj is not HoldbackConditionData newHoldback)
                return false;

            if ((newHoldback.Appl_EnfSrv_Cd == Appl_EnfSrv_Cd) &&
                (newHoldback.Appl_CtrlCd == Appl_CtrlCd) &&
                (newHoldback.IntFinH_Dte == IntFinH_Dte) &&
                (newHoldback.EnfSrv_Cd == EnfSrv_Cd) &&
                (newHoldback.HldbCnd_MxmPerChq_Money == HldbCnd_MxmPerChq_Money) &&
                (newHoldback.HldbCnd_SrcHldbAmn_Money == HldbCnd_SrcHldbAmn_Money) &&
                (newHoldback.HldbCnd_SrcHldbPrcnt == HldbCnd_SrcHldbPrcnt) &&
                (newHoldback.HldbCnd_LiStCd == HldbCnd_LiStCd) &&
                (newHoldback.HldbCtg_Cd == HldbCtg_Cd) &&
                (newHoldback.ActvSt_Cd == ActvSt_Cd))
            {
                return true;
            }
            else
                return false;
        }
    }
}
