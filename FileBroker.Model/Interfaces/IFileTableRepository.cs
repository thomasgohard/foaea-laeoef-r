using DBHelper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IFileTableRepository
    {
        IDBToolsAsync MainDB { get; }

        Task<FileTableData> GetFileTableDataForFileNameAsync(string fileNameNoExt);
        Task<List<FileTableData>> GetFileTableDataForCategoryAsync(string category);
        Task<List<FileTableData>> GetAllActiveAsync();

        Task SetNextCycleForFileTypeAsync(FileTableData fileData, int length = 6);
        Task<bool> IsFileLoadingAsync(int processId);
        Task SetIsFileLoadingValueAsync(int processId, bool newValue);
    }
}
