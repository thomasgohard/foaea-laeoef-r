using FOAEA3.Model.Base;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IApplicationCategoryRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<ApplicationCategoryData> GetApplicationCategories();
    }
}
