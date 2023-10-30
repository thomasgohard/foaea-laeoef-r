using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IAffidavitRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<AffidavitData> GetAffidavitData(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task InsertAffidavitData(AffidavitData data);
        Task CloseAffidavitData(AffidavitData data);
    }
}
