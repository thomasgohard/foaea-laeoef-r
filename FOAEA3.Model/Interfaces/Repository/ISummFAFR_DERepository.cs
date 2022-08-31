using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummFAFR_DERepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<SummFAFR_DE_Data>> GetSummFaFrDeAsync(int summFAFR_Id);
        Task<DataList<SummFAFR_DE_Data>> GetSummFaFrDeReadyBatchesAsync(string enfSrv_Src_Cd, string DAFABatchId);
    }
}
