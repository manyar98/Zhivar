using OMF.Business;
using OMF.Common;
using OMF.Common.ActivityLog;
using OMF.Common.Cache;
using OMF.Common.Configuration;
using OMF.Common.Cryptography;
using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Repositories;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security;
using OMF.Security.Captcha;
//using OMF.Security.ForgotPassword;
using OMF.Security.Model;
//using OMF.Security.OTP;
//using OMF.Security.OTP;
using OMF.Security.TokenManagement;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using static OMF.Common.Enums;

namespace OMF.Enterprise.MVC.Security
{
    public class SecurityController : ApiController
    {
        public virtual HttpResponseMessage GetAuthenticatedUser()
        {
            try
            {
                UserContext authenticatedUser = this.DoGetAuthenticatedUser();
                if (authenticatedUser == null)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 3
                    });
                return this.CreateSuccessLoginResponse(authenticatedUser);
            }
            catch (Exception ex)
            {
                ExceptionManager.SaveException(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
        }

        protected virtual UserContext DoGetAuthenticatedUser()
        {
            if (SecurityManager.CurrentUserContext == null || TokenManager.GetUser(SecurityManager.CurrentUserContext.Token) == null)
                return (UserContext)null;
            return SecurityManager.CurrentUserContext;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Login(
          [FromBody] SecurityController.LoginRequest request)
        {
            try
            {
    
                if (request == null)
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest);
                string message = this.Validate(request);
                if (!string.IsNullOrWhiteSpace(message))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>() { message }
                    });
                message = this.BeforeLogin(request.UserName);
                if (!string.IsNullOrWhiteSpace(message))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>() { message }
                    });
                UserContext userContext = await this.DoLoginAsync(request.UserName, request.Password, false);
                if (userContext == null)
                    message = this.OnUserContextIsNull(request.UserName, request.Password, out userContext);
                if (!string.IsNullOrWhiteSpace(message))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>() { message }
                    });
                bool needOTP = false;// await OTPCodeManager.NeedOTPAsync(userContext);
                if (needOTP)
                {
                    HttpResponseMessage response = await this.CreateOTPAsync(userContext);
                    SecurityManager.CurrentUserContext = (UserContext)null;
                    SessionManager.Add("__CurrentOTPUserContext__", (object)userContext);
                    return response;
                }
                await SecurityManager.SaveActivityLogForLoginActionAsync(userContext.UserName);
                message = await this.AfterLoginAsync(userContext);
                if (string.IsNullOrWhiteSpace(message))
                    return this.CreateSuccessLoginResponse(userContext);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = new List<string>() { message }
                });
            }
            catch (LoginException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = new List<string>() { ex.Message }
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        protected virtual async Task<HttpResponseMessage> CreateOTPAsync(
          UserContext userContext)
        {
            DateTime? nullable1 = userContext.LastOTPDate;
            int? nullable2;
            if (nullable1.HasValue)
            {
                bool lastOTPDateExpired = false;
                TimeSpan? otpCodeExpireTime = ConfigurationController.OTPCodeExpireTime;
                if (otpCodeExpireTime.HasValue)
                {
                    DateTime? nullable3 = userContext.LastOTPDate;
                    DateTime dateTime = nullable3.Value;
                    otpCodeExpireTime = ConfigurationController.OTPCodeExpireTime;
                    DateTime? nullable4;
                    if (!otpCodeExpireTime.HasValue)
                    {
                        nullable3 = new DateTime?();
                        nullable4 = nullable3;
                    }
                    else
                        nullable4 = new DateTime?(dateTime + otpCodeExpireTime.GetValueOrDefault());
                    nullable1 = nullable4;
                    DateTime now = DateTime.Now;
                    lastOTPDateExpired = nullable1.HasValue && nullable1.GetValueOrDefault() > now;
                }
                else
                {
                    nullable1 = userContext.LastOTPDate;
                    DateTime now = nullable1.Value;
                    DateTime date1 = now.Date;
                    now = DateTime.Now;
                    DateTime date2 = now.Date;
                    lastOTPDateExpired = date1 != date2;
                }
                nullable2 = userContext.OTPTryNo;
                int num1;
                if (nullable2.HasValue)
                {
                    nullable2 = userContext.OTPTryNo;
                    num1 = nullable2.Value >= (int)ConfigurationController.OTPTryNo ? 1 : 0;
                }
                else
                    num1 = 0;
                if (num1 != 0)
                    lastOTPDateExpired = true;
                if (lastOTPDateExpired)
                {
                    OMFSecurityProvider instance = OMFSecurityProvider.Instance;
                    string userName = userContext.UserName;
                    nullable2 = new int?();
                    int? applicationId = nullable2;
                    UserInfo userInfo = await instance.GetUserAsync(userName, applicationId);
                    //using (IBusinessRuleBaseAsync<UserInfo> userBusiness = BusinessContext.GetBusinessRule<UserInfo>((IUnitOfWorkAsync)new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext())))
                    //{
                    //    UserContext userContext1 = userContext;
                    //    UserInfo userInfo1 = userInfo;
                    //    nullable1 = new DateTime?();
                    //    DateTime? nullable3 = nullable1;
                    //    userInfo1.LastOTPDate = nullable3;
                    //    DateTime? nullable4 = nullable1;
                    //    userContext1.LastOTPDate = nullable4;
                    //    UserContext userContext2 = userContext;
                    //    userInfo.OTPTryNo = nullable2 = new int?(0);
                    //    int? nullable5 = nullable2;
                    //    userContext2.OTPTryNo = nullable5;
                    //    userContext.OTPCode = userInfo.OTPCode = (string)null;
                    //    userContext.NeedOTP = userInfo.NeedOTP = new bool?(true);
                    //    userBusiness.UseForAnonymousUser = true;
                    //    userBusiness.Update(userInfo);
                    //    int num2 = await userBusiness.SaveChangesAsync();
                    //}
                    userInfo = (UserInfo)null;
                }
            }
            //if (string.IsNullOrWhiteSpace(userContext.OTPCode))
            //{
            //    string otpCode = string.Empty;// await OTPCodeManager.CreateCodeAsync(userContext);
            //    if (string.IsNullOrWhiteSpace(otpCode))
            //        return this.Request.CreateResponse(HttpStatusCode.OK, new
            //        {
            //            resultCode = 1,
            //            failures = new List<string>()
            //{
            //  "بروز خطای پیکربندی در ارسال کد امنیتی با پشتیبانی تماس بگیرید."
            //}
            //        });
            ////    OTPSendRequest otpRequest = new OTPSendRequest();
            ////    otpRequest.Code = otpCode;
            ////    otpRequest.UserId = userContext.UserId;
            ////    otpRequest.UserName = userContext.UserName;
            ////    otpRequest.UserEmail = userContext.Email;
            ////    otpRequest.UserMobile = userContext.MobileNo;
            ////    OTPSendResponse response = await OTPCodeManager.SendCodeAsync(otpRequest);
            ////    if (response.Status == OTPSendStatus.Failed)
            ////        return this.Request.CreateResponse(HttpStatusCode.OK, new
            ////        {
            ////            resultCode = 1,
            ////            failures = new List<string>()
            ////{
            ////  "ارسال کد امنیتی با خطا مواجه شد، لطفا با پشتیبانی تماس بگیرید"
            ////}
            ////        });
            //    otpRequest.SendDate = DateTime.Now;
            //    OMFSecurityProvider instance = OMFSecurityProvider.Instance;
            //    string userName = userContext.UserName;
            //    nullable2 = new int?();
            //    int? applicationId = nullable2;
            //    UserInfo userInfo = await instance.GetUserAsync(userName, applicationId);
            //    using (IBusinessRuleBaseAsync<UserInfo> userBusiness = BusinessContext.GetBusinessRule<UserInfo>((IUnitOfWorkAsync)new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext())))
            //    {
            //        userContext.OTPCode = userInfo.OTPCode = otpRequest.Code;
            //        UserContext userContext1 = userContext;
            //        userInfo.LastOTPDate = nullable1 = new DateTime?(otpRequest.SendDate);
            //        DateTime? nullable3 = nullable1;
            //        userContext1.LastOTPDate = nullable3;
            //        UserContext userContext2 = userContext;
            //        userInfo.OTPTryNo = nullable2 = new int?(0);
            //        int? nullable4 = nullable2;
            //        userContext2.OTPTryNo = nullable4;
            //        userBusiness.UseForAnonymousUser = true;
            //        userBusiness.Update(userInfo);
            //        int num = await userBusiness.SaveChangesAsync();
            //    }
            //    userInfo = (UserInfo)null;
            //    otpCode = (string)null;
            //    otpRequest = (OTPSendRequest)null;
            //    response = (OTPSendResponse)null;
            //}
            SecurityManager.CurrentUserContext = (UserContext)null;
            SessionManager.Add("__CurrentOTPUserContext__", (object)userContext);
            return this.Request.CreateResponse(HttpStatusCode.OK, new
            {
                resultCode = 0,
                data = new
                {
                    OTPCodeEnabled = true,
                    FullName = userContext.FullName,
                    UserName = userContext.UserName,
                    MobileNo = userContext.MobileNo,
                    Email = userContext.Email
                }
            });
        }

        //[HttpPost]
        //public virtual async Task<HttpResponseMessage> LoginWithOTPCode(
        //  [FromBody] SecurityController.LoginWithOTPCodeRequest request)
        //{
        //    try
        //    {
        //        UserContext userContext = (UserContext)null;
        //        string message = string.Empty;
        //        if (SessionManager.Contains("__CurrentOTPUserContext__"))
        //        {
        //            userContext = SessionManager.GetData("__CurrentOTPUserContext__") as UserContext;
        //            if (userContext == null)
        //                message = this.OnUserContextIsNull(string.Empty, string.Empty, out userContext);
        //            if (!string.IsNullOrWhiteSpace(message))
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>() { message }
        //                });
        //            bool flag1 = await OTPCodeManager.NeedOTPAsync(userContext);
        //            if (!flag1)
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "اطلاعات معتبر نمی باشد"
        //      }
        //                });
        //            bool flag2 = await OTPCodeManager.VerifyCodeAsync(userContext, request.OTPCode);
        //            if (!flag2)
        //            {
        //                UserInfo userInfo = await OMFSecurityProvider.Instance.GetUserAsync(userContext.UserName, new int?());
        //                int? otpTryNo = userInfo.OTPTryNo;
        //                int num1;
        //                if (otpTryNo.HasValue)
        //                {
        //                    otpTryNo = userInfo.OTPTryNo;
        //                    num1 = otpTryNo.Value >= (int)ConfigurationController.OTPTryNo ? 1 : 0;
        //                }
        //                else
        //                    num1 = 0;
        //                if (num1 != 0)
        //                    return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                    {
        //                        resultCode = 2,
        //                        message = "کد امنیتی منقضی گردید، لطفا مجددا به سامانه وارد شوید"
        //                    });
        //                using (SecurityDbContext context = new SecurityDbContext())
        //                {
        //                    UserInfo userInfo1 = userInfo;
        //                    otpTryNo = userInfo1.OTPTryNo;
        //                    userInfo1.OTPTryNo = otpTryNo.HasValue ? new int?(otpTryNo.GetValueOrDefault() + 1) : new int?();
        //                    userInfo.ObjectState = ObjectState.Modified;
        //                    context.SyncObjectState<UserInfo>(userInfo);
        //                    int num2 = await context.SaveChangesAsync();
        //                }
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "کد وارد شده صحیح نمی باشد"
        //      }
        //                });
        //            }
        //            SessionManager.Remove("__CurrentOTPUserContext__");
        //            SecurityManager.CurrentUserContext = userContext;
        //            SecurityManager.CurrentUserToken = userContext.Token;
        //            await SecurityManager.SaveActivityLogForLoginActionAsync(SecurityManager.CurrentUserContext.UserName);
        //            message = await this.AfterLoginAsync(userContext);
        //            if (string.IsNullOrWhiteSpace(message))
        //                return this.CreateSuccessLoginResponse(userContext);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>() { message }
        //            });
        //        }
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 1,
        //            failures = new List<string>()
        //  {
        //    "اطلاعات یافت نشد"
        //  }
        //        });
        //    }
        //    catch (LoginException ex)
        //    {
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 1,
        //            failures = new List<string>() { ex.Message }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionManager.SaveExceptionAsync(ex);
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 4,
        //            message = ExceptionManager.GetExceptionMessage(ex),
        //            stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
        //        });
        //    }
        //    HttpResponseMessage httpResponseMessage;
        //    return httpResponseMessage;
        //}

        protected virtual HttpResponseMessage CreateSuccessLoginResponse(
          UserContext userContext)
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, new
            {
                resultCode = 0,
                data = new
                {
                    UserId = userContext.UserId,
                    Gender = userContext.Gender,
                    FullName = userContext.FullName,
                    NationalCode = userContext.NationalCode,
                    UserName = userContext.UserName,
                    AuthenticationType = userContext.AuthenticationType,
                    MobileNo = userContext.MobileNo,
                    OrganizationId = userContext.OrganizationId,
                    Roles = userContext.Roles.OrderBy<RoleData, int>((Func<RoleData, int>)(r => r.RoleID)).Select(r => new
                    {
                        RoleName = r.RoleName,
                        RoleCode = r.RoleCode
                    }),
                    Token = userContext.Token,
                    Tag1 = userContext.Tag1,
                    Tag2 = userContext.Tag2,
                    Tag3 = userContext.Tag3,
                    Tag4 = userContext.Tag4,
                    Tag5 = userContext.Tag5,
                    Tag6 = userContext.Tag6,
                    Tag7 = userContext.Tag7,
                    Tag8 = userContext.Tag8,
                    Tag9 = userContext.Tag9,
                    Tag10 = userContext.Tag10,
                    TagInt1 = userContext.TagInt1,
                    TagInt2 = userContext.TagInt2,
                    TagInt3 = userContext.TagInt3,
                    TagInt4 = userContext.TagInt4,
                    TagInt5 = userContext.TagInt5
                }
            });
        }

        protected virtual string BeforeLogin(string userName)
        {
            return (string)null;
        }

        protected virtual async Task<UserContext> DoLoginAsync(
          string userName,
          string password,
          bool needToLog)
        {
            UserContext userContext = await SecurityManager.LoginAsync(userName, password, needToLog);
            return userContext;
        }

        protected virtual async Task<string> AfterLoginAsync(UserContext userContext)
        {
            return (string)null;
        }

        protected virtual string OnUserContextIsNull(
          string userName,
          string password,
          out UserContext userContext)
        {
            userContext = (UserContext)null;
            //return ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "نام کاربری یا رمز عبور صحیح نمی باشد" : "invalid username or password.";
            return "نام کاربری یا رمز عبور صحیح نمی باشد";
        }

        protected virtual string Validate(SecurityController.LoginRequest request)
        {
            string str = "";
            if (string.IsNullOrWhiteSpace(request.UserName))
                str += "نام کاربری اجباری می باشد";
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                if (!string.IsNullOrEmpty(str))
                    str += "<br />";
                str +=  "رمز عبور اجباری می باشد";
            }
            if ( (string.IsNullOrWhiteSpace(request.Captcha) || !CaptchaManager.VerifyCaptcha(request.Captcha)))
            {
                if (!string.IsNullOrEmpty(str))
                    str += "<br />";
                str +=  "کد امنیتی صحیح نمی باشد";
            }
            if (!string.IsNullOrEmpty(str) || !request.UserName.IsInvalidWebInput() && !request.Password.IsInvalidWebInput() && request.Password.Length <= 30)
                return str;
            str =  "اطلاعات وارد شده معتبر نمی باشد";
            return str;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ChangeUserPassword(
          [FromBody] SecurityController.ChangeUserPasswodRequest request)
        {
            try
            {
                if (request == null)
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest);
                SecurityManager.ThrowIfUserContextNull();
                bool flag = await SecurityManager.HasAccessAsync("Security-UserInfo-ChangeUserPassword");
                if (!flag)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 2,
                        message = OperationAccessException.GetMessage("Security-UserInfo-ChangeUserPassword")
                    });
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "رمز عبور جدید اجباری می باشد."
            }
                    });
                if (request.NewPassword.IsInvalidWebInput())
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "رمز عبور جدید معتبر نمی باشد."
            }
                    });
                if (ConfigurationController.CaptchaEnable && (string.IsNullOrWhiteSpace(request.Captcha) || !CaptchaManager.VerifyCaptcha(request.Captcha)))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "کد امنیتی صحیح نمی باشد."
            }
                    });
                if (request.NewPassword.Length > 30)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "اطلاعات وارد شده معتبر نمی باشد"
            }
                    });
                UserInfo userInfo = await OMFSecurityProvider.Instance.GetUserAsync(request.UserName, new int?());
                if (userInfo == null)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "کاربری پیدا نشد."
            }
                    });
                string message = await this.DoChangePasswordAsync(userInfo, request.NewPassword);
                if (string.IsNullOrWhiteSpace(message))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 0,
                        data = "عملیات با موفقیت انجام شد."
                    });
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = new List<string>() { message }
                });
            }
            catch (UserContextNullException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 3
                });
            }
            catch (OMFValidationException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = ex.Failures
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword(
          [FromBody] SecurityController.ChangePasswodRequest request)
        {
            try
            {
                if (request == null)
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest);
                SecurityManager.ThrowIfUserContextNull();
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "رمز عبور جدید اجباری می باشد."
            }
                    });
                if (request.NewPassword.IsInvalidWebInput() || request.OldPassword.IsInvalidWebInput())
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "رمز عبور جدید یا رمز عبور قبلی معتبر نمی باشد."
            }
                    });
                if (request.NewPassword.Length > 30)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "اطلاعات وارد شده معتبر نمی باشد"
            }
                    });
                UserInfo userInfo = await OMFSecurityProvider.Instance.GetUserAsync(SecurityManager.CurrentUserContext.UserId);
                if (userInfo == null)
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>()
            {
              "کاربری پیدا نشد."
            }
                    });
                string message = this.BeforeChangePassword(userInfo, request);
                if (!string.IsNullOrWhiteSpace(message))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>() { message }
                    });
                message = await this.DoChangePasswordAsync(userInfo, request.NewPassword);
                if (!string.IsNullOrWhiteSpace(message))
                    return this.Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        resultCode = 1,
                        failures = new List<string>() { message }
                    });
                await this.AfterChangePassword(userInfo, request);
                await SecurityManager.SaveActivityLogForChangePassActionAsync(userInfo.ID, userInfo.UserName);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = "عملیات با موفقیت انجام شد."
                });
            }
            catch (OMFValidationException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = ex.Failures
                });
            }
            catch (UserContextNullException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 3
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        protected virtual string BeforeChangePassword(
          UserInfo userInfo,
          SecurityController.ChangePasswodRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword))
                return "رمز عبور قبلی اجباری می باشد.";
            if (request.OldPassword.Length > 30 || !OperationAccess.PasswordEqual(userInfo.Password, request.OldPassword))
                return "رمز عبور قبلی نادرست می باشد.";
            return (string)null;
        }

        protected virtual async Task<string> DoChangePasswordAsync(
          UserInfo userInfo,
          string newPassword)
        {
            userInfo.PlainPassword = newPassword;
            userInfo.Password = CryptoHelper.ComputeHash(newPassword, "SHA256", (byte[])null);
            //using (IBusinessRuleBaseAsync<UserInfo> userBusiness = BusinessContext.GetBusinessRule<UserInfo>((IUnitOfWorkAsync)new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext())))
            //{
            //    userBusiness.UseForAnonymousUser = true;
            //    userBusiness.Update(userInfo);
            //    int num = await userBusiness.SaveChangesAsync();
            //}
            return (string)null;
        }

        protected virtual async Task AfterChangePassword(
          UserInfo userInfo,
          SecurityController.ChangePasswodRequest request)
        {
            UserContext userContext = await SecurityManager.LoginAsync(userInfo.UserName, request.NewPassword, false);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ChangeUserName(
          [FromBody] SecurityController.ChangeUserNameRequest request)
        {
            try
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (OMFValidationException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 1,
                    failures = ex.Failures
                });
            }
            catch (UserContextNullException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 3
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        protected virtual async Task<string> DoChangeUserNameAsync(
          UserInfo userInfo,
          string newUserName)
        {
            userInfo.UserName = newUserName;
            //using (IBusinessRuleBaseAsync<UserInfo> userBusiness = BusinessContext.GetBusinessRule<UserInfo>((IUnitOfWorkAsync)new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext())))
            //{
            //    userBusiness.UseForAnonymousUser = true;
            //    userBusiness.Update(userInfo);
            //    int num = await userBusiness.SaveChangesAsync();
            //}
            return (string)null;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> LogOff()
        {
            try
            {
                if (SecurityManager.CurrentUserToken != null)
                    await SecurityManager.LogoffAsync(SecurityManager.CurrentUserToken);
                SessionManager.Clear();
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = new { }
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                SessionManager.Clear();
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetCaptcha()
        {
            try
            {
                string captcha = CaptchaManager.GetCaptchaBase64(5, CaptchaFormat.Numeric, 120, 50);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = captcha
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> GetMenus()
        {
            try
            {

               // List<Operation> operationList;
        

                SecurityManager.ThrowIfUserContextNull();
                List<SecurityController.MenuOperationVM> menus = await this.GetApplicationMenus();
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 0,
                    data = menus
                });
            }
            catch (UserContextNullException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 3
                });
            }
            catch (Exception ex)
            {
                await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    message = ExceptionManager.GetExceptionMessage(ex),
                    stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        protected virtual async Task<List<SecurityController.MenuOperationVM>> GetApplicationMenus()
        {
            List<Operation> mainMenus = await OMFSecurityProvider.Instance.GetUserOperationsAsync(OperationType.MainMenu);
            List<SecurityController.MenuOperationVM> menus = new List<SecurityController.MenuOperationVM>();
            foreach (Operation operation in mainMenus)
            {
                Operation mainMenu = operation;
                List<SecurityController.MenuOperationVM> subMenus = await this.GetChildOperationsAsync(mainMenu);
                menus.AddRange((IEnumerable<SecurityController.MenuOperationVM>)subMenus);
                subMenus = (List<SecurityController.MenuOperationVM>)null;
                mainMenu = (Operation)null;
            }
            return menus;
        }

        protected async Task<List<SecurityController.MenuOperationVM>> GetChildOperationsAsync(
          Operation parent)
        {
            OperationType childOprType = OperationType.SubMenu;
            switch (parent.OperationType)
            {
                case OperationType.Entity:
                    childOprType = OperationType.Entity;
                    break;
                case OperationType.MainMenu:
                    childOprType = OperationType.SubMenu;
                    break;
                case OperationType.SubMenu:
                    childOprType = OperationType.SubMenu;
                    break;
                case OperationType.Other:
                    childOprType = OperationType.Other;
                    break;
            }
            List<Operation> subMenus = await OMFSecurityProvider.Instance.GetUserOperationsAsync(childOprType);
            IEnumerable<Operation> childeren = subMenus.Where<Operation>((Func<Operation, bool>)(menu =>
            {
                int? parentId = menu.ParentId;
                int id = parent.ID;
                if (parentId.GetValueOrDefault() != id)
                    return false;
                return parentId.HasValue;
            }));
            List<SecurityController.MenuOperationVM> subMenuOperations = new List<SecurityController.MenuOperationVM>();
            foreach (Operation operation in childeren)
            {
                Operation childMenu = operation;
                SecurityController.MenuOperationVM subMenuOperation = new SecurityController.MenuOperationVM();
                subMenuOperation.Title = childMenu.Name;
                subMenuOperation.OrderNo = childMenu.OrderNo;
                subMenuOperation.Url = childMenu.Tag1;
                subMenuOperation.CssClass = childMenu.Tag2;
                subMenuOperation.TitleCss = childMenu.Tag4;
                subMenuOperation.IsExternalLink = Convert.ToBoolean(childMenu.Tag3);
                subMenuOperation.Parent = new SecurityController.MenuOperationVM()
                {
                    Title = parent.Name,
                    OrderNo = parent.OrderNo,
                    Url = parent.Tag1,
                    CssClass = parent.Tag2,
                    IsExternalLink = Convert.ToBoolean(parent.Tag3),
                    TitleCss = parent.Tag4
                };
                SecurityController.MenuOperationVM menuOperationVm = subMenuOperation;
                List<SecurityController.MenuOperationVM> menuOperationVmList = await this.GetChildOperationsAsync(childMenu);
                menuOperationVm.Childeren = menuOperationVmList;
                menuOperationVm = (SecurityController.MenuOperationVM)null;
                menuOperationVmList = (List<SecurityController.MenuOperationVM>)null;
                subMenuOperations.Add(subMenuOperation);
                subMenuOperation = (SecurityController.MenuOperationVM)null;
                childMenu = (Operation)null;
            }
            return subMenuOperations;
        }

        //[HttpPost]
        //public async Task<HttpResponseMessage> SendForgotPasswordRequest(
        //  [FromBody] SecurityController.ForgotPasswordRequest request)
        //{
        //    try
        //    {
        //        if (ForgotPasswordManager.ForgotPasswordHandler == null)
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>()
        //    {
        //      "ارسال درخواست تغییر رمز عبور پیاده سازی نشده است."
        //    }
        //            });
        //        if (request == null)
        //            return this.Request.CreateResponse(HttpStatusCode.BadRequest);
        //        string message = "";
        //        if (string.IsNullOrWhiteSpace(request.UserName))
        //            message += ConfigurationController.ApplicationLanguage == AppLanguage.English ? "Username is mandatory" : "نام کاربری اجباری می باشد";
        //        if (ConfigurationController.CaptchaEnable && (string.IsNullOrWhiteSpace(request.Captcha) || !CaptchaManager.VerifyCaptcha(request.Captcha)))
        //        {
        //            if (!string.IsNullOrEmpty(message))
        //                message += "<br />";
        //            message += ConfigurationController.ApplicationLanguage == AppLanguage.English ? "captcha is incorrect" : "کد امنیتی صحیح نمی باشد";
        //        }
        //        if (!string.IsNullOrEmpty(message))
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>() { message }
        //            });
        //        if (request.UserName.IsInvalidWebInput() || request.Captcha.IsInvalidWebInput())
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>()
        //    {
        //      "اطلاعات وارد شده معتبر نمی باشد."
        //    }
        //            });
        //        using (OMF.EntityFramework.Ef6.UnitOfWork uow = new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext()))
        //        {
        //            IRepositoryAsync<UserInfo> userInfoRep = uow.RepositoryAsync<UserInfo>();
        //            IRepositoryAsync<ForgotPasswordEntity> forgotPassRep = uow.RepositoryAsync<ForgotPasswordEntity>();
        //            UserInfo forgetUserInfo = await userInfoRep.Queryable(false, true, (List<Expression<Func<UserInfo, object>>>)null).FirstOrDefaultAsync<UserInfo>((Expression<Func<UserInfo, bool>>)(user => user.UserName == request.UserName));
        //            if (forgetUserInfo == null)
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "امکان ارسال درخواست برای نام کاربری وارد شده وجود ندارد."
        //      }
        //                });
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode == ForgotPasswordMode.Email && (string.IsNullOrWhiteSpace(forgetUserInfo.Email) || !new Regex("^[_a-z0-9A-Z]+(\\.[_a-z0-9A-Z]+)*@[a-z0-9-A-Z]+(\\.[a-z0-9-A-Z]+)*(\\.[a-zA-Z]{2,3})$").IsMatch(forgetUserInfo.Email)))
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "امکان ارسال درخواست برای نام کاربری وارد شده وجود ندارد."
        //      }
        //                });
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode == ForgotPasswordMode.Mobile && (string.IsNullOrWhiteSpace(forgetUserInfo.MobileNo) || !new Regex("^9[0|1|2|3|4|9][0-9]{8}$").IsMatch(forgetUserInfo.MobileNo)))
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "امکان ارسال درخواست برای نام کاربری وارد شده وجود ندارد."
        //      }
        //                });
        //            string forgotPassCode = await ForgotPasswordManager.CreateCodeAsync(forgetUserInfo.UserName, forgetUserInfo.Email, forgetUserInfo.MobileNo);
        //            if (string.IsNullOrWhiteSpace(forgotPassCode))
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "بروز خطای پیکربندی در ارسال درخواست تغییر رمز عبور. با پشتیبانی تماس بگیرید."
        //      }
        //                });
        //            ForgotPasswordEntity forgotPassword = new ForgotPasswordEntity();
        //            forgotPassword.Code = forgotPassCode;
        //            forgotPassword.UserId = forgetUserInfo.ID;
        //            forgotPassword.UserName = forgetUserInfo.UserName;
        //            forgotPassword.UserEmail = forgetUserInfo.Email;
        //            forgotPassword.UserMobile = forgetUserInfo.MobileNo;
        //            forgotPassword.ClientIP = ActivityLogManager.ClientIP;
        //            forgotPassword.IsActive = true;
        //            forgotPassword.InsertDateTime = DateTime.Now;
        //            forgotPassRep.Insert(forgotPassword);
        //            int num1 = await uow.SaveChangesAsync();
        //            message = await ForgotPasswordManager.SendRequestAsync(forgotPassword);
        //            if (!string.IsNullOrWhiteSpace(message))
        //            {
        //                int num2 = await forgotPassRep.DeleteAsync((object)forgotPassword.ID) ? 1 : 0;
        //                int num3 = await uow.SaveChangesAsync();
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>() { message }
        //                });
        //            }
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                SessionManager.Add(string.Format("RetryForgotPassForStep2_'{0}'", (object)request.UserName), (object)1);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 0,
        //                data = ""
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionManager.SaveExceptionAsync(ex);
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 4,
        //            message = ExceptionManager.GetExceptionMessage(ex)
        //        });
        //    }
        //    HttpResponseMessage httpResponseMessage;
        //    return httpResponseMessage;
        //}

        //[HttpPost]
        //public async Task<HttpResponseMessage> GetUserNameByForgotPassCode(
        //  SecurityController.ForgotPasswordRequest request)
        //{
        //    try
        //    {
        //        if (request == null)
        //            return this.Request.CreateResponse(HttpStatusCode.BadRequest);
        //        int retryForgotPassForStep2SessionValue = 0;
        //        if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //        {
        //            if (request.Code.IsInvalidWebInput())
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "کد وارد شده معتبر نمی باشد"
        //      }
        //                });
        //            object retryForgotPassSession = SessionManager.GetData(string.Format("RetryForgotPassForStep2_'{0}'", (object)request.UserName));
        //            if (retryForgotPassSession == null || !int.TryParse(retryForgotPassSession.ToString(), out retryForgotPassForStep2SessionValue))
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 5,
        //                    failures = new List<string>()
        //      {
        //        "کد ثبت نشده است"
        //      }
        //                });
        //            if (retryForgotPassForStep2SessionValue > 3)
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 5,
        //                    failures = new List<string>()
        //      {
        //        "کد وارد شده منقضی می باشد، لطفا دوباره سعی نمایید."
        //      }
        //                });
        //            retryForgotPassSession = (object)null;
        //        }
        //        using (OMF.EntityFramework.Ef6.UnitOfWork uow = new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext()))
        //        {
        //            DateTime expireDate = DateTime.Now.AddMinutes(-30.0);
        //            IQueryable<ForgotPasswordEntity> forgotPassQuery = uow.RepositoryAsync<ForgotPasswordEntity>().Queryable(false, true, (List<Expression<Func<ForgotPasswordEntity, object>>>)null).Where<ForgotPasswordEntity>((Expression<Func<ForgotPasswordEntity, bool>>)(fpe => fpe.Code == request.Code && fpe.InsertDateTime > expireDate && !string.IsNullOrEmpty(fpe.UserName) && fpe.UserName == request.UserName));
        //            ForgotPasswordEntity forgotPassUser = await forgotPassQuery.OrderByDescending<ForgotPasswordEntity, DateTime>((Expression<Func<ForgotPasswordEntity, DateTime>>)(fpe => fpe.InsertDateTime)).FirstOrDefaultAsync<ForgotPasswordEntity>();
        //            if (forgotPassUser == null || !forgotPassUser.IsActive)
        //            {
        //                if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                    SessionManager.Add(string.Format("RetryForgotPassForStep2_'{0}'", (object)request.UserName), (object)++retryForgotPassForStep2SessionValue);
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "کد وارد شده صحیح نمی باشد."
        //      }
        //                });
        //            }
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                SessionManager.Add(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName), (object)1);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 0,
        //                data = forgotPassUser.UserName
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionManager.SaveExceptionAsync(ex);
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 4,
        //            message = ExceptionManager.GetExceptionMessage(ex)
        //        });
        //    }
        //    HttpResponseMessage httpResponseMessage;
        //    return httpResponseMessage;
        //}

        //[HttpPost]
        //public async Task<HttpResponseMessage> UpdateForgotPassword(
        //  [FromBody] SecurityController.ForgotPasswordRequest request)
        //{
        //    try
        //    {
        //        if (request == null)
        //            return this.Request.CreateResponse(HttpStatusCode.BadRequest);
        //        int retryForgotPassForStep3SessionValue = 0;
        //        if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //        {
        //            object retryForgotPassSession = SessionManager.GetData(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName));
        //            if (retryForgotPassSession == null || !int.TryParse(retryForgotPassSession.ToString(), out retryForgotPassForStep3SessionValue))
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 5,
        //                    failures = new List<string>()
        //      {
        //        "کد امنیتی اعتبار سنجی نشده است"
        //      }
        //                });
        //            if (retryForgotPassForStep3SessionValue > 3)
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 5,
        //                    failures = new List<string>()
        //      {
        //        "اعتبار این مرحله به پایان رسیده است، لطفا دوباره سعی نمایید."
        //      }
        //                });
        //            retryForgotPassSession = (object)null;
        //        }
        //        if (request.Password.IsInvalidWebInput() || request.Code.IsInvalidWebInput() || request.UserName.IsInvalidWebInput() || request.Captcha.IsInvalidWebInput())
        //        {
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                SessionManager.Add(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName), (object)++retryForgotPassForStep3SessionValue);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>()
        //    {
        //      "اطلاعات ارسالی معتبر نمی باشد"
        //    }
        //            });
        //        }
        //        if (request.Password.Length > 30)
        //        {
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                SessionManager.Add(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName), (object)++retryForgotPassForStep3SessionValue);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>()
        //    {
        //      "اطلاعات وارد شده معتبر نمی باشد"
        //    }
        //            });
        //        }
        //        if (ConfigurationController.CaptchaEnable && (string.IsNullOrWhiteSpace(request.Captcha) || !CaptchaManager.VerifyCaptcha(request.Captcha)))
        //        {
        //            if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                SessionManager.Add(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName), (object)++retryForgotPassForStep3SessionValue);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>()
        //    {
        //      "کد امنیتی صحیح نمی باشد."
        //    }
        //            });
        //        }
        //        using (OMF.EntityFramework.Ef6.UnitOfWork uow = new OMF.EntityFramework.Ef6.UnitOfWork((IDataContextAsync)new SecurityDbContext()))
        //        {
        //            DateTime expireDate = DateTime.Now.AddMinutes(-35.0);
        //            IRepositoryAsync<ForgotPasswordEntity> forgotPassRep = uow.RepositoryAsync<ForgotPasswordEntity>();
        //            ForgotPasswordEntity forgotPassEntity;
        //            ForgotPasswordEntity forgotPasswordEntity = new ForgotPasswordEntity();// forgotPassEntity;
        //            forgotPassEntity = await forgotPassRep.Queryable(false, true, (List<Expression<Func<ForgotPasswordEntity, object>>>)null).OrderByDescending<ForgotPasswordEntity, DateTime>((Expression<Func<ForgotPasswordEntity, DateTime>>)(fpe => fpe.InsertDateTime)).FirstOrDefaultAsync<ForgotPasswordEntity>((Expression<Func<ForgotPasswordEntity, bool>>)(fpe => fpe.Code == request.Code && fpe.InsertDateTime > expireDate && !string.IsNullOrEmpty(fpe.UserName) && fpe.UserName == request.UserName));
        //            if (forgotPassEntity == null || !forgotPassEntity.IsActive)
        //            {
        //                if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                    SessionManager.Add(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName), (object)++retryForgotPassForStep3SessionValue);
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "درخواست تغییر رمز عبور معتبر نمی باشد"
        //      }
        //                });
        //            }
        //            forgotPassEntity.IsActive = false;
        //            forgotPassEntity.UpdateDateTime = DateTime.Now;
        //            forgotPassRep.Update(forgotPassEntity);
        //            int num = await uow.SaveChangesAsync();
        //            IRepositoryAsync<UserInfo> userInfoRep = uow.RepositoryAsync<UserInfo>();
        //            UserInfo forgetUserInfo = await userInfoRep.Queryable(false, true, (List<Expression<Func<UserInfo, object>>>)null).SingleOrDefaultAsync<UserInfo>((Expression<Func<UserInfo, bool>>)(user => user.ID == forgotPassEntity.UserId));
        //            if (forgetUserInfo == null)
        //            {
        //                if (ForgotPasswordManager.ForgotPasswordHandler.Mode != ForgotPasswordMode.Email)
        //                    SessionManager.Add(string.Format("RetryForgotPassForStep3_'{0}'", (object)request.UserName), (object)++retryForgotPassForStep3SessionValue);
        //                return this.Request.CreateResponse(HttpStatusCode.OK, new
        //                {
        //                    resultCode = 1,
        //                    failures = new List<string>()
        //      {
        //        "درخواست تغییر رمز عبور معتبر نمی باشد"
        //      }
        //                });
        //            }
        //            string str = await this.DoChangePasswordAsync(forgetUserInfo, request.Password);
        //            await SecurityManager.SaveActivityLogForChangePassActionAsync(forgetUserInfo.ID, forgetUserInfo.UserName);
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 0,
        //                data = "عملیات با موفقیت انجام شد"
        //            });
        //        }
        //    }
        //    catch (OMFValidationException ex)
        //    {
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 1,
        //            failures = ex.Failures
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionManager.SaveExceptionAsync(ex);
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 4,
        //            message = ExceptionManager.GetExceptionMessage(ex)
        //        });
        //    }
        //    HttpResponseMessage httpResponseMessage;
        //    return httpResponseMessage;
        //}

        protected string EncryptSSOToken(string token)
        {
            return CryptoHelper.Encrypt(token, "BCSM3H95");
        }

        public class ForgotPasswordRequest
        {
            public string UserName { get; set; }

            public string Code { get; set; }

            public string Password { get; set; }

            public string Captcha { get; set; }
        }

        public class MenuOperationVM
        {
            public string Title { get; set; }

            public string Url { get; set; }

            public string CssClass { get; set; }

            public bool IsExternalLink { get; set; }

            public string TitleCss { get; set; }

            public int OrderNo { get; set; }

            public List<SecurityController.MenuOperationVM> Childeren { get; set; }

            public SecurityController.MenuOperationVM Parent { get; set; }
        }

        public class LoginRequest
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public string Captcha { get; set; }
        }

        public class LoginWithOTPCodeRequest
        {
            public string OTPCode { get; set; }
        }

        public class ChangeUserNameRequest
        {
            public string OldUserName { get; set; }

            public string NewUserName { get; set; }
        }

        public class ChangePasswodRequest
        {
            public string OldPassword { get; set; }

            public string NewPassword { get; set; }
        }

        public class ChangeUserPasswodRequest
        {
            public string UserName { get; set; }

            public string NewPassword { get; set; }

            public string Captcha { get; set; }
        }
    }
}
