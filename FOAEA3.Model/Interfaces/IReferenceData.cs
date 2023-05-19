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

        public Task LoadActiveStatusesAsync(IActiveStatusRepository activeStatusRepository);
        public Task LoadApplicationLifeStatesAsync(IApplicationLifeStateRepository applicationLifeStateRepository);
        public Task LoadFoaEventsAsync(IFoaEventsRepository foaMessageRepository);
        public Task LoadGendersAsync(IGenderRepository genderRepository);
        public Task LoadProvincesAsync(IProvinceRepository provinceRepository);
        public Task LoadMediumsAsync(IMediumRepository mediumRepository);
        public Task LoadLanguagesAsync(ILanguageRepository languageRepository);
        public Task LoadApplicationCategoriesAsync(IApplicationCategoryRepository applicationCategoryRepository);
        public Task LoadApplicationReasonsAsync(IApplicationReasonRepository applicationReasonRepository);
        public Task LoadCountriesAsync(ICountryRepository countryRepository);
        public Task LoadDocumentTypesAsync(IDocumentTypeRepository documentTypeRepository);
        public Task LoadApplicationCommentsAsync(IApplicationCommentsRepository applicationCommentsRepository);
    }
}
