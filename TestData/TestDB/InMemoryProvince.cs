﻿using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryProvince : IProvinceRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<List<ProvinceData>> GetProvincesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
