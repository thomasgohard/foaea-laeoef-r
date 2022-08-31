using DBHelper;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryFileTable : IFileTableRepository
    {
        public bool FileLoading { get; set; }
        public int NextCycle { get; set; }

        public IDBTools MainDB => throw new System.NotImplementedException();

        IDBToolsAsync IFileTableRepository.MainDB => throw new System.NotImplementedException();

        public InMemoryFileTable()
        {
            FileLoading = false;
            NextCycle = 1;
        }

        public async Task<FileTableData> GetFileTableDataForFileNameAsync(string fileNameNoExt)
        {
            return await Task.Run(() =>
            {
                var result = new FileTableData();

                switch (fileNameNoExt)
                {
                    case "RC3STSIT": // NETP
                        result.PrcId = 2;
                        result.Cycle = 1;
                        result.Address = "dsarrazi@justice.gc.ca";
                        break;
                    case "HR3STSIT": // EI 
                        result.PrcId = 3;
                        result.Cycle = 1;
                        result.Address = "dsarrazi@justice.gc.ca";
                        break;
                    case "EI3STSIT": // CRA 
                        result.PrcId = 23;
                        result.Cycle = 1;
                        result.Address = "dsarrazi@justice.gc.ca";
                        break;
                }

                return result;
            });
        }

        public async Task<List<FileTableData>> GetFileTableDataForCategoryAsync(string category)
        {
            return await Task.Run(() =>
            {
                return new List<FileTableData>
                {
                    new FileTableData
                    {
                        Name = "RC3STSOT",
                        PrcId = 17,
                        Cycle = 1,
                        Path = "C:\\Work",
                        Address = "dsarrazi@justice.gc.ca"
                    },

                    new FileTableData
                    {
                        Name = "HR3STSIT",
                        PrcId = 18,
                        Cycle = 1,
                        Path = "C:\\Work",
                        Address = "dsarrazi@justice.gc.ca"
                    },

                    new FileTableData
                    {
                        Name = "EI3STSIT",
                        PrcId = 24,
                        Cycle = 1,
                        Path = "C:\\Work",
                        Address = "dsarrazi@justice.gc.ca"
                    }
                };
            });
        }

        public async Task<List<FileTableData>> GetAllActiveAsync()
        {
            await Task.Run(() => { });
            throw new System.NotImplementedException();
        }

        public async Task SetNextCycleForFileTypeAsync(FileTableData fileData, int length = 6)
        {
            await Task.Run(() =>
            {
                if (fileData.PrcId.In(2, 3, 23))
                {
                    NextCycle++;
                    if (NextCycle.ToString().Trim().Length > length)
                        NextCycle = 1;
                }
            });
        }

        public async Task<bool> IsFileLoadingAsync(int processId)
        {
            return await Task.Run(() =>
            {
                return FileLoading;
            });
        }

        public async Task SetIsFileLoadingValueAsync(int processId, bool newValue)
        {
            await Task.Run(() =>
            {
                if (processId.In(2, 3, 23))
                    FileLoading = newValue;
            });
        }
    }
}
