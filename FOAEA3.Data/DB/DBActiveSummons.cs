using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBActiveSummons : DBbase, IActiveSummonsRepository
    {
        public DBActiveSummons(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<ActiveSummonsCoreData> GetActiveSummonsCore(DateTime payableDate, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"CtrlFaDtePayable", payableDate },
                    {"CtrlApplEnfSrvCd", appl_EnfSrv_Cd },
                    {"CtrlApplCtrlCd", appl_CtrlCd }
                };

            List<ActiveSummonsCoreData> data;
            data = await MainDB.GetDataFromStoredProcAsync<ActiveSummonsCoreData>("GetActSummonsForOwedOrVary", parameters, FillDataFromReader);

            return data.FirstOrDefault();
        }

        public async Task<ActiveSummonsData> GetActiveSummonsData(DateTime payableDate, string appl_CtrlCd, string appl_EnfSrv_Cd, bool isVariation = false)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"CtrlEnfSrvCd", string.Empty}, // not used anymore
                    {"CtrlFaDtePayable", payableDate },
                    {"chrApplEnfSrvCd", appl_EnfSrv_Cd },
                    {"chrApplCtrlCd", appl_CtrlCd }
                };

            List<ActiveSummonsData> data;
            if (!isVariation)
                data = await MainDB.GetDataFromStoredProcAsync<ActiveSummonsData>("GetActiveSummonsesForDebtor", parameters, FillDataFromReaderForActiveDebtor);
            else
                data = await MainDB.GetDataFromStoredProcAsync<ActiveSummonsData>("GetPendingSummonsesForDebtor", parameters, FillDataFromReaderForActiveDebtor);

            return data.FirstOrDefault();
        }

        public async Task<DateTime> GetLegalDate(string appl_CtrlCd, string appl_EnfSrv_Cd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            return await MainDB.GetDataFromStoredProcAsync<DateTime>("Appl_GetLegalDate", parameters);
        }

        // 
        private void FillDataFromReader(IDBHelperReader rdr, ActiveSummonsCoreData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Appl_TotalAmnt = (decimal)rdr["Appl_TotalAmnt"];
        }

        private void FillDataFromReaderForActiveDebtor(IDBHelperReader rdr, ActiveSummonsData data)
        {
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Appl_JusticeNr = rdr["Appl_JusticeNr"] as string; // can be null 
            data.IntFinH_LmpSum_Money = (decimal)rdr["IntFinH_LmpSum_Money"];
            data.IntFinH_PerPym_Money = rdr["IntFinH_PerPym_Money"] as decimal?; // can be null 
            data.IntFinH_MxmTtl_Money = rdr["IntFinH_MxmTtl_Money"] as decimal?; // can be null 
            data.PymPr_Cd = rdr["PymPr_Cd"] as string; // can be null 
            if (rdr.ColumnExists("IntFinH_NextRecalcDate_Cd"))
                data.IntFinH_NextRecalcDate_Cd = rdr["IntFinH_NextRecalcDate_Cd"] as int?;
            data.IntFinH_CmlPrPym_Ind = rdr["IntFinH_CmlPrPym_Ind"] as byte?; // can be null 
            data.HldbCtg_Cd = rdr["HldbCtg_Cd"] as string;
            data.IntFinH_DefHldbPrcnt = rdr["IntFinH_DefHldbPrcnt"] as int?; // can be null 
            data.IntFinH_DefHldbAmn_Money = rdr["IntFinH_DefHldbAmn_Money"] as decimal?; // can be null 
            data.IntFinH_DefHldbAmn_Period = rdr["IntFinH_DefHldbAmn_Period"] as string; // can be null 
            data.HldbTyp_Cd = rdr["HldbTyp_Cd"] as string; // can be null 
            data.Start_Dte = (DateTime)rdr["Start_Dte"];
            data.FeeDivertedTtl_Money = (decimal)rdr["FeeDivertedTtl_Money"];
            data.LmpSumDivertedTtl_Money = (decimal)rdr["LmpSumDivertedTtl_Money"];
            data.PerPymDivertedTtl_Money = (decimal)rdr["PerPymDivertedTtl_Money"];
            data.HldbAmtTtl_Money = (decimal)rdr["HldbAmtTtl_Money"];
            data.Appl_TotalAmnt = (decimal)rdr["Appl_TotalAmnt"];
            data.IntFinH_Dte = (DateTime)rdr["IntFinH_Dte"];
            data.End_Dte = (DateTime)rdr["End_Dte"];
            data.Appl_RecvAffdvt_Dte = rdr["Appl_RecvAffdvt_Dte"] as DateTime?; // can be null 
            data.IntFinH_VarIss_Dte = rdr["IntFinH_VarIss_Dte"] as DateTime?; // can be null 
            data.LmpSumDivertedTtl_Money = (decimal)rdr["LmpSumDivertedTtl_Money"];
            data.PerPymDivertedTtl_Money = (decimal)rdr["PerPymDivertedTtl_Money"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.VarEnterDte = rdr["VarEnterDte"] as DateTime?; // can be null
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
        }

    }
}
