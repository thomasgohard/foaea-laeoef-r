using System;

namespace FOAEA3.Model
{
    public class CR_PADReventData
    {
        public int Event_Id { get; set; }
        public string Batch_Id { get; set; }
        public DateTime Event_TimeStamp { get; set; }
        public DateTime? Event_Compl_Dte { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
