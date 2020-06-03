using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class LogManagementException : OMFException
    {
        public LogManagementException(Exception ex)
          : base("Log Management Failed", ex)
        {
        }

        public LogManagementException(string message, Exception ex)
          : base(message, ex)
        {
        }

        public LogManagementException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.LogManagement;
            }
        }
    }
}
