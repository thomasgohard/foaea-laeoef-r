using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ICaseManagementRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task CreateCaseManagementAsync(CaseManagementData caseManagementData);
    }
}
