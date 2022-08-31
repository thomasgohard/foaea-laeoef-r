using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBEnfOff : DBbase, IEnfOffRepository
    {
        public DBEnfOff(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<List<EnfOffData>> GetEnfOffAsync(string enfOffName = null, string enfOffCode = null, 
            string province = null, string enfServCode = null)
        {
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(enfOffName))
                parameters["EnfOff_Nme"] = enfOffName;
            else
                parameters["EnfOff_Nme"] = DBNull.Value;

            if (!string.IsNullOrEmpty(enfOffCode))
                parameters["EnfOff_City_LocCd"] = enfOffCode;
            else
                parameters["EnfOff_City_LocCd"] = DBNull.Value;

            if (!string.IsNullOrEmpty(province))
                parameters["EnfOff_Addr_PrvCd"] = province;
            else
                parameters["EnfOff_Addr_PrvCd"] = DBNull.Value;

            if (!string.IsNullOrEmpty(enfServCode))
                parameters["EnfSrv_Cd"] = enfServCode;
            else
                parameters["EnfSrv_Cd"] = DBNull.Value;

            return await MainDB.GetDataFromStoredProcAsync<EnfOffData>("EnfOffGetEnforcementOffice", parameters, FillEnfOffDataFromReader);
        }

        private void FillEnfOffDataFromReader(IDBHelperReader rdr, EnfOffData data)
        {
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.EnfOff_City_LocCd = rdr["EnfOff_City_LocCd"] as string;
            data.EnfOff_AbbrCd = rdr["EnfOff_AbbrCd"] as string;
            data.EnfOff_OrgCd = rdr["EnfOff_OrgCd"] as string; // can be null
            data.EnfOff_Nme = rdr["EnfOff_Nme"] as string; // can be null
            data.Subm_Auth_SubmCd = rdr["Subm_Auth_SubmCd"] as string; // can be null
            data.EnfOff_Tel_AreaC = (short)rdr["EnfOff_Tel_AreaC"];
            data.EnfOff_TelNr = (int)rdr["EnfOff_TelNr"];
            data.EnfOff_TelEx = rdr["EnfOff_TelEx"] as int?; // can be null
            data.EnfOff_Fax_AreaC = (short)rdr["EnfOff_Fax_AreaC"];
            data.EnfOff_FaxNr = (int)rdr["EnfOff_FaxNr"];
            data.EnfOff_Addr_Ln = rdr["EnfOff_Addr_Ln"] as string;
            data.EnfOff_Addr_Ln1 = rdr["EnfOff_Addr_Ln1"] as string; // can be null
            data.EnfOff_Addr_CityNme = rdr["EnfOff_Addr_CityNme"] as string;
            data.EnfOff_Addr_PrvCd = rdr["EnfOff_Addr_PrvCd"] as string;
            data.EnfOff_Addr_CtryCd = rdr["EnfOff_Addr_CtryCd"] as string;
            data.EnfOff_Addr_PCd = rdr["EnfOff_Addr_PCd"] as string;
            data.EnfOff_Dstrct_Nme = rdr["EnfOff_Dstrct_Nme"] as string;
            data.EnfOff_Lic_AccsPrvCd = (byte)rdr["EnfOff_Lic_AccsPrvCd"];
            data.EnfOff_Trcn_AccsPrvCd = (byte)rdr["EnfOff_Trcn_AccsPrvCd"];
            data.EnfOff_Intrc_AccsPrvCd = (byte)rdr["EnfOff_Intrc_AccsPrvCd"];
            data.EnfOff_Fin_VndrCd = rdr["EnfOff_Fin_VndrCd"] as string;
            data.EnfOff_PrvAddr_Ln = rdr["EnfOff_PrvAddr_Ln"] as string; // can be null
            data.EnfOff_PrvAddr_Ln1 = rdr["EnfOff_PrvAddr_Ln1"] as string; // can be null
            data.EnfOff_PrvAddr_CityNme = rdr["EnfOff_PrvAddr_CityNme"] as string; // can be null
            data.EnfOff_PrvAddr_CtryCd = rdr["EnfOff_PrvAddr_CtryCd"] as string; // can be null
            data.EnfOff_PrvAddr_PrvCd = rdr["EnfOff_PrvAddr_PrvCd"] as string; // can be null
            data.EnfOff_PrvAddr_PCd = rdr["EnfOff_PrvAddr_PCd"] as string; // can be null
            data.EnfOff_Comments = rdr["EnfOff_Comments"] as string; // can be null
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.EnfOff_Tel_AreaC2 = rdr["EnfOff_Tel_AreaC2"] as short?; // can be null
            data.EnfOff_TelNr2 = rdr["EnfOff_TelNr2"] as int?; // can be null
            data.EnfOff_TelEx2 = rdr["EnfOff_TelEx2"] as int?; // can be null

            data.EnfSrv_Nme = rdr["EnfSrv_Nme"] as string;
        }

    }
}
