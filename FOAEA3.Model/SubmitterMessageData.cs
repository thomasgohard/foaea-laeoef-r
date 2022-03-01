using System;

namespace FOAEA3.Model
{
    public class SubmitterMessageData
    {
        public string Subm_SubmCd { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public short? AppLiSt_Cd { get; set; }
        public int? Msg_Nr { get; set; }
        public string Owner_EnfSrv_Cd { get; set; }
        public string Owner_SubmCd { get; set; }
        public string CustomMessage_en { get; set; }
        public string CustomMessage_fr { get; set; }
        public Guid? DisplayContext { get; set; }
        public string AppLiSt_Txt_E { get; set; }
        public string AppLiSt_Txt_F { get; set; }
        public string Description { get; set; }
        public string Appl_JusticeNr { get; set; }
        public string AppCtgy_Cd { get; set; }

    }
}
