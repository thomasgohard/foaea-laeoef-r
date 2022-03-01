namespace FOAEA3.Model
{
    public class SINOutgoingFederalData
    {
        public int Event_dtl_Id { get; set; }
        public string Event_Reas_Cd { get; set; }
        public string Event_Reas_Text { get; set; }
        public string ActvSt_Cd { get; set; }
        public string Recordtype { get; set; }
        public string Appl_EnfSrv_Cd { get; set; } // Val_1
        public string Appl_CtrlCd { get; set; } // Val_2
        public string Appl_Dbtr_Entrd_SIN { get; set; } // Val_3
        public string Appl_Dbtr_FrstNme { get; set; } // Val_4
        public string Appl_Dbtr_MddleNme { get; set; } // Val_5
        public string Appl_Dbtr_SurNme { get; set; } // Val_6
        public string Appl_Dbtr_Parent_SurNme { get; set; } // Val_7
        public string Appl_Dbtr_Gendr_Cd { get; set; } // Val_8
        public string Appl_Dbtr_Brth_Dte { get; set; } // Val_9
    }
}
