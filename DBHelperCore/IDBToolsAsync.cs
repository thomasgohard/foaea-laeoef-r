using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DBHelper
{
    public interface IDBToolsAsync
    {
        // ============================================================================================================
        // properties
        // ------------------------------------------------------------------------------------------------------------

        string ConnectionString { get; }

        string UserId { get; set; }

        string Submitter { get; set; }
        string UpdateSubmitter { get; set; }

        string LastError { get; set; }

        int LastReturnValue { get; set; }

        // ============================================================================================================
        // standard CRUD -- associated proc needs to follow convention for the given table name:
        //      <tablename>_Create
        //      <tablename>_Select
        //      <tablename>_Update
        //      <tablename>_Delete
        // ------------------------------------------------------------------------------------------------------------

        Task<Tkey> CreateDataAsync<Tdata, Tkey>(string tableName, Tdata data, string columnName,
                                     Action<Tdata, Dictionary<string, object>> SetParametersForData, Tkey initialValueForId);

        Task<Tdata> GetDataForKeyAsync<Tdata, Tkey>(string tableName, Tkey key, string keyColumnName,
                                         Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        Task<List<Tdata>> GetAllDataAsync<Tdata>(string tableName,
                                      Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        Task UpdateDataAsync<Tdata, Tkey>(string tableName, Tdata data, string columnName, Tkey id,
                                     Action<Tdata, Dictionary<string, object>> SetParametersForData);

        Task<int> DeleteDataAsync<Tkey>(string tableName, string columnName, Tkey id);

        Task<int> DeleteDataCheckReturnAsync<Tkey>(string tableName, string columnName, Tkey id);

        // ============================================================================================================
        // custom calls to procs (only call when the above CRUD procs cannot be used)
        // ------------------------------------------------------------------------------------------------------------

        Task<Tdata> GetDataFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters);

        Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata>(string procName,
                                                 Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters,
                                                 Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata, Tkey>(string procName, string paramName, Tkey paramVal,
                                                       Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        Task<List<Tdata>> GetDataFromSqlAsync<Tdata>(string procName, Dictionary<string, object> parameters,
                                                       Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        Task<List<Tdata>> GetRecordsFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters,
                                                    ActionOut<IDBHelperReader, Tdata> fillDataFromReader);

        Task<Tdata> GetDataFromProcSingleValueAsync<Tdata>(string procName, Dictionary<string, object> parameters);

        Task<Tdata> GetDataFromSqlSingleValueAsync<Tdata>(string sql, Dictionary<string, object> parameters);

        Task<Tdata> GetDataFromStoredProcViaReturnParameterAsync<Tdata>(string procName, Dictionary<string, object> parameters, string returnParameter);

        Task<Dictionary<string, object>> GetDataFromStoredProcViaReturnParametersAsync(string procName, Dictionary<string, object> parameters, Dictionary<string, string> returnParameters);

        Task<int> ExecProcAsync(string procName, Dictionary<string, object> parameters);

        Task ExecProcAsync(string procName);

        Task BulkUpdateAsync<T>(List<T> data, string tableName);

        DataTable CreateAndFillDataTable<T>(List<T> data);
    }
}
