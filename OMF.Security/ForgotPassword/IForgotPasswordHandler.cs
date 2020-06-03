using OMF.Common;
using OMF.Security.Model;
using static OMF.Common.Enums;

namespace OMF.Security.ForgotPassword
{
    public interface IForgotPasswordHandler
    {
        ForgotPasswordMode Mode { get; }

        string CreateCode(string userName, string email, string mobile);

        string SendRequest(ForgotPasswordEntity forgotPassEntity);
    }
}
