using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;

namespace TestData.TestDB
{
    public class InMemoryProvince : IProvinceRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public List<ProvinceData> GetProvinces()
        {
            throw new NotImplementedException();
        }
    }
}
