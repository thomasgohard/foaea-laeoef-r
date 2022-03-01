using FOAEA3.Model.Base;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IGenderRepository  : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<GenderData> GetGenders();
    }
}