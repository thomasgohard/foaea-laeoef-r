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

        public async Task<SubjectData> GetSubjectAsync(string subjectName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<SubjectData>> GetSubjectsForSubmitterAsync(string submCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
