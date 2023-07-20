using System.Diagnostics;

namespace FOAEA3.Model
{
    [DebuggerDisplay("{FieldName}={FieldValue}")]
    public class TraceFinancialResponseDetailValueData
    {
        public int TrcRspFin_Dtl_Value_Id { get; set; }
        public int TrcRspFin_Dtl_Id { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
