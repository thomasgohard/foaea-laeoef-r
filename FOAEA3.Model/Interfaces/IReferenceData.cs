using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface IReferenceData
    {
        public Dictionary<string, ActiveStatusData> ActiveStatuses { get; }
        public Dictionary<ApplicationState, ApplicationLifeStateData> ApplicationLifeStates { get; }
        public FoaEventDataDictionary FoaEvents { get; } 
        public Dictionary<string, GenderData> Genders { get; } 
        public Dictionary<string, string> Configuration { get; }
        public DataList<ApplicationCommentsData> ApplicationComments { get; }
        public void LoadApplicationComments(IApplicationCommentsRepository applicationCommentsRepository);
        public void LoadActiveStatuses(IActiveStatusRepository activeStatusRepository);
        public void LoadApplicationLifeStates(IApplicationLifeStateRepository applicationLifeStateRepository);
        public void LoadGenders(IGenderRepository genderRepository);
        public void LoadFoaEvents(IFoaEventsRepository foaMessageRepository);
    }
}
