using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    internal class SubjectRoleManager
    {
        private readonly IRepositories Repositories;


        public SubjectRoleManager(IRepositories repositories)
        {
            Repositories = repositories;
        }
        public async Task<List<SubjectRoleData>> GetSubjectRolesAsync(string subjectName)
        {
            return await Repositories.SubjectRoleRepository.GetSubjectRolesAsync(subjectName);
        }
        public async Task<List<string>> GetAssumedRolesForSubjectAsync(string subjectName)
        {
            return await Repositories.SubjectRoleRepository.GetAssumedRolesForSubjectAsync(subjectName);
        }
    }
}
