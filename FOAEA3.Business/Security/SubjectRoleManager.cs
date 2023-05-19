using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    internal class SubjectRoleManager
    {
        private readonly IRepositories DB;


        public SubjectRoleManager(IRepositories repositories)
        {
            DB = repositories;
        }
        public async Task<List<SubjectRoleData>> GetSubjectRolesAsync(string subjectName)
        {
            return await DB.SubjectRoleTable.GetSubjectRolesAsync(subjectName);
        }
        public async Task<List<string>> GetAssumedRolesForSubjectAsync(string subjectName)
        {
            return await DB.SubjectRoleTable.GetAssumedRolesForSubjectAsync(subjectName);
        }
    }
}
