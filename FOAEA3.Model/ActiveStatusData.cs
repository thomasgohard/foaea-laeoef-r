using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class ActiveStatusData
    {
        public string ActvSt_Cd { get; set; }
        public string ActvSt_Txt_E { get; set; }
        public string ActvSt_Txt_F { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? ActvSt_Txt_E : ActvSt_Txt_F;
        }
    }
}
