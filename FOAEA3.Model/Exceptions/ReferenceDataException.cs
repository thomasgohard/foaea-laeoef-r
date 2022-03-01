using System;
using System.Runtime.Serialization;

namespace FOAEA3.Model.Exceptions
{
    [Serializable]
    public class ReferenceDataException : Exception
    {
        public ReferenceDataException()
        {
        }

        public ReferenceDataException(string message) : base(message)
        {
        }

        public ReferenceDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReferenceDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
