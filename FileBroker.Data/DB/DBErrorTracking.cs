using DBHelper;
using FileBroker.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FileBroker.Data.DB
{

    public class DBErrorTracking : IErrorTrackingRepository
    {
        private IDBTools MainDB { get; }

        public DBErrorTracking(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public void MessageBrokerError(string errorType, string errorSubject, Exception e, bool displayExceptionError,
                                       DataRow row = null)
        {
            var rowValues = new StringBuilder();
            if (row != null)
            {
                foreach(DataColumn column in row.Table.Columns)
                {
                    if (column.ColumnName.ToLower() == "sin")
                        rowValues.AppendLine($"{column.ColumnName}: (SIN not displayed)");
                    else
                        rowValues.AppendLine($"{column.ColumnName}: {row[column]}");
                }
            }

            if (string.IsNullOrEmpty(errorSubject))
                errorSubject = "Processing File Error";

            var parameters = new Dictionary<string, object>
            {
                {"errorMessage", displayExceptionError ? e.Message : string.Empty },
                {"errorStack", displayExceptionError ? e.StackTrace : string.Empty },
                {"errorRow", rowValues.ToString() },
                {"errorType", errorType },
                {"errorSubject", errorSubject }
            };

            MainDB.ExecProc("MessageBrokerError", parameters);
        }

    }
}
