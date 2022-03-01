using System.Threading;
using System.Globalization;

namespace FOAEA3.Resources.Helpers
{
    public class LanguageHelper
    {
        public const string LANGUAGE_ID = "CRDP_Language";

        public const string ENGLISH_LABEL = "English";
        public const string FRENCH_LABEL = "Français";

        public const string ENGLISH_LANGUAGE = "en";
        public const string FRENCH_LANGUAGE = "fr";

        public static bool IsEnglish() => Thread.CurrentThread.CurrentUICulture.Name.ToLower().StartsWith(ENGLISH_LANGUAGE);

        public static void SetLanguage(string language)
        {
            if (language.ToLower().StartsWith("fr"))
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(FRENCH_LANGUAGE);
            else
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ENGLISH_LANGUAGE);
        }
    }

}
