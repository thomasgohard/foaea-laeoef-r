using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class GarnPeriodData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public int SummFaFr_Id { get; set; }
        public short Period_no { get; set; }
        public decimal Garn_Amt { get; set; }
    }
}
