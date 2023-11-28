using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISubmitterRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<List<SubmitterData>> GetSubmitter(string submCode = null, string submName = null,
                                               string enfOffCode = null, string enfServCode = null,
                                               string submFName = null, string submMName = null,
                                               string prov = null);
        Task<List<string>> GetSubmitterCodesForOffice(string service, string office);
        Task<string> GetFOAEAOfficersEmail();
        Task<string> GetMaxSubmitterCode(string submCodePart);

        Task CreateSubmitter(SubmitterData newSubmitter);

        Task UpdateSubmitter(SubmitterData newSubmitter);
        Task<DateTime> UpdateSubmitterLastLogin(string submCd);

        Task<string> GetSignAuthorityForSubmitter(string submCd);

        Task<List<CommissionerData>> GetCommissioners(string locationCode, string currentSubmitter);

        #region SubmitterMessage

        Task SubmitterMessageDelete(string submitterID);
        Task CreateSubmitterMessage(SubmitterMessageData submitterMessage);
        Task<List<SubmitterMessageData>> GetSubmitterMessageForSubmitter(string submitterID, int languageCode);

        #endregion
    }

}
