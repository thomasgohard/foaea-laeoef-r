using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class EnforcementOfficesAPI : BaseAPI
    {
        internal List<EnfOffData> GetEnfOff(string enfOffName = null, string enfOffCode = null, string province = null, string enfServCode = null)
        {
            return GetDataAsync<List<EnfOffData>>($"api/v1/enfOffices?enfOffName={enfOffName}&enfOffCode={enfOffCode}&province={province}&enfServCode={enfServCode}").Result;
        }
    }
}
