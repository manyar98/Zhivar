using OMF.Common.Security;
using System.Threading.Tasks;

namespace OMF.Security.OTP
{
    public class OTPCodeManager
    {
        private static IOTPCodeHandler otpHandler = (IOTPCodeHandler)new OTPCodeHandler();

        public static IOTPCodeHandler OTPHandler
        {
            get
            {
                return OTPCodeManager.otpHandler;
            }
        }

        public static void InitiateOTPCodeHandler(IOTPCodeHandler otpCodeHandler)
        {
            OTPCodeManager.otpHandler = otpCodeHandler;
        }

        public static bool NeedOTP(UserContext userContext)
        {
            return OTPCodeManager.otpHandler.NeedOTP(userContext);
        }

        public static bool NeedOTP(string userName)
        {
            return OTPCodeManager.otpHandler.NeedOTP(userName);
        }

        public static string CreateCode(UserContext userContext)
        {
            return OTPCodeManager.otpHandler.CreateCode(userContext);
        }

        public static OTPSendResponse SendCode(OTPSendRequest request)
        {
            return OTPCodeManager.otpHandler.SendCode(request);
        }

        public static bool VerifyCode(UserContext userContext, string code)
        {
            return OTPCodeManager.otpHandler.VerifyCode(userContext, code);
        }

        public static async Task<bool> NeedOTPAsync(UserContext userContext)
        {
            bool need = false;
            if (OTPCodeManager.otpHandler is IOTPCodeHandlerAsync)
                need = await ((IOTPCodeHandlerAsync)OTPCodeManager.otpHandler).NeedOTPAsync(userContext);
            else
                need = OTPCodeManager.otpHandler.NeedOTP(userContext);
            return need;
        }

        public static async Task<bool> NeedOTPAsync(string userName)
        {
            bool need = false;
            if (OTPCodeManager.otpHandler is IOTPCodeHandlerAsync)
                need = await ((IOTPCodeHandlerAsync)OTPCodeManager.otpHandler).NeedOTPAsync(userName);
            else
                need = OTPCodeManager.otpHandler.NeedOTP(userName);
            return need;
        }

        public static async Task<string> CreateCodeAsync(UserContext userContext)
        {
            string code = (string)null;
            if (OTPCodeManager.otpHandler is IOTPCodeHandlerAsync)
                code = await ((IOTPCodeHandlerAsync)OTPCodeManager.otpHandler).CreateCodeAsync(userContext);
            else
                code = OTPCodeManager.otpHandler.CreateCode(userContext);
            return code;
        }

        public static async Task<OTPSendResponse> SendCodeAsync(
          OTPSendRequest request)
        {
            if (!(OTPCodeManager.otpHandler is IOTPCodeHandlerAsync))
                return OTPCodeManager.otpHandler.SendCode(request);
            OTPSendResponse otpSendResponse = await ((IOTPCodeHandlerAsync)OTPCodeManager.otpHandler).SendCodeAsync(request);
            return otpSendResponse;
        }

        public static async Task<bool> VerifyCodeAsync(UserContext userContext, string code)
        {
            if (!(OTPCodeManager.otpHandler is IOTPCodeHandlerAsync))
                return OTPCodeManager.otpHandler.VerifyCode(userContext, code);
            bool flag = await ((IOTPCodeHandlerAsync)OTPCodeManager.otpHandler).VerifyCodeAsync(userContext, code);
            return flag;
        }
    }
}
