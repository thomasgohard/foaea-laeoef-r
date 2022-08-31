using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationLifeStateRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<DataList<ApplicationLifeStateData>> GetApplicationLifeStatesAsync();
    }
}