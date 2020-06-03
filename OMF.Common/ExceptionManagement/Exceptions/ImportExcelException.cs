using OMF.Common.ExceptionManagement.Exceptions;
using System;
using System.Runtime.Serialization;
using static OMF.Common.Enums;

namespace BPJ.Common.ExceptionManagement.Exceptions
{
    [Serializable]
    public class ImportExcelException : OMFException
    {
        public ImportExcelException()
        {
        }

        public ImportExcelException(string message)
          : base(message)
        {
        }

        public ImportExcelException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public ImportExcelException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public override ExceptionType ExType
        {
            get
            {
                return ExceptionType.ImportExcel;
            }
        }
    }
}
