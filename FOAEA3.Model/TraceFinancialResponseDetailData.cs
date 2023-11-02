using System.Collections.Generic;
using System.Diagnostics;

namespace FOAEA3.Model
{
    [DebuggerDisplay("{FiscalYear}/{TaxForm}")]

    public class TraceFinancialResponseDetailData
    {
        public int TrcRspFin_Dtl_Id { get; set; }
        public int TrcRspFin_Id { get; set; }
        public short FiscalYear { get; set; }
        public string TaxForm { get; set; }

        public List<TraceFinancialResponseDetailValueData> TraceDetailValues { get; set; }
    }
}
