
using static OMF.Common.Enums;

namespace OMF.Common.Sms
{
    public class SmsData
    {
        public string MessageBody { get; set; }

        public string MobileNo { get; set; }

        public int? RecieverID { get; set; }

        public SmsReceiverType SmsReceiverType { get; set; }

        public string EntityID { get; set; }

        public string EntityName { get; set; }
    }
}
