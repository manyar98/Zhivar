using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class DataAccessException : OMFException
    {
        public DataAccessException()
        {
        }

        public DataAccessException(string message)
          : base(message)
        {
        }

        public DataAccessException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public DataAccessException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.DataAccess;
            }
        }
    }
}
