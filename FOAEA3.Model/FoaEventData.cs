using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class FoaEventData
    {
        public int Error { get; set; }
        public short Severity { get; set; }
        public short Dlevel { get; set; }
        public string Description_e { get; set; }
        public string Description_f { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? Description_e : Description_f;
        }
    }
}
