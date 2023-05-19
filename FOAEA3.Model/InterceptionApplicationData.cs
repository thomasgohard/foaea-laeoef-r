using System;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class InterceptionApplicationData : ApplicationData
    {
        public InterceptionApplicationData()
        {
            AppCtgy_Cd = "I01";
            ActvSt_Cd = "A";
            Appl_SIN_Cnfrmd_Ind = 0;
            Appl_Create_Dte = DateTime.Now;
            Appl_Rcptfrm_Dte = Appl_Create_Dte.Date; // only date, no time
            Appl_Lgl_Dte = Appl_Create_Dte;
            Appl_LastUpdate_Dte = Appl_Create_Dte;
            AppReas_Cd = "11";
            Appl_Affdvt_DocTypCd = "IXX";

            IntFinH = new InterceptionFinancialHoldbackData();
            HldbCnd = new List<HoldbackConditionData>();
        }

        public InterceptionApplicationData(ApplicationData baseData) : this()
        {
            base.Merge(baseData);
        }

        public InterceptionFinancialHoldbackData IntFinH { get; set; }

        public List<HoldbackConditionData> HldbCnd { get; set; }

    }
}
