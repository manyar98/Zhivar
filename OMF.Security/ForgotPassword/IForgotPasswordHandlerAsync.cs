using OMF.Security.Model;
using System.Threading.Tasks;

namespace OMF.Security.ForgotPassword
{
    public interface IForgotPasswordHandlerAsync : IForgotPasswordHandler
    {
        Task<string> CreateCodeAsync(string userName, string email, string mobile);

        Task<string> SendRequestAsync(ForgotPasswordEntity forgotPassEntity);
    }
}
