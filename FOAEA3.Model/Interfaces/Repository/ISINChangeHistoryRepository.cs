using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISINChangeHistoryRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> CreateSINChangeHistory(SINChangeHistoryData data);

    }
}
