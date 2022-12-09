using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DBHelper
{
    public class DBToolsAsync : IDBToolsAsync
    {
        private readonly string _connectionString;

        public string LastError { get; set; }

        public int LastReturnValue { get; set; }

        public string UserId { get; set; }

        public string Submitter { get; set; }
        
        public string UpdateSubmitter { get; set; }

        private Exception LastException
        {
            set
            {
                // try to log the error to the database
                if (value != null)
                    try
                    {
                        LastError = value.Message + ": " + value.InnerException?.Message;

                        using var con = new SqlConnection(ConnectionString);
                        using var cmd = CreateCommand("Logs_Insert", con);
                        cmd.Parameters.AddWithValue("@Message", LastError);
                        cmd.Parameters.AddWithValue("@Level", "");
                        cmd.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
                        if (!string.IsNullOrEmpty(value.StackTrace))
                            cmd.Parameters.AddWithValue("@Exception", value.StackTrace.Trim());
                        cmd.Parameters.AddWithValue("@SubmitterCode", "");
                        cmd.Parameters.AddWithValue("@ActionName", "");
                        cmd.Parameters.AddWithValue("@EnvironmentUserName", (!string.IsNullOrEmpty(UserId)) ? UserId : "System");
                        cmd.Parameters.AddWithValue("@MachineName", Environment.MachineName);

                        con.Open();

                        cmd.ExecuteNonQuery();

                    }
                    catch // (Exception e)
                    {
                        // string error = e.Message;
                        // ignore error -- can't log it to the database for some reason so nowhere to log?
                    }
                else
                    LastError = string.Empty;
            }
        }

        public DBToolsAsync(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get
            {
                string connectionString = _connectionString;

                if (!connectionString.EndsWith(";"))
                    connectionString += ";";
                connectionString += "Workstation ID=" + Environment.MachineName;

                if (!string.IsNullOrEmpty(UserId))
                    connectionString += "/" + UserId;
                else
                    connectionString += "/System";

                return connectionString;
            }
        }

        public async Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata>(string procName,
                                                        Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            return await GetDataFromStoredProcAsync<Tdata>(procName, null, fillDataFromReader);
        }

        public async Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters,
                                                        Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {

            ValidateConfiguration();

            var result = new ConcurrentBag<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();

                    using SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                    {

                        var data = new Tdata();
                        
                        var dataReader = new DBHelperReader(rdr);

                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    await rdr.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }

            }

            return result.ToList();
        }

        public async Task<List<Tdata>> GetDataFromSqlAsync<Tdata>(string sql, Dictionary<string, object> parameters,
                                                             Action<IDBHelperReader, Tdata> fillDataFromReader)
                                                            where Tdata : class, new()
        {

            ValidateConfiguration();

            var result = new ConcurrentBag<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommandFromSql(sql, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }

                LastException = null;
                try
                {
                    await con.OpenAsync();

                    using SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                    {

                        var data = new Tdata();
                        var dataReader = new DBHelperReader(rdr);

                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    await rdr.CloseAsync();

                }
                catch (Exception e)
                {
                    LastException = new Exception("sql", e);
                }

            }

            return result.ToList();
        }

        public async Task<List<Tdata>> GetRecordsFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters,
                                                           ActionOut<IDBHelperReader, Tdata> fillDataFromReader)
        {

            ValidateConfiguration();

            var result = new ConcurrentBag<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();

                    using SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                    {
                        var dataReader = new DBHelperReader(rdr);
                        fillDataFromReader(dataReader, out var data);

                        result.Add(data);
                    }

                    await rdr.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }

            }

            return result.ToList();
        }

        public async Task<Tdata> GetDataFromStoredProcAsync<Tdata>(string procName, Dictionary<string, object> parameters)
        {
            ValidateConfiguration();

            Tdata result = default;

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }


                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();

                    object data = await cmd.ExecuteScalarAsync();
                    if ((data != null) && (data != DBNull.Value))
                    {
                        result = (Tdata)data;
                    }

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }

            }

            return result;
        }

        public async Task<List<Tdata>> GetDataFromStoredProcAsync<Tdata, Tkey>(string procName, string paramName, Tkey paramVal,
                                                              Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            ValidateConfiguration();

            var result = new ConcurrentBag<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {

                using SqlCommand cmd = CreateCommand(procName, con);
                cmd.Parameters.AddWithValue("@" + paramName, paramVal);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    using SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                    {

                        var data = new Tdata();

                        var dataReader = new DBHelperReader(rdr);

                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    await rdr.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }
            }

            return result.ToList();
        }

        public async Task<Tdata> GetDataFromProcSingleValueAsync<Tdata>(string procName, Dictionary<string, object> parameters)
        {
            ValidateConfiguration();

            Tdata result = default;

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                    foreach (var item in parameters)
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    var rdr = await cmd.ExecuteReaderAsync();
                    if (await rdr.ReadAsync())
                        result = (Tdata)rdr[0];
                    else
                        result = default;

                    await rdr.CloseAsync();

                    if (retParameter?.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                    await con.CloseAsync();
                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }

            }

            return result;
        }

        public async Task<Tdata> GetDataFromSqlSingleValueAsync<Tdata>(string sql, Dictionary<string, object> parameters)
        {
            ValidateConfiguration();

            Tdata result = default;

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommandFromSql(sql, con);
                if ((parameters != null) && (parameters.Count > 0))
                    foreach (var item in parameters)
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    var rdr = await cmd.ExecuteReaderAsync();
                    if (await rdr.ReadAsync())
                        result = (Tdata)rdr[0];
                    else
                        result = default;

                    await rdr.CloseAsync();
                    await con.CloseAsync();
                }
                catch (Exception e)
                {
                    LastException = new Exception("error", e);
                }

            }

            return result;
        }

        public async Task<Tdata> GetDataFromStoredProcViaReturnParameterAsync<Tdata>(string procName, Dictionary<string, object> parameters, string returnParameter)
        {
            ValidateConfiguration();

            Tdata result = default;

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }

                var outParameter = cmd.Parameters.AddWithValue("@" + returnParameter, default(Tdata));
                outParameter.Direction = ParameterDirection.Output;

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }

                result = (Tdata)outParameter.Value;
            }

            return result;
        }

        public async Task<Dictionary<string, object>> GetDataFromStoredProcViaReturnParametersAsync(string procName, Dictionary<string, object> parameters, Dictionary<string, string> returnParameters)
        {
            ValidateConfiguration();

            var result = new Dictionary<string, object>();

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        if (item.Value == null)
                            cmd.Parameters.AddWithValue("@" + item.Key, DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }

                foreach (var (fieldName, returnedValue) in returnParameters)
                {
                    var returnType = returnedValue[0] switch
                    {
                        'S' => SqlDbType.VarChar,
                        'C' => SqlDbType.Char,
                        'D' => SqlDbType.DateTime,
                        'B' => SqlDbType.Bit,
                        'I' => SqlDbType.Int,
                        _ => throw new Exception($"Unsupported DB type! ({returnedValue})"),
                    };
                    var outParameter = cmd.Parameters.Add("@" + fieldName, returnType);
                    if (returnedValue[0].In('S', 'C'))
                    {
                        int length = int.Parse(returnedValue[1..]);
                        outParameter.Size = length;
                    }

                    outParameter.Direction = ParameterDirection.Output;
                }

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }

                foreach (var (fieldName, _) in returnParameters)
                {
                    result.Add(fieldName, cmd.Parameters["@" + fieldName].Value);
                }

            }

            return result;
        }

        public async Task<int> ExecProcAsync(string procName, Dictionary<string, object> parameters)
        {
            int result = default;

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(procName, con);
                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    }
                }

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();

                    result = await cmd.ExecuteNonQueryAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                }
                catch (Exception e)
                {
                    LastException = new Exception(procName, e);
                }
            }

            return result;
        }

        public async Task ExecProcAsync(string procName)
        {
            SqlTransaction tran = null;

            using var con = new SqlConnection(ConnectionString);
            using var cmd = CreateCommand(procName, con);

            var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
            retParameter.Direction = ParameterDirection.ReturnValue;

            LastException = null;
            try
            {
                await con.OpenAsync();
                tran = con.BeginTransaction();
                cmd.Transaction = tran;

                await cmd.ExecuteNonQueryAsync();

                await tran.CommitAsync();

                if (retParameter.Value is not null)
                    LastReturnValue = (int)retParameter.Value;
            }
            catch (Exception e)
            {
                tran?.Rollback();
                LastException = new Exception(procName, e);
            }

        }


        public async Task<List<Tdata>> GetAllDataAsync<Tdata>(string tableName,
                                             Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            ValidateConfiguration();

            var result = new ConcurrentBag<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {
                using var cmd = CreateCommand(tableName + "_Select", con);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    using SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                    {

                        var data = new Tdata();
                        
                        var dataReader = new DBHelperReader(rdr);
                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    await rdr.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception("GetAllData " + tableName, e);
                }
            }

            return result.ToList();
        }

        public async Task<Tdata> GetDataForKeyAsync<Tdata, Tkey>(string tableName, Tkey key, string keyColumnName,
                                                Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            ValidateConfiguration();

            var data = new Tdata();
            
            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(tableName + "_Select", con);
                cmd.Parameters.AddWithValue("@" + keyColumnName, key);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    using SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                    {
                        var dataReader = new DBHelperReader(rdr);
                        fillDataFromReader(dataReader, data);
                    }

                    await rdr.CloseAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception("GetDataForKey " + tableName, e);
                }
            }

            return data;
        }

        public async Task UpdateDataAsync<Tdata, Tkey>(string tableName, Tdata data, string columnName, Tkey id,
                                            Action<Tdata, Dictionary<string, object>> SetParametersForData)
        {
            ValidateConfiguration();

            using var con = new SqlConnection(ConnectionString);
            using var cmd = CreateCommand(tableName + "_Update", con);
            cmd.Parameters.AddWithValue("@" + columnName, id);

            var extraParameters = new Dictionary<string, object>();

            SetParametersForData(data, extraParameters);

            if (extraParameters.Count > 0)
                foreach (var extraParameter in extraParameters)
                    if (extraParameter.Value is null)
                        cmd.Parameters.AddWithValue("@" + extraParameter.Key, DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@" + extraParameter.Key, extraParameter.Value);

            var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
            retParameter.Direction = ParameterDirection.ReturnValue;

            LastException = null;
            try
            {
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                if (retParameter.Value is not null)
                    LastReturnValue = (int)retParameter.Value;
            }
            catch (Exception e)
            {
                LastException = new Exception("UpdateData " + tableName, e);
            }

        }

        public async Task<Tkey> CreateDataAsync<Tdata, Tkey>(string tableName, Tdata data, string columnName,
                                            Action<Tdata, Dictionary<string, object>> SetParametersForData, Tkey initialValueForId)
        {
            ValidateConfiguration();

            Tkey result = default;

            using (var con = new SqlConnection(ConnectionString))
            {

                using SqlCommand cmd = CreateCommand(tableName + "_Insert", con);
                var extraParameters = new Dictionary<string, object>();

                SetParametersForData(data, extraParameters);

                if (extraParameters.Count > 0)
                    foreach (var extraParameter in extraParameters)
                        cmd.Parameters.AddWithValue("@" + extraParameter.Key, extraParameter.Value);

                SqlParameter newID = cmd.Parameters.AddWithValue("@" + columnName, initialValueForId);
                if (!initialValueForId.Equals(default(Tkey)))
                {
                    newID.Direction = System.Data.ParameterDirection.InputOutput;
                    newID.Value = initialValueForId;
                }
                else
                    newID.Direction = System.Data.ParameterDirection.Output;

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    if (cmd.Parameters["@" + columnName].Value != DBNull.Value)
                    {
                        result = (Tkey)cmd.Parameters["@" + columnName].Value;
                    }

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception("CreateData " + tableName, e);
                }
            }

            return result;
        }

        public async Task<int> DeleteDataAsync<Tkey>(string tableName, string columnName, Tkey id)
        {
            ValidateConfiguration();

            int result = 1; // success

            try
            {
                using var con = new SqlConnection(ConnectionString);
                using var cmd = CreateCommand(tableName + "_Delete", con);
                cmd.Parameters.AddWithValue("@" + columnName, id);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = new Exception("DeleteData " + tableName, e);
                }
            }
            catch
            {
                result = -1; // failure, ignore error
            }

            return result;
        }

        public async Task<int> DeleteDataCheckReturnAsync<Tkey>(string tableName, string columnName, Tkey id)
        {
            ValidateConfiguration();

            int result = 1; // success

            try
            {
                using var con = new SqlConnection(ConnectionString);
                using var cmd = CreateCommand(tableName + "_Delete", con);
                cmd.Parameters.AddWithValue("@" + columnName, id);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    result = (int)retParameter.Value;

                    LastReturnValue = result;
                }
                catch (Exception e)
                {
                    LastException = new Exception("DeleteDataCheckReturn " + tableName, e);
                }
            }
            catch
            {
                result = -1; // failure, ignore error
            }

            return result;
        }

        public async Task BulkUpdateAsync<T>(List<T> data, string tableName)
        {
            ValidateConfiguration();

            using var con = new SqlConnection(ConnectionString);

            SqlTransaction tran = null;

            LastException = null;
            try
            {
                var dataTable = CreateAndFillDataTable<T>(data);

                await con.OpenAsync();
                tran = con.BeginTransaction();

                var bulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.FireTriggers, tran)
                {
                    DestinationTableName = tableName
                };

                await bulkCopy.WriteToServerAsync(dataTable);

                tran.Commit();
            }
            catch (Exception e)
            {
                tran?.Rollback();
                LastException = new Exception("BulkUpdate " + tableName, e);
            }
        }

        public DataTable CreateAndFillDataTable<T>(List<T> data)
        {
            var dataType = typeof(T);
            var newDataTable = new DataTable();

            foreach (var info in dataType.GetProperties())
            {
                if ((info.PropertyType is not null) && (info.PropertyType.FullName is not null) &&
                    (info.PropertyType.FullName.Contains("Nullable")))
                {
                    //     FullName: "System.Nullable`1[[System.DateTime, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]"

                    if (info.PropertyType.FullName.Contains("System.DateTime"))
                    {
                        var newColumn = new DataColumn(info.Name, typeof(DateTime))
                        {
                            AllowDBNull = true
                        };
                        newDataTable.Columns.Add(newColumn);
                    }
                    else if (info.PropertyType.FullName.Contains("System.Int32"))
                    {
                        var newColumn = new DataColumn(info.Name, typeof(Int32))
                        {
                            AllowDBNull = true
                        };
                        newDataTable.Columns.Add(newColumn);
                    }
                    else if (info.PropertyType.FullName.Contains("System.Int16"))
                    {
                        var newColumn = new DataColumn(info.Name, typeof(Int16))
                        {
                            AllowDBNull = true
                        };
                        newDataTable.Columns.Add(newColumn);
                    }
                }
                else if (info.PropertyType is not null)
                    newDataTable.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }

            foreach (var row in data)
            {
                var newRow = newDataTable.NewRow();

                foreach (var info in dataType.GetProperties())
                {
                    object value = info.GetValue(row);
                    if (value is null)
                        newRow[info.Name] = DBNull.Value;
                    else
                        newRow[info.Name] = info.GetValue(row);
                }

                newDataTable.Rows.Add(newRow);
            }

            return newDataTable;
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("Missing database connection string!!!");
        }

        private static SqlCommand CreateCommand(string procName, SqlConnection con)
        {
            return new SqlCommand(procName, con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 240
            };
        }

        private static SqlCommand CreateCommandFromSql(string sql, SqlConnection con)
        {
            return new SqlCommand(sql, con)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 240
            };
        }

    }
}
