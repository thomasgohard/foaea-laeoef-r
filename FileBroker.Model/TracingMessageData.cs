using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.Model
{
    public class TracingMessageData
    {
        public const string ACTION_ADD = "A";
        public const string ACTION_CHANGE = "C";

        public TracingApplicationData TracingApplication { get; set; }
        public string NewRecipientSubmitter { get; set; }
        public string NewIssuingSubmitter { get; set; }
        public string MaintenanceAction { get; set; }
        public string MaintenanceLifeState { get; set; }
    }
}
