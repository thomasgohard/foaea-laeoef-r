using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IAffidavitRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<AffidavitData> GetAffidavitDataAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
