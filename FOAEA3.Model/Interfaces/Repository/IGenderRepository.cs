using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IGenderRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<DataList<GenderData>> GetGendersAsync();
    }
}