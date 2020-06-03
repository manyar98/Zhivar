using OMF.EntityFramework.Ef6;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using OMF.Common;
using OMF.Common.Security;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Http;
using OMF.Security.Model;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.ExceptionManagement;
using OMF.Security;
using OMF.Security.TokenManagement;
using OMF.Common.Cache;
using Zhivar.DomainClasses;
using static Zhivar.DomainClasses.ZhivarEnums;
using Zhivar.DomainClasses.Security;
using OMF.Enterprise.MVC.Security;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DataLayer.Context;

namespace Zhivar.Web.Controllers.Security
{
    public class LoginController : SecurityController
    {
        public LoginController()
        {
        }

      
        //protected override string BeforeLogin(string userName)
        //{
        //    if (TokenManager.ListUsers().Any(userContext => userContext.UserName.ToLower() == userName.ToLower()))
        //        return "امکان ورود برای کاربر یکسان به صورت همزمان وجود ندارد.";

        //    return base.BeforeLogin(userName);
        //}

        protected override Task<UserContext> DoLoginAsync(string userName, string password, bool needToLog)
        {

        

            //TODO Uncomment Azari
            //string sessionId = EncryptSessionId(SessionManager.SessionId);
            //SignInData request = new SignInData { UserName = userName, Password = password, SessionId = sessionId };

            //try
            //{
            //    UserContext userContext = BCSProxy.SignIn(request);

            //    if (userContext == null)
            //        ExceptionManager.SaveExceptionAsync(new Exception($"BCSProxy SignIn: UserContext is null with Username {userName}!"));
            //}
            //catch (Exception ex)
            //{
            //    ExceptionManager.SaveExceptionAsync(ex);
            //}

            return base.DoLoginAsync(userName, password, needToLog);
        }

        //protected async override Task<string> AfterLoginAsync(UserContext userContext)
        //{
        //    if (userContext.AuthenticationType == (int)Enums.VBSUserType.OrganizationPersonnel)
        //    {
        //        //TODO حذف کدهای ذیل و انتقال آنها به زیر ساخت
        //        int personnelId;
        //        if (string.IsNullOrWhiteSpace(userContext.Tag1) || !Int32.TryParse(userContext.Tag1, out personnelId))
        //            return await base.AfterLoginAsync(userContext);

        //        using (UnitOfWork uow = new UnitOfWork())
        //        {
        //           // var personnelQuery = uow.RepositoryAsync<Personel>().Queryable().Where(pr => pr.ID == personnelId);
        //            var organizationQuery = uow.RepositoryAsync<Organization>().Queryable();
        //           // var shahrQuery = uow.RepositoryAsync<Shahr>().Queryable();
        //            var positionQuery = uow.RepositoryAsync<Position>().Queryable();

        //            var joinQuery = from personnel in personnelQuery
        //                            join organShahr in (from organization in organizationQuery
        //                                                join shahr in shahrQuery
        //                                                on organization.CityId equals shahr.ID
        //                                                select new
        //                                                {
        //                                                    OrganizationId = organization.ID,
        //                                                    OrganizationOnvan = organization.Title,
        //                                                    ShahrId = shahr.ID
        //                                                })
        //                            on personnel.OrganizationId equals organShahr.OrganizationId
        //                            join position in positionQuery
        //                            on personnel.PositionId equals position.ID
        //                            select new
        //                            {
        //                                OrganizationId = organShahr.OrganizationId,
        //                                OrganizationOnvan = organShahr.OrganizationOnvan,
        //                                ShahrId = organShahr.ShahrId,
        //                                PositionId = position.ID,
        //                                PositionOnvan = position.Name,
        //                                PositionCode = position.Code,
        //                                ShakhsId = personnel.ShakhsId
        //                            };

        //            var result = await joinQuery.FirstOrDefaultAsync();

        //            if (result == null)
        //                return await base.AfterLoginAsync(userContext);

        //            VBSUserContext.Current = new VBSUserContext()
        //            {
        //                OrganizationId = result.OrganizationId,
        //                OrganizationOnvan = result.OrganizationOnvan,
        //                PersonnelId = personnelId,
        //                PositionId = result.PositionId,
        //                PositionOnvan = result.PositionOnvan,
        //                PositionCode = result.PositionCode,
        //                ShahrId = result.ShahrId,
        //                UserId = userContext.UserId,
        //                ShakhsId = result.ShakhsId
        //            };

        //            //SecurityManager.CurrentUserContext.RoleCodes.Add(VBSUserContext.Current.PositionCode);
        //            userContext.Tag7 = result.PositionOnvan;
        //            userContext.Tag8 = result.PositionCode;
        //            userContext.Tag9 = result.OrganizationId.ToString();
        //            userContext.Tag10 = result.OrganizationOnvan;

        //            //Must be here.
        //            VBSUserContext.Current.UserRoles = await new VBSCartableHandler().GetCurrentUserRoles();
        //        }
        //    }
        //    else if (userContext.AuthenticationType == (int)Enums.VBSUserType.DrugStoreUser)
        //    {
        //        switch (userContext.Tag8)
        //        {
        //            case "0":
        //                userContext.Tag10 = userContext.Tag5 == null ? "کاربر داروخانه" : "کاربر مؤسس";
        //                break;
        //            case "1":
        //                userContext.Tag10 = "کاربر مسئول فنی";
        //                break;
        //            case "2":
        //                userContext.Tag10 = "کاربر مؤسس و مسئول فنی";
        //                break;
        //        }
        //        if (string.IsNullOrWhiteSpace(userContext.Tag6))
        //        {
        //            using (UnitOfWork uow = new UnitOfWork())
        //            {
        //                var daroukhaneh = await uow.RepositoryAsync<Daroukhaneh>()
        //                                           .Queryable()
        //                                           .OrderByDescending(dar => dar.ID)
        //                                           .Select(dar => new { dar.ID, dar.ShomarehParvane, dar.CodePosti, dar.TelHamrah, dar.LogData, dar.VazeyatTaeedSabt })
        //                                           .FirstOrDefaultAsync(dar => dar.LogData.InsertUserName == userContext.UserName && !dar.VazeyatTaeedSabt);
        //                if (daroukhaneh != null)
        //                {
        //                    var userInfoRep = uow.RepositoryAsync<VBSUserInfo>();
        //                    var userInfo = await userInfoRep.Queryable()
        //                                                    .SingleOrDefaultAsync(user => user.ID == userContext.UserId);

