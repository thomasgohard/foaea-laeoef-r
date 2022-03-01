using FOAEA3.Model.Enums;

namespace FOAEA3.Model
{
    public class ApplicationConfirmedSINData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Subm_Recpt_SubmCd { get; set; }
        public string Appl_Crdtr_SurNme { get; set; }
        public ApplicationState AppLiSt_Cd { get; set; }
    }
}
