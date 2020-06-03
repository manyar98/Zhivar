using System;

namespace OMF.Security.OTP
{
    public class OTPSendRequest
    {
        public string Code { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserMobile { get; set; }

        public DateTime SendDate { get; set; }
    }
}
