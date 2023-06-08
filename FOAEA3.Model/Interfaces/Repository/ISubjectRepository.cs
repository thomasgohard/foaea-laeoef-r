using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISubjectRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<SubjectData>> GetSubjectsForSubmitter(string submCd);
        Task<SubjectData> GetSubject(string subjectName);
        Task<SubjectData> GetSubjectByConfirmationCode(string confirmationCode);
    }
}