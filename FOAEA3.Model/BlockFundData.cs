using System;

namespace FOAEA3.Model
{
    public class BlockFundData
    {
        public string Dbtr_Id { get; set; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }
        public DateTime Start_Dte { get; set; }
        public DateTime End_Dte { get; set; }
        public string Appl_Dbtr_FrstNme { get; set; }
        public string Appl_Dbtr_MddleNme { get; set; }
        public string Appl_Dbtr_SurNme { get; set; }
        public string Appl_Dbtr_LngCd { get; set; }

        public string Appl_Dbtr_Addr_Ln { get; set; }
        public string Appl_Dbtr_Addr_Ln1 { get; set; }
        public string Appl_Dbtr_Addr_CityNme { get; set; }
        public string Appl_Dbtr_Addr_PrvCd { get; set; }
        public string Appl_Dbtr_Addr_CtryCd { get; set; }
        public string Appl_Dbtr_Addr_PCd { get; set; }

        public string Appl_Dbtr_Gendr_Cd { get; set; }
        public DateTime Appl_Dbtr_Brth_Dte { get; set; }
    }
}
