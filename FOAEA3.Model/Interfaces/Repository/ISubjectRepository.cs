using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISubjectRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<SubjectData>> GetSubjectsForSubmitterAsync(string submCd);
        Task<SubjectData> GetSubjectAsync(string subjectName);
        Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode);
    }
}