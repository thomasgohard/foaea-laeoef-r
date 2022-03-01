using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DBHelper
{
    public class DebugLogger
    {
        private const int LOCK_TIMEOUT = 60000; // 1 minutes = 60,000 milliseconds

        private string m_logFileName;
        private const string m_BusinessLogFile = @"C:\FOALogging\FOALogging.txt";

        public bool IncludeTimeStamp { get; set; }

        public DebugLogger() : this(m_BusinessLogFile)
        {
        }

        public DebugLogger(string filename)
        {
            IncludeTimeStamp = true;

            try
            {
                m_logFileName = filename;

                object thisLock = new object();
                bool isOk = Monitor.TryEnter(thisLock, LOCK_TIMEOUT);
                if (isOk)
                {
                    string thisPath = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(thisPath))
                        Directory.CreateDirectory(thisPath);
                    Monitor.Exit(thisLock);
                }
                else
                    throw new Exception("Could not lock to create log directory");
            }
            catch (Exception ex)
            {
                // Do nothing to avoid issue when releasing in production in DEBUG mode by mistake
                throw new Exception(ex.Message);
            }
        }

        public void ClearLog()
        {
            ClearLog(m_BusinessLogFile);
        }
        public void ClearLog(string filename)
        {
            try
            {
                m_logFileName = filename;

                object thisLock = new object();
                bool isOk = Monitor.TryEnter(thisLock, LOCK_TIMEOUT);
                if (isOk)
                {
                    string thisPath = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(thisPath))
                        Directory.CreateDirectory(thisPath);
                    if (File.Exists(filename))
                        File.Delete(filename);
                    Monitor.Exit(thisLock);
                }
                else
                    throw new Exception("Could not lock to create log directory");
            }
            catch (Exception ex)
            {
                // Do nothing to avoid issue when releasing in production in DEBUG mode by mistake
                throw new Exception(ex.Message);
            }
        }

        public void AppendLine(string info)
        {
            try
            {

                object thisLock = new object();
                bool isOk = Monitor.TryEnter(thisLock, LOCK_TIMEOUT);
                if (isOk)
                {
                    using (var outfile = new StreamWriter(m_logFileName, true))
                    {
                        outfile.WriteLine(GetTimestamp() + info);
                        outfile.Flush();
                        outfile.Close();
                    }
                    Monitor.Exit(thisLock);
                }
                else
                    throw new Exception("Could not write to log file (AppendLine)");
            }
            catch (Exception ex)
            {
                // Do nothing to avoid issue when releasing in production in DEBUG mode by mistake
                throw new Exception(ex.Message);
            }
        }

        public void AppendFormat(string format, params object[] args)
        {
            try
            {

                object thisLock = new object();
                bool isOk = Monitor.TryEnter(thisLock, LOCK_TIMEOUT);
                if (isOk)
                {
                    using (var outfile = new StreamWriter(m_logFileName, true))
                    {
                        outfile.WriteLine(GetTimestamp() + string.Format(format, args));
                        outfile.Flush();
                        outfile.Close();
                    }
                    Monitor.Exit(thisLock);
                }
                else
                    throw new Exception("Could not write to log file (AppendFormat)");
            }
            catch (Exception ex)
            {
                // Do nothing to avoid issue when releasing in production in DEBUG mode by mistake
                throw new Exception(ex.Message);
            }
        }

        public void AppendDataTable(DataTable data, string sep = "\t")
        {

            AppendFormat("Data.Rows.Count = {0}", data.Rows.Count);

            string titleRow = string.Empty;
            foreach (DataColumn col in data.Columns)
                titleRow += col.ColumnName + sep;

            AppendLine(titleRow.Trim());

            foreach (DataRow row in data.Rows)
            {
                string infoRow = string.Empty;
                foreach (DataColumn col in data.Columns)
                {
                    if (row[col] != DBNull.Value)
                        infoRow += string.Format("{0}", row[col]) + sep;
                    else
                        infoRow += "NULL" + sep;
                }
                AppendLine(infoRow.Trim());
            }

        }

        private string GetTimestamp()
        {
            if (IncludeTimeStamp)
                return DateTime.Now.ToString("[yyyy.MM.dd hh:mm:ss] ");
            else
                return string.Empty;
        }

        public static string GetInfo([CallerFilePath] string file = null,
                                     [CallerLineNumber] int line = 0,
                                     [CallerMemberName] string member = null)
        {
            return $"Path: {file} / Line: {line} / Member: {member}";
        }

    }

}
