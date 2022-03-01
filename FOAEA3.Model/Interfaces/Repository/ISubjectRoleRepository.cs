using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ISubjectRoleRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<SubjectRoleData> GetSubjectRoles(string subjectName);
        List<string> GetAssumedRolesForSubject(string subjectName);
    }
}
