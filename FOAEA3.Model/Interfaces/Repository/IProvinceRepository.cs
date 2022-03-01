using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IProvinceRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<ProvinceData> GetProvinces();
    }
}