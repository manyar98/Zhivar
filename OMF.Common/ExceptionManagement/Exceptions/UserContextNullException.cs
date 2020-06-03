using System;
using System.Runtime.Serialization;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class UserContextNullException : OMFException
    {
        public UserContextNullException()
        {
        }

        public UserContextNullException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
