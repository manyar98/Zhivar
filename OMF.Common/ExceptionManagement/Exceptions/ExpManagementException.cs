using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class ExpManagementException : OMFException
    {
        public ExpManagementException(Exception ex)
          : base("Log Management Failed", ex)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.ExceptionManagement;
            }
        }

        public ExpManagementException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
