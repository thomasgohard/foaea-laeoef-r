using FOAEA3.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FOAEA3.API.broker
{
    public class InfoBanksAPI : BaseAPI
    {
        public List<InfoBankData> GetInfoBanks()
        {
            return GetDataAsync<List<InfoBankData>>($"api/v1/infoBanks").Result;
        }

        internal IEnumerable GetInfoActiveBanksForProvince(string provinceCode)
        {
            var data = GetDataAsync<List<InfoBankData>>($"api/v1/infoBanks").Result;
            return data.FindAll(m => (m.ActvSt_Cd == "A") && (m.Prv_Cd == provinceCode));
        }
    }
}
