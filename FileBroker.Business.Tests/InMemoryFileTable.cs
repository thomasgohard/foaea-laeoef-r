using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;

namespace FileBroker.Business.Tests
{
    public class InMemoryFileTable : IFileTableRepository
    {
        public bool FileLoading { get; set; }
        public int NextCycle { get; set; }

        public InMemoryFileTable()
        {
            FileLoading = false;
            NextCycle = 1;
        }

        public FileTableData GetFileTableDataForFileName(string fileNameNoExt)
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
        }

        public List<FileTableData> GetFileTableDataForCategory(string category)
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
        }

        public bool IsFileLoading(int processId)
        {
            return FileLoading;
        }

        public void SetIsFileLoadingValue(int processId, bool newValue)
        {
            if (processId.In(2, 3, 23))
                FileLoading = newValue;
        }

        public void SetNextCycleForFileType(FileTableData fileData, int length = 6)
        {
            if (fileData.PrcId.In(2, 3, 23))
            {
                NextCycle++;
                if (NextCycle.ToString().Trim().Length > length)
                    NextCycle = 1;
            }
        }

    }
}
