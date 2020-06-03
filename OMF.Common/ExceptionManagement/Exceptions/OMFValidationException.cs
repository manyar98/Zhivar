using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace OMF.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class OMFValidationException : OMFException
    {
        public List<string> Failures { get; set; }

        public OMFValidationException(List<string> failureList)
          : base(string.Join(Constants.NewLine, (IEnumerable<string>)failureList))
        {
            this.Failures = failureList;
        }

        public OMFValidationException(string failure)
          : base(failure)
        {
            this.Failures = new List<string>() { failure };
        }

        public OMFValidationException(string failure, int code)
          : base(string.Format("{0}, Code: {1}", (object)failure, (object)code))
        {
            this.Failures = new List<string>() { failure };
            this.Code = code;
        }

        public OMFValidationException(SerializationInfo info, StreamingContext context)
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

        public int Code { get; set; } = -1;

        public override string ToString()
        {
            if (this.Failures == null || this.Failures.Count == 0)
                return base.ToString();
            return string.Format("{0}{1}{2}", (object)string.Join(Constants.NewLine, (IEnumerable<string>)this.Failures), (object)Constants.NewLine, (object)base.ToString());
        }
    }
}
