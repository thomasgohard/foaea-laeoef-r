using FOAEA3.Resources.Helpers;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DBHelper
{
    public class DBTools : IDBTools
    {

        private readonly string _connectionString;

        public string LastError { get; set; }

        public int LastReturnValue { get; set; }

        public string UserId { get; set; }

        public string Submitter { get; set; }

        private Exception LastException
        {
            set
            {

                // try to log the error to the database
                if (value != null)
                    try
                    {
                        LastError = value.Message;

                        using var con = new SqlConnection(ConnectionString);
                        using var cmd = CreateCommand("Logs_Insert", con);
                        cmd.Parameters.AddWithValue("@Message", value.Message);
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
                        // ignore error -- can't log it to the database for some reason so nowhere to log
                    }
                else
                    LastError = string.Empty;
            }
        }

        public DBTools(string connectionString)
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

        public List<Tdata> GetDataFromStoredProc<Tdata>(string procName,
                                                        Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            return GetDataFromStoredProc<Tdata>(procName, null, fillDataFromReader);
        }

        public List<Tdata> GetDataFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters,
                                                        Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {

            ValidateConfiguration();

            var result = new List<Tdata>();

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
                    con.Open();

                    using SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {

                        var data = new Tdata();
                        //Tdata data = (Tdata)Activator.CreateInstance(typeof(Tdata));
                        var dataReader = new DBHelperReader(rdr);

                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    rdr.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                }
                catch (Exception e)
                {
                    LastException = e;
                }

            }

            return result;
        }

        public List<Tdata> GetRecordsFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters,
                                                           ActionOut<IDBHelperReader, Tdata> fillDataFromReader)
        {

            ValidateConfiguration();

            var result = new List<Tdata>();

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
                    con.Open();

                    using SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var dataReader = new DBHelperReader(rdr);
                        fillDataFromReader(dataReader, out var data);

                        result.Add(data);
                    }

                    rdr.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                }
                catch (Exception e)
                {
                    LastException = e;
                }

            }

            return result;
        }

        public Tdata GetDataFromStoredProc<Tdata>(string procName, Dictionary<string, object> parameters)
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
                    con.Open();

                    object data = cmd.ExecuteScalar();
                    if ((data != null) && (data != DBNull.Value))
                    {
                        result = (Tdata)data;
                    }

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }

            }

            return result;
        }

        public List<Tdata> GetDataFromStoredProc<Tdata, Tkey>(string procName, string paramName, Tkey paramVal,
                                                              Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            ValidateConfiguration();

            var result = new List<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {

                using SqlCommand cmd = CreateCommand(procName, con);
                cmd.Parameters.AddWithValue("@" + paramName, paramVal);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    con.Open();
                    using SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {

                        var data = new Tdata();

                        var dataReader = new DBHelperReader(rdr);

                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    rdr.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }

            return result;
        }

        public Tdata GetDataFromProcSingleValue<Tdata>(string procName, Dictionary<string, object> parameters)
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
                    con.Open();
                    var rdr = cmd.ExecuteReader();
                    rdr.Read();
                    result = (Tdata)rdr[0];

                    rdr.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                    con.Close();
                }
                catch (Exception e)
                {
                    LastException = e;
                }

            }

            return result;
        }

        public Tdata GetDataFromStoredProcViaReturnParameter<Tdata>(string procName, Dictionary<string, object> parameters, string returnParameter)
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
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }

                result = (Tdata)outParameter.Value;
            }

            return result;
        }

        public Dictionary<string, object> GetDataFromStoredProcViaReturnParameters(string procName, Dictionary<string, object> parameters, Dictionary<string, string> returnParameters)
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
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }

                foreach (var (fieldName, _) in returnParameters)
                {
                    result.Add(fieldName, cmd.Parameters["@" + fieldName].Value);
                }

            }

            return result;
        }

        public int ExecProc(string procName, Dictionary<string, object> parameters)
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
                    con.Open();

                    result = cmd.ExecuteNonQuery();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;

                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }

            return result;
        }

        public void ExecProc(string procName)
        {
            SqlTransaction tran = null;

            using var con = new SqlConnection(ConnectionString);
            using var cmd = CreateCommand(procName, con);

            var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
            retParameter.Direction = ParameterDirection.ReturnValue;

            LastException = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();

                cmd.ExecuteNonQuery();

                tran.Commit();

                if (retParameter.Value is not null)
                    LastReturnValue = (int)retParameter.Value;
            }
            catch (Exception e)
            {
                tran?.Rollback();
                LastException = e;
            }

        }


        public List<Tdata> GetAllData<Tdata>(string tableName,
                                             Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            ValidateConfiguration();

            var result = new List<Tdata>();

            using (var con = new SqlConnection(ConnectionString))
            {
                using var cmd = CreateCommand(tableName + "_Select", con);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    con.Open();
                    using SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {

                        var data = new Tdata();
                        //Tdata data = (Tdata)Activator.CreateInstance(typeof(Tdata));
                        var dataReader = new DBHelperReader(rdr);
                        fillDataFromReader(dataReader, data);

                        result.Add(data);

                    }

                    rdr.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }

            return result;
        }

        public Tdata GetDataForKey<Tdata, Tkey>(string tableName, Tkey key, string keyColumnName,
                                                Action<IDBHelperReader, Tdata> fillDataFromReader) where Tdata : class, new()
        {
            ValidateConfiguration();

            var data = new Tdata();
            //Tdata data = (Tdata)Activator.CreateInstance(typeof(Tdata));

            using (var con = new SqlConnection(ConnectionString))
            {
                using SqlCommand cmd = CreateCommand(tableName + "_Select", con);
                cmd.Parameters.AddWithValue("@" + keyColumnName, key);

                var retParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                retParameter.Direction = ParameterDirection.ReturnValue;

                LastException = null;
                try
                {
                    con.Open();
                    using SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var dataReader = new DBHelperReader(rdr);
                        fillDataFromReader(dataReader, data);
                    }

                    rdr.Close();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }

            return data;
        }

        public void UpdateData<Tdata, Tkey>(string tableName, Tdata data, string columnName, Tkey id,
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
                con.Open();
                cmd.ExecuteNonQuery();

                if (retParameter.Value is not null)
                    LastReturnValue = (int)retParameter.Value;
            }
            catch (Exception e)
            {
                LastException = e;
            }

        }

        public Tkey CreateData<Tdata, Tkey>(string tableName, Tdata data, string columnName,
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
                    con.Open();
                    cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@" + columnName].Value != DBNull.Value)
                    {
                        result = (Tkey)cmd.Parameters["@" + columnName].Value;
                    }

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }

            return result;
        }

        public int DeleteData<Tkey>(string tableName, string columnName, Tkey id)
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
                    con.Open();
                    cmd.ExecuteNonQuery();

                    if (retParameter.Value is not null)
                        LastReturnValue = (int)retParameter.Value;
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }
            catch
            {
                result = -1; // failure, ignore error
            }

            return result;
        }

        public int DeleteDataCheckReturn<Tkey>(string tableName, string columnName, Tkey id)
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
                    con.Open();
                    cmd.ExecuteNonQuery();

                    result = (int)retParameter.Value;

                    LastReturnValue = result;
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }
            catch
            {
                result = -1; // failure, ignore error
            }

            return result;
        }

        public void BulkUpdate<T>(List<T> data, string tableName)
        {
            ValidateConfiguration();

            using var con = new SqlConnection(ConnectionString);

            SqlTransaction tran = null;

            LastException = null;
            try
            {
                var dataTable = CreateAndFillDataTable<T>(data);

                con.Open();
                tran = con.BeginTransaction();

                var bulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.FireTriggers, tran)
                {
                    DestinationTableName = tableName
                };

                bulkCopy.WriteToServer(dataTable);

                tran.Commit();
            }
            catch (Exception e)
            {
                tran?.Rollback();
                LastException = e;
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
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = 240
            };
        }

    }
}
