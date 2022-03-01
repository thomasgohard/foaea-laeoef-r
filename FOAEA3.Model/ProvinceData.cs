using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class ProvinceData
    {
        public string PrvCd { get; set; }
        public string PrvTxtE { get; set; }
        public string PrvTxtF { get; set; }
        public string PrvCtryCd { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? PrvTxtE : PrvTxtF;
        }
    }
}
