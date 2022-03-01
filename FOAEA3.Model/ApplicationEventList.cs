using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Model
{
    public class ApplicationEventList : List<ApplicationEventData>
    {
        public bool ContainsEventCode(EventCode eventCode)
        {

            var result = this.FirstOrDefault(s => s.Event_Reas_Cd.HasValue && (s.Event_Reas_Cd.Value == eventCode));

            return (result != null);

        }
               
    }
}
