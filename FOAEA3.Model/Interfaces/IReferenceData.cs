using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IReferenceData
    {
        public ConcurrentDictionary<string, ActiveStatusData> ActiveStatuses { get; }
        public ConcurrentDictionary<ApplicationState, ApplicationLifeStateData> ApplicationLifeStates { get; }
        public FoaEventDataDictionary FoaEvents { get; }
        public ConcurrentDictionary<string, GenderData> Genders { get; }
        public ConcurrentDictionary<string, ProvinceData> Provinces { get; }
        public ConcurrentDictionary<string, MediumData> Mediums { get; }
        public ConcurrentDictionary<string, LanguageData> Languages { get; }
        public ConcurrentDictionary<string, ApplicationCategoryData> ApplicationCategories { get; }
        public ConcurrentDictionary<string, ApplicationReasonData> ApplicationReasons { get; }
        public ConcurrentDictionary<string, CountryData> Countries { get; }
        public ConcurrentDictionary<string, DocumentTypeData> DocumentTypes { get; }
        public List<ApplicationCommentsData> ApplicationComments { get; }

        public Task LoadActiveStatuses(IActiveStatusRepository activeStatusRepository);
        public Task LoadApplicationLifeStates(IApplicationLifeStateRepository applicationLifeStateRepository);
        public Task LoadFoaEvents(IFoaEventsRepository foaMessageRepository);
        public Task LoadGenders(IGenderRepository genderRepository);
        public Task LoadProvinces(IProvinceRepository provinceRepository);
        public Task LoadMediums(IMediumRepository mediumRepository);
        public Task LoadLanguages(ILanguageRepository languageRepository);
        public Task LoadApplicationCategories(IApplicationCategoryRepository applicationCategoryRepository);
        public Task LoadApplicationReasons(IApplicationReasonRepository applicationReasonRepository);
        public Task LoadCountries(ICountryRepository countryRepository);
        public Task LoadDocumentTypes(IDocumentTypeRepository documentTypeRepository);
        public Task LoadApplicationComments(IApplicationCommentsRepository applicationCommentsRepository);
    }
}
