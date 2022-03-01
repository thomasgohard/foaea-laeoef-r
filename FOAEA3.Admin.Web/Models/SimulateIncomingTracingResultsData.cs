using FOAEA3.Resources;
using System.ComponentModel.DataAnnotations;

namespace FOAEA3.Admin.Web.Models
{
    public class SimulateIncomingTracingResultsData
    {
        [Display(Name = "ENFORCEMENT_SERVICE", ResourceType = typeof(LanguageResource))]
        [Required]
        [MaxLength(6)]
        [MinLength(4)]
        public string EnfService { get; set; }

        [Display(Name = "CONTROL_CODE", ResourceType = typeof(LanguageResource))]
        [Required]
        [MaxLength(6)]
        public string ControlCode { get; set; }

        [Required]
        public string IncomingTraceSource { get; set; }

        [Required]
        public string Cycle { get; set; }
    }
}
