using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISubjectRoleRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<SubjectRoleData>> GetSubjectRolesAsync(string subjectName);
        Task<List<string>> GetAssumedRolesForSubjectAsync(string subjectName);
    }
}
