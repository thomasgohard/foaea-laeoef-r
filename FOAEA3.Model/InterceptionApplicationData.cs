using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class InterceptionApplicationData : ApplicationData
    {
        public InterceptionFinancialHoldbackData IntFinH { get; set; }

        public List<HoldbackConditionData> HldbCnd { get; set; }

        public InterceptionApplicationData()
        {
            AppCtgy_Cd = "I01";

            IntFinH = new InterceptionFinancialHoldbackData();
            HldbCnd = new List<HoldbackConditionData>();
        }

    }
}
