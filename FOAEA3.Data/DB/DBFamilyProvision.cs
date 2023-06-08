using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBFamilyProvision : DBbase, IFamilyProvisionRepository
    {
        public DBFamilyProvision(IDBToolsAsync mainDB) : base(mainDB)
        {
        }

        public async Task<List<FamilyProvisionData>> GetFamilyProvisions()
        {
            return await MainDB.GetAllDataAsync<FamilyProvisionData>("FamPro", FillDataFromReader);
        }

        private void FillDataFromReader(IDBHelperReader rdr, FamilyProvisionData data)
        {
            data.FamPro_Cd = rdr["FamPro_Cd"] as string;
            data.FamPro_Txt_E = rdr["FamPro_Txt_E"] as string;
            data.FamPro_Txt_F = rdr["FamPro_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
