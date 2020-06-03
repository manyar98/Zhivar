using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class ConvertTypeException : OMFException
    {
        public ConvertTypeException()
        {
        }

        public ConvertTypeException(string message)
          : base(message)
        {
        }

        public ConvertTypeException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public ConvertTypeException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.General;
            }
        }
    }
}
