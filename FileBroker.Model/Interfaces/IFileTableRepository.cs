using System.Collections.Generic;

namespace FileBroker.Model.Interfaces
{
    public interface IFileTableRepository
    {
        FileTableData GetFileTableDataForFileName(string fileNameNoExt);
        List<FileTableData> GetFileTableDataForCategory(string category);
        List<FileTableData> GetAllActive();

        void SetNextCycleForFileType(FileTableData fileData, int length = 6);
        bool IsFileLoading(int processId);
        void SetIsFileLoadingValue(int processId, bool newValue);
    }
}
