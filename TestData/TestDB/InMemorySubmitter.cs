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

        public async Task CreateSubmitterAsync(SubmitterData newSubmitter)
        {
            await Task.Run(() =>
            {
                newSubmitter.Subm_Create_Usr = "Test";
                newSubmitter.Subm_CourtUsr_Ind = true;
            });
        }

        public async Task CreateSubmitterMessageAsync(SubmitterMessageData submitterMessage)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<CommissionerData>> GetCommissionersAsync(string locationCode, string currentSubmitter)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<string> GetMaxSubmitterCodeAsync(string submCodePart)
        {
            await Task.Run(() => { });
            return "02";
        }

        public async Task<string> GetSignAuthorityForSubmitterAsync(string submCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<SubmitterData>> GetSubmitterAsync(string submCode = null, string submName = null,
                                                string enfOffCode = null, string enfServCode = null,
                                                string submFName = null, string submMName = null,
                                                string prov = null)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<SubmitterMessageData>> GetSubmitterMessageForSubmitterAsync(string submitterID, int languageCode)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task SubmitterMessageDeleteAsync(string submitterID)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateSubmitterAsync(SubmitterData newSubmitter)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
        public async Task<DateTime> UpdateSubmitterLastLoginAsync(string submCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
