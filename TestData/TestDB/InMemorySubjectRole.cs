using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySubjectRole : ISubjectRoleRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public InMemorySubjectRole()
        {

        }

        public Task<List<SubjectRoleData>> GetSubjectRolesAsync(string subjectName)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetAssumedRolesForSubjectAsync(string subjectName)
        {
            throw new NotImplementedException();
        }
    }
}

