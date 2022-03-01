using FOAEA3.Resources;
using System.ComponentModel.DataAnnotations;

namespace FOAEA3.Model
{
    public class CommissionerData
    {
        [Display(Name = "SUBMITTER_CODE", ResourceType = typeof(LanguageResource))]
        public string Subm_SubmCd { get; set; }
        
        public string Subm_Name { get; set; }
    }
}
