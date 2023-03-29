using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    [Serializable]
    public class TracingApplicationData : ApplicationData
    {
        public int Trace_Cycl_Qty { get; set; }
        public DateTime Trace_LstCyclStr_Dte { get; set; }
        public DateTime? Trace_LstCyclCmp_Dte { get; set; }
        public short Trace_LiSt_Cd { get; set; }

        // location trace
        public string InfoBank_Cd { get; set; }
        public string Trace_Child_Text { get; set; }
        public string Trace_Breach_Text { get; set; }
        public string Trace_ReasGround_Text { get; set; }
        public string FamPro_Cd { get; set; }
        public string Statute_Cd { get; set; }

        // financial trace
        public int TraceFin_Id { get; set; }
        public string EntityType { get; set; }
        public bool RequestedSIN { get; set; }
        public List<TraceFinancialDetailData> Financials { get; set; }

        public TracingApplicationData()
        {
            AppCtgy_Cd = "T01";
            AppReas_Cd = ReasonCode.T01_Default_of_support_provision_1;
            ActvSt_Cd = "A";
            Appl_SIN_Cnfrmd_Ind = 0;
            Appl_Create_Dte = DateTime.Now;
            Appl_Rcptfrm_Dte = Appl_Create_Dte.Date; // only date, no time
            Appl_Lgl_Dte = Appl_Create_Dte;
            Appl_Affdvt_DocTypCd = "T02";
            Appl_LastUpdate_Dte = Appl_Create_Dte;

            Financials = new List<TraceFinancialDetailData>();
        }

        public void Merge(TracingApplicationData data)
        {
            Trace_Child_Text = data.Trace_Child_Text;
            Trace_Breach_Text = data.Trace_Breach_Text;
            Trace_ReasGround_Text = data.Trace_ReasGround_Text;
            FamPro_Cd = data.FamPro_Cd;
            Statute_Cd = data.Statute_Cd;
            Trace_Cycl_Qty = data.Trace_Cycl_Qty;
            Trace_LstCyclStr_Dte = data.Trace_LstCyclStr_Dte;
            Trace_LstCyclCmp_Dte = data.Trace_LstCyclCmp_Dte;
            Trace_LiSt_Cd = data.Trace_LiSt_Cd;
            InfoBank_Cd = data.InfoBank_Cd;

            TraceFin_Id = data.TraceFin_Id;
            EntityType = data.EntityType;
            Financials.Clear();
            foreach(var financial in Financials)
                Financials.Add(financial);
        }

    }
}
