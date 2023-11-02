using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySubject : ISubjectRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<SubjectData> GetSubject(string subjectName)
        {
            var subjectData = new SubjectData
            {
                SubjectName = subjectName,
                IsAccountLocked = false,
                Password = "9KnDVQ6m/Te4rH8Mae5aX+2gVVSMzTktJvAQTNMgSfA=",
                PasswordSalt = ""
            };

            return Task.FromResult(subjectData);
        }

        public Task<SubjectData> GetSubjectByConfirmationCode(string confirmationCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubjectData>> GetSubjectsForSubmitter(string submCd)
        {
            throw new NotImplementedException();
        }

    }
}
