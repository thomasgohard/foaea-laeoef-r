using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;

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

        public void LoadActiveStatuses(IActiveStatusRepository activeStatusRepository);
        public void LoadApplicationLifeStates(IApplicationLifeStateRepository applicationLifeStateRepository);
        public void LoadFoaEvents(IFoaEventsRepository foaMessageRepository);
        public void LoadGenders(IGenderRepository genderRepository);
        public void LoadProvinces(IProvinceRepository provinceRepository);
        public void LoadMediums(IMediumRepository mediumRepository);
        public void LoadLanguages(ILanguageRepository languageRepository);
        public void LoadApplicationCategories(IApplicationCategoryRepository applicationCategoryRepository);
        public void LoadApplicationReasons(IApplicationReasonRepository applicationReasonRepository);
        public void LoadCountries(ICountryRepository countryRepository);
        public void LoadDocumentTypes(IDocumentTypeRepository documentTypeRepository);
        public void LoadApplicationComments(IApplicationCommentsRepository applicationCommentsRepository);
    }
}
