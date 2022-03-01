using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Data.DB
{
    internal class DBSubmitterProfile : DBbase, ISubmitterProfileRepository
    {

        public DBSubmitterProfile(IDBTools mainDB) : base(mainDB)
        {

        }
        public SubmitterProfileData GetSubmitterProfile(string submitterCode)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Subm_SubmCd", submitterCode}
                };
            return MainDB.GetDataFromStoredProc<SubmitterProfileData>("UserGetSubmData", parameters, FillUserDataFromReader).ElementAt(0);
        }

        private void FillUserDataFromReader(IDBHelperReader rdr, SubmitterProfileData data)
        {
                data.Subm_SubmCd = (string)(rdr["Subm_SubmCd"]);
                data.Subm_FrstNme = (string)(rdr["Subm_FrstNme"]);
                data.Subm_MddleNme = rdr["Subm_MddleNme"] as string ?? string.Empty;
                data.Subm_SurNme = (string)(rdr["Subm_SurNme"]);
                data.Lng_Cd = (string)(rdr["Lng_Cd"]);
                data.Subm_LstLgn_Dte = (DateTime?)(rdr["Subm_LstLgn_Dte"]);
                data.Subm_Assg_Email = rdr["Subm_Assg_Email"] as string ?? string.Empty;
                data.Subm_Lic_AccsPrvCd = (byte?)(rdr["Subm_Lic_AccsPrvCd"]);
                data.Subm_Trcn_AccsPrvCd = (byte?)(rdr["Subm_Trcn_AccsPrvCd"]);
                data.Subm_Intrc_AccsPrvCd = (byte?)(rdr["Subm_Intrc_AccsPrvCd"]);
                data.Subm_Last_SeqNr = rdr["Subm_Last_SeqNr"] as string ?? string.Empty;
                data.Subm_Altrn_SubmCd = rdr["Subm_Altrn_SubmCd"] as string ?? string.Empty;
                data.EnfSrv_Cd = (string)(rdr["EnfSrv_Cd"]);
                data.EnfOff_City_LocCd = (string)(rdr["EnfOff_City_LocCd"]);
                data.Subm_TrcNtf_Ind = (byte?)(rdr["Subm_TrcNtf_Ind"]);
                data.Subm_LglSgnAuth_Ind = (byte?)(rdr["Subm_LglSgnAuth_Ind"]);
                data.Subm_SgnAuth_SubmCd = rdr["Subm_SgnAuth_SubmCd"] as string ?? string.Empty; 
                data.Subm_EnfSrvAuth_Ind = (byte?)(rdr["Subm_EnfSrvAuth_Ind"]);
                data.Subm_EnfOffAuth_Ind = (byte?)(rdr["Subm_EnfOffAuth_Ind"]);
                data.Subm_SysMgr_Ind = (byte?)(rdr["Subm_SysMgr_Ind"]);
                data.Subm_AppMgr_Ind = (byte?)(rdr["Subm_AppMgr_Ind"]);
                data.EnfSrv_Nme = (string)(rdr["EnfSrv_Nme"]);
                data.EnfSrv_Subm_Auth_SubmCd = rdr["EnfSrv_Subm_Auth_SubmCd"] as string ?? string.Empty;
                data.EnfOff_Nme = rdr["EnfOff_Nme"] as string ?? string.Empty;
                data.EnfOff_Subm_Auth_SubmCd = rdr["EnfOff_Subm_Auth_SubmCd"] as string ?? string.Empty;
                data.EnfOff_Addr_CityNme = (string)(rdr["EnfOff_Addr_CityNme"]);
                data.EnfOff_Dstrct_Nme = (string)(rdr["EnfOff_Dstrct_Nme"]);
                data.Prv_Txt_E = rdr["Prv_Txt_E"] as string ?? string.Empty;
                data.Prv_Cd = (string)(rdr["Prv_Cd"]);
                data.Ctry_Txt_E = (string)(rdr["Ctry_Txt_E"]);
                data.Ctry_Cd = (string)(rdr["Ctry_Cd"]);
                data.Subm_Class = rdr["Subm_Class"] as string ?? string.Empty;
                if (rdr.ColumnExists("ActvSt_Cd"))
                    data.ActvSt_Cd = (string)(rdr["ActvSt_Cd"]);
                data.Subm_Fin_Ind = (byte?)(rdr["Subm_Fin_Ind"]);
        }
    }
}
