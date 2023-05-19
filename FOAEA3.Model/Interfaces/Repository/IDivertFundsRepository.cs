using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IDivertFundsRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<decimal> GetTotalDivertedForPeriodAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, int period);
        Task<decimal> GetTotalFeesDivertedAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isCumulativeFees);
    }
}
