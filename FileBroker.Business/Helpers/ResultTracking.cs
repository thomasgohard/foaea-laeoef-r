namespace FileBroker.Business.Helpers
{
    internal class ResultTracking
    {
        public int ErrorCount { get; set; } = 0;
        public int WarningCount { get; set; } = 0;
        public int SuccessCount { get; set; } = 0;
    }
}
