using System;
using System.Collections.Generic;
using System.Data;

namespace DBHelper
{
    public interface IDBTools
    {
        // ============================================================================================================
        // properties
        // ------------------------------------------------------------------------------------------------------------

        string ConnectionString { get; }

        string UserId { get; set; }

        string Submitter { get; set; }

        string LastError { get; set; }

        int LastReturnValue { get; set; }

        // ============================================================================================================
        // standard CRUD -- associated proc needs to follow convention for the given table name:
        //      <tablename>_Create
        //      <tablename>_Select
        //      <tablename>_Update
        //      <tablename>_Delete
        // ------------------------------------------------------------------------------------------------------------

        Tkey CreateData<Tdata, Tkey>(string tableName, Tdata data, string columnName,
                                     Action<Tdata, Dictionary<string, object>> SetParametersForData, Tkey initialValueForId);

        Tdata GetDataForKey<Tdata, Tkey>(string tableName, Tkey key, string keyColumnName,
                                         Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        List<Tdata> GetAllData<Tdata>(string tableName,
                                      Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        void UpdateData<Tdata, Tkey>(string tableName, Tdata data, string columnName, Tkey id,
                                     Action<Tdata, Dictionary<string, object>> SetParametersForData);

        int DeleteData<Tkey>(string tableName, string columnName, Tkey id);

        int DeleteDataCheckReturn<Tkey>(string tableName, string columnName, Tkey id);

        // ============================================================================================================
        // custom calls to procs (only call when the above CRUD procs cannot be used)
        // ------------------------------------------------------------------------------------------------------------

        Tdata GetDataFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters);

        List<Tdata> GetDataFromStoredProc<Tdata>(string procName,
                                                 Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        List<Tdata> GetDataFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters,
                                                 Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        List<Tdata> GetDataFromStoredProc<Tdata, Tkey>(string procName, string paramName, Tkey paramVal,
                                                       Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        List<Tdata> GetDataFromSql<Tdata>(string procName, Dictionary<string, object> parameters,
                                                       Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new();

        List<Tdata> GetRecordsFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters,
                                                    ActionOut<IDBHelperReader, Tdata> fillDataFromReader);

        Tdata GetDataFromProcSingleValue<Tdata>(string procName, Dictionary<string, object> parameters);
        
        Tdata GetDataFromSqlSingleValue<Tdata>(string sql, Dictionary<string, object> parameters);

        Tdata GetDataFromStoredProcViaReturnParameter<Tdata>(string procName, Dictionary<string, object> parameters, string returnParameter);

        Dictionary<string, object> GetDataFromStoredProcViaReturnParameters(string procName, Dictionary<string, object> parameters, Dictionary<string, string> returnParameters);

        int ExecProc(string procName, Dictionary<string, object> parameters);

        void ExecProc(string procName);

        void BulkUpdate<T>(List<T> data, string tableName);

        DataTable CreateAndFillDataTable<T>(List<T> data);
    }
}
