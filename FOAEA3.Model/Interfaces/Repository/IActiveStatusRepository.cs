using FOAEA3.Model.Base;

namespace FOAEA3.Model.Interfaces
{
    public interface IActiveStatusRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<ActiveStatusData> GetActiveStatus();
    }
}