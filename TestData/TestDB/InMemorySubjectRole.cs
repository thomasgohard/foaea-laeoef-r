using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    public class InMemorySubjectRole : ISubjectRoleRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public InMemorySubjectRole()
        {

        }
        public List<SubjectRoleData> GetSubjectRoles(string subjectName)
        {
            throw new NotImplementedException();
        }
        public List<string> GetAssumedRolesForSubject(string subjectName)
        {
            throw new NotImplementedException();
        }
    }
}

