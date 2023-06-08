using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{

    internal class DBAffidavit : DBbase, IAffidavitRepository
    {
        public DBAffidavit(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<AffidavitData> GetAffidavitData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            List<AffidavitData> data = await MainDB.GetDataFromStoredProcAsync<AffidavitData>("GetAffidavitSwearingData",
                                                                                   parameters, FillDataFromReader);
            if (data.Count > 0)
                return data[0];
            else
                return new AffidavitData();
        }

        private void FillDataFromReader(IDBHelperReader rdr, AffidavitData data)
        {
            data.Event_Id = (int)rdr["Event_Id"];
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Subm_Affdvt_SubmCd = rdr["Subm_Affdvt_SubmCd"] as string;
            data.Affdvt_FileRecv_Dte = (DateTime)rdr["Affdvt_FileRecv_Dte"];
            data.Affdvt_Sworn_Dte = (DateTime)rdr["Affdvt_Sworn_Dte"];
            data.AppCtgy_Cd = rdr["AppCtgy_Cd"] as string;
            data.OriginalFileName = rdr["OriginalFileName"] as string;
            data.Affidvt_Prcs_Dte = rdr["Affidvt_Prcs_Dte"] as DateTime?; // can be null 
            data.Affidvt_ActvSt_Cd = rdr["Affidvt_ActvSt_Cd"] as string;
        }

    }
}
