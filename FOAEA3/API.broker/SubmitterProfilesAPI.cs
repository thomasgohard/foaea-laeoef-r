using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class SubmitterProfilesAPI : BaseAPI
    {
        internal SubmitterProfileData GetSubmitterProfile(string submCd)
        {
            return GetDataAsync<SubmitterProfileData>($"api/v1/submitterProfiles/{submCd}").Result;
        }
    }
}
