using FOAEA3.Model.Enums;

namespace FOAEA3.Model
{
    public struct MessageData
    {
        public EventCode Code { get; }
        public string Field { get; }
        public string Description { get; }
        public string URL { get; }
        public MessageType Severity { get; }
        public bool IsSystemMessage { get; }
        public bool IsUrl => !string.IsNullOrEmpty(URL); 

        public MessageData(EventCode code, string field, string description, MessageType severity, bool isSystemMessage = false, string url = null)
        {
            Code = code;
            Field = field;
            Description = description;
            Severity = severity;
            IsSystemMessage = isSystemMessage;
            URL = url;
        }

    }

}
