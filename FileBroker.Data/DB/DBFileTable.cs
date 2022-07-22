using DBHelper;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileBroker.Data.DB
{
    public class DBFileTable : IFileTableRepository
    {
        private IDBTools MainDB { get; }

        public DBFileTable(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public FileTableData GetFileTableDataForFileName(string fileNameNoExt)
        {
            var fileTableData = MainDB.GetAllData<FileTableData>("FileTable", FillFileTableDataFromReader);

            return fileTableData.Where(f => f.Name.ToUpper() == fileNameNoExt.ToUpper()).FirstOrDefault();
        }

        public List<FileTableData> GetFileTableDataForCategory(string category)
        {
            var fileTableData = MainDB.GetAllData<FileTableData>("FileTable", FillFileTableDataFromReader);

            return fileTableData.Where(f => f.Category == category).ToList();
        }

        public List<FileTableData> GetAllActive()
        {
            var fileTableData = MainDB.GetAllData<FileTableData>("FileTable", FillFileTableDataFromReader);

            return fileTableData.Where(f => f.Active is true).ToList();
        }

        public void SetNextCycleForFileType(FileTableData fileData, int length = 6)
        {

            int newCycle = fileData.Cycle + 1;
            string newCycleStr = newCycle.ToString();
            if (newCycleStr.Length > length)
                newCycle = 1;

            var parameters = new Dictionary<string, object>
            {
                {"nProcessID", fileData.PrcId},
                {"nCycle", newCycle}
            };

            _ = MainDB.ExecProc("MessageBrokerConfigSetCycle", parameters);

        }

        public bool IsFileLoading(int processId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"nProcessID", processId}
            };

            return MainDB.GetDataFromProcSingleValue<bool>("MessageBrokerConfigIsFileLoading", parameters);
        }

        public void SetIsFileLoadingValue(int processId, bool newValue)
        {
            var parameters = new Dictionary<string, object>
            {
                {"nProcessID", processId},
                {"isLoading", newValue}
            };

            MainDB.ExecProc("MessageBrokerConfigSetFileLoadingValue", parameters);
        }

        public static void FillFileTableDataFromReader(IDBHelperReader rdr, FileTableData data)
        {
            data.PrcId = (int)rdr["PrcId"];
            data.Type = rdr["type"] as string; // can be null 
            data.Name = rdr["name"] as string; // can be null 
            data.Cycle = (int)rdr["cycle"];
            data.Transform = (bool)rdr["Transform"];
            data.Meduim = rdr["meduim"] as string; // can be null 
            data.Address = rdr["address"] as string; // can be null 
            data.Path = rdr["path"] as string; // can be null 
            data.Frequency = rdr["frequency"] as int?; // can be null 
            data.Nextrun = rdr["nextrun"] as DateTime?; // can be null 
            data.Category = rdr["Category"] as string; // can be null 
            data.Active = rdr["active"] as bool?; // can be null 
            data.IsXML = (bool)rdr["IsXML"];
            data.IsReg = (bool)rdr["IsReg"];
            data.UsePADRSource = (bool)rdr["UsePADRSource"];
            data.StartDate = rdr["StartDate"] as DateTime?; // can be null 
            data.UseFixedTag = (short)rdr["UseFixedTag"];
            data.IsLoading = (bool)rdr["IsLoading"];
        }
    }
}
