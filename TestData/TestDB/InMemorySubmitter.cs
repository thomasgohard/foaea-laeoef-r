using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    class InMemorySubmitter : ISubmitterRepository
    {

        public InMemorySubmitter()
        {

        }

        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task CreateSubmitterAsync(SubmitterData newSubmitter)
        {
            newSubmitter.Subm_Create_Usr = "Test";
            newSubmitter.Subm_CourtUsr_Ind = true;

            return Task.CompletedTask;   
        }

        public Task CreateSubmitterMessageAsync(SubmitterMessageData submitterMessage)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommissionerData>> GetCommissionersAsync(string locationCode, string currentSubmitter)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetMaxSubmitterCodeAsync(string submCodePart)
        {
            return Task.FromResult("02");
        }

        public Task<string> GetSignAuthorityForSubmitterAsync(string submCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubmitterData>> GetSubmitterAsync(string submCode = null, string submName = null,
                                                string enfOffCode = null, string enfServCode = null,
                                                string submFName = null, string submMName = null,
                                                string prov = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubmitterMessageData>> GetSubmitterMessageForSubmitterAsync(string submitterID, int languageCode)
        {
            throw new NotImplementedException();
        }

        public Task SubmitterMessageDeleteAsync(string submitterID)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSubmitterAsync(SubmitterData newSubmitter)
        {
            throw new NotImplementedException();
        }
        public Task<DateTime> UpdateSubmitterLastLoginAsync(string submCd)
        {
            throw new NotImplementedException();
        }
    }
}
