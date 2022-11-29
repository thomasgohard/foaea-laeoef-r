using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Model
{
    public class MessageDataList : List<MessageData>
    {
        public bool ContainsField(string field)
        {

            MessageData result = this.FirstOrDefault(s => s.Field.Equals(field, StringComparison.CurrentCultureIgnoreCase));

            bool isEmpty = (result.Code == EventCode.UNDEFINED && string.IsNullOrEmpty(result.Field) && string.IsNullOrEmpty(result.Description));

            return (!isEmpty);

        }

        public bool ContainsEventCode(EventCode eventCode)
        {

            MessageData result = this.FirstOrDefault(s => s.Code == eventCode);

            bool isEmpty = (result.Code == EventCode.UNDEFINED && string.IsNullOrEmpty(result.Field) && string.IsNullOrEmpty(result.Description));

            return (!isEmpty);

        }

        public MessageData GetMessageForField(string field)
        {
            return this.FirstOrDefault(s => s.Field.Equals(field, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<MessageData> GetMessagesForType(MessageType severity)
        {
            var result = from error in this
                         where error.Severity == severity
                         select error;

            return result.ToList();
        }

        public List<MessageData> GetSystemMessagesForType(MessageType severity)
        {
            var result = from error in this
                         where error.Severity == severity && error.IsSystemMessage == true
                         select error;

            return result.ToList();
        }

        public bool ContainsMessagesOfType(MessageType severity)
        {
            var result = from error in this
                         where error.Severity == severity
                         select error;

            return result.Any();
        }

        public bool ContainsNonSystemMessagesOfType(MessageType severity)
        {
            var result = from error in this
                         where error.Severity == severity && error.IsSystemMessage == false
                         select error;

            return result.Any();
        }

        public bool ContainsSystemMessagesOfType(MessageType severity)
        {
            var result = from error in this
                         where error.Severity == severity && error.IsSystemMessage == true
                         select error;

            return result.Any();
        }


        public void AddError(EventCode code, string field = "")
        {
            AddMessage(code, field, MessageType.Error);
        }

        public void AddError(string description, string field = "")
        {
            AddMessage(description, field, MessageType.Error);
        }

        public void AddSystemError(string description, string field = "")
        {
            AddMessage(description, field, MessageType.Error, isSystemMessage: true);
        }

        public void AddWarning(EventCode code, string field = "")
        {
            AddMessage(code, field, MessageType.Warning);
        }
        public void AddWarning(string description, string field = "")
        {
            AddMessage(description, field, MessageType.Warning);
        }

        public void AddSystemWarning(string description, string field = "")
        {
            AddMessage(description, field, MessageType.Warning, isSystemMessage: true);
        }

        public void AddInformation(EventCode code, string field = "")
        {
            AddMessage(code, field, MessageType.Information);

        }
        public void AddInformation(string description, string field = "")
        {
            AddMessage(description, field, MessageType.Information);
        }

        public void AddSystemInformation(string description, string field = "")
        {
            AddMessage(description, field, MessageType.Information, isSystemMessage: true);
        }

        public void AddMessage(EventCode code, string field, MessageType severity)
        {
            this.Add(new MessageData(code, field, string.Empty, severity));
        }

        public void AddURL(string description, string url, MessageType severity = MessageType.Information)
        {
            this.Add(new MessageData(EventCode.UNDEFINED, null, description, severity, url: url));
        }

        public void AddMessage(string description, string field, MessageType severity, bool isSystemMessage = false)
        {
            this.Add(new MessageData(EventCode.UNDEFINED, field, description, severity, isSystemMessage));
        }

        public void Merge(MessageDataList newList)
        {
            foreach (var error in newList)
            {
                this.Add(new MessageData(error.Code, error.Field, error.Description, error.Severity, error.IsSystemMessage, error.URL));
            }
        }

    }
}
