using System;
using System.Runtime.Serialization;

namespace FOAEA3.Model.Exceptions
{
    [Serializable]
    public class SubmitterException : Exception
    {
        public SubmitterException()
        {
        }

        public SubmitterException(string message) : base(message)
        {
        }

        public SubmitterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SubmitterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
