using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface IDivertFundsRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public decimal GetTotalDivertedForPeriod(string appl_EnfSrv_Cd, string appl_CtrlCd, int period);
        public decimal GetTotalFeesDiverted(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isCumulativeFees);
    }
}
