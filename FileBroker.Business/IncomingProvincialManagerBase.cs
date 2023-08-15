namespace FileBroker.Business
{
    public class IncomingProvincialManagerBase
    {
        protected const string LIFESTATE_00 = "00";
        protected const string LIFESTATE_0 = "0";
        protected const string LIFESTATE_CANCEL = "14";
        protected const string LIFESTATE_VARY = "17";
        protected const string LIFESTATE_TRANSFER = "29";
        protected const string LIFESTATE_SUSPEND = "35";

        protected RepositoryList DB { get; }
        protected APIBrokerList APIs { get; }
        protected string FileName { get; }
        protected IFileBrokerConfigurationHelper Config { get; }
        protected IncomingProvincialHelper IncomingFileHelper { get; }
        protected FoaeaSystemAccess FoaeaAccess { get; }

        protected bool IsFrench { get; }

        private Dictionary<string, string> Translations { get; }

        public IncomingProvincialManagerBase(RepositoryList db, APIBrokerList foaeaApis, string fileName, 
                                             IFileBrokerConfigurationHelper config)
        {
            DB = db;
            APIs = foaeaApis;
            FileName = fileName;
            Config = config;

            string provinceCode = fileName[0..2].ToUpper();
            IncomingFileHelper = new IncomingProvincialHelper(config, provinceCode);
            IsFrench = Config.ProvinceConfig.FrenchAuditProvinceCodes?.Contains(provinceCode) ?? false;

            FoaeaAccess = new FoaeaSystemAccess(foaeaApis, Config.FoaeaLogin);

            Translations = LoadTranslations();
        }

        protected Dictionary<string, string> LoadTranslations()
        {
            var translations = new Dictionary<string, string>();

            if (IsFrench)
            {
                var Translations = DB.TranslationTable.GetTranslations().Result;
                foreach (var translation in Translations)
                    translations.Add(translation.EnglishText, translation.FrenchText);

                APIs.InterceptionApplications.ApiHelper.CurrentLanguage = LanguageHelper.FRENCH_LANGUAGE;
                LanguageHelper.SetLanguage(LanguageHelper.FRENCH_LANGUAGE);
            }

            return translations;
        }

        protected string Translate(string englishText)
        {
            if (IsFrench && Translations.ContainsKey(englishText))
            {
                return Translations[englishText];
            }
            else
                return englishText;
        }
    }
}
