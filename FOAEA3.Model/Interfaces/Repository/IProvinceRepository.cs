using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IProvinceRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<ProvinceData>> GetProvincesAsync();
    }
}