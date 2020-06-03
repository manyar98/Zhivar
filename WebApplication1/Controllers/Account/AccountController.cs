using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.Contracts;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Web.UI;
using Zhivar.ServiceLayer.Contracts.Account;
using Zhivar.DomainClasses.Account;
using Zhivar.Web.ViewModels.Identity;
using Zhivar.Web.Utility;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.Utilities;
using Zhivar.ViewModel.Common;
using System.Web.Http;
using System.Net.Http;
using Microsoft.Owin;
using Zhivar.ServiceLayer.Contracts.Accunting;
//using Zhivar.Web.Filters;
using OMF.Security.Captcha;
using static OMF.Common.Enums;
using OMF.Common.Configuration;
using OMF.Common.Extensions;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.ExceptionManagement;
using OMF.Common.Security;
using OMF.Security.Model;
using OMF.Security;
using ServiceLayer.Security;
using Zhivar.ServiceLayer.Contracts.Security;
using OMF.Security.TokenManagement;
using OMF.Common.Cache;
using System.Web;
using static Zhivar.DomainClasses.ZhivarEnums;
using OMF.Common;


namespace Zhivar.Web.Controllers.Account
{
    [RoutePrefix("api/Account")]
    public partial class AccountController :  ApiController
    {
        //[Route("Login")]
        //[HttpPost]
        //[AllowAnonymous]
        //public  async Task<HttpResponseMessage> Login([FromBody] LoginViewModel model)
        //{
        //    //createAccountForOrgan();
        //    if (!ModelState.IsValid)
        //    {
        //        //return View(model);
        //    }

        //    // if (!CapchaIsValid(form))
        //    //   {
        //    //      ViewBag.Message = "Sorry, please validate the reCAPTCHA";
        //    //       return View(model);
        //    //   }

        //    var result = SignInStatus.Failure;

