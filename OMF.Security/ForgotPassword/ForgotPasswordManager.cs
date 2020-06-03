using OMF.Common.ExceptionManagement;
using OMF.Security.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OMF.Security.ForgotPassword
{
    public class ForgotPasswordManager
    {
        private static IForgotPasswordHandler forgotPassHandler;

        public static IForgotPasswordHandler ForgotPasswordHandler
        {
            get
            {
                return ForgotPasswordManager.forgotPassHandler;
            }
        }

        public static void InitiateForgotPasswordHandler(IForgotPasswordHandler forgotPasswordHandler)
        {
            ForgotPasswordManager.forgotPassHandler = forgotPasswordHandler;
        }

        public static string CreateCode(string userName, string email, string mobile)
        {
            return ForgotPasswordManager.forgotPassHandler.CreateCode(userName, email, mobile);
        }

        public static string SendRequest(ForgotPasswordEntity forgotPassEntity)
        {
            return ForgotPasswordManager.forgotPassHandler.SendRequest(forgotPassEntity);
        }

        public static string GetLastForgotPassCode(string userName)
        {
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
                return securityDbContext.ForgotPasswordEntities.Where<ForgotPasswordEntity>((Expression<Func<ForgotPasswordEntity, bool>>)(fpe => fpe.UserName == userName && fpe.IsActive)).OrderByDescending<ForgotPasswordEntity, int>((Expression<Func<ForgotPasswordEntity, int>>)(fpe => fpe.ID)).Select<ForgotPasswordEntity, string>((Expression<Func<ForgotPasswordEntity, string>>)(fpe => fpe.Code)).FirstOrDefault<string>();
        }

        public static async Task<string> CreateCodeAsync(
          string userName,
          string email,
          string mobile)
        {
            try
            {
                string code = (string)null;
                if (ForgotPasswordManager.forgotPassHandler is IForgotPasswordHandlerAsync)
                    code = await ((IForgotPasswordHandlerAsync)ForgotPasswordManager.forgotPassHandler).CreateCodeAsync(userName, email, mobile);
                else
                    code = ForgotPasswordManager.forgotPassHandler.CreateCode(userName, email, mobile);
                return code;
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return (string)null;
            }
            string str;
            return str;
        }

        public static async Task<string> SendRequestAsync(ForgotPasswordEntity forgotPassEntity)
        {
            if (!(ForgotPasswordManager.forgotPassHandler is IForgotPasswordHandlerAsync))
                return ForgotPasswordManager.forgotPassHandler.SendRequest(forgotPassEntity);
            string str = await ((IForgotPasswordHandlerAsync)ForgotPasswordManager.forgotPassHandler).SendRequestAsync(forgotPassEntity);
            return str;
        }

        public static async Task<string> GetLastForgotPassCodeAsync(string userName)
        {
            string str;
            using (SecurityDbContext db = new SecurityDbContext())
            {
                string code = await db.ForgotPasswordEntities.Where<ForgotPasswordEntity>((Expression<Func<ForgotPasswordEntity, bool>>)(fpe => fpe.UserName == userName && fpe.IsActive)).OrderByDescending<ForgotPasswordEntity, int>((Expression<Func<ForgotPasswordEntity, int>>)(fpe => fpe.ID)).Select<ForgotPasswordEntity, string>((Expression<Func<ForgotPasswordEntity, string>>)(fpe => fpe.Code)).FirstOrDefaultAsync<string>();
                str = code;
            }
            return str;
        }
    }
}
