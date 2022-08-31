using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISINChangeHistoryRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> CreateSINChangeHistoryAsync(SINChangeHistoryData data);

    }
}
