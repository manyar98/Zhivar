using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class TranslatedException : OMFException
    {
        public TranslatedException()
        {
        }

        public TranslatedException(string message)
          : base(message)
        {
        }

        public TranslatedException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public TranslatedException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                if (this.InnerException is OMFException)
                    return (this.InnerException as OMFException).ExType;
                return base.ExType;
            }
        }
    }
}
