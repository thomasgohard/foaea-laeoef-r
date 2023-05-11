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

        public Task<List<FileAuditData>> GetFileAuditDataForFileAsync(string fileName)
        {
            return Task.FromResult(FileAuditTable);
        }

        public Task InsertFileAuditData(FileAuditData data)
        {
            FileAuditTable.Add(data);

            return Task.CompletedTask;
        }

        public Task MarkFileAuditCompletedForFile(string fileName)
        {
            foreach (var auditItem in FileAuditTable)
                if (auditItem.InboundFilename == fileName)
                    auditItem.IsCompleted = true;

            return Task.CompletedTask;
        }

        public Task MarkFileAuditCompletedForItemAsync(FileAuditData data)
        {
            var item = FileAuditTable.Where(m => (m.InboundFilename == data.InboundFilename) &&
                                     (m.Appl_EnfSrv_Cd == data.Appl_EnfSrv_Cd) &&
                                     (m.Appl_CtrlCd == data.Appl_CtrlCd)).FirstOrDefault();
            if (item != null)
                item.IsCompleted = true;

            return Task.CompletedTask;
        }
    }
}
