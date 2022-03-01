using System;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ISubmitterRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<SubmitterData> GetSubmitter(string submCode = null, string submName = null,
                                         string enfOffCode = null, string enfServCode = null,
                                         string submFName = null, string submMName = null,
                                         string prov = null);

        string GetMaxSubmitterCode(string submCodePart);

        void CreateSubmitter(SubmitterData newSubmitter);

        void UpdateSubmitter(SubmitterData newSubmitter);
        DateTime UpdateSubmitterLastLogin(string submCd);

        string GetSignAuthorityForSubmitter(string submCd);

        List<CommissionerData> GetCommissioners(string locationCode, string currentSubmitter);

        #region SubmitterMessage

        void SubmitterMessageDelete(string submitterID);
        void CreateSubmitterMessage(SubmitterMessageData submitterMessage);
        List<SubmitterMessageData> GetSubmitterMessageForSubmitter(string submitterID, int languageCode);

        #endregion
    }

}
