using FOAEA3.Data.Base;
using FOAEA3.Model;

namespace FOAEA3.ViewModels
{
    public class TracingDataEntryViewModel
    {

        private TracingApplicationData tracingData;

        public TracingApplicationData Tracing
        {
            get { return tracingData; }
            set
            {
                tracingData = value;
                //tracingData.ReferenceData = ReferenceData.Instance();
            }
        }
        public string Declarant { get; set; }
        public string CourtValue { get; set; }
        public bool CanEditCore { get; set; } = true;
        public bool CanEditCommentsAndRef { get; set; } = true;
        public bool IsCreate { get; set; }
    }
}
