using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;

namespace TestData.TestDB
{
    public class InMemorySubject : ISubjectRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public List<SubjectData> GetSubjectsForSubmitter(string submCd)
        {
            throw new NotImplementedException();
        }
        public SubjectData GetSubject(string submCd)
        {
            throw new NotImplementedException();
        }

        public SubjectData GetSubjectByConfirmationCode(string confirmationCode)
        {
            throw new NotImplementedException();
        }
    }
}
