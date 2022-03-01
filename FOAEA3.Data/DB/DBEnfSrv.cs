using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    internal class DBEnfSrv : DBbase, IEnfSrvRepository
    {
        public DBEnfSrv(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<EnfSrvData> GetEnfService(string enforcementServiceCode = null, string enforcementServiceName = null,
                                          string enforcementServiceProvince = null, string enforcementServiceCategory = null)
        {
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(enforcementServiceCode))
                parameters["EnfSrv_Cd"] = enforcementServiceCode;
            else
                parameters["EnfSrv_Cd"] = DBNull.Value;

            if (!string.IsNullOrEmpty(enforcementServiceName))
                parameters["EnfSrv_Nme"] = enforcementServiceName;
            else
                parameters["EnfSrv_Nme"] = DBNull.Value;

            if (!string.IsNullOrEmpty(enforcementServiceProvince))
                parameters["EnfSrv_Addr_PrvCd"] = enforcementServiceProvince;
            else
                parameters["EnfSrv_Addr_PrvCd"] = DBNull.Value;

            if (!string.IsNullOrEmpty(enforcementServiceCategory))
                parameters["EnfCtgy_Cd"] = enforcementServiceCategory;
            else
                parameters["EnfCtgy_Cd"] = DBNull.Value;

            return MainDB.GetDataFromStoredProc<EnfSrvData>("EnfSrvGetEnfService", parameters, FillEnfSrvDataFromReader);
        }

        private void FillEnfSrvDataFromReader(IDBHelperReader rdr, EnfSrvData data)
        {
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.EnfSrv_Last_SeqNr = (int)rdr["EnfSrv_Last_SeqNr"];
            data.EnfSrv_Nme = rdr["EnfSrv_Nme"] as string;
            data.Subm_Auth_SubmCd = rdr["Subm_Auth_SubmCd"] as string; // can be null
            data.EnfSrv_Tel_AreaC = (short)rdr["EnfSrv_Tel_AreaC"];
            data.EnfSrv_TelNr = (int)rdr["EnfSrv_TelNr"];
            data.EnfSrv_TelEx = rdr["EnfSrv_TelEx"] as int?; // can be null
            data.EnfSrv_Addr_Ln = (string)rdr["EnfSrv_Addr_Ln"];
            data.EnfSrv_Addr_Ln1 = rdr["EnfSrv_Addr_Ln1"] as string; // can be null
            data.EnfSrv_Addr_CityNme = rdr["EnfSrv_Addr_CityNme"] as string;
            data.EnfSrv_Addr_PrvCd = rdr["EnfSrv_Addr_PrvCd"] as string;
            data.EnfSrv_Addr_CtryCd = rdr["EnfSrv_Addr_CtryCd"] as string;
            data.EnfSrv_Addr_PCd = rdr["EnfSrv_Addr_PCd"] as string;
            data.EnfSrv_PrvAddr_Ln = rdr["EnfSrv_PrvAddr_Ln"] as string; // can be null
            data.EnfSrv_PrvAddr_Ln1 = rdr["EnfSrv_PrvAddr_Ln1"] as string; // can be null
            data.EnfSrv_PrvAddr_CityNme = rdr["EnfSrv_PrvAddr_CityNme"] as string; // can be null
            data.EnfSrv_PrvAddr_PrvCd = rdr["EnfSrv_PrvAddr_PrvCd"] as string; // can be null
            data.EnfSrv_PrvAddr_CtryCd = rdr["EnfSrv_PrvAddr_CtryCd"] as string; // can be null
            data.EnfSrv_PrvAddr_PCd = rdr["EnfSrv_PrvAddr_PCd"] as string; // can be null
            data.EnfSrv_Comments = rdr["EnfSrv_Comments"] as string; // can be null
            data.EnfCtgy_Cd = rdr["EnfCtgy_Cd"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.PaymentId_Is_SIN_Ind = rdr["PaymentId_Is_SIN_Ind"] as string; // can be null
            data.Ctrl_cd_char = rdr["Ctrl_cd_char"] as string; // can be null

            data.EnfSrv_Nme_F = rdr["EnfSrv_Nme_F"] as string; // can be null
        }
    }
}