        //                    userInfo.Tag1 = daroukhaneh.ShomarehParvane.ToString();
        //                    userInfo.Tag2 = daroukhaneh.CodePosti.ToString();
        //                    userInfo.Tag6 = daroukhaneh.ID.ToString();
        //                    userInfo.MobileNo = daroukhaneh.TelHamrah;
        //                    userInfoRep.Update(userInfo);
        //                    await uow.SaveChangesAsync();

        //                    userContext.Tag6 = daroukhaneh.ID.ToString();
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return await base.AfterLoginAsync(userContext);
        //    }

        //    return null;
        //}

        public async Task<HttpResponseMessage> GetChangePasswordOperationAccess()
        {
            var hasAccess = await SecurityManager.HasAccessAsync(ZhivarResourceIds.Zhivar_Security_ChangePassword);
            var oprAccess = new OperationAccess() { CanView = hasAccess };
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = oprAccess });
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ChangeRoleAsync([FromBody]RoleData roleData)
        {
            return null;

            try
            {
                //if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel())
                //    throw new OMFValidationException("کاربر شما دسترسی مجاز برای تغییر سمت را دارا نمی باشد");

                if (!roleData.RelatedUserId.HasValue)
                    throw new OMFValidationException("لطفا سمت مد نظر را انتخاب نمایید");

                //    var userIdList = await VBSCartableHandler.GetAllUserIdsAsync();
                //if (roleData.RelatedUserId.HasValue && !userIdList.Contains(roleData.RelatedUserId.Value))
                //    throw new OMFValidationException("سمت انتخاب شده صحیح نمی باشد");

                //using (var uow = new UnitOfWork())
                //{
                //    var changedUser = await uow.RepositoryAsync<ZhivarUserInfo>()
                //                               .Queryable()
                //                               .SingleOrDefaultAsync(u => u.ID == roleData.RelatedUserId.Value);

                //    var userContext = await SecurityManager.LoginAsync(changedUser.UserName, changedUser.Password, true);

                //    var message = await AfterLoginAsync(userContext);

                //    if (!string.IsNullOrEmpty(message))
                //        throw new OMFValidationException(message);

                //    var resultContext = new
                //    {
                //        UserId = userContext.UserId,
                //        Gender = userContext.Gender,
                //        FullName = userContext.FullName,
                //        NationalCode = userContext.NationalCode,
                //        UserName = userContext.UserName,
                //        AuthenticationType = userContext.AuthenticationType,
                //        MobileNo = userContext.MobileNo,
                //        OrganizationId = userContext.OrganizationId,
                //        Tag1 = userContext.Tag1,
                //        Tag2 = userContext.Tag2,
                //        Tag3 = userContext.Tag3,
                //        Tag4 = userContext.Tag4,
                //        Tag5 = userContext.Tag5,
                //        Tag6 = userContext.Tag6,
                //        Tag7 = userContext.Tag7,
                //        Tag8 = userContext.Tag8,
                //        Tag9 = userContext.Tag9,
                //        Tag10 = userContext.Tag10,
                //        TagInt1 = userContext.TagInt1,
                //        TagInt2 = userContext.TagInt2,
                //        TagInt3 = userContext.TagInt3,
                //        TagInt4 = userContext.TagInt4,
                //        TagInt5 = userContext.TagInt5
                //    };

                //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = resultContext });
                //}
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> IsUserAuthenticated()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = "" });
            //TODO Uncomment Azari
            //try
            //{
            //    string sessionId = EncryptSessionId(SessionManager.SessionId);
            //    UserAuthenticatedInfo authenticateRequest = new UserAuthenticatedInfo { SessionId = sessionId };
            //    UserContext securityUserContext = BCSProxy.IsUserAuthenticated(authenticateRequest);

            //    if (securityUserContext == null)
            //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = "" });

            //    var userInfo = OMFSecurityProvider.Instance.GetUser(securityUserContext.UserName);
            //    if (userInfo == null)
            //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = "" });

            //    var tokenInfo = TokenManager.GetTokenInfoByToken(securityUserContext.Token);
            //    if (tokenInfo == null)
            //    {
            //        UserContext userContext = userInfo.ConvertToUserContext();
            //        userContext.LastLoginDateTime = DateTime.Now;
            //        TokenManager.TokenizeUser(userContext, securityUserContext.Token);
            //        SecurityManager.CurrentUserContext = userContext;
            //        SecurityManager.CurrentUserToken = userContext.Token;
            //        await AfterLoginAsync(SecurityManager.CurrentUserContext);

            //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = userContext });
            //    }

            //    await AfterLoginAsync(tokenInfo.UserContext);
            //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = tokenInfo.UserContext });
            //}
            //catch (Exception ex)
            //{
            //    return await this.HandleExceptionAsync(ex);
            //}
        }
    }
}
