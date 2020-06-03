using System;
using System.Runtime.Serialization;

namespace OMF.Common.Globalization
{
    [Serializable]
    public class InvalidPersianDateException : Exception
    {
        public InvalidPersianDateException(Exception innerException)
          : base("Invalid format.", innerException)
        {
        }

        public InvalidPersianDateException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public InvalidPersianDateException()
          : base("Invalid format.")
        {
        }
    }
}
