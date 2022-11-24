using FOAEA3.Model.Enums;
using System;

namespace FOAEA3.Model
{
    public class ElectronicSummonsDocumentRequiredData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public ESDrequired ESDRequired { get; set; }
        public DateTime ESDRequiredDate { get; set; }
        public DateTime? ESDReceivedDate { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Subm_Recpt_SubmCd { get; set; }
        public ApplicationState AppLiSt_Cd { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
