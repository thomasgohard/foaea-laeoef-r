using DBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace TestData.TestDataBase
{
    public class InMemory_MainDB : IDBTools
    {
        public string ConnectionString => throw new NotImplementedException();

        public string LastError { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Submitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int LastReturnValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void BulkUpdate<T>(List<T> data, string tableName)
        {
            throw new NotImplementedException();
        }

        public DataTable CreateAndFillDataTable<T>(List<T> data)
        {
            throw new NotImplementedException();
        }

        public Tkey CreateData<Tdata, Tkey>(string tableName, Tdata data, string columnName, Action<Tdata, Dictionary<string, object>> SetParametersForData, Tkey initialValueForId)
        {
            throw new NotImplementedException();
        }

        public int DeleteData<Tkey>(string tableName, string columnName, Tkey id)
        {
            throw new NotImplementedException();
        }

        public int DeleteDataCheckReturn<Tkey>(string tableName, string columnName, Tkey id)
        {
            throw new NotImplementedException();
        }

        public int ExecProc(string procName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public void ExecProc(string procName)
        {
            throw new NotImplementedException();
        }

        public List<Tdata> GetAllData<Tdata>(string tableName, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Tdata GetDataForKey<Tdata, Tkey>(string tableName, Tkey key, string keyColumnName, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Tdata GetDataFromProcSingleValue<Tdata>(string procName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public Tdata GetDataFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public List<Tdata> GetDataFromStoredProc<Tdata>(string procName, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public List<Tdata> GetDataFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public List<Tdata> GetDataFromStoredProc<Tdata, Tkey>(string procName, string paramName, Tkey paramVal, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Tdata GetDataFromStoredProcViaReturnParameter<Tdata>(string procName, Dictionary<string, object> parameters, string returnParameter)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> GetDataFromStoredProcViaReturnParameters(string procName, Dictionary<string, object> parameters, Dictionary<string, string> returnParameters)
        {
            throw new NotImplementedException();
        }

        public void UpdateData<Tdata, Tkey>(string tableName, Tdata data, string columnName, Tkey id, Action<Tdata, Dictionary<string, object>> SetParametersForData)
        {
            throw new NotImplementedException();
        }
    }
}
