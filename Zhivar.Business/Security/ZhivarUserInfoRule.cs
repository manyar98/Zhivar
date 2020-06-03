using Zhivar.DomainClasses;
//using Zhivar.DomainClasses.Profile;
using Zhivar.DomainClasses.Security;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static OMF.Common.Enums;

namespace Zhivar.Business.Security
{
    public class ZhivarUserInfoRule : BusinessRuleBase<UserInfo>
    {
        public ZhivarUserInfoRule()
               : base()
        {

        }

        public ZhivarUserInfoRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ZhivarUserInfoRule(OperationAccess operationAccess, IUnitOfWorkAsync uow)
            : base(uow)
        {
            OperationAccess = operationAccess;
        }

        public ZhivarUserInfoRule(OperationAccess operationAccess, bool useForAnonymousUser)
            : base()
        {
            OperationAccess = operationAccess;
            UseForAnonymousUser = useForAnonymousUser;
        }

        public ZhivarUserInfoRule(OperationAccess operationAccess, IUnitOfWorkAsync uow, bool useForAnonymousUser)
            : base(uow)
        {
            OperationAccess = operationAccess;
            UseForAnonymousUser = useForAnonymousUser;
        }

        public ZhivarUserInfoRule(IUnitOfWorkAsync uow, bool useForAnonymousUser)
            : base(uow)
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public ZhivarUserInfoRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public async Task<UserInfo> RegisterUserAsync(RegisterUserBusiClass user)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {

                string roleCode = "";
                string roleName = "";
                int authenticationType = 0;
                string userName = "";
                //switch (user.NoeApplication)
                //{
                //    case ZhivarEnums.Application.BimarApp:
                //        authenticationType = (int)ZhivarEnums.ZhivarUserType.MobileAppUser;
                //        userName = "m" + user.Mobile;
                //        roleCode = ZhivarConstants.BIMAR_APP_ROLE_CODE;
                //        roleName = ZhivarConstants.BIMAR_APP_ROLE_NAME;
                //        break;
                //    case ZhivarEnums.Application.PezeshkApp:
                //        authenticationType = (int)ZhivarEnums.ZhivarUserType.PezeshkUser;
                //        userName = "p" + user.Mobile;
                //        roleCode = ZhivarConstants.PEZESHK_APP_ROLE_CODE;
                //        roleName = ZhivarConstants.PEZESHK_APP_ROLE_NAME;
                //        break;
                //    default:
                //        break;
                //}
                UserInfo ZhivarUserInfo;

                if (this.Queryable().Any(u => u.UserName == userName))
                {
                    //throw new BusinessException($"کاربری با این شماره موبایل قبلاً در سیستم ثبت شده است.", null, 13);
                    ZhivarUserInfo = this.Queryable().SingleOrDefault(us => us.UserName == userName &&
                                                                               us.IsActive);

                    ZhivarUserInfo.Tag6 = user.DeviceToken;
                    this.UpdateEntity(ZhivarUserInfo);
                    await this.SaveChangesAsync();

                }
                else
                {

                    ZhivarUserInfo = new UserInfo()
                    {
                        AuthenticationType = authenticationType,
                        UserName = userName,
                        ApplicationId = ConfigurationController.ApplicationID.Value,
                        FirstName = user.Nam,
                        IsActive = true,
                        LastName = user.NameKhanevadegi,
                        MobileNo = user.Mobile.Length == 10 ? user.Mobile : user.Mobile.Substring(1, 10),
                        Password = "123",
                        PlainPassword = "123",
                        //ShakhsId = shakhs.ID,
                        //Tag1 = shakhs.ID.ToString(),
                        Tag6 = user.DeviceToken,
                        IsDeleted = false
                    };

                    ZhivarUserInfo.Tag3 = false.ToString();
                    this.InsertEntity(ZhivarUserInfo);
                    await this.SaveChangesAsync();
                }
                #region Processing Role
                Role role = null;
                using (UnitOfWork uow = new UnitOfWork(new SecurityDbContext()))
                {
                    var roleRep = uow.RepositoryAsync<Role>();

                    role = roleRep.Queryable(true).SingleOrDefault(r => r.Code == roleCode);

                    if (role == null)
                    {
                        role = new Role()
                        {
                            ApplicationId = ConfigurationController.ApplicationID.Value,
                            Code = roleCode,
                            IsActive = true,
                            IsDeleted = false,
                            Name = roleName,
                            ParentId = null,
                        };

                        roleRep.Insert(role);
                        uow.SaveChanges();
                    }

                    else if (role.IsDeleted || !role.IsActive)
                    {
                        role.IsActive = true;
                        role.IsDeleted = false;

                        roleRep.Update(role);
                        uow.SaveChanges();
                    }
                    //}

                    var userRoles = uow.RepositoryAsync<UserRole>();

                    var userRole = userRoles.Queryable().SingleOrDefault(r => r.UserId == ZhivarUserInfo.ID &&
                                                                              r.RoleId == role.ID);


                    if (userRole == null)
                    {
                        ZhivarUserInfo.UserRoles = new List<UserRole>();
                        ZhivarUserInfo.UserRoles.Add(new UserRole() { RoleId = role.ID, ObjectState = ObjectState.Added });
                    }
                }
                #endregion

                this.InsertOrUpdateGraph(ZhivarUserInfo);
                await this.SaveChangesAsync();

                //var shakhsRep = this.UnitOfWork.Repository<Shakhs>();
                //var shakhses = await shakhsRep.Queryable().Where(sh => sh.Mobile == user.Mobile).ToListAsync();

                //if (!shakhses.Any())
                //{
                //    Shakhs shakhs = new Shakhs()
                //    {
                //        Mobile = user.Mobile.Length == 10 ? user.Mobile : user.Mobile.Substring(1, 10),
                //        Nam = user.Nam,
                //        NameKhanevadegi = user.NameKhanevadegi,
                //    };
                //    shakhsRep.Insert(shakhs);
                //    await this.SaveChangesAsync();

                //    ZhivarUserInfo.ShakhsId = shakhs.ID;
                //    ZhivarUserInfo.Tag1 = shakhs.ID.ToString();
                //    this.UpdateEntity(ZhivarUserInfo);
                //    await this.SaveChangesAsync();
                //}
                //else
                //{
                    
                //    if (shakhses.Any(sh => sh.ID == ZhivarUserInfo.ShakhsId))
                //    {
                //        var shakhs = shakhses.SingleOrDefault(sh => sh.ID == ZhivarUserInfo.ShakhsId);
                //        shakhs.Mobile = user.Mobile.Length == 10 ? user.Mobile : user.Mobile.Substring(1, 10);
                //        shakhs.Nam = user.Nam;
                //        shakhs.NameKhanevadegi = user.NameKhanevadegi;
                //        shakhsRep.Update(shakhs);
                //        await this.SaveChangesAsync();
                //    }
                //    else
                //    {
                //        Shakhs shakhs = new Shakhs()
                //        {
                //            Mobile = user.Mobile.Length == 10 ? user.Mobile : user.Mobile.Substring(1, 10),
                //            Nam = user.Nam,
                //            NameKhanevadegi = user.NameKhanevadegi,
                //        };
                //        shakhsRep.Insert(shakhs);
                //        await this.SaveChangesAsync();

                //        ZhivarUserInfo.ShakhsId = shakhs.ID;
                //        ZhivarUserInfo.Tag1 = shakhs.ID.ToString();
                //        this.UpdateEntity(ZhivarUserInfo);
                //        await this.SaveChangesAsync();
                //    }
                //}
                scope.Complete();

                return ZhivarUserInfo;
               
            }
        }

