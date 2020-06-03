using OMF.Common.Security;
using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class OperationAccessException : TranslatedException
    {
        public static string GetMessage(string securityKey)
        {
            return string.Format(OperationAccess.MessagePattern, (object)securityKey);
        }

        public OperationAccessException(string securityKey)
          : base(OperationAccessException.GetMessage(securityKey))
        {
        }

        public OperationAccessException(OperationAccessType opAccessType)
          : this(opAccessType.ToString())
        {
        }

        public OperationAccessException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.PermissionException;
            }
        }
    }
}
