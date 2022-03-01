using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    internal class DBDivertFunds : DBbase, IDivertFundsRepository
    {
        public DBDivertFunds(IDBTools mainDB) : base(mainDB)
        {

        }

        public decimal GetTotalDivertedForPeriod(string appl_EnfSrv_Cd, string appl_CtrlCd, int period)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"CPdCnt", period },
                    {"chrAppl_EnfSrv_Cd", appl_EnfSrv_Cd },
                    {"chrAppl_CtrlCd", appl_CtrlCd }
                };

            return MainDB.GetDataFromStoredProc<decimal>("GetTtlPymDivertedForCurrPeriod", parameters);
        }

        public decimal GetTotalFeesDiverted(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isCumulativeFees)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"chrAppl_EnfSrv_Cd", appl_EnfSrv_Cd },
                    {"chrAppl_CtrlCd", appl_CtrlCd }
                };

            if (isCumulativeFees)
                return MainDB.GetDataFromStoredProc<decimal>("GetTtlFeesDiverted", parameters);
            else
                return MainDB.GetDataFromStoredProc<decimal>("GetTtlFeesDivertedNonCumulative", parameters);
        }
    }
}
