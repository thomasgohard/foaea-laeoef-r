using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class TracingOutgoingProvincialData
    {
        public List<TracingOutgoingProvincialTracingData> TracingData { get; set; }
        public List<TracingOutgoingProvincialFinancialData> FinancialData { get; set; }
    }
}
