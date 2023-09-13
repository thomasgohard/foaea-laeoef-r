using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Model
{
    public class ApplicationEventDetailsList : Queue<ApplicationEventDetailData>
    {
        public ApplicationEventDetailsList()
        {            
        }

        public ApplicationEventDetailsList(IEnumerable<ApplicationEventDetailData> applicationEventDetails): base(applicationEventDetails)
        {

        }

        public bool ContainsEventCode(EventCode eventCode)
        {
            var result = this.FirstOrDefault(s => s.Event_Reas_Cd.HasValue && (s.Event_Reas_Cd.Value == eventCode));

            return (result != null);
        }

        public void Add(ApplicationEventDetailData eventData)
        {
            Enqueue(eventData);
        }

        public void AddRange(ApplicationEventDetailsList eventDetails)
        {
            if (eventDetails is not null)
                foreach(var item in eventDetails)
                    Enqueue(item);
        }
    }
}
