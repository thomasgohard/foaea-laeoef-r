using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryAffidavit : IAffidavitRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task CloseAffidavitData(AffidavitData data)
        {
            throw new NotImplementedException();
        }

        public Task<AffidavitData> GetAffidavitData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task InsertAffidavitData(AffidavitData data)
        {
            throw new NotImplementedException();
        }
    }
}
