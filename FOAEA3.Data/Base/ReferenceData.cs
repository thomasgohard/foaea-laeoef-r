using FOAEA3.Model;
using FOAEA3.Model.Base;
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

            data.Items.Sort();

            foreach (var gender in data.Items)
                Genders.Add(gender.Gender_Cd, gender);
        }

        public void LoadProvinces(IProvinceRepository provinceRepository)
        {
            var data = provinceRepository.GetProvinces();

            data.Sort();

            foreach (var province in data)
                Provinces.Add(province.PrvCd, province);
        }

        public void LoadMediums(IMediumRepository mediumRepository)
        {
            var data = mediumRepository.GetMediums();

            if (mediumRepository.Messages.Count > 0)
                Messages.AddRange(mediumRepository.Messages);

            data.Items.Sort();

            foreach (var medium in data.Items)
                Mediums.Add(medium.Medium_Cd, medium);
        }

        public void LoadLanguages(ILanguageRepository languageRepository)
        {
            var data = languageRepository.GetLanguages();

            if (languageRepository.Messages.Count > 0)
                Messages.AddRange(languageRepository.Messages);

            data.Items.Sort();

            foreach (var language in data.Items)
                Languages.Add(language.Lng_Cd, language);
        }

        public void LoadFoaEvents(IFoaEventsRepository foaMessageRepository)
        {
            var data = foaMessageRepository.GetAllFoaMessages();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var item in data.FoaEvents)
            {
                FoaEvents.FoaEvents.Add(item.Key, item.Value);
            }
        }

    }
}
