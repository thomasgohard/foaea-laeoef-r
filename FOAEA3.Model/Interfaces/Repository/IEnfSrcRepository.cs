using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IEnfSrcRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<int> GetEnfSrcCount(string enfSrv_Src_Cd, string enfSrv_Loc_Cd, string enfSrv_SubLoc_Cd);
    }
}
