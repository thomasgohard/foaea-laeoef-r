using System;

namespace FOAEA3.Model
{
    public class CraSinPendingData
    {
        public string oldSin { get; set; }
        public string newSin { get; set; }
        public DateTime EnterDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
