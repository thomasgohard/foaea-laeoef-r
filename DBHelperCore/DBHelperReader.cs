using System.Collections;
using Microsoft.Data.SqlClient;

namespace DBHelper
{
    public class DBHelperReader : IDBHelperReader
    {
        private readonly SqlDataReader rdr;

        public DBHelperReader(SqlDataReader rdr)
        {
            this.rdr = rdr;
        }

        public object this[string fieldName]
        {
            get
            {
                if (rdr.IsDBNull(rdr.GetOrdinal(fieldName)))
                    return null;
                else
                    return rdr[fieldName];
            }
        }

        public object this[int index]
        {
            get
            {
                if (rdr.IsDBNull(index))
                    return null;
                else
                    return rdr[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return rdr.GetEnumerator();
        }

        public bool ColumnExists(string fieldName)
        {
            string fieldNameUpper = fieldName.ToUpper();

            bool result = false;
            for(int i = 0; i< rdr.FieldCount; i++)
            {
                if (rdr.GetName(i).ToUpper() == fieldNameUpper)
                {
                    result = true;
                    break;
                }
            }

            return result;

        }
    }
}
