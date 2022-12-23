using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBEnfSrc : DBbase, IEnfSrcRepository
    {
        public DBEnfSrc(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<int> GetEnfSrcCount(string enfSrv_Src_Cd, string enfSrv_Loc_Cd, string enfSrv_SubLoc_Cd)
        {
            var parameters = new Dictionary<string, object> {
                { "chrEnfSrv_Src_Cd",  enfSrv_Src_Cd},
                { "chrEnfSrv_Loc_Cd",  enfSrv_Loc_Cd}                
            };

            if (!string.IsNullOrEmpty(enfSrv_SubLoc_Cd))
                parameters.Add("chrEnfSrv_SubLoc_Cd", enfSrv_SubLoc_Cd);
            else
                parameters.Add("chrEnfSrv_SubLoc_Cd", null);

            return await MainDB.GetDataFromProcSingleValueAsync<int>("GetEnfSrcCount", parameters);
        }
    }
}
