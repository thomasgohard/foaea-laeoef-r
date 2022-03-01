using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Business.Security
{
    internal class SubjectRoleManager
    {

        private readonly IRepositories Repositories;


        internal SubjectRoleManager(IRepositories repositories)
        {
            Repositories = repositories;
        }
        internal List<SubjectRoleData> GetSubjectRoles(string subjectName)
        {
            return Repositories.SubjectRoleRepository.GetSubjectRoles(subjectName);
        }
        internal List<string> GetAssumedRolesForSubject(string subjectName)
        {
            return Repositories.SubjectRoleRepository.GetAssumedRolesForSubject(subjectName);
        }
    }
}
