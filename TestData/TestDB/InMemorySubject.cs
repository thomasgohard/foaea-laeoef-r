using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySubject : ISubjectRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task ClearRefreshToken(string subjectName)
        {
            throw new NotImplementedException();
        }

        public Task<SubjectData> GetSubjectAsync(string subjectName)
        {
            throw new NotImplementedException();
        }

        public Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubjectData>> GetSubjectsForSubmitterAsync(string submCd)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRefreshToken(string subjectName, byte[] refreshToken, DateTime refreshTokenExpiration)
        {
            throw new NotImplementedException();
        }
    }
}
