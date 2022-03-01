using FOAEA3.Model.Base;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationLifeStateRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<ApplicationLifeStateData> GetApplicationLifeStates();
    }
}