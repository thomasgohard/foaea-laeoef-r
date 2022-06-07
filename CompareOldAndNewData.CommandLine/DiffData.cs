namespace CompareOldAndNewData.CommandLine
{
    internal struct DiffData
    {
        public string Key { get; }
        public string ColName { get; }
        public object BadValue { get; }
        public object GoodValue { get; }

        public DiffData(string key, string colName, object badValue, object goodValue)
        {
            Key = key;
            ColName = colName;
            BadValue = badValue;
            GoodValue = goodValue;
        }
    }
}
