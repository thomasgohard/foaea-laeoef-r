using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IReferenceData
    {
        public Dictionary<string, ActiveStatusData> ActiveStatuses { get; }
        public Dictionary<ApplicationState, ApplicationLifeStateData> ApplicationLifeStates { get; }
        public FoaEventDataDictionary FoaEvents { get; }
        public Dictionary<string, GenderData> Genders { get; }
        public Dictionary<string, ProvinceData> Provinces { get; }
        public Dictionary<string, MediumData> Mediums { get; }
        public Dictionary<string, LanguageData> Languages { get; }
        public Dictionary<string, ApplicationCategoryData> ApplicationCategories { get; }
        public Dictionary<string, ApplicationReasonData> ApplicationReasons { get; }
        public Dictionary<string, CountryData> Countries { get; }
        public Dictionary<string, DocumentTypeData> DocumentTypes { get; }
        public List<ApplicationCommentsData> ApplicationComments { get; }

        public Dictionary<string, string> Configuration { get; }

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
