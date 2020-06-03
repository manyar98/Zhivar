using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class BusinessException : OMFException
    {
        public BusinessException()
        {
        }

        public BusinessException(string message)
          : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public BusinessException(string message, Exception innerException, object entityInfo)
          : base(message, innerException)
        {
            this.EntityInfo = entityInfo;
        }

        public BusinessException(string message, Exception innerException, int busiCode)
          : base(message, innerException)
        {
            this.BusinessCode = busiCode;
        }

        public BusinessException(
          string message,
          Exception innerException,
          int busiCode,
          object entityInfo)
          : base(message, innerException)
        {
            this.BusinessCode = busiCode;
            this.EntityInfo = entityInfo;
        }

        public BusinessException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.Business;
            }
        }

        public int BusinessCode { get; set; } = -1;

        public object EntityInfo { get; }
    }
}
