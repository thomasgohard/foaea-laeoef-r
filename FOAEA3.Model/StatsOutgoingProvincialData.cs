using System;

namespace FOAEA3.Model
{
    public class StatsOutgoingProvincialData
    {
        public int Event_dtl_Id { get; set; }
        public int Event_Reas_Cd_Zero { get; set; }
        public int Event_Reas_Text_Zero { get; set; }
        public string Source_ActvSt_Cd { get; set; }
        public string Recordtype { get; set; }
        public string EnfSrv_Cd { get; set; }  // Val_1
        public string Subm_SubmCd { get; set; } // Val_2
        public string Appl_CtrlCd { get; set; } // Val_3
        public string Appl_Source_RfrNr { get; set; } // Val_4
        public string AppCtgy_Cd { get; set; } // Val_5
        public string Subm_Recpt_SubmCd { get; set; } // Val_6
        public DateTime Event_TimeStamp { get; set; } // Val_7
        public DateTime Event_TimeStamp2 { get; set; } // Val_8
        public string ActvSt_Cd { get; set; } // Val_9
        public string AppLiSt_Cd { get; set; } // Val_10
        public string Event_Priority_Ind { get; set; } // Val_11
        public string Event_Reas_Cd { get; set; } // Val_12
        public DateTime Event_Effctv_Dte { get; set; } // Val_13
        public DateTime Event_Compl_Dte { get; set; } // Val_14
        public string Event_Reas_Text { get; set; } // Val_15
    }
}
