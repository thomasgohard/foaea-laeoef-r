using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class SubjectsAPI : BaseAPI
    {
        internal List<SubjectData> GetSubjectsForSubmitter(string submCd)
        {
            return GetDataAsync<List<SubjectData>>($"api/v1/subjects?submCd={submCd}").Result;
        }

        internal SubjectData GetSubject(string subjectName)
        {
            return GetDataAsync<SubjectData>($"api/v1/subjects/{subjectName}").Result;
        }

        internal SubjectData AcceptNewTermsOfReference(SubjectData subject)
        {
            return PutDataAsync<SubjectData, SubjectData>($"api/v1/subjects/AcceptTermsOfReference", subject).Result;
        }
    }
}
