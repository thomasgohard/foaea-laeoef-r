using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryFileAudit : IFileAuditRepository
    {
        public List<FileAuditData> FileAuditTable { get; }

        public InMemoryFileAudit()
        {
            FileAuditTable = new List<FileAuditData>();
        }

        public async Task<List<FileAuditData>> GetFileAuditDataForFileAsync(string fileName)
        {
            await Task.Run(() => { });
            return FileAuditTable;
        }

        public async Task InsertFileAuditDataAsync(FileAuditData data)
        {
            await Task.Run(() => { });
            FileAuditTable.Add(data);
        }

        public async Task MarkFileAuditCompletedForFileAsync(string fileName)
        {
            await Task.Run(() => { });
            foreach (var auditItem in FileAuditTable)
                if (auditItem.InboundFilename == fileName)
                    auditItem.IsCompleted = true;
        }

        public async Task MarkFileAuditCompletedForItemAsync(FileAuditData data)
        {
            await Task.Run(() => { });
            var item = FileAuditTable.Where(m => (m.InboundFilename == data.InboundFilename) &&
                                     (m.Appl_EnfSrv_Cd == data.Appl_EnfSrv_Cd) &&
                                     (m.Appl_CtrlCd == data.Appl_CtrlCd)).FirstOrDefault();
            if (item != null)
                item.IsCompleted = true;
        }
    }
}
