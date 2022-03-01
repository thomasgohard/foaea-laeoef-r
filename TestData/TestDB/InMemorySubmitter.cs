using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;

namespace TestData.TestDB
{
    class InMemorySubmitter : ISubmitterRepository
    {

        public InMemorySubmitter()
        {

        }

        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void CreateSubmitter(SubmitterData newSubmitter)
        {
            newSubmitter.Subm_Create_Usr = "Test";
            newSubmitter.Subm_CourtUsr_Ind = true;
        }

        public void CreateSubmitterMessage(SubmitterMessageData submitterMessage)
        {
            throw new NotImplementedException();
        }

        public List<CommissionerData> GetCommissioners(string locationCode, string currentSubmitter)
        {
            throw new NotImplementedException();
        }

        public string GetMaxSubmitterCode(string submCodePart)
        {
            return "02";
        }

        public string GetSignAuthorityForSubmitter(string submCd)
        {
            throw new NotImplementedException();
        }

        public List<SubmitterData> GetSubmitter(string submCode = null, string submName = null,
                                                string enfOffCode = null, string enfServCode = null,
                                                string submFName = null, string submMName = null,
                                                string prov = null)
        {
            throw new NotImplementedException();
        }

        public List<SubmitterMessageData> GetSubmitterMessageForSubmitter(string submitterID, int languageCode)
        {
            throw new NotImplementedException();
        }

        public void SubmitterMessageDelete(string submitterID)
        {
            throw new NotImplementedException();
        }

        public void UpdateSubmitter(SubmitterData newSubmitter)
        {
            throw new NotImplementedException();
        }
        public DateTime UpdateSubmitterLastLogin(string submCd)
        {
            throw new NotImplementedException();
        }
    }
}
