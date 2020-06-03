using OMF.Common.Security;
using System.Threading.Tasks;

namespace OMF.Security.OTP
{
    public interface IOTPCodeHandlerAsync : IOTPCodeHandler
    {
        Task<bool> NeedOTPAsync(UserContext userContext);

        Task<bool> NeedOTPAsync(string userName);

        Task<string> CreateCodeAsync(UserContext userContext);

        Task<OTPSendResponse> SendCodeAsync(OTPSendRequest request);

        Task<bool> VerifyCodeAsync(UserContext userContext, string code);
    }
}
