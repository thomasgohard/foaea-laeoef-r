using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;

namespace TestData.TestDB
{
    public class InMemoryAffidavit : IAffidavitRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public AffidavitData GetAffidavitData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }
    }
}
