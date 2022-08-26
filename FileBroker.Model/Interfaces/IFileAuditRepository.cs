using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IFileAuditRepository
    {
        Task<List<FileAuditData>> GetFileAuditDataForFileAsync(string fileName);
        Task InsertFileAuditDataAsync(FileAuditData data);
        Task MarkFileAuditCompletedForFileAsync(string fileName);
        Task MarkFileAuditCompletedForItemAsync(FileAuditData data);
    }
}
