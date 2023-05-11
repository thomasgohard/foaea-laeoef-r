namespace FileBroker.Model
{
    public class MessageData<T>
    {
        public const string ACTION_ADD = "A";
        public const string ACTION_CHANGE = "C";

        public T Application { get; set; }
        public string NewRecipientSubmitter { get; set; }
        public string NewIssuingSubmitter { get; set; }
        public string NewUpdateSubmitter { get; set; }
        public string MaintenanceAction { get; set; }
        public string MaintenanceLifeState { get; set; }
    }
}
