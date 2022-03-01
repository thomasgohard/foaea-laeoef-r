using DBHelper;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Model.Base
{
    public class DataList<T> : IMessageList where T : class, new()
    {
        public MessageDataList Messages { get; set; }

        public List<T> Items { get; set; }

        public DataList()
        {
            Messages = new MessageDataList();

            Items = new List<T>();
        }

        public DataList(List<T> data, string lastError) : this()
        {
            Items.AddRange(data);

            if (!string.IsNullOrEmpty(lastError))
                Messages.AddSystemError(lastError);

        }
    }
}
