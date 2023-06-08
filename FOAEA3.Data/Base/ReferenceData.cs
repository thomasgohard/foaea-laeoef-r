using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.Base
{
    public sealed class ReferenceData : IReferenceData, IMessageList
    {
        public ConcurrentDictionary<string, ActiveStatusData> ActiveStatuses { get; }
        public ConcurrentDictionary<ApplicationState, ApplicationLifeStateData> ApplicationLifeStates { get; }
        public ConcurrentDictionary<string, GenderData> Genders { get; }
        public ConcurrentDictionary<string, ProvinceData> Provinces { get; }
        public ConcurrentDictionary<string, MediumData> Mediums { get; }
        public ConcurrentDictionary<string, LanguageData> Languages { get; }
        public ConcurrentDictionary<string, ApplicationCategoryData> ApplicationCategories { get; }
        public ConcurrentDictionary<string, ApplicationReasonData> ApplicationReasons { get; }
        public ConcurrentDictionary<string, CountryData> Countries { get; }
        public ConcurrentDictionary<string, DocumentTypeData> DocumentTypes { get; }
        public List<ApplicationCommentsData> ApplicationComments { get; }
        public FoaEventDataDictionary FoaEvents { get; }

        private static ReferenceData instance = null;

        private static readonly object syncLock = new();

        private ReferenceData()
        {
            Messages = new MessageDataList();
            ActiveStatuses = new ConcurrentDictionary<string, ActiveStatusData>();
            ApplicationLifeStates = new ConcurrentDictionary<ApplicationState, ApplicationLifeStateData>();
            Genders = new ConcurrentDictionary<string, GenderData>();
            Provinces = new ConcurrentDictionary<string, ProvinceData>();
            Mediums = new ConcurrentDictionary<string, MediumData>();
            Languages = new ConcurrentDictionary<string, LanguageData>();
            ApplicationCategories = new ConcurrentDictionary<string, ApplicationCategoryData>();
            ApplicationReasons = new ConcurrentDictionary<string, ApplicationReasonData>();
            Countries = new ConcurrentDictionary<string, CountryData>();
            DocumentTypes = new ConcurrentDictionary<string, DocumentTypeData>();
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

        public async Task LoadApplicationComments(IApplicationCommentsRepository applicationCommentsRepository)
        {
            var applicationComments = await applicationCommentsRepository.GetApplicationComments();
            ApplicationComments.AddRange(applicationComments.Items);
        }

        public async Task LoadActiveStatuses(IActiveStatusRepository activeStatusRepository)
        {
            var data = await activeStatusRepository.GetActiveStatus();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var activeStatus in data.Items)
                ActiveStatuses.TryAdd(activeStatus.ActvSt_Cd, activeStatus);
        }

        public async Task LoadApplicationLifeStates(IApplicationLifeStateRepository applicationLifeStateRepository)
        {
            var data = await applicationLifeStateRepository.GetApplicationLifeStates();

            foreach (var applicationLifeState in data.Items)
                ApplicationLifeStates.TryAdd(applicationLifeState.AppLiSt_Cd, applicationLifeState);
        }

        public async Task LoadGenders(IGenderRepository genderRepository)
        {
            var data = await genderRepository.GetGenders();

            if (genderRepository.Messages.Count > 0)
                Messages.AddRange(genderRepository.Messages);

            foreach (var gender in data.Items)
                Genders.TryAdd(gender.Gender_Cd, gender);
        }

        public async Task LoadProvinces(IProvinceRepository provinceRepository)
        {
            var data = await provinceRepository.GetProvinces();

            foreach (var province in data)
                Provinces.TryAdd(province.PrvCd, province);
        }

        public async Task LoadMediums(IMediumRepository mediumRepository)
        {
            var data = await mediumRepository.GetMediums();

            if (mediumRepository.Messages.Count > 0)
                Messages.AddRange(mediumRepository.Messages);

            foreach (var medium in data.Items)
                Mediums.TryAdd(medium.Medium_Cd, medium);
        }

        public async Task LoadLanguages(ILanguageRepository languageRepository)
        {
            var data = await languageRepository.GetLanguages();

            if (languageRepository.Messages.Count > 0)
                Messages.AddRange(languageRepository.Messages);

            foreach (var language in data.Items)
                Languages.TryAdd(language.Lng_Cd, language);
        }

        public async Task LoadFoaEvents(IFoaEventsRepository foaMessageRepository)
        {
            var data = await foaMessageRepository.GetAllFoaMessages();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var item in data.FoaEvents)
            {
                FoaEvents.FoaEvents.TryAdd(item.Key, item.Value);
                //FoaEvents.FoaEvents.Add(item.Key, item.Value);
            }
            
        }

        public async Task LoadApplicationCategories(IApplicationCategoryRepository applicationCategoryRepository)
        {
            var data = await applicationCategoryRepository.GetApplicationCategories();

            if (applicationCategoryRepository.Messages.Count > 0)
                Messages.AddRange(applicationCategoryRepository.Messages);

            foreach (var applicationCategory in data.Items)
                ApplicationCategories.TryAdd(applicationCategory.AppCtgy_Cd, applicationCategory);

        }

        public async Task LoadApplicationReasons(IApplicationReasonRepository applicationReasonRepository)
        {
            var data = await applicationReasonRepository.GetApplicationReasons();

            if (applicationReasonRepository.Messages.Count > 0)
                Messages.AddRange(applicationReasonRepository.Messages);

            foreach (var applicationReason in data.Items)
                ApplicationReasons.TryAdd(applicationReason.AppReas_Cd.Trim(), applicationReason);
        }

        public async Task LoadCountries(ICountryRepository countryRepository)
        {
            var data = await countryRepository.GetCountries();

            if (countryRepository.Messages.Count > 0)
                Messages.AddRange(countryRepository.Messages);

            foreach (var country in data.Items)
                Countries.TryAdd(country.Ctry_Cd, country);
        }

        public async Task LoadDocumentTypes(IDocumentTypeRepository documentTypeRepository)
        {
            var data = await documentTypeRepository.GetDocumentTypes();

            if (documentTypeRepository.Messages.Count > 0)
                Messages.AddRange(documentTypeRepository.Messages);

            foreach (var docType in data.Items)
                DocumentTypes.TryAdd(docType.DocTyp_Cd, docType);
        }
    }
}
