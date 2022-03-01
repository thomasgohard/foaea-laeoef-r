using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{

    public class ApplicationLifeStateData
    {
        public ApplicationState AppLiSt_Cd { get; set; }
        public string AppList_Txt_E { get; set; }
        public string AppList_Txt_F { get; set; }
        public string ActvSt_Cd { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? AppList_Txt_E : AppList_Txt_F;
        }
    }
}
