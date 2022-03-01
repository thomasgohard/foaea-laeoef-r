using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace FileBroker.Business.Tests
{
    public class InMemoryFileAudit : IFileAuditRepository
    {
        public List<FileAuditData> FileAuditTable { get; }

        public InMemoryFileAudit()
        {
            FileAuditTable = new List<FileAuditData>();
        }

        public List<FileAuditData> GetFileAuditDataForFile(string fileName)
        {
            return FileAuditTable;
        }

        public void InsertFileAuditData(FileAuditData data)
        {
            FileAuditTable.Add(data);
        }

        public void MarkFileAuditCompletedForFile(string fileName)
        {
            foreach (var auditItem in FileAuditTable)
                if (auditItem.InboundFilename == fileName)
                    auditItem.IsCompleted = true;
        }

        public void MarkFileAuditCompletedForItem(FileAuditData data)
        {
            var item = FileAuditTable.Where(m => (m.InboundFilename == data.InboundFilename) && 
                                                 (m.Appl_EnfSrv_Cd == data.Appl_EnfSrv_Cd) &&
                                                 (m.Appl_CtrlCd == data.Appl_CtrlCd)).FirstOrDefault();
            if (item != null)
                item.IsCompleted = true;
        }
    }
}
