using System.Collections;

namespace DBHelper
{
    public interface IDBHelperReader : IEnumerable
    {
        object this[string fieldName] { get; }
        
        bool ColumnExists(string fieldName);
    }

}
