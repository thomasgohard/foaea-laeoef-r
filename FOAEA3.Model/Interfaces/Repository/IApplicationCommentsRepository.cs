using FOAEA3.Model.Base;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationCommentsRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<ApplicationCommentsData> GetApplicationComments();
    }
}
