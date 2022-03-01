using System;
using System.Runtime.Serialization;

namespace FOAEA3.Model.Exceptions
{
    [Serializable]
    public class GenderException : Exception
    {
        public GenderException()
        {
        }

        public GenderException(string message) : base(message)
        {
        }

        public GenderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GenderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
