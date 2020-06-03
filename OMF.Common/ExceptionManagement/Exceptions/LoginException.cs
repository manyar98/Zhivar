using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class LoginException : OMFException
    {
        public LoginException()
        {
        }

        public LoginException(string message)
          : base(message)
        {
        }

        public LoginException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public LoginException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.Login;
            }
        }
    }
}
