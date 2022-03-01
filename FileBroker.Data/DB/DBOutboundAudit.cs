using DBHelper;
using FileBroker.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.Data.DB
{
    public class DBOutboundAudit : IOutboundAuditRepository
    {
        private IDBTools MainDB { get; }

        public DBOutboundAudit(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public void InsertIntoOutboundAudit(string fileName, DateTime fileDate, bool fileCreated, string message)
        {
            var parameters = new Dictionary<string, object>
            {
                {"fileName", fileName },
                {"fileDate", fileDate },
                {"fileCreated", fileCreated },
                {"message", message }
            };

            MainDB.ExecProc("MessageBrokerInsertOutboundAudit", parameters);
        }

    }
}
