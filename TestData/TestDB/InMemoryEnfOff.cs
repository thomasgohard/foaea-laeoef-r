using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    class InMemoryEnfOff : IEnfOffRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public async Task<List<EnfOffData>> GetEnfOffAsync(string enfOffName = null, string enfOffCode = null, string province = null, string enfServCode = null)
        {
            return await Task.Run(() =>
            {
                var data = new List<EnfOffData>
                {
                    new EnfOffData { EnfSrv_Cd = "ON01", EnfOff_City_LocCd = "DOW1", EnfOff_AbbrCd = "D", EnfOff_Nme = "Family Responsibility Off", ActvSt_Cd = "A" },
                    new EnfOffData { EnfSrv_Cd = "ON02", EnfOff_City_LocCd = "ON15", EnfOff_AbbrCd = "P", EnfOff_Nme = "Whitby District Court", ActvSt_Cd = "A" },
                    new EnfOffData { EnfSrv_Cd = "FO01", EnfOff_City_LocCd = "OTT1", EnfOff_AbbrCd = "X", EnfOff_Nme = "Application Entry Service", ActvSt_Cd = "A" }
                };

                var result = data.FindAll(m => m.EnfSrv_Cd == enfServCode && m.EnfOff_City_LocCd == enfOffCode);

                return result;
            });
        }
    }
}
