using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IApplicationCategoryRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<ApplicationCategoryData>> GetApplicationCategoriesAsync();
    }
}
