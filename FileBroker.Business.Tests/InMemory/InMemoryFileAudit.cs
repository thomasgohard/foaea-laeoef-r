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

        public List<FileAuditData> GetFileAuditDataForFileAsync(string fileName)
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

        Task<List<FileAuditData>> IFileAuditRepository.GetFileAuditDataForFileAsync(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertFileAuditDataAsync(FileAuditData data)
        {
            throw new System.NotImplementedException();
        }

        public Task MarkFileAuditCompletedForFileAsync(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public Task MarkFileAuditCompletedForItemAsync(FileAuditData data)
        {
            throw new System.NotImplementedException();
        }
    }
}
