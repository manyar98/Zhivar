
using static OMF.Common.Enums;

namespace OMF.Common.Sms
{
    public class SmsResponse
    {
        public SmsResponseStatus Status { get; set; } = SmsResponseStatus.Succeed;

        public string Code { get; set; }

        public string Message { get; set; }

        public string ReferenceID { get; set; }
    }
}