        public void UpdateUserAsync(int? userId, ZhivarUserInfo2 user)
        {
            var userInfo = this.FindEntity(userId);

            if (userInfo == null)
            {
                throw new BusinessException($"کاربر وجود ندارد", null, 13);
            }

            userInfo.Email = user.Email != null ? user.Email : userInfo.Email;
            userInfo.FirstName = !string.IsNullOrEmpty(user.Nam) ? user.Nam : userInfo.FirstName;
            userInfo.Gender = user.Jensiat != null ? user.Jensiat : userInfo.Gender;
            userInfo.LastName = !string.IsNullOrEmpty(user.NameKhanevadegi) ? user.NameKhanevadegi : userInfo.LastName;
            //userInfo.MobileNo = user.Mobile;
            userInfo.NationalCode = !string.IsNullOrEmpty(user.CodeMeli) ? user.CodeMeli : userInfo.NationalCode;
            //userInfo.Password = user.Password;
            //userInfo.PlainPassword = user.PlainPassword;
            userInfo.Tel = !string.IsNullOrEmpty(user.Tel) ? user.Tel : userInfo.Tel;
            
            this.Repository.Update(userInfo);
            this.SaveChanges();

            //Shakhs shakhs = BusinessContext.GetBusinessRule<Shakhs>(this.OperationAccess, this.UnitOfWork)
            //                               .Queryable()
            //                               .Where(sh => sh.ID == userInfo.ShakhsId)
            //                               .FirstOrDefault();

            //if (shakhs == null)
            //    shakhs = new Shakhs();

            //shakhs.CodeMeli = !string.IsNullOrEmpty(user.CodeMeli) ? user.CodeMeli : shakhs.CodeMeli;
            //shakhs.Jensiat = user.Jensiat != null ? EnumHelper.GetEnum<ZhivarEnums.Jensiat>(Convert.ToInt32(user.Jensiat)) : shakhs.Jensiat;
            //shakhs.Mobile = !string.IsNullOrEmpty(user.Mobile) ? user.Mobile : shakhs.Mobile;
            //shakhs.Nam = !string.IsNullOrEmpty(user.Nam) ? user.Nam : shakhs.Nam;
            //shakhs.NameKhanevadegi = !string.IsNullOrEmpty(user.NameKhanevadegi) ? user.NameKhanevadegi : shakhs.NameKhanevadegi;
            //shakhs.TarikheTavallod = user.TarikheTavallod != null ? user.TarikheTavallod : shakhs.TarikheTavallod;

            //using (UnitOfWork uow = new OMF.EntityFramework.Ef6.UnitOfWork())
            //{
            //    var shakhsRep = uow.Repository<Shakhs>();
            //    shakhsRep.InsertOrUpdateGraph(shakhs);
            //    uow.SaveChanges();
            //}
        }

