using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBCraSinPending : DBbase, ICraSinPendingRepository
    {
        public DBCraSinPending(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task Insert(string oldSin, string newSin)
        {
            var parameters = new Dictionary<string, object> {
                    { "SIN_old", oldSin },
                    { "SIN_new", newSin }
                };

            await MainDB.ExecProcAsync("CRASINPending_Insert", parameters);
        }

        public async Task Delete(string newSin)
        {
            var parameters = new Dictionary<string, object> {
                    { "SIN", newSin }
                };

            await MainDB.ExecProcAsync("CRASINPending_Delete", parameters);
        }
    }
}
