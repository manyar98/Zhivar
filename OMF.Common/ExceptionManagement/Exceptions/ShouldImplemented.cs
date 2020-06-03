using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class ShouldImplemented : OMFException
    {
        public ShouldImplemented(string className, string methodName)
          : base(string.Format("The method '{0}()' in class '{1}' should be implemented!!!", (object)methodName, (object)className))
        {
        }

        public ShouldImplemented(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.ShouldImplemented;
            }
        }
    }
}
