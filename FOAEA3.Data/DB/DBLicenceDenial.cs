using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Data.DB
{
    internal class DBLicenceDenial : DBbase, ILicenceDenialRepository
    {
        public DBLicenceDenial(IDBTools mainDB) : base(mainDB)
        {

        }

        public LicenceDenialApplicationData GetLicenceDenialData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_CtrlCd", appl_CtrlCd },
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd}
                    };

            var data = MainDB.GetDataFromStoredProc<LicenceDenialApplicationData>("LicSusp_Select", parameters, FillDataFromReader);

            //            if (!string.IsNullOrEmpty(MainDB.LastError))

            var result = data.FirstOrDefault();

            return result; // returns null if no data found
        }

        private void FillDataFromReader(IDBHelperReader rdr, LicenceDenialApplicationData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.LicSusp_SupportOrder_Dte = (DateTime)rdr["LicSusp_SupportOrder_Dte"];
            data.LicSusp_NoticeSentToDbtr_Dte = (DateTime)rdr["LicSusp_NoticeSentToDbtr_Dte"];
            data.LicSusp_CourtNme = rdr["LicSusp_CourtNme"] as string;
            data.PymPr_Cd = rdr["PymPr_Cd"] as string;
            data.LicSusp_NrOfPymntsInDefault = rdr["LicSusp_NrOfPymntsInDefault"] as short?; // can be null 
            data.LicSusp_AmntOfArrears = rdr["LicSusp_AmntOfArrears"] as decimal?; // can be null 
            data.LicSusp_Dbtr_EmplNme = rdr["LicSusp_Dbtr_EmplNme"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_Ln = rdr["LicSusp_Dbtr_EmplAddr_Ln"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_Ln1 = rdr["LicSusp_Dbtr_EmplAddr_Ln1"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_CityNme = rdr["LicSusp_Dbtr_EmplAddr_CityNme"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_PrvCd = rdr["LicSusp_Dbtr_EmplAddr_PrvCd"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_CtryCd = rdr["LicSusp_Dbtr_EmplAddr_CtryCd"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_PCd = rdr["LicSusp_Dbtr_EmplAddr_PCd"] as string; // can be null 
            data.LicSusp_Dbtr_EyesColorCd = rdr["LicSusp_Dbtr_EyesColorCd"] as string; // can be null 
            data.LicSusp_Dbtr_HeightUOMCd = rdr["LicSusp_Dbtr_HeightUOMCd"] as string; // can be null 
            data.LicSusp_Dbtr_HeightQty = rdr["LicSusp_Dbtr_HeightQty"] as int?; // can be null 
            data.LicSusp_Dbtr_Brth_CityNme = rdr["LicSusp_Dbtr_Brth_CityNme"] as string; // can be null 
            data.LicSusp_Dbtr_Brth_CtryCd = rdr["LicSusp_Dbtr_Brth_CtryCd"] as string;
            data.LicSusp_TermRequestDte = rdr["LicSusp_TermRequestDte"] as DateTime?; // can be null 
            data.LicSusp_Still_InEffect_Ind = (byte)rdr["LicSusp_Still_InEffect_Ind"];
            data.LicSusp_AnyLicRvkd_Ind = (byte)rdr["LicSusp_AnyLicRvkd_Ind"];
            data.LicSusp_AnyLicReinst_Ind = (byte)rdr["LicSusp_AnyLicReinst_Ind"];
            data.LicSusp_LiStCd = (short)rdr["LicSusp_LiStCd"];
            data.LicSusp_Appl_CtrlCd = rdr["LicSusp_Appl_CtrlCd"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_Ln = rdr["LicSusp_Dbtr_LastAddr_Ln"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_Ln1 = rdr["LicSusp_Dbtr_LastAddr_Ln1"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_CityNme = rdr["LicSusp_Dbtr_LastAddr_CityNme"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_PrvCd = rdr["LicSusp_Dbtr_LastAddr_PrvCd"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_CtryCd = rdr["LicSusp_Dbtr_LastAddr_CtryCd"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_PCd = rdr["LicSusp_Dbtr_LastAddr_PCd"] as string; // can be null 
        }
    }
}
