namespace FOAEA3.Model
{
    public class LicenceDenialOutgoingFederalData
    {
        public int Event_dtl_Id { get; set; }
        public int Event_Reas_Cd { get; set; }
        public string Event_Reas_Text { get; set; }
        public string ActvSt_Cd { get; set; }
        public string Recordtype { get; set; }
        public string RequestType { get; set; }                   // Val_1 
        public string Appl_EnfSrv_Cd { get; set; }                // Val_2
        public string Appl_CtrlCd { get; set; }                   // Val_3
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }          // Val_4
        public string Appl_Dbtr_FrstNme { get; set; }             // Val_5
        public string Appl_Dbtr_MddleNme { get; set; }            // Val_6
        public string Appl_Dbtr_SurNme { get; set; }              // Val_7
        public string Appl_Dbtr_Parent_SurNme { get; set; }       // Val_8
        public string Appl_Dbtr_Gendr_Cd { get; set; }            // Val_9
        public string Appl_Dbtr_Brth_Dte { get; set; }            // Val_10
        public string LicSusp_Dbtr_Brth_CityNme { get; set; }     // Val_11
        public string LicSusp_Dbtr_Brth_CtryCd { get; set; }      // Val_12
        public string LicSusp_Dbtr_EyesColorCd { get; set; }      // Val_13
        public string LicSusp_Dbtr_HeightQty { get; set; }        // Val_14
        public string Dbtr_Addr_Ln { get; set; }                  // Val_15
        public string Dbtr_Addr_Ln1 { get; set; }                 // Val_16
        public string Dbtr_Addr_CityNme { get; set; }             // Val_17
        public string Dbtr_Addr_PrvCd { get; set; }               // Val_18
        public string Dbtr_Addr_PCd { get; set; }                 // Val_19
        public string Dbtr_Addr_CtryCd { get; set; }              // Val_20
        public string LicSusp_Dbtr_EmplNme { get; set; }          // Val_21
        public string LicSusp_Dbtr_EmplAddr_Ln { get; set; }      // Val_22
        public string LicSusp_Dbtr_EmplAddr_Ln1 { get; set; }     // Val_23
        public string LicSusp_Dbtr_EmplAddr_CityNme { get; set; } // Val_24
        public string LicSusp_Dbtr_EmplAddr_PrvCd { get; set; }   // Val_25
        public string LicSusp_Dbtr_EmplAddr_PCd { get; set; }     // Val_26
        public string LicSusp_Dbtr_EmplAddr_CtryCd { get; set; }  // Val_27
    }
}
