using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISubmitterProfileRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<SubmitterProfileData> GetSubmitterProfile(string submitterCode);
    }
}
