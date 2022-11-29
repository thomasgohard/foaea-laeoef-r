using DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TestData.TestDataBase
{
    public class InMemory_MainDB : IDBToolsAsync
    {
        public string ConnectionString => "";

        public string LastError { get; set; }
        public string Submitter { get; set; }
        public string UserId { get; set; }
        public int LastReturnValue { get; set; }
        public string UpdateSubmitter { get; set; }

        public Task BulkUpdateAsync<T>(List<T> data, string tableName)
        {
            throw new NotImplementedException();
        }

        public DataTable CreateAndFillDataTable<T>(List<T> data)
        {
            throw new NotImplementedException();
        }

        public Task<Tkey> CreateDataAsync<Tdata, Tkey>(string tableName, Tdata data, string columnName, Action<Tdata, Dictionary<string, object>> SetParametersForData, Tkey initialValueForId)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteDataAsync<Tkey>(string tableName, string columnName, Tkey id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteDataCheckReturnAsync<Tkey>(string tableName, string columnName, Tkey id)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecProcAsync(string procName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public Task ExecProcAsync(string procName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tdata>> GetAllDataAsync<Tdata>(string tableName, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<Tdata> GetDataForKeyAsync<Tdata, Tkey>(string tableName, Tkey key, string keyColumnName, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<Tdata> GetDataFromProcSingleValueAsync<Tdata>(string procName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tdata>> GetDataFromSqlAsync<Tdata>(string procName, Dictionary<string, object> parameters, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<Tdata> GetDataFromSqlSingleValueAsync<Tdata>(string sql, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<Tdata> GetDataFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata>(string procName, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata, Tkey>(string procName, string paramName, Tkey paramVal, Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<Tdata> GetDataFromStoredProcViaReturnParameterAsync<Tdata>(string procName, Dictionary<string, object> parameters, string returnParameter)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, object>> GetDataFromStoredProcViaReturnParametersAsync(string procName, Dictionary<string, object> parameters, Dictionary<string, string> returnParameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tdata>> GetRecordsFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters, ActionOut<IDBHelperReader, Tdata> fillDataFromReader)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDataAsync<Tdata, Tkey>(string tableName, Tdata data, string columnName, Tkey id, Action<Tdata, Dictionary<string, object>> SetParametersForData)
        {
            throw new NotImplementedException();
        }
    }
}
