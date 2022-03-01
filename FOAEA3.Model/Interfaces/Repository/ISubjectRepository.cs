using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ISubjectRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<SubjectData> GetSubjectsForSubmitter(string submCd);
        SubjectData GetSubject(string subjectName);
        SubjectData GetSubjectByConfirmationCode(string confirmationCode);

    }
}