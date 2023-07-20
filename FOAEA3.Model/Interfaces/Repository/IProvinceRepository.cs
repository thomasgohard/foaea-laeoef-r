using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IProvinceRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<ProvinceData>> GetProvinces();
    }
}