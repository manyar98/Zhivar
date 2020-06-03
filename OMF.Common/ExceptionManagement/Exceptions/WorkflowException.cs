using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class WorkflowException : OMFException
    {
        public WorkflowException(string message)
          : base(message)
        {
        }

        public WorkflowException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public WorkflowException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.Workflow;
            }
        }
    }
}
