using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ICraSinPendingRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task Insert(string oldSin, string newSin);
        Task Delete(string newSin);
    }
}
