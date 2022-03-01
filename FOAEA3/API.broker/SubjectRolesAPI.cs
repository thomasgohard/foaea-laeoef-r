using FOAEA3.Model;
using System.Collections.Generic;

namespace FOAEA3.API.broker
{
    public class SubjectRolesAPI :BaseAPI
    {
        internal List<SubjectRoleData> GetSubjectRoles(string subjectName)
        {
            return GetDataAsync<List<SubjectRoleData>>($"api/v1/subjectroles?subjectName={subjectName}").Result;
        }

        internal List<string> GetAssumedRolesForSubject(string subjectName)           
        {
            return GetDataAsync<List<string>>($"api/v1/subjectroles/{subjectName}").Result;
            //return GetDataAsync<List<string>>($"api/v1/GetAssumedRolesForSubject?subjectName={subjectName}").Result;
        }
    }
}
