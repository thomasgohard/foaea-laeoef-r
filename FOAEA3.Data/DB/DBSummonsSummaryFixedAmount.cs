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
    internal class DBSummonsSummaryFixedAmount : DBbase, ISummonsSummaryFixedAmountRepository
    {
        public DBSummonsSummaryFixedAmount(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<SummonsSummaryFixedAmountData> GetSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            List<SummonsSummaryFixedAmountData> data = await MainDB.GetDataFromStoredProcAsync<SummonsSummaryFixedAmountData>("GetSummSmryFixedAmountRecalcDateData", parameters, FillDataFromReader);

            return data.FirstOrDefault(); // returns null if no data found

        }

        private void FillDataFromReader(IDBHelperReader rdr, SummonsSummaryFixedAmountData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.SummSmry_LastFixedAmountCalc_Dte = (DateTime)rdr["SummSmry_LastFixedAmountCalc_Dte"];
            data.SummSmry_FixedAmount_Recalc_Dte = (DateTime)rdr["SummSmry_FixedAmount_Recalc_Dte"];
        }

        public async Task CreateSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate)
        {
            var parameters = new Dictionary<string, object> {
                { "Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                { "Appl_CtrlCd" , appl_CtrlCd },
                { "SummSmry_LastFixedAmountCalc_Dte", DateTime.Now },
                { "SummSmry_FixedAmount_Recalc_Dte", fixedAmountRecalcDate }
            };

            _ = await MainDB.ExecProcAsync("SummSmryFixedAmountRecalcDate_Insert", parameters);

        }

        public async Task UpdateSummonsSummaryFixedAmount(SummonsSummaryFixedAmountData summSmryFixedAmount)
        {
            var parameters = new Dictionary<string, object> {
                { "Appl_EnfSrv_Cd", summSmryFixedAmount.Appl_EnfSrv_Cd },
                { "Appl_CtrlCd" , summSmryFixedAmount.Appl_CtrlCd },
                { "SummSmry_LastFixedAmountCalc_Dte", summSmryFixedAmount.SummSmry_LastFixedAmountCalc_Dte },
                { "SummSmry_FixedAmount_Recalc_Dte", summSmryFixedAmount.SummSmry_FixedAmount_Recalc_Dte }
            };

            _ = await MainDB.ExecProcAsync("SummSmryFixedAmountRecalcDate_Update", parameters);
        }

        public async Task DeleteSummSmryFixedAmountRecalcDate(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object> {
                { "Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                { "Appl_CtrlCd" , appl_CtrlCd }
            };

            _ = await MainDB.ExecProcAsync("DeleteSummSmryFixedAmountRecalcDateData", parameters);
        }

    }
}
