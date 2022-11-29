using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class FamilyProvisionData
    {
        public string FamPro_Cd { get; set; }
        public string FamPro_Txt_E { get; set; }
        public string FamPro_Txt_F { get; set; }
        public string ActvSt_Cd { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? FamPro_Txt_E : FamPro_Txt_F;
        }
    }
}
