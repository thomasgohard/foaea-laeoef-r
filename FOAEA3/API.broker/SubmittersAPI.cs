using FOAEA3.Data.Base;
using FOAEA3.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FOAEA3.API.broker
{
    public class SubmittersAPI : BaseAPI
    {
        internal SubmitterData GetSubmitter(string submCd)
        {
            SubmitterData data = GetDataAsync<SubmitterData>($"api/v1/submitters/{submCd}").Result;

            if (data.Messages is null)
                data.Messages = new MessageDataList();

            return data;
        }

        internal List<SubmitterData> GetSubmittersForProvince(string provCd, bool onlyActive)
        {
            return GetDataAsync<List<SubmitterData>>($"api/v1/submitters?provCd={provCd}&onlyActive={onlyActive}").Result;
        }

        internal List<SubmitterData> GetSubmittersForProvinceAndOffice(string provCd, string enfOffCode, string enfSrvCode, bool onlyActive)
        {
            return GetDataAsync<List<SubmitterData>>($"api/v1/submitters?provCd={provCd}&enfOffCode={enfOffCode}&enfSrvCode={enfSrvCode}&onlyActive={onlyActive}").Result;
        }
        
        internal string GetDeclarant(string submCd)
        {
            return GetStringAsync($"api/v1/submitters/{submCd}/Declarant").Result;
        }

        internal SubmitterData CreateSubmitter(SubmitterData submitterData, string suffixCode, bool readOnlyAccess)
        {
            return PostDataAsync<SubmitterData, SubmitterData>($"api/v1/submitters?suffixCode={suffixCode}&readOnlyAccess={readOnlyAccess}", submitterData).Result;
        }
        
        internal SubmitterData UpdateSubmitter(SubmitterData submitterData)
        {
            var data = PutDataAsync<SubmitterData, SubmitterData>($"api/v1/submitters", submitterData).Result;

            if (data.Messages is null)
                data.Messages = new MessageDataList();

            return data;
        }

        internal IEnumerable GetCommissionersForLocation(string enfOffLocCode)
        {
            return GetDataAsync<List<CommissionerData>>($"api/v1/submitters/commissioners/{enfOffLocCode}").Result;
        }
    }
}
