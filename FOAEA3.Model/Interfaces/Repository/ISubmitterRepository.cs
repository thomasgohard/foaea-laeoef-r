using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISubmitterRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<List<SubmitterData>> GetSubmitterAsync(string submCode = null, string submName = null,
                                         string enfOffCode = null, string enfServCode = null,
                                         string submFName = null, string submMName = null,
                                         string prov = null);
        Task<List<string>> GetSubmitterCodesForOffice(string service, string office);
        Task<string> GetMaxSubmitterCodeAsync(string submCodePart);

        Task CreateSubmitterAsync(SubmitterData newSubmitter);

        Task UpdateSubmitterAsync(SubmitterData newSubmitter);
        Task<DateTime> UpdateSubmitterLastLoginAsync(string submCd);

        Task<string> GetSignAuthorityForSubmitterAsync(string submCd);

        Task<List<CommissionerData>> GetCommissionersAsync(string locationCode, string currentSubmitter);

        #region SubmitterMessage

        Task SubmitterMessageDeleteAsync(string submitterID);
        Task CreateSubmitterMessageAsync(SubmitterMessageData submitterMessage);
        Task<List<SubmitterMessageData>> GetSubmitterMessageForSubmitterAsync(string submitterID, int languageCode);

        #endregion
    }

}
