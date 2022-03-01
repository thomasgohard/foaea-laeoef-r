using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FOAEA3.Data.Base
{
    public sealed class ReferenceData : IReferenceData, IMessageList
    {

        private static ReferenceData instance = null;

        private static readonly object syncLock = new();

        private ReferenceData()
        {
            Messages = new MessageDataList();
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

        public Dictionary<string, ActiveStatusData> ActiveStatuses { get; set; }
        public Dictionary<ApplicationState, ApplicationLifeStateData> ApplicationLifeStates { get; set; }
        public FoaEventDataDictionary FoaEvents { get; set; }
        public Dictionary<string, GenderData> Genders { get; set; }
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();
        public DataList<ApplicationCommentsData> ApplicationComments { get; set; }

        public MessageDataList Messages { get; set; }

        public void LoadApplicationComments(IApplicationCommentsRepository applicationCommentsRepository)
        {
            ApplicationComments = applicationCommentsRepository.GetApplicationComments();
        }

        public void LoadActiveStatuses(IActiveStatusRepository activeStatusRepository)
        {
            ActiveStatuses = new Dictionary<string, ActiveStatusData>();

            DataList<ActiveStatusData> data = activeStatusRepository.GetActiveStatus();

            if (data.Messages.Count > 0)
                Messages.AddRange(data.Messages);

            foreach (var activeStatus in data.Items)
                ActiveStatuses.Add(activeStatus.ActvSt_Cd, activeStatus);

        }

        public void LoadApplicationLifeStates(IApplicationLifeStateRepository applicationLifeStateRepository)
        {
            ApplicationLifeStates = new Dictionary<ApplicationState, ApplicationLifeStateData>();

            DataList<ApplicationLifeStateData> data = applicationLifeStateRepository.GetApplicationLifeStates();

            foreach (var applicationLifeState in data.Items)
                ApplicationLifeStates.Add(applicationLifeState.AppLiSt_Cd, applicationLifeState);
        }

        public void LoadGenders(IGenderRepository genderRepository)
        {
            Genders = new Dictionary<string, GenderData>();

            DataList<GenderData> data = genderRepository.GetGenders();

            if (genderRepository.Messages.Count > 0)
                Messages.AddRange(genderRepository.Messages);

            data.Items.Sort();

            foreach (var gender in data.Items)
                Genders.Add(gender.Gender_Cd, gender);
        }

        public void LoadFoaEvents(IFoaEventsRepository foaMessageRepository)
        {
            FoaEvents = new FoaEventDataDictionary();

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
