using System;
using System.Collections.Generic;
using System.Text;

namespace FileBroker.Model.Interfaces
{
    public interface IFileTableRepository
    {
        FileTableData GetFileTableDataForFileName(string fileNameNoExt);
        List<FileTableData> GetFileTableDataForCategory(string category);

        void SetNextCycleForFileType(FileTableData fileData, int length = 6);
        bool IsFileLoading(int processId);
        void SetIsFileLoadingValue(int processId, bool newValue);
    }
}
