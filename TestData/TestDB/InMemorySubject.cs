using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySubject : ISubjectRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<SubjectData> GetSubjectAsync(string subjectName)
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

        public Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubjectData>> GetSubjectsForSubmitterAsync(string submCd)
        {
            throw new NotImplementedException();
        }

    }
}