        public void UpdateDeviceToken(int? userId, string deviceToken)
        {
            var userInfo = this.FindEntity(userId);

            if (userInfo == null)
                throw new BusinessException($"کاربر وجود ندارد", null, 13);

            userInfo.Tag6 = deviceToken;

            this.Repository.Update(userInfo);
            this.SaveChanges();

        }

        public async Task<ZhivarUserInfo2> GetUserInfoAsync(int? id, string userName)
        {
            using (var uow = this.UnitOfWork)
            {
                UserInfo userInfo;
                if (id != null)
                    userInfo = await this.Queryable().Where(u => u.ID == id).FirstOrDefaultAsync();
                else
                    userInfo = await this.Queryable().Where(u => u.UserName == userName).FirstOrDefaultAsync();

                if (userInfo == null)
                    throw new BusinessException($"کاربر وجود ندارد", null, 13);

                UserInfo user = userInfo.Translate<UserInfo>();

                //Shakhs shakhs = await this.UnitOfWork.RepositoryAsync<Shakhs>()
                //                                     .Queryable()
                //                                     .Where(sh => sh.ID == user.ShakhsId)
                //                                     .FirstOrDefaultAsync();

                ZhivarUserInfo2 userData = new ZhivarUserInfo2()
                {
                    CodeMeli = user.NationalCode,
                    Email = user.Email,
                    UserName = user.UserName,
                    Jensiat = user.Gender,
                    ID = user.ID,
                    //MarkazDarmaniId = user.MarkazDarmaniId,
                    Mobile = user.MobileNo,
                    Nam = user.FirstName,
                    NameKhanevadegi = user.LastName,
                    //NoeKarbar = EnumHelper.GetEnum<ZhivarEnums.ZhivarUserType>(user.AuthenticationType),
                  //  ShakhsId = user.ShakhsId,
                  //  TarikheTavallod = shakhs != null ? shakhs.TarikheTavallod : null,
                    Tel = user.Tel,
                };

               
                return userData;
            }
        }

        public async Task<ZhivarUserInfo2> CheckRegisterUserAsync(string userName,string deviceToken)
        {
            using (var uow = this.UnitOfWork)
            {
                UserInfo userInfo = await this.Queryable().Where(u => u.UserName == userName).FirstOrDefaultAsync();

                if (userInfo == null)
                    throw new BusinessException($"کاربر وجود ندارد", null, 13);

                UserInfo user = userInfo.Translate<UserInfo>();

                //Shakhs shakhs = await this.UnitOfWork.RepositoryAsync<Shakhs>()
                //                                     .Queryable()
                //                                     .Where(sh => sh.ID == user.ShakhsId)
                //                                     .FirstOrDefaultAsync();

                ZhivarUserInfo2 userData = new ZhivarUserInfo2()
                {
                    CodeMeli = user.NationalCode,
                    Email = user.Email,
                    UserName = user.UserName,
                    Jensiat = user.Gender,
                    ID = user.ID,
                    //MarkazDarmaniId = user.MarkazDarmaniId,
                    Mobile = user.MobileNo,
                    Nam = user.FirstName,
                    NameKhanevadegi = user.LastName,
                    //NoeKarbar = EnumHelper.GetEnum<ZhivarEnums.ZhivarUserType>(user.AuthenticationType),
                   // ShakhsId = user.ShakhsId,
                 //   TarikheTavallod = shakhs != null ? shakhs.TarikheTavallod : null,
                    Tel = user.Tel,
                };

                if (!string.IsNullOrEmpty(deviceToken))
                {
                    user.Tag6 = deviceToken;

                    this.Repository.Update(user);
                    this.SaveChanges();
                }

                return userData;
            }
        }
    }

    public class RegisterUserBusiClass
    {
        public string Mobile { get; set; }
        public string DeviceToken { get; set; }
      //  public ZhivarEnums.Application NoeApplication { get; set; }
        public string Nam { get; set; }
        public string NameKhanevadegi { get; set; }
    }
}

