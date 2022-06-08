namespace CompareOldAndNewData.CommandLine
{
    internal struct DiffData
    {
        public string TableName { get; }
        public string Key { get; }
        public string ColName { get; }
        public object BadValue { get; }
        public object GoodValue { get; }

        public DiffData(string tableName, string key, string colName, object badValue, object goodValue)
        {
            TableName = tableName;
            Key = key;
            ColName = colName;
            BadValue = badValue;
            GoodValue = goodValue;
        }
    }
}