        //    //var user = await _userManager.FindByNameAsync(model.UserName);
        //   // if (user != null)
        //   // {
        //        //result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: true);
        //   // }

        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            {
        //                var p = 34;
        //                var person = new Person();//  personRule.GetPersonByUserId(p);// (user.Id);


        //                if (person == null || (person.Vaziat == Enums.Vaziat.Faal && person.IsOrgan == true) || person.IsOrgan == false)
        //                {
        //                    var fullname = "مدیر";
        //                    if (person != null)
        //                    {
        //                        fullname = person.Nam + " " + person.NamKhanvadegi;
        //                    }
        //                    var profileData = new UserProfileSessionData
        //                    {
        //                       // UserId = user.Id,
        //                      //  EmailAddress = user.Email,
        //                        FullName = fullname
        //                    };

        //                    //this.Session["UserProfile"] = profileData;
        //                   // return redirectToLocal(returnUrl, user.Roles);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "مدیر سیستم هنوز کاربری شما را فعال نکرده است.");

        //                    //return View(model);
        //                }
        //                break;
        //            }

        //        case SignInStatus.LockedOut:
        //            //return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
        //        default:
        //            break;
        //            ModelState.AddModelError("", "نام کاربری یا کلمه عبور اشتباه است");
        //            //if (!isUser)
        //            //    return View(model);
        //            //else
        //            //    return View("UserLogin", model);


        //    }

        //    if(model.UserName =="09186664327" && model.Password=="123456")
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = "" });
        //    else
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.NotFound, data = "" });
        //}

        //private void createAccountForOrgan()
        //{
        //    var baseAccounts = _baseAccount.GetAll();
        //    var goroheAccounts = baseAccounts.Where(x => x.Level == Zhivar.DomainClasses.Enums.AccountType.Gorohe);
        //    var kolAccounts = baseAccounts.Where(x => x.Level == Zhivar.DomainClasses.Enums.AccountType.Kol);
        //    var moienAccounts = baseAccounts.Where(x => x.Level == Zhivar.DomainClasses.Enums.AccountType.Moen);

        //    foreach (var goroheAccount in goroheAccounts)
        //    {
        //        DomainClasses.Accounting.Account accountGoroheTemp = new DomainClasses.Accounting.Account();

        //        accountGoroheTemp.Coding = goroheAccount.Coding;
        //        accountGoroheTemp.ComplteCoding = goroheAccount.ComplteCoding;
        //        accountGoroheTemp.Level = goroheAccount.Level;
        //        accountGoroheTemp.Name = goroheAccount.Onvan;
        //        accountGoroheTemp.OrganId = 22;
        //        accountGoroheTemp.ParentId = 0;

        //        _account.Insert(accountGoroheTemp);
        //        _unitOfWork.SaveAllChanges();

        //        foreach (var kol in kolAccounts.Where(x => x.ParentId == goroheAccount.ID))
        //        {
        //            DomainClasses.Accounting.Account accountKolTemp = new DomainClasses.Accounting.Account();

        //            accountKolTemp.Coding = kol.Coding;
        //            accountKolTemp.ComplteCoding = kol.ComplteCoding;
        //            accountKolTemp.Level = kol.Level;
        //            accountKolTemp.Name = kol.Onvan;
        //            accountKolTemp.OrganId = 20;
        //            accountKolTemp.ParentId = accountGoroheTemp.ID;

        //            _account.Insert(accountKolTemp);
        //            _unitOfWork.SaveAllChanges();

        //            foreach (var moien in moienAccounts.Where(x => x.ParentId == kol.ID))
        //            {
        //                DomainClasses.Accounting.Account accountMoenTemp = new DomainClasses.Accounting.Account();

        //                accountMoenTemp.Coding = moien.Coding;
        //                accountMoenTemp.ComplteCoding = moien.ComplteCoding;
        //                accountMoenTemp.Level = moien.Level;
        //                accountMoenTemp.Name = moien.Onvan;
        //                accountMoenTemp.OrganId = 20;
        //                accountMoenTemp.ParentId = accountKolTemp.ID;

        //                _account.Insert(accountMoenTemp);
        //                _unitOfWork.SaveAllChanges();
        //            }
        //        }

        //    }
        //}
        //[Route("CaptchaImage")]
        ////[NoBrowserCache]
        ////[OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "None")]
        //public virtual CaptchaImageResult CaptchaImage()
        //{
        //    return new CaptchaImageResult();
        //}
        //private HttpResponseMessage redirectToLocal(string returnUrl, ICollection<CustomUserRole> roles)
        //{
        //    var roleAdmin = _roleManager.FindRoleByName("Admin");
        //    var roleManage = _roleManager.FindRoleByName("Manage");

        //    //if (roles.Any(x => x.RoleId == roleAdmin.Id))
        //    //    return RedirectToAction(MVC.AdminPanel.Dashboard.ActionNames.Index, MVC.AdminPanel.Dashboard.Name, new { area = MVC.AdminPanel.Name });
        //    //else if (roles.Any(x => x.RoleId == roleManage.Id))
        //    //    return RedirectToAction(MVC.AdminPanel.Dashboard.ActionNames.Index, MVC.AdminPanel.Dashboard.Name, new { area = MVC.AdminPanel.Name });
        //    //if (string.IsNullOrEmpty(returnUrl))
        //    //    return Redirect("/");

        //    //return Redirect(returnUrl);
        //    return Request.CreateResponse();


        //}

        //[Route("LogOff")]
        //[HttpPost]

        //public  async Task<HttpResponseMessage> LogOff()
        //{
        //    //  var isCustomer = false;
        //    //   if (User.IsInRole("Coustomer"))
        //    //    {
        //    //       isCustomer = true;
        //    //   }
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    await _userManager.UpdateSecurityStampAsync(userId);
        //    //Session.Abandon();
        //    //     if (isCustomer)
        //    //     {
        //    return redirectToLocal("/");
        //    // }

        //    //  return RedirectToAction((string)MVC.Account.ActionNames.Login, (string)MVC.Account.Name);
        //}

        //[Route("ResetPassword")]
        //[AllowAnonymous]
        //public  HttpResponseMessage ResetPassword(string code)
        //{
        //    // return code == null ? View("Error") : View();
        //    return Request.CreateResponse();
        //}

        //[Route("ResetPassword")]
        //[HttpPost]
        //[AllowAnonymous]
        //public  async Task<HttpResponseMessage> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return View(model);
        //    }
        //    var user = await _userManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        //return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        //return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    addErrors(result);
        //    //return View();
        //    return Request.CreateResponse();
        //}

        ////[Route("Register")]
        ////[AllowAnonymous]
        ////public  HttpResponseMessage Register(string returnUrl)
        ////{
        ////    ViewBag.ReturnUrl = returnUrl;
        ////    ViewData["NoeShakhs"] = new SelectList(EnumHelper.GetKeyValues<Enums.NoeShakhs>(), "Key", "Value");
        ////    ViewData["TypeHoghoghi"] = new SelectList(EnumHelper.GetKeyValues<Enums.TypeHoghoghi>(), "Key", "Value");

        ////    //ViewData["NoeShakhs"] = new SelectList(EnumHelper.GetKeyValues<Enums.NoeShakhs>(), "Id", "Name");
        ////    return View();
        ////}

        //[Route("Register")]
        //[HttpPost]
        //[ValidateCaptcha]
        //public  async Task<HttpResponseMessage> Register(RegisterViewModel userViewModel, string returnUrl)
        //{
        //    //if (!CapchaIsValid(form))
        //    //{
        //    //    ViewBag.Message = "Sorry, please validate the reCAPTCHA";
        //    //    return View(userViewModel);
        //    //}

        //    var user2 = await _userManager.FindByNameAsync(userViewModel.UserName);

        //    if (user2 != null)
        //    {
        //        ModelState.AddModelError("", "این شماره موبایل قبلا در سایت ثبت شده است.");
        //    }

        //    ValidatePerson(userViewModel);

        //    if (!ModelState.IsValid || user2 != null)
        //    {
        //        //return View(userViewModel);
        //    }


        //    Random rnd = new Random();
        //    var code = rnd.Next(10000, 99999);


        //    //TempData["code"] = 12345; //code;
        //    //TempData["UserName"] = userViewModel.UserName;
        //    //TempData["Pass"] = userViewModel.Password;
        //    //TempData["RegisterViewModel"] = userViewModel;
        //    //TempData["returnUrl"] = returnUrl;
        //    SmsHandler sms = new SmsHandler();
        //    var resultSms = sms.Register(Convert.ToInt64(userViewModel.UserName), code.ToString());

        //    //return RedirectToAction("Verification", "Account");
        //    return Request.CreateResponse();
        //}

        //private void ValidatePerson(RegisterViewModel userViewModel)
        //{


        //    switch (userViewModel.NoeShakhs)
        //    {
        //        case Enums.NoeShakhs.Haghighi:
        //            {

        //                if (string.IsNullOrEmpty(userViewModel.NamKhanvadegi))
        //                {
        //                    ModelState.AddModelError("", "لطفا نام خانوادگی را وارد کنید.");
        //                }
        //                if (string.IsNullOrEmpty(userViewModel.CodeMeli))
        //                {
        //                    ModelState.AddModelError("", "لطفا کد ملی را وارد کنید.");
        //                }
        //            }
        //            break;
        //        case Enums.NoeShakhs.Hoghoghi:
        //            {
        //                if (userViewModel.TypeHoghoghi == null)
        //                {
        //                    ModelState.AddModelError("", "لطفا نوع شخص حقوقی را انتخاب کنید.");
        //                }

        //                switch (userViewModel.TypeHoghoghi)
        //                {
        //                    case Enums.TypeHoghoghi.Sherkat:
        //                        {
        //                            if (string.IsNullOrEmpty(userViewModel.CodeMeli))
        //                            {
        //                                ModelState.AddModelError("", "لطفا شناسه ملی را وارد کنید.");
        //                            }
        //                            if (string.IsNullOrEmpty(userViewModel.CodeEghtesadi))
        //                            {
        //                                ModelState.AddModelError("", "لطفا کد اقتصادی را وارد کنید.");
        //                            }
        //                            if (string.IsNullOrEmpty(userViewModel.ShomareSabt))
        //                            {
        //                                ModelState.AddModelError("", "لطفا شماره ثبت را وارد کنید.");
        //                            }
        //                        }
        //                        break;
        //                    case Enums.TypeHoghoghi.Edare:
        //                        break;
        //                    case Enums.TypeHoghoghi.Kanon:
        //                        {
        //                            if (userViewModel.TarikhSoudor == null)
        //                            {
        //                                ModelState.AddModelError("", "لطفا تاریخ صدور پروانه را وارد کنید.");
        //                            }
        //                            if (string.IsNullOrEmpty(userViewModel.ModateEtebar))
        //                            {
        //                                ModelState.AddModelError("", "لطفا مدت اعتبار پروانه را وارد کنید.");
        //                            }
        //                            if (string.IsNullOrEmpty(userViewModel.SahebEmtiaz))
        //                            {
        //                                ModelState.AddModelError("", "لطفا نام و نام خانوادگی صاحب امتیاز را وارد کنید.");
        //                            }

        //                        }
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //            break;
        //        default:
        //            break;
        //    }

        //}

        //[Route("Verification")]
        //[AllowAnonymous]
        //public  HttpResponseMessage Verification()
        //{
        //    //TempData.Keep();
        //    //return View();
        //    return Request.CreateResponse();
        //}
        //[Route("Verification")]
        //[HttpPost]
        //public  async Task<HttpResponseMessage> Verification(VerificationViewModel verificationViewModel)
        //{

        //    //var Code = TempData["code"].ToString();
        //    //var UserName = TempData["UserName"].ToString();
        //    //var Password = TempData["Pass"].ToString();
        //    //var returnUrl = TempData["returnUrl"].ToString();

        //    //var userViewModel = (RegisterViewModel)TempData["RegisterViewModel"];

        //    //if (verificationViewModel.Code != Code)
        //    //{
        //    //    ModelState.AddModelError("", "کد امنیتی اشتباه است");
        //    //}


        //    //if (!ModelState.IsValid || verificationViewModel.Code != Code)
        //    //{
        //    //    TempData.Keep();
        //    //    return View(verificationViewModel);
        //    //}

        //    //Person person = new Person();

        //    //person.Address = userViewModel.Address;
        //    //person.CodeEghtesadi = userViewModel.CodeEghtesadi;
        //    //person.CodeMeli = userViewModel.CodeMeli;
        //    //person.CodePosti = userViewModel.CodePosti;
        //    //person.ModateEtebar = userViewModel.ModateEtebar;
        //    //person.Nam = userViewModel.Nam;
        //    //person.NamKhanvadegi = userViewModel.NamKhanvadegi;
        //    //person.NamPedar = userViewModel.NamPedar;
        //    //person.NoeShakhs = userViewModel.NoeShakhs;
        //    //person.SahebEmtiaz = userViewModel.SahebEmtiaz;
        //    //person.ShomareSabt = userViewModel.ShomareSabt;
        //    //person.TarikhSoudor = userViewModel.TarikhSoudor;
        //    //person.Tel = userViewModel.Tel;
        //    //person.TypeHoghoghi = userViewModel.TypeHoghoghi;
        //    //person.IsOrgan = true;
        //    //person.Vaziat = Enums.Vaziat.GhireFaal;
        //    //person.CreatedBy = "AnonmousUser";
        //    //person.CreatedOn = DateTime.Now;

        //  //  personRule.Insert(person);
        //    _unitOfWork.SaveAllChanges();



        //    //var user = new ApplicationUser
        //    //{
        //    //    UserName = UserName,
        //    //    Email = UserName + "@smbt.ir",
        //    //    EmailConfirmed = true,
        //    //    Tag1Int = person.ID,

        //    //};

        //    //  var adminresult = await _userManager.CreateAsync(user, Password);

        //    //if (adminresult.Succeeded)
        //    //{
        //    //    const string roleName = "Manage";

        //    //    //Create Role Admin if it does not exist
        //    //    var role = _roleManager.FindRoleByName(roleName);
        //    //    if (role == null)
        //    //    {
        //    //        role = new CustomRole(roleName);
        //    //        var roleresult = _roleManager.CreateRole(role);
        //    //    }

        //    //    var result = await _userManager.AddToRolesAsync(user.Id, roleName);

        //    //    if (!result.Succeeded)
        //    //    {
        //    //        ModelState.AddModelError("", result.Errors.First());
        //    //        return View();
        //    //    }


        //    //}
        //    //else
        //    //{
        //    //    ModelState.AddModelError("", adminresult.Errors.First());

        //    //    return View();
        //    //}

        //    //await _signInManager.PasswordSignInAsync(UserName, Password, false, shouldLockout: false);


        //    //return redirectToLocal(returnUrl);
        //    return Request.CreateResponse();

        //}

        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public  HttpResponseMessage ResetPasswordConfirmation()
        //{
        //    //return View();
        //    return Request.CreateResponse();
        //}


        //[Route("ForgotPassword")]
        //[AllowAnonymous]
        //public  HttpResponseMessage ForgotPassword()
        //{
        //    //return View();
        //    return Request.CreateResponse();
        //}


        //[Route("ForgotPassword")]
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateCaptchaAttribute]
        //public  async Task<HttpResponseMessage> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    //if (!CapchaIsValid(form))
        //    //{
        //    //    ViewBag.Message = "Sorry, please validate the reCAPTCHA";
        //    //    return View(model);
        //    //}

        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(model.UserName);
        //        if (user == null)
        //        {
        //            ModelState.AddModelError("", "این شماره موبایل قبلا در سایت ثبت نشده است.");
        //            // Don't reveal that the user does not exist or is not confirmed
        //            //  return View("ForgotPasswordConfirmation");
        //        }

        //        if (!await _forgetPasswordUserService.CanUseService(user.Id))
        //        {
        //            int day = (int)await _forgetPasswordUserService.Calculate(user.Id);
        //            ModelState.AddModelError("", "این شماره موبایل  تا " + day.ToString() + "روز دیگر نمی تواند از این سرویس استفاده کند.");
        //        }

        //        if (!ModelState.IsValid || user == null)
        //        {
        //            //return View(model);
        //        }


        //        Random rnd = new Random();
        //        var code = rnd.Next(10000, 99999);


        //        //TempData["code"] = code;
        //        //TempData["UserName"] = model.UserName;

        //        SmsHandler sms = new SmsHandler();
        //        var resultSms = sms.Register(Convert.ToInt64(model.UserName), code.ToString());

        //        await _forgetPasswordUserService.Add(user.Id);
        //        await _unitOfWork.SaveAllChangesAsync();
        //        //return RedirectToAction("ForgotPasswordConfirmation", "Account");


        //        //var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
        //        //var callbackUrl = Url.Action("ResetPassword", "Account",
        //        //    new { userId = user.Id, code }, protocol: Request.Url.Scheme);
        //        //await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
        //        //ViewBag.Link = callbackUrl;
        //        //return View("ForgotPasswordConfirmation");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    //return View(model);
        //    return Request.CreateResponse();
        //}

        //[Route("ForgotPasswordConfirmation")]
        //[AllowAnonymous]
        //public  HttpResponseMessage ForgotPasswordConfirmation()
        //{
        //    //TempData.Keep();
        //    //return View();
        //    return Request.CreateResponse();
        //}
        //[Route("ForgotPasswordConfirmation")]
        //[HttpPost]
        //[ValidateCaptchaAttribute]
        //public  async Task<HttpResponseMessage> ForgotPasswordConfirmation(ForgotPasswordConfirmationViewModel userViewModel, FormCollection form)
        //{
        //    //if (!CapchaIsValid(form))
        //    //{
        //    //    ViewBag.Message = "Sorry, please validate the reCAPTCHA";
        //    //    return View(userViewModel);
        //    //}

        //    //var code = TempData["code"].ToString();
        //    //var userName = TempData["UserName"].ToString();

        //    //if (userViewModel.Code != code)
        //    //{
        //    //    ModelState.AddModelError("", "کد امنیتی اشتباه است");
        //    //}



        //    //if (!ModelState.IsValid || userViewModel.Code != code)
        //    //{
        //    //    TempData.Keep();
        //    //    return View(userViewModel);
        //    //}

        //    // var hasher = new PasswordHasher();

        //   // var user = await _userManager.FindByNameAsync(userName);


        //    //    user.PasswordHash = hasher.HashPassword(userViewModel.Password);


        //    //  await _userManager.UpdateAsync(user);
        //    //TempData["message"] = "کلمه عبور با موفقیت ویرایش شد";
        //    //return redirectToLocal("/");
        //    return Request.CreateResponse();

        //}

        //[Route("ChangePassword")]
        //public  HttpResponseMessage ChangePassword()
        //{
        //    //return View();
        //    return Request.CreateResponse();
        //}

        //[Route("ChangeUserPassword")]
        //public  HttpResponseMessage ChangeUserPassword()
        //{
        //    //return View("ChangeUserPassword");
        //    return Request.CreateResponse();
        //}

        //[Route("ChangeUserPassword")]
        //[HttpPost]
        //public  async Task<HttpResponseMessage> ChangeUserPassword(ChangePasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return View("ChangeUserPassword", model);
        //    }
        //    var result = await _userManager.ChangePasswordAsync(_userManager.GetCurrentUserId(), model.OldPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        var user = await _userManager.GetCurrentUserAsync();
        //        if (user != null)
        //        {
        //            await signInAsync(user, isPersistent: false);
        //        }
        //        ModelState.AddModelError("کلمه عبور با موفقیت ویرایش شد", "");
        //    }
        //    addErrors(result);
        //    //return View("ChangeUserPassword", model);
        //    return Request.CreateResponse();
        //}

        //[Route("ChangePassword")]
        //[HttpPost]
        //public  async Task<HttpResponseMessage> ChangePassword(ChangePasswordViewModel model)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    return View(model);
        //    //}
        //    var result = await _userManager.ChangePasswordAsync(_userManager.GetCurrentUserId(), model.OldPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        var user = await _userManager.GetCurrentUserAsync();
        //        if (user != null)
        //        {
        //            await signInAsync(user, isPersistent: false);
        //        }
        //        //TempData["message"] = "کلمه عبور با موفقیت ویرایش شد";
        //    }
        //    addErrors(result);
        //    //return View(model);
        //    return Request.CreateResponse();
        //}


        //private void addErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private HttpResponseMessage redirectToLocal(string returnUrl)
        //{
        //    //if (Url.IsLocalUrl(returnUrl))
        //        //{
        //        //    return Redirect(returnUrl);
        //        //}
        //        //if (User.IsInRole("Admin"))
        //        //    return RedirectToAction(MVC.AdminPanel.Dashboard.ActionNames.Index, MVC.AdminPanel.Dashboard.Name, new { area = MVC.AdminPanel.Name });
        //        //else
        //        //    return RedirectToAction(MVC.AdminPanel.Dashboard.ActionNames.Index, MVC.AdminPanel.Dashboard.Name, new { area = MVC.AdminPanel.Name });

        //        return Request.CreateResponse();
        //}

        //private async Task signInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
        //    _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent },
        //        await _userManager.GenerateUserIdentityAsync(user));
        //}

        ////private bool CapchaIsValid(FormCollection form)
        ////{
        ////    string urlToPost = "https://www.google.com/recaptcha/api/siteverify";
        ////    string secretKey = "6LecfkoUAAAAAGNDg7j8bu_j2b601Tzg8xk_lkOe"; // change this
        ////    string gRecaptchaResponse = form["g-recaptcha-response"];

        ////    var postData = "secret=" + secretKey + "&response=" + gRecaptchaResponse;

        ////    // send post data
        ////    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToPost);
        ////    request.Method = "POST";
        ////    request.ContentLength = postData.Length;
        ////    request.ContentType = "application/x-www-form-urlencoded";

        ////    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        ////    {
        ////        streamWriter.Write(postData);
        ////    }

        ////    // receive the response now
        ////    string result = string.Empty;
        ////    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        ////    {
        ////        using (var reader = new StreamReader(response.GetResponseStream()))
        ////        {
        ////            result = reader.ReadToEnd();
        ////        }
        ////    }

        ////    // validate the response from Google reCaptcha
        ////    var captChaesponse = JsonConvert.DeserializeObject<Zhivar.ViewModels.ReCaptchaResponse>(result);
        ////    if (!captChaesponse.Success)
        ////    {
        ////        return false;
        ////    }
        ////    return true;
        ////}

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
                //await ExceptionManager.SaveExceptionAsync(ex);
                return this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    resultCode = 4,
                    //message = ExceptionManager.GetExceptionMessage(ex),
                    //stackTrace = ConfigurationController.WithDebugInfo ? ExceptionManager.GetExceptionMessageWithDebugInfo(ex) : ""
                });
            }
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }


        //[Route("Login")]
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<HttpResponseMessage> Login([FromBody] LoginRequest request)
        //{
        //    try
        //    {
        //        if (request == null)
        //            return this.Request.CreateResponse(HttpStatusCode.BadRequest);
        //        string message = this.Validate(request);
        //        if (!string.IsNullOrWhiteSpace(message))
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>() { message }
        //            });
        //        message = this.BeforeLogin(request.UserName);
        //        if (!string.IsNullOrWhiteSpace(message))
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>() { message }
        //            });



        //        UserContext userContext = await this.DoLoginAsync(request.UserName, request.Password, false);
        //        if (userContext == null)
        //            message = this.OnUserContextIsNull(request.UserName, request.Password, out userContext);
        //        if (!string.IsNullOrWhiteSpace(message))
        //            return this.Request.CreateResponse(HttpStatusCode.OK, new
        //            {
        //                resultCode = 1,
        //                failures = new List<string>() { message }
        //            });
        //        //bool needOTP = await OTPCodeManager.NeedOTPAsync(userContext);
        //        //if (needOTP)
        //        //{
        //        //    HttpResponseMessage response = await this.CreateOTPAsync(userContext);
        //        //    SecurityManager.CurrentUserContext = (UserContext)null;
        //        //    SessionManager.Add("__CurrentOTPUserContext__", (object)userContext);
        //        //    return response;
        //        //}
        //        //await SecurityManager.SaveActivityLogForLoginActionAsync(userContext.UserName);
        //        message = await this.AfterLoginAsync(userContext);
        //        if (string.IsNullOrWhiteSpace(message))
        //            return this.CreateSuccessLoginResponse(userContext);
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 1,
        //            failures = new List<string>() { message }
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

        //[Route("GetMenus")]
        //public async Task<HttpResponseMessage> GetMenus()
        //{
        //    try
        //    {
        //        //SecurityManager.ThrowIfUserContextNull();
        //        List<MenuOperationVM> menus = await this.GetApplicationMenus();
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 0,
        //            data = menus
        //        });
        //    }
        //    catch (UserContextNullException ex)
        //    {
        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 3
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // await ExceptionManager.SaveExceptionAsync(ex);
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
        //protected  async Task<List<MenuOperationVM>> GetApplicationMenus()
        //{
        //    // List<Operation> mainMenus = await OMFSecurityProvider.Instance.GetUserOperationsAsync(OperationType.MainMenu);

        //    List<Operation> mainMenus = await _operationService.GetUserOperationsAsync(OperationType.MainMenu);


        //    List<MenuOperationVM> menus = new List<MenuOperationVM>();
        //    foreach (Operation operation in mainMenus)
        //    {
        //        Operation mainMenu = operation;
        //        List<MenuOperationVM> subMenus = await this.GetChildOperationsAsync(mainMenu);
        //        menus.AddRange((IEnumerable<MenuOperationVM>)subMenus);
        //        subMenus = (List<MenuOperationVM>)null;
        //        mainMenu = (Operation)null;
        //    }
        //    return menus;
        //}

        protected async Task<List<MenuOperationVM>> GetChildOperationsAsync(
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
            //List<Operation> subMenus = await _operationService.GetUserOperationsAsync(childOprType);

            IEnumerable<Operation> childeren = subMenus.Where<Operation>((Func<Operation, bool>)(menu =>
            {
                int? parentId = menu.ParentId;
                int id = parent.ID;
                if (parentId.GetValueOrDefault() != id)
                    return false;
                return parentId.HasValue;
            }));
            List<MenuOperationVM> subMenuOperations = new List<MenuOperationVM>();
            foreach (Operation operation in childeren)
            {
                Operation childMenu = operation;
                MenuOperationVM subMenuOperation = new MenuOperationVM();
                subMenuOperation.Title = childMenu.Name;
                subMenuOperation.OrderNo = childMenu.OrderNo;
                subMenuOperation.Url = childMenu.Tag1;
                subMenuOperation.CssClass = childMenu.Tag2;
                subMenuOperation.TitleCss = childMenu.Tag4;
                subMenuOperation.IsExternalLink = Convert.ToBoolean(childMenu.Tag3);
                subMenuOperation.Parent = new MenuOperationVM()
                {
                    Title = parent.Name,
                    OrderNo = parent.OrderNo,
                    Url = parent.Tag1,
                    CssClass = parent.Tag2,
                    IsExternalLink = Convert.ToBoolean(parent.Tag3),
                    TitleCss = parent.Tag4
                };
                MenuOperationVM menuOperationVm = subMenuOperation;
                List<MenuOperationVM> menuOperationVmList = await this.GetChildOperationsAsync(childMenu);
                menuOperationVm.Childeren = menuOperationVmList;
                menuOperationVm = (MenuOperationVM)null;
                menuOperationVmList = (List<MenuOperationVM>)null;
                subMenuOperations.Add(subMenuOperation);
                subMenuOperation = (MenuOperationVM)null;
                childMenu = (Operation)null;
            }
            return subMenuOperations;
        }

        protected virtual string BeforeLogin(string userName)
        {
            return (string)null;
        }

        protected virtual string Validate(LoginRequest request)
        {
            string str = "";
            if (string.IsNullOrWhiteSpace(request.UserName))
                str += "نام کاربری اجباری می باشد";
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                if (!string.IsNullOrEmpty(str))
                    str += "<br />";
                str += "رمز عبور اجباری می باشد";
            }
            if ((string.IsNullOrWhiteSpace(request.Captcha)) || !CaptchaManager.VerifyCaptcha(request.Captcha))
            {
                if (!string.IsNullOrEmpty(str))
                    str += "<br />";
                str += "کد امنیتی صحیح نمی باشد";
            }
            if (!string.IsNullOrEmpty(str) || !request.UserName.IsInvalidWebInput() && !request.Password.IsInvalidWebInput() && request.Password.Length <= 30)
                return str;
            str = "اطلاعات وارد شده معتبر نمی باشد";
            return str;
        }

        protected virtual string OnUserContextIsNull(
          string userName,
          string password,
          out UserContext userContext)
        {
            userContext = (UserContext)null;
            return "نام کاربری یا رمز عبور صحیح نمی باشد";
        }

        //protected virtual async Task<UserContext> DoLoginAsync(
        //  string userName,
        //  string password,
        //  bool needToLog)
        //{
        //    UserContext userContext = null;
        //    var user = await _userManager.FindByNameAsync(userName);
        //    if (user != null)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(userName, password, true, shouldLockout: true);
        //       // result = SignInStatus.Success;
        //        switch (result)
        //        {
        //            case SignInStatus.Success:
        //                {
        //                    userContext = new UserContext()
        //                    {
        //                        ApplicationID = user.ApplicationID,
        //                        AuthenticationType = user.AuthenticationType,
        //                        ClientIP = user.ClientIP,
        //                        Email = user.Email,
        //                        FirstName = user.FirstName,
        //                        Gender =user.Gender,
        //                        LastLoginDateTime = user.LastLoginDateTime,
        //                        LastName = user.LastName,
        //                        LastOTPDate = user.LastOTPDate,
        //                        MobileNo = user.MobileNo,
        //                        NationalCode = user.NationalCode,
        //                        NeedOTP = user.NeedOTP,
        //                        OrganizationId = user.OrganizationId,
        //                        OTPCode = user.OTPCode,
        //                        OTPTryNo = user.OTPTryNo,
        //                        Password = user.Password,
        //                        //Roles = user.Roles,
        //                        Tag1 = user.Tag1,
        //                        Tag2 = user.Tag2,
        //                        Tag3 = user.Tag3,
        //                        Tag4 = user.Tag4,
        //                        Tag5 = user.Tag5,
        //                        Tag6 = user.Tag6,
        //                        Tag7 = user.Tag7,
        //                        Tag8 = user.Tag8,
        //                        Tag9 = user.Tag9,
        //                        Tag10 = user.Tag10,
        //                        TagInt1 = user.TagInt1,
        //                        TagInt2 = user.TagInt2,
        //                        TagInt3 = user.TagInt3,
        //                        TagInt4 = user.TagInt4,
        //                        TagInt5 = user.TagInt5,

        //                        Tel = user.Tel,
        //                        Token = user.Token,
        //                        UserId = user.Id,
        //                        UserName = user.UserName

        //                    };

        //                    TokenManager.TokenizeUser(userContext);

        //                    SecurityManager.CurrentUserToken = userContext.Token;
        //                    SecurityManager.CurrentUserContext = userContext;

        //                    break;
        //                }
        //            default:
        //                break;
        //        }
        //    }



        //    //UserContext userContext = await SecurityManager.LoginAsync(userName, password, needToLog);
        //    return userContext;
        //}

        protected virtual async Task<string> AfterLoginAsync(UserContext userContext)
        {
            return (string)null;
        }

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
                    //Roles = userContext.Roles.OrderBy<RoleData, int>((Func<RoleData, int>)(r => r.RoleID)).Select(r => new
                    //{
                    //    RoleName = r.RoleName,
                    //    RoleCode = r.RoleCode
                    //}),
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

        //[Route("LogOff")]
        //[HttpPost]
        //public async Task<HttpResponseMessage> LogOff()
        //{
        //    try
        //    {
        //        if (SecurityManager.CurrentUserToken != null)
        //        {
        //            var ctx = Request.GetOwinContext();  //HttpContext.GetOwinContext().Authentication;
        //            var authManager = ctx.Authentication;

        //            authManager.SignOut();

        //            UserContext userContext = TokenManager.GetUser(SecurityManager.CurrentUserToken);

        //            if (userContext != null)
        //                TokenManager.DeleteToken(SecurityManager.CurrentUserToken);

        //            Action<string> afterLogoffHandler = this.AfterLogoffHandler;
        //            if (afterLogoffHandler != null)
        //                afterLogoffHandler(SecurityManager.CurrentUserToken);
        //            //UserContext userContext = SecurityManager.SecurityProvider.Logoff(userToken);
        //            // if (userContext == null)
        //            //   return;
        //            // SecurityManager.SaveActivityLogForLogOffAction(userContext);

        //            // await SecurityManager.LogoffAsync(SecurityManager.CurrentUserToken);
        //        }


        //        SessionManager.Clear();

        //        return this.Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            resultCode = 0,
        //            data = new { }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // await ExceptionManager.SaveExceptionAsync(ex);
        //        SessionManager.Clear();
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

        public event Action<string> AfterLogoffHandler;



    }

    public class LoginRequest
    {
        public string UserName { get; set; }

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

        public List<MenuOperationVM> Childeren { get; set; }

        public MenuOperationVM Parent { get; set; }
    }

}