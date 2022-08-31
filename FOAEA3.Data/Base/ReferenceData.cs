using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task LoadApplicationCommentsAsync(IApplicationCommentsRepository applicationCommentsRepository)
        {
            var applicationComments = await applicationCommentsRepository.GetApplicationCommentsAsync();
            ApplicationComments.AddRange(applicationComments.Items);
        }

        public async Task LoadActiveStatusesAsync(IActiveStatusRepository activeStatusRepository)
        {
            var data = await activeStatusRepository.GetActiveStatusAsync();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var activeStatus in data.Items)
                ActiveStatuses.Add(activeStatus.ActvSt_Cd, activeStatus);
        }

        public async Task LoadApplicationLifeStatesAsync(IApplicationLifeStateRepository applicationLifeStateRepository)
        {
            var data = await applicationLifeStateRepository.GetApplicationLifeStatesAsync();

            foreach (var applicationLifeState in data.Items)
                ApplicationLifeStates.Add(applicationLifeState.AppLiSt_Cd, applicationLifeState);
        }

        public async Task LoadGendersAsync(IGenderRepository genderRepository)
        {
            var data = await genderRepository.GetGendersAsync();

            if (genderRepository.Messages.Count > 0)
                Messages.AddRange(genderRepository.Messages);

            foreach (var gender in data.Items)
                Genders.Add(gender.Gender_Cd, gender);
        }

        public async Task LoadProvincesAsync(IProvinceRepository provinceRepository)
        {
            var data = await provinceRepository.GetProvincesAsync();

            foreach (var province in data)
                Provinces.Add(province.PrvCd, province);
        }

        public async Task LoadMediumsAsync(IMediumRepository mediumRepository)
        {
            var data = await mediumRepository.GetMediumsAsync();

            if (mediumRepository.Messages.Count > 0)
                Messages.AddRange(mediumRepository.Messages);

            foreach (var medium in data.Items)
                Mediums.Add(medium.Medium_Cd, medium);
        }

        public async Task LoadLanguagesAsync(ILanguageRepository languageRepository)
        {
            var data = await languageRepository.GetLanguagesAsync();

            if (languageRepository.Messages.Count > 0)
                Messages.AddRange(languageRepository.Messages);

            foreach (var language in data.Items)
                Languages.Add(language.Lng_Cd, language);
        }

        public async Task LoadFoaEventsAsync(IFoaEventsRepository foaMessageRepository)
        {
            var data = await foaMessageRepository.GetAllFoaMessagesAsync();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var item in data.FoaEvents)
                FoaEvents.FoaEvents.Add(item.Key, item.Value);
        }

        public async Task LoadApplicationCategoriesAsync(IApplicationCategoryRepository applicationCategoryRepository)
        {
            var data = await applicationCategoryRepository.GetApplicationCategoriesAsync();

            if (applicationCategoryRepository.Messages.Count > 0)
                Messages.AddRange(applicationCategoryRepository.Messages);

            foreach (var applicationCategory in data.Items)
                ApplicationCategories.Add(applicationCategory.AppCtgy_Cd, applicationCategory);

        }

        public async Task LoadApplicationReasonsAsync(IApplicationReasonRepository applicationReasonRepository)
        {
            var data = await applicationReasonRepository.GetApplicationReasonsAsync();

            if (applicationReasonRepository.Messages.Count > 0)
                Messages.AddRange(applicationReasonRepository.Messages);

            foreach (var applicationReason in data.Items)
                ApplicationReasons.Add(applicationReason.AppReas_Cd.Trim(), applicationReason);
        }

        public async Task LoadCountriesAsync(ICountryRepository countryRepository)
        {
            var data = await countryRepository.GetCountriesAsync();

            if (countryRepository.Messages.Count > 0)
                Messages.AddRange(countryRepository.Messages);

            foreach (var country in data.Items)
                Countries.Add(country.Ctry_Cd, country);
        }

        public async Task LoadDocumentTypesAsync(IDocumentTypeRepository documentTypeRepository)
        {
            var data = await documentTypeRepository.GetDocumentTypesAsync();

            if (documentTypeRepository.Messages.Count > 0)
                Messages.AddRange(documentTypeRepository.Messages);

            foreach (var docType in data.Items)
                DocumentTypes.Add(docType.DocTyp_Cd, docType);
        }
    }
}
