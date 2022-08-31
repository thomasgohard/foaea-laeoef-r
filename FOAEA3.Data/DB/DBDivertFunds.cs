using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBDivertFunds : DBbase, IDivertFundsRepository
    {
        public DBDivertFunds(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<decimal> GetTotalDivertedForPeriodAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, int period)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"CPdCnt", period },
                    {"chrAppl_EnfSrv_Cd", appl_EnfSrv_Cd },
                    {"chrAppl_CtrlCd", appl_CtrlCd }
                };

            return await MainDB.GetDataFromStoredProcAsync<decimal>("GetTtlPymDivertedForCurrPeriod", parameters);
        }

        public async Task<decimal> GetTotalFeesDivertedAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isCumulativeFees)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"chrAppl_EnfSrv_Cd", appl_EnfSrv_Cd },
                    {"chrAppl_CtrlCd", appl_CtrlCd }
                };

            if (isCumulativeFees)
                return await MainDB.GetDataFromStoredProcAsync<decimal>("GetTtlFeesDiverted", parameters);
            else
                return await MainDB.GetDataFromStoredProcAsync<decimal>("GetTtlFeesDivertedNonCumulative", parameters);
        }
    }
}
