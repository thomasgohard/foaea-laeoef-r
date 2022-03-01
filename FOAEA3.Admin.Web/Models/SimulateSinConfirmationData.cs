using FOAEA3.Resources;
using System.ComponentModel.DataAnnotations;

namespace FOAEA3.Admin.Web.Models
{
    public class SimulateSinConfirmationData
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

        [Display(Name = "SIN", ResourceType = typeof(LanguageResource))]
        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        public string Sin { get; set; }
    }
}
