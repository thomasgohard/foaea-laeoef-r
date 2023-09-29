using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class DataModificationData
    {
        public List<ApplicationData> Applications { get; set; }
        public DataModAction UpdateAction { get; set; }
        public string PreviousConfirmedSIN { get; set; }
        public string SinUpdateComment { get; set; }
    }
}
