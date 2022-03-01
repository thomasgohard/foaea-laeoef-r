using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class TraceToApplData
    {
        public int Event_Id { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public int Tot_Childs { get; set; }
        public int Tot_Closed { get; set; }
        public int Tot_Invalid { get; set; }
        public string ActvSt_Cd { get; set; }
        public ApplicationState AppLiSt_Cd { get; set; }
    }
}
