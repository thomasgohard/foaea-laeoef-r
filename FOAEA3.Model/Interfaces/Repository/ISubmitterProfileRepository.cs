using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISubmitterProfileRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<SubmitterProfileData> GetSubmitterProfileAsync(string submitterCode);
    }
}
