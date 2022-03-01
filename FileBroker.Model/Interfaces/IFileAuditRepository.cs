using System;
using System.Collections.Generic;
using System.Text;

namespace FileBroker.Model.Interfaces
{
    public interface IFileAuditRepository
    {
        List<FileAuditData> GetFileAuditDataForFile(string fileName);
        void InsertFileAuditData(FileAuditData data);
        void MarkFileAuditCompletedForFile(string fileName);
        void MarkFileAuditCompletedForItem(FileAuditData data);
    }
}
