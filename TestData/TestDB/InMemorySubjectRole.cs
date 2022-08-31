using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<List<SubjectRoleData>> GetSubjectRolesAsync(string subjectName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetAssumedRolesForSubjectAsync(string subjectName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}

