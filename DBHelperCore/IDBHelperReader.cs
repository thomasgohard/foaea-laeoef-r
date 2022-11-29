using System.Collections;

namespace DBHelper
{
    public interface IDBHelperReader : IEnumerable
    {
        object this[string fieldName] { get; }
        object this[int index] { get; }
        
        bool ColumnExists(string fieldName);
    }

}
