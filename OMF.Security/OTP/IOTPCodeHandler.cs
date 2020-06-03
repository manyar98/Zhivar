using OMF.Common.Security;

namespace OMF.Security.OTP
{
    public interface IOTPCodeHandler
    {
        bool NeedOTP(UserContext userContext);

        bool NeedOTP(string userName);

        string CreateCode(UserContext userContext);

        OTPSendResponse SendCode(OTPSendRequest request);

        bool VerifyCode(UserContext userContext, string code);
    }
}
