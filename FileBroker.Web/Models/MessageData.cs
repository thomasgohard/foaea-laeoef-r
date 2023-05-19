using FOAEA3.Model.Enums;

namespace FileBroker.Web.Models
{
    public class MessageData
    {
        public string Message { get; set; } 
        public MessageType MessageType { get; set; }

        public MessageData()
        {
            Message = string.Empty;
            MessageType = MessageType.Information;
        }
    }
}
