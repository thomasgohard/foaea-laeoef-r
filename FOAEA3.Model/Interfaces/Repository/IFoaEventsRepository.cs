using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IFoaEventsRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        FoaEventDataDictionary GetAllFoaMessages();
    }
}