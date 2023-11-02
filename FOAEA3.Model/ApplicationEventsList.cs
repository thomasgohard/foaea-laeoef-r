using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Model
{
    public class ApplicationEventsList : Queue<ApplicationEventData>
    {
        public ApplicationEventsList()
        {
            
        }

        public ApplicationEventsList(IEnumerable<ApplicationEventData> applicationEvents) : base(applicationEvents)
        {

        }

        public bool ContainsEventCode(EventCode eventCode)
        {
            var result = this.FirstOrDefault(s => s.Event_Reas_Cd.HasValue && (s.Event_Reas_Cd.Value == eventCode));

            return (result != null);
        }

        public void Add(ApplicationEventData eventData)
        {
            Enqueue(eventData);
        }

        public void AddRange(ApplicationEventsList events)
        {
            if (events is not null)
                foreach (var item in events)
                    Enqueue(item);
        }

    }
}
