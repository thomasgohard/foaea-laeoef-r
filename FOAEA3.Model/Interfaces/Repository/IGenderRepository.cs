using FOAEA3.Model.Base;

namespace FOAEA3.Model.Interfaces
{
    public interface IGenderRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<GenderData> GetGenders();
    }
}