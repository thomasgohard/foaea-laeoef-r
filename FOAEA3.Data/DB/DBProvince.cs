using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;
using System;

namespace FOAEA3.Data.DB
{
    internal class DBProvince : DBbase, IProvinceRepository
    {
        public DBProvince(IDBTools mainDB) : base(mainDB)
        {

        }
        public List<ProvinceData> GetProvinces()
        {
            var data = MainDB.GetAllData<ProvinceData>("Prv", FillProvinceDataFromReader);

            if (!string.IsNullOrEmpty(MainDB.LastError))
                throw new Exception(MainDB.LastError);

            return data;
        }

        private void FillProvinceDataFromReader(IDBHelperReader rdr, ProvinceData data)
        {
            data.PrvCd = rdr["Prv_Cd"] as string;
            data.PrvTxtE = rdr["Prv_Txt_E"] as string; // can be null
            data.PrvTxtF = rdr["Prv_Txt_F"] as string; // can be null
            data.PrvCtryCd = rdr["Prv_Ctry_Cd"] as string; // can be null
        }
    }

}
