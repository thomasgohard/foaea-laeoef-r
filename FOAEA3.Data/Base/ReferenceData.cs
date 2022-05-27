using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;

namespace FOAEA3.Data.Base
{
    public sealed class ReferenceData : IReferenceData, IMessageList
    {
        public Dictionary<string, ActiveStatusData> ActiveStatuses { get; }
        public Dictionary<ApplicationState, ApplicationLifeStateData> ApplicationLifeStates { get; }
        public Dictionary<string, GenderData> Genders { get; }
        public Dictionary<string, ProvinceData> Provinces { get; }
        public Dictionary<string, MediumData> Mediums { get; }
        public Dictionary<string, LanguageData> Languages { get; }
        public Dictionary<string, ApplicationCategoryData> ApplicationCategories { get; }
        public Dictionary<string, ApplicationReasonData> ApplicationReasons { get; }
        public Dictionary<string, CountryData> Countries { get; }
        public Dictionary<string, DocumentTypeData> DocumentTypes { get; }
        public List<ApplicationCommentsData> ApplicationComments { get; }
        public FoaEventDataDictionary FoaEvents { get; }

        public Dictionary<string, string> Configuration { get; }

        private static ReferenceData instance = null;

        private static readonly object syncLock = new();

        private ReferenceData()
        {
            Messages = new MessageDataList();
            ActiveStatuses = new Dictionary<string, ActiveStatusData>();
            ApplicationLifeStates = new Dictionary<ApplicationState, ApplicationLifeStateData>();
            Genders = new Dictionary<string, GenderData>();
            Provinces = new Dictionary<string, ProvinceData>();
            Mediums = new Dictionary<string, MediumData>();
            Languages = new Dictionary<string, LanguageData>();
            ApplicationCategories = new Dictionary<string, ApplicationCategoryData>();
            ApplicationReasons = new Dictionary<string, ApplicationReasonData>();
            Countries = new Dictionary<string, CountryData>();
            DocumentTypes = new Dictionary<string, DocumentTypeData>();
            Configuration = new Dictionary<string, string>();
            ApplicationComments = new List<ApplicationCommentsData>();
            FoaEvents = new FoaEventDataDictionary();
        }

        public static ReferenceData Instance()
        {
            lock (syncLock) // thread safe
            {
                if (instance is null)
                {
                    instance = new ReferenceData();
                }
            }

            return instance;
        }

        public string StartupFailure { get; set; } = string.Empty;

        public MessageDataList Messages { get; set; }

        public void LoadApplicationComments(IApplicationCommentsRepository applicationCommentsRepository)
        {
            var applicationComments = applicationCommentsRepository.GetApplicationComments();
            ApplicationComments.AddRange(applicationComments.Items);
        }

        public void LoadActiveStatuses(IActiveStatusRepository activeStatusRepository)
        {
            var data = activeStatusRepository.GetActiveStatus();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var activeStatus in data.Items)
                ActiveStatuses.Add(activeStatus.ActvSt_Cd, activeStatus);
        }

        public void LoadApplicationLifeStates(IApplicationLifeStateRepository applicationLifeStateRepository)
        {
            var data = applicationLifeStateRepository.GetApplicationLifeStates();

            foreach (var applicationLifeState in data.Items)
                ApplicationLifeStates.Add(applicationLifeState.AppLiSt_Cd, applicationLifeState);
        }

        public void LoadGenders(IGenderRepository genderRepository)
        {
            var data = genderRepository.GetGenders();

            if (genderRepository.Messages.Count > 0)
                Messages.AddRange(genderRepository.Messages);

            foreach (var gender in data.Items)
                Genders.Add(gender.Gender_Cd, gender);
        }

        public void LoadProvinces(IProvinceRepository provinceRepository)
        {
            var data = provinceRepository.GetProvinces();

            foreach (var province in data)
                Provinces.Add(province.PrvCd, province);
        }

        public void LoadMediums(IMediumRepository mediumRepository)
        {
            var data = mediumRepository.GetMediums();

            if (mediumRepository.Messages.Count > 0)
                Messages.AddRange(mediumRepository.Messages);

            foreach (var medium in data.Items)
                Mediums.Add(medium.Medium_Cd, medium);
        }

        public void LoadLanguages(ILanguageRepository languageRepository)
        {
            var data = languageRepository.GetLanguages();

            if (languageRepository.Messages.Count > 0)
                Messages.AddRange(languageRepository.Messages);

            foreach (var language in data.Items)
                Languages.Add(language.Lng_Cd, language);
        }

        public void LoadFoaEvents(IFoaEventsRepository foaMessageRepository)
        {
            var data = foaMessageRepository.GetAllFoaMessages();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var item in data.FoaEvents)
                FoaEvents.FoaEvents.Add(item.Key, item.Value);
        }

        public void LoadApplicationCategories(IApplicationCategoryRepository applicationCategoryRepository)
        {
            var data = applicationCategoryRepository.GetApplicationCategories();

            if (applicationCategoryRepository.Messages.Count > 0)
                Messages.AddRange(applicationCategoryRepository.Messages);

            foreach (var applicationCategory in data.Items)
                ApplicationCategories.Add(applicationCategory.AppCtgy_Cd, applicationCategory);

        }

        public void LoadApplicationReasons(IApplicationReasonRepository applicationReasonRepository)
        {
            var data = applicationReasonRepository.GetApplicationReasons();

            if (applicationReasonRepository.Messages.Count > 0)
                Messages.AddRange(applicationReasonRepository.Messages);

            foreach (var applicationReason in data.Items)
                ApplicationReasons.Add(applicationReason.AppReas_Cd.Trim(), applicationReason);
        }

        public void LoadCountries(ICountryRepository countryRepository)
        {
            var data = countryRepository.GetCountries();

            if (countryRepository.Messages.Count > 0)
                Messages.AddRange(countryRepository.Messages);

            foreach (var country in data.Items)
                Countries.Add(country.Ctry_Cd, country);
        }

        public void LoadDocumentTypes(IDocumentTypeRepository documentTypeRepository)
        {
            var data = documentTypeRepository.GetDocumentTypes();

            if (documentTypeRepository.Messages.Count > 0)
                Messages.AddRange(documentTypeRepository.Messages);

            foreach (var docType in data.Items)
                DocumentTypes.Add(docType.DocTyp_Cd, docType);
        }
    }
}
