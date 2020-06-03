using OMF.Business;
using OMF.Common;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DomainClasses.BaseInfo;
using System.Data.Entity;
using Zhivar.Common.Security;
using System.Linq.Expressions;
using Zhivar.Common;
using OMF.Common.Extensions;
using Zhivar.Business.Common;
using OMF.Common.Cryptography;
using OMF.Common.Configuration;
using Zhivar.Business.BaseInfo;
using OMF.Security.Model;
using Zhivar.Business.Workflows;
using OMF.Common.ExceptionManagement.Exceptions;
using Zhivar.Business.Security;
using Zhivar.ViewModel.BaseInfo;
using static OMF.Common.Enums;
using Zhivar.DomainClasses;
using static Zhivar.DomainClasses.ZhivarEnums;
using OMF.Workflow.Model;
using Zhivar.Utilities;

namespace Zhivar.Web.Controllers.BaseInfo
{
    [RoutePrefix("api/Personel")]
    public class PersonelController : ApiControllerCustomQueryBaseAsync<Personel, PersonelVM, PersonelVM>
    {
        public PersonelRule Rule => this.BusinessRule as PersonelRule;

        protected override IBusinessRuleBaseAsync<Personel> CreateBusinessRule() => new PersonelRule();

        //protected override Expression<Func<Personel, bool>> CreateDefaultSearchExpression()
        //{
        //    if (SecurityManager.CurrentUserContext.IsOrganizationPersonnel())
        //        return person => person.OrganizationId == VBSUserContext.Current.OrganizationId;

        //    if (SecurityManager.CurrentUserContext.IsDeveloperUser() || SecurityManager.CurrentUserContext.IsAdminUser() ||
        //        SecurityManager.CurrentUserContext.IsSupplyChainUser() || SecurityManager.CurrentUserContext.IsSystematicallyPermissionUser())
        //        return base.CreateDefaultSearchExpression();

        //    return person => person.ID == 0;
        //}

        protected override IQueryable<PersonelVM> CreateCustomQuery(IQueryable<Personel> query, QueryInfo searchRequestInfo)
        {
            IQueryable<UserInfo> shakhsQuery = BusinessContext.GetBusinessRule<UserInfo>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();
            IQueryable<Organization> organizationQuery = BusinessContext.GetBusinessRule<Organization>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();
            IQueryable<Position> positionQuery = BusinessContext.GetBusinessRule<Position>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();

            #region Query Processing
            if (searchRequestInfo.Filter != null && searchRequestInfo.Filter.Filters != null && searchRequestInfo.Filter.Filters.Any())
            {
                FilterInfo shakhsFilterInfo = new FilterInfo();
                FilterInfo organizationFilterInfo = new FilterInfo();
                FilterInfo positionFilterInfo = new FilterInfo();
                FilterInfo personelFilterInfo = new FilterInfo();

                string namPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.FirstName);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == namPropName))
                {
                    FilterData namFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == namPropName);

                    string NamProp = Entity.GetPropertyName<UserInfo>(shakhs => shakhs.FirstName);
                    shakhsFilterInfo.Filters.Add(new FilterData() { Field = NamProp, Operator = namFilterData.Operator, Value = namFilterData.Value });

                    var shakhsExpression = shakhsFilterInfo.TranslateFilter<UserInfo>();
                    shakhsQuery = shakhsQuery.Where(shakhsExpression);
                }

                string namKhanevadegiPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.LastName);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == namKhanevadegiPropName))
                {
                    FilterData namKhanevadegiFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == namKhanevadegiPropName);

                    string NamKhanevadegiProp = Entity.GetPropertyName<UserInfo>(shakhs => shakhs.LastName);
                    shakhsFilterInfo.Filters.Add(new FilterData() { Field = NamKhanevadegiProp, Operator = namKhanevadegiFilterData.Operator, Value = namKhanevadegiFilterData.Value });

                    var shakhsExpression = shakhsFilterInfo.TranslateFilter<UserInfo>();
                    shakhsQuery = shakhsQuery.Where(shakhsExpression);
                }

                string codeMeliPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.NationalCode);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == codeMeliPropName))
                {
                    FilterData codeMeliFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == codeMeliPropName);

                    string CodeMeliProp = Entity.GetPropertyName<UserInfo>(shakhs => shakhs.NationalCode);
                    shakhsFilterInfo.Filters.Add(new FilterData() { Field = CodeMeliProp, Operator = codeMeliFilterData.Operator, Value = codeMeliFilterData.Value });

                    var shakhsExpression = shakhsFilterInfo.TranslateFilter<UserInfo>();
                    shakhsQuery = shakhsQuery.Where(shakhsExpression);
                }

                string telHamrahPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.MobileNo);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == telHamrahPropName))
                {
                    FilterData telHamrahFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == telHamrahPropName);

                    string TelHamrahProp = Entity.GetPropertyName<UserInfo>(shakhs => shakhs.MobileNo);
                    shakhsFilterInfo.Filters.Add(new FilterData() { Field = TelHamrahProp, Operator = telHamrahFilterData.Operator, Value = telHamrahFilterData.Value });

                    var shakhsExpression = shakhsFilterInfo.TranslateFilter<UserInfo>();
                    shakhsQuery = shakhsQuery.Where(shakhsExpression);
                }

                string onvanOrganizationPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.OnvanOrganization);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == onvanOrganizationPropName))
                {
                    FilterData onvanFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == onvanOrganizationPropName);

                    string onvanProp = Entity.GetPropertyName<Organization>(organization => organization.Title);
                    organizationFilterInfo.Filters.Add(new FilterData() { Field = onvanProp, Operator = onvanFilterData.Operator, Value = onvanFilterData.Value });

                    var organizationExpression = organizationFilterInfo.TranslateFilter<Organization>();
                    organizationQuery = organizationQuery.Where(organizationExpression);
                }

                string onvanPositionPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.OnvanPosition);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == onvanPositionPropName))
                {
                    FilterData onvanFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == onvanPositionPropName);

                    string onvanProp = Entity.GetPropertyName<Position>(position => position.Name);
                    positionFilterInfo.Filters.Add(new FilterData() { Field = onvanProp, Operator = onvanFilterData.Operator, Value = onvanFilterData.Value });

                    var positionExpression = positionFilterInfo.TranslateFilter<Position>();
                    positionQuery = positionQuery.Where(positionExpression);
                }

                string taTarikhPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.TaTarikh);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == taTarikhPropName))
                {
                    FilterData taTarikhFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == taTarikhPropName);

                    string taTarikhProp = Entity.GetPropertyName<Personel>(Personel => Personel.TaTarikh);
                    personelFilterInfo.Filters.Add(new FilterData() { Field = taTarikhPropName, Operator = taTarikhFilterData.Operator, Value = taTarikhFilterData.Value });

                    var personelExpression = personelFilterInfo.TranslateFilter<Personel>();
                    query = query.Where(personelExpression);
                }
                string azTarikhPropName = Entity.GetPropertyName<PersonelVM>(personel => personel.AzTarikh);
                if (searchRequestInfo.Filter.Filters.Any(filter => filter.Field == azTarikhPropName))
                {
                    FilterData azTarikhFilterData = searchRequestInfo.Filter.Filters.FirstOrDefault(filter => filter.Field == taTarikhPropName);

                    string azTarikhProp = Entity.GetPropertyName<Personel>(Personel => Personel.AzTarikh);
                    personelFilterInfo.Filters.Add(new FilterData() { Field = taTarikhPropName, Operator = azTarikhFilterData.Operator, Value = azTarikhFilterData.Value });

                    var personelExpression = personelFilterInfo.TranslateFilter<Personel>();
                    query = query.Where(personelExpression);
                }

            }

            #endregion Query Processing

            IQueryable<PersonelVM> joinQuery = from personel in query
                                               join shakhs in shakhsQuery
                                               on personel.UserID equals shakhs.ID
                                               join organization in organizationQuery
                                               on personel.OrganizationId equals organization.ID
                                               join position in positionQuery
                                               on personel.RoleID equals position.ID
                                               select new PersonelVM
                                               {
                                                   ID = personel.ID,
                                                   FirstName = shakhs.FirstName,
                                                   LastName = shakhs.LastName,
                                                   NationalCode = shakhs.NationalCode,
                                                   MobileNo = shakhs.MobileNo,
                                                   OnvanOrganization = organization.Title,
                                                   OnvanPosition = position.Name,
                                                   AzTarikh = personel.AzTarikh,
                                                   DarHalKhedmat = personel.DarHalKhedmat,
                                                   TaTarikh = personel.TaTarikh,
                                                   RoleID = personel.RoleID,
                                                   OrganizationId = personel.OrganizationId
                                               };

            return joinQuery;
        }


        [Route("GetAllByOrganId")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                IQueryable<Personel> query = BusinessContext.GetBusinessRule<Personel>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();
                IQueryable<UserInfo> shakhsQuery = BusinessContext.GetBusinessRule<UserInfo>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();
               // IQueryable<Organization> organizationQuery = BusinessContext.GetBusinessRule<Organization>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();
                IQueryable<Role> positionQuery = BusinessContext.GetBusinessRule<Role>(Rule.OperationAccess, Rule.UnitOfWork).Queryable();


                IQueryable<PersonelVM> joinQuery = from personel in query
                                                   join shakhs in shakhsQuery
                                                   on personel.UserID equals shakhs.ID
                                                   //join organization in organizationQuery
                                                   //on personel.OrganizationId equals organization.ID
                                                   join position in positionQuery
                                                   on personel.RoleID equals position.ID
                                                   select new PersonelVM
                                                   {
                                                       ID = personel.ID,
                                                       FirstName = shakhs.FirstName,
                                                       LastName = shakhs.LastName,
                                                       NationalCode = shakhs.NationalCode,
                                                       MobileNo = shakhs.MobileNo,
                                                       //OnvanOrganization = organization.Title,
                                                       OnvanPosition = position.Name,
                                                       AzTarikh = personel.AzTarikh,
                                                       DarHalKhedmat = personel.DarHalKhedmat,
                                                       TaTarikh = personel.TaTarikh,
                                                       RoleID = personel.RoleID,
                                                       OrganizationId = personel.OrganizationId
                                                   };

            
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = joinQuery });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        protected override PersonelVM PrepareResponseAfterGetByID(PersonelVM entityVm)
        {
            //entityVm.OrganizationUnitChartId = Rule.UnitOfWork
            //                                       .RepositoryAsync<Position>()
            //                                      .Queryable()
            //                                      .Where(pos => pos.ID == entityVm.PositionId)
            //                                      .Select(pos => pos.OrganizationUnitChartId)
            //                                      .SingleOrDefault();

            //UserInfoRule userInfoRule = new VBSUserInfoRule(Rule.OperationAccess, Rule.UnitOfWork);
            //var userInfo = userInfoRule.Queryable()
            //                           .Where(u => u.PersonnelID == entityVm.ID)
            //                           .Select(u => new { u.UserName, u.Tag5 })
            //                           .SingleOrDefault();

            //if (userInfo != null)
            //{
            //    entityVm.NeedToSaveUser = true;
            //    entityVm.UserName = userInfo.UserName;
            //    entityVm.NeedToSign = userInfo.Tag5 == null ? 2 : userInfo.Tag5.ConvertTo<int>();
            //}

            return entityVm;
        }

        //[HttpPost]
        //public async Task<HttpResponseMessage> ChangeStatusAsync([FromBody]int personnelId)
        //{
        //    try
        //    {
        //        SecurityManager.ThrowIfUserContextNull();
        //        if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
        //            !SecurityManager.CurrentUserContext.IsAdminUser() && !SecurityManager.CurrentUserContext.IsSupplyChainUser())
        //            throw new Exception(VBSConstants.IlegalAccess);

        //        bool canUpdate = await SecurityManager.HasAccessAsync(ResourceIds.VBS_BaseInfo_Personel_ChangeStatus);
        //        if (!canUpdate)
        //            throw new OperationAccessException(ResourceIds.VBS_BaseInfo_Personel_ChangeStatus);

        //        Personel personnel = await Rule.FindAsync(personnelId);
        //        if (personnel == null)
        //            throw new BPJValidationException("کارمندی یافت نشد");

        //        personnel.DarHalKhedmat = !personnel.DarHalKhedmat;

        //        OperationAccess oprAccess = this.BusinessRule.OperationAccess.Clone() as OperationAccess;
        //        oprAccess.CanUpdate = canUpdate;
        //        Rule.OperationAccess = oprAccess;

        //        VBSUserInfoRule userInfoRule = new VBSUserInfoRule(Rule.OperationAccess, Rule.UnitOfWork);
        //        VBSUserInfo personnelUser = await userInfoRule.Queryable().SingleOrDefaultAsync(user => user.PersonnelID == personnelId);

        //        Rule.Update(personnel);
        //        if (personnelUser != null)
        //        {
        //            personnelUser.IsActive = personnel.DarHalKhedmat;
        //            personnelUser.NeedOTP = true;
        //            personnelUser.OTPCode = null;
        //            personnelUser.LastOTPDate = null;
        //            personnelUser.OTPTryNo = null;
        //            userInfoRule.Update(personnelUser);
        //        }
        //        await Rule.SaveChangesAsync();

        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = personnelId });
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!(ex is BPJValidationException || ex is OperationAccessException || ex is UserContextNullException))
        //        {
        //            var newEx = new Exception($"PersonnelID: {personnelId}, Message: {ex.Message}", ex); //when assign innerException with exception or ex, StackTrace and other data is set;
        //            return await this.HandleExceptionAsync(newEx);
        //        }
        //        else
        //            return await this.HandleExceptionAsync(ex);
        //    }
        //}

        //public async Task<HttpResponseMessage> GetChangeStatusOperationAccess()
        //{
        //    try
        //    {
        //        SecurityManager.ThrowIfUserContextNull();
        //        OperationAccess operationAccess = new OperationAccess { CanUpdate = await SecurityManager.HasAccessAsync(ResourceIds.VBS_BaseInfo_Personel_ChangeStatus) };
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = operationAccess });
        //    }
        //    catch (Exception ex)
        //    {
        //        return await this.HandleExceptionAsync(ex);
        //    }
        //}

        public async Task<HttpResponseMessage> SavePersonel(PersonelVM personelVM)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                //if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
                //    !SecurityManager.CurrentUserContext.IsAdminUser() && !SecurityManager.CurrentUserContext.IsSupplyChainUser())
                //    throw new Exception(VBSConstants.IlegalAccess);

                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                Personel personel = TranslateEntityVMToEntity(personelVM);

                //ShakhsRule shakhsRule = new ShakhsRule(Rule.OperationAccess, Rule.UnitOfWork);
                //int personelShakhsId = await shakhsRule.Queryable()
                //                                       .Where(sh => sh.CodeMeli == personel.Shakhs.CodeMeli)
                //                                       .Select(sh => sh.ID)
                //                                       .SingleOrDefaultAsync();

                //if (personelShakhsId > 0)
                //    personel.Shakhs.ID = personelShakhsId;

                //if (personel.Shakhs.ID > 0)
                //    personel.Shakhs.ObjectState = ObjectState.Modified;
                //else
                //    personel.Shakhs.ObjectState = ObjectState.Added;

                //چون کاربران سازمانی دسترسی اضافه نمودن کارمند دارند ولی دسترسی ویرایش کارمند ندارند، 
                //این کد باعث می شود در صورتیکه کارمند اضافه گردد ولی شخص آن به روز رسانی گردد ایجاد مشکل محدویت دسترسی نداشته باشد
                //if (personel.ID <= 0 && personel.Shakhs.ID > 0)
                //{
                //    OperationAccess oprAccess = Rule.OperationAccess.Clone() as OperationAccess;
                //    oprAccess.CanUpdate = Rule.OperationAccess.CanInsert;
                //    Rule.OperationAccess = oprAccess;
                //    shakhsRule.OperationAccess = oprAccess;
                //}

                //shakhsRule.InsertOrUpdateGraph(personel.Shakhs);
                //await shakhsRule.SaveChangesAsync();

                //personel.ShakhsId = personel.Shakhs.ID;

                personel.AzTarikh = DateTime.Now;// Utilities.PersianDateUtils.ToDateTime(personelVM.DisplayAzTarikh);
                personel.OrganizationId = organId;

                #region UserInfo
                ZhivarUserInfoRule userInfoRule = new ZhivarUserInfoRule(Rule.OperationAccess, Rule.UnitOfWork);
                UserInfo userInfo = await userInfoRule.Queryable()
                                                         .Where(user => user.ID == personel.UserID)
                                                         .SingleOrDefaultAsync();
                int oldPositionId = 0;
                if (personel.ID > 0)
                {
                    //Delete Old Position
                    oldPositionId = await Rule.Queryable()
                                              .Where(p => p.ID == personel.ID)
                                              .Select(p => p.RoleID)
                                              .SingleOrDefaultAsync();
                }

                if (userInfo != null)
                {
                    //if (personelVM.NeedToSaveUser)
                   // {
                        //userInfo.PersonnelID = personel.ID;
                        userInfo.OrganizationId = organId;
                        userInfo.FirstName = personelVM.FirstName;
                        userInfo.LastName = personelVM.LastName;
                        userInfo.MobileNo = personelVM.MobileNo;
                        userInfo.NationalCode = personelVM.NationalCode;
                        userInfo.Gender = personel.User.Gender ;
                        userInfo.Email = personel.User.Email;
                        userInfo.Tel = personel.User.Tel;
                        userInfo.UserName = personelVM.UserName;
                        userInfo.Tag5 = personelVM.NeedToSign.ToString();
                        userInfo.IsActive = personel.DarHalKhedmat;
                        userInfo.NeedOTP = true;
                        userInfo.OTPCode = null;
                        userInfo.LastOTPDate = null;
                        userInfo.OTPTryNo = null;
                        userInfo.IsDeleted = false;
                        userInfo.AuthenticationType = 3;

                    await userInfoRule.LoadCollectionAsync(userInfo, u => u.UserRoles);
                        if (userInfo.UserRoles == null)
                            userInfo.UserRoles = new List<UserRole>();

                        if (oldPositionId != 0 && oldPositionId != personel.RoleID)
                            foreach (var userRole in userInfo.UserRoles.Where(ur => ur.RoleId == oldPositionId))
                                userRole.ObjectState = ObjectState.Deleted;

                        if (!userInfo.UserRoles.Any(ur => ur.RoleId == personel.RoleID))
                            userInfo.UserRoles.Add(new UserRole() { ObjectState = ObjectState.Added, RoleId = personel.RoleID, UserId = userInfo.ID });
                    //}
                    //else
                    //{
                    //    userInfo.NeedOTP = true;
                    //    userInfo.OTPCode = null;
                    //    userInfo.LastOTPDate = null;
                    //    userInfo.OTPTryNo = null;
                    //    userInfo.IsActive = false;
                    //    userInfo.IsDeleted = true;
                    //}

                    userInfo.ObjectState = ObjectState.Modified;
                    userInfoRule.InsertOrUpdateGraph(userInfo);
                    await Rule.SaveChangesAsync();
                }
                else
                {
                    userInfo = new UserInfo()
                    {
                        ApplicationId = ConfigurationController.ApplicationID,
                        ObjectState = ObjectState.Added,
                        //PersonnelID = personel.ID,
                        OrganizationId = personel.OrganizationId,
                        //Tag1 = personel.ID.ToString(),
                        FirstName = personelVM.FirstName,
                        LastName = personelVM.LastName,
                        MobileNo = personelVM.MobileNo,
                        NationalCode = personelVM.NationalCode,
                        Gender = personelVM.Gender,
                        AuthenticationType = (int)ZhivarEnums.ZhivarUserType.Organization,
                        Email = personelVM.Email,
                        Password = CryptoHelper.ComputeHash(personelVM.Password),
                        PlainPassword = personelVM.Password,
                        Tel = personelVM.Tel,
                        UserName = personelVM.UserName,
                        NeedOTP = false,
                        OTPCode = null,
                        LastOTPDate = null,
                        OTPTryNo = null,
                        //Tag5 = personelVM.NeedToSign.ToString(),
                        IsActive = true,
                        IsDeleted = false,
                        LoginTryTime = 0,
                    };

                    userInfo.UserRoles = new List<UserRole>() { new UserRole() { ObjectState = ObjectState.Added, RoleId = personel.RoleID } };

                    userInfoRule.InsertOrUpdateGraph(userInfo);
                    await Rule.SaveChangesAsync();
                }
                #endregion

                personel.UserID = userInfo.ID;

                if (personel.ID > 0)
                {
                    Rule.Update(personel);
                }
                else
                    Rule.Insert(personel);

                await Rule.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = personel.ID });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        public async Task<HttpResponseMessage> GetUserInfoByCodeMeli([FromUri]string codeMeli)
        {
            try
            {
                //SecurityManager.ThrowIfUserContextNull();
                //if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
                //    !SecurityManager.CurrentUserContext.IsAdminUser() && !SecurityManager.CurrentUserContext.IsSupplyChainUser())
                //    throw new Exception(VBSConstants.IlegalAccess);

                ZhivarUserInfoRule userInfoRule = new ZhivarUserInfoRule(Rule.OperationAccess, Rule.UnitOfWork);
                bool status = await userInfoRule.Queryable().AnyAsync(user => user.NationalCode == codeMeli);
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = status });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }
        [Route("GetPositionByOrganizationId")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPositionByOrganizationId()
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                //if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
                //    !SecurityManager.CurrentUserContext.IsAdminUser() && !SecurityManager.CurrentUserContext.IsSupplyChainUser())
                //    throw new Exception(VBSConstants.IlegalAccess);

                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                IQueryable<Role> positionQuery = Rule.UnitOfWork
                                                         .RepositoryAsync<Role>()
                                                         .Queryable();

                List<KeyValueVM> result = await (from position in positionQuery
                                                 
                                                 select new KeyValueVM
                                                 {
                                                     Key = position.ID,
                                                     Value = position.Name
                                                 }).ToListAsync();

                var response = new
                {
                    Positions = result,
                    //Position = result.FirstOrDefault(ps => ps.Key == positionId)
                };

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = response });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        //public async Task<HttpResponseMessage> GetPositionByOrganizationUser()
        //{
        //    try
        //    {
        //        //SecurityManager.ThrowIfUserContextNull();
        //        //if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel())
        //        //    throw new Exception(VBSConstants.IlegalAccess);
        //        //TODO Shahmoradi
        //        IQueryable<Position> positionQuery = Rule.UnitOfWork
        //                                                 .RepositoryAsync<Position>()
        //                                                 .Queryable()
        //                                                 .Where(po => po.Code != VBSConstants.MonshiJalaseh &&
        //                                                              po.Code != VBSConstants.KarshenasMarkaz);

        //        IQueryable<Personel> personelQuery = Rule.UnitOfWork
        //                                                 .RepositoryAsync<Personel>()
        //                                                 .Queryable()
        //                                                 .Where(p => p.OrganizationId == SecurityManager.CurrentUserContext.OrganizationId);

        //        IQueryable<Shakhs> shakhsQuery = Rule.UnitOfWork.RepositoryAsync<Shakhs>().Queryable();

        //        List<KeyValueVM> response = await (from personel in personelQuery
        //                                           join position in positionQuery
        //                                           on personel.PositionId equals position.ID
        //                                           join shakhs in shakhsQuery
        //                                           on personel.ShakhsId equals shakhs.ID
        //                                           select new KeyValueVM
        //                                           {
        //                                               Key = shakhs.ID,
        //                                               Value = shakhs.Nam + " " + shakhs.NamKhanevadegi + " " + position.Name,
        //                                           })
        //                                           .ToListAsync();

        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { records = response } });
        //    }
        //    catch (Exception ex)
        //    {
        //        return await this.HandleExceptionAsync(ex);
        //    }
        //}

        [HttpPost]
        public async Task<HttpResponseMessage> LoadPersonel([FromBody]PersonelRequest request)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();
                //if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
                //    !SecurityManager.CurrentUserContext.IsAdminUser() && !SecurityManager.CurrentUserContext.IsSupplyChainUser())
                //    throw new Exception(VBSConstants.IlegalAccess);

                PersonelVM personelVM = new PersonelVM();
                Personel personel = await Rule.GetPersonelInfo(request);
                if (personel != null)
                {
                    personelVM = personel.Translate<PersonelVM>();
                    //if (personel.SignatureContent != null)
                    //    personelVM.FileSpaceBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(personel.SignatureContent));

                    ZhivarUserInfoRule userInfoRule = new ZhivarUserInfoRule(Rule.OperationAccess, Rule.UnitOfWork);
                    var userInfo = userInfoRule.Queryable()
                                               .Where(u => u.ID == personelVM.UserID)
                                               .Select(u => new { u.UserName, u.Tag5 })
                                               .SingleOrDefault();

                    if (userInfo != null)
                    {
                        personelVM.NeedToSaveUser = true;
                        personelVM.UserName = userInfo.UserName;
                        personelVM.NeedToSign = userInfo.Tag5 == null ? 2 : userInfo.Tag5.ConvertTo<int>();
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = personelVM });
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }

        [HttpPost]
        [Route("DeletePersonel")]
        public async Task<HttpResponseMessage> DeletePersonel([FromBody]List<Personel> items)
        {
            try
            {
                string failurs = "";

                foreach (var item in items)
                {
                    var itemFind = await Rule.FindAsync(item.ID);

                    if (itemFind != null)
                    {
                        using (var uow = new UnitOfWork())
                        {
                            var instanceQuery = uow.RepositoryAsync<WorkflowInstanceState>()
                                                        .Queryable()
                                                        .Where(ins => ins.UserId == itemFind.UserID);

                            if (instanceQuery.Any())
                            {
                                failurs += "<br/>" + " این شخص اقداماتی را در گردش کارانجام داده است امکان حذف وجود ندارد.";
                            }
                            else
                            {
                                await Rule.DeleteAsync(item.ID);
                                await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                            }
                        }

                
                  
                    }
                }
                if (!string.IsNullOrEmpty(failurs))
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = items });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "خطای به وجود آمده است." });
            }






        }
        //public async Task<HttpResponseMessage> GetMoveMessageOperationAccess()
        //{
        //    try
        //    {
        //        SecurityManager.ThrowIfUserContextNull();
        //        OperationAccess operationAccess = await VBSCartableHandler.GetMoveMessageOperationAccess();
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = operationAccess });
        //    }
        //    catch (Exception ex)
        //    {
        //        return await this.HandleExceptionAsync(ex);
        //    }
        //}

        //public async Task<HttpResponseMessage> GetAllUserIdsInSamePositionAsync([FromUri]int personnelId)
        //{
        //    try
        //    {
        //        SecurityManager.ThrowIfUserContextNull();
        //        if (!SecurityManager.CurrentUserContext.IsOrganizationPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
        //            !SecurityManager.CurrentUserContext.IsAdminUser() && !SecurityManager.CurrentUserContext.IsSupplyChainUser())
        //            throw new Exception(VBSConstants.IlegalAccess);
        //        List<PersonnelUserResponse> response = await Rule.GetAllUserIdsInSamePositionAsync(personnelId);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = response });
        //    }
        //    catch (Exception ex)
        //    {
        //        return await this.HandleExceptionAsync(ex);
        //    }
        //}

        //[HttpPost]
        //public async Task<HttpResponseMessage> PersonnelCartableMessageMoving([FromBody]PersonnelCartableMessageBusiClass request)
        //{
        //    try
        //    {
        //        SecurityManager.ThrowIfUserContextNull();
        //        if (!SecurityManager.CurrentUserContext.IsOrganizationITPersonnel() && !SecurityManager.CurrentUserContext.IsDeveloperUser() &&
        //            !SecurityManager.CurrentUserContext.IsAdminUser())
        //            throw new Exception(VBSConstants.IlegalAccess);
        //        await Rule.PersonnelCartableMessageMoving(request);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { } });
        //    }
        //    catch (Exception ex)
        //    {
        //        return await this.HandleExceptionAsync(ex);
        //    }
        //}

        //protected override PersonelVM TranslateEntityToCustomQueryEntity(Personel customQueryEntity)
        //{
        //    PersonelVM personelVM = base.TranslateEntityToCustomQueryEntity(customQueryEntity);

        //    personelVM.Shakhs = ShakhsHelper.TranslateShakhsToShakhsVM(customQueryEntity.Shakhs);
        //    personelVM.Shakhs.TahsilatShakhsVms = null;
        //    personelVM.Shakhs.AddressShakhsVms = null;
        //    if (customQueryEntity.SignatureContent != null)
        //    {
        //        personelVM.FileSpaceBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(customQueryEntity.SignatureContent));
        //    }
        //    //personelVM.UserName = SecurityManager.CurrentUserContext == null ? "" : SecurityManager.CurrentUserContext.UserName;
        //    return personelVM;
        //}

        //protected override Personel TranslateEntityVMToEntity(PersonelVM personelVM)
        //{
        //    Personel personel = base.TranslateEntityVMToEntity(personelVM);
        //    if (!string.IsNullOrEmpty(personelVM.FileSpaceBase64))
        //        personel.SignatureContent = Convert.FromBase64String(personelVM.FileSpaceBase64);
        //    personel.Shakhs = ShakhsHelper.TranslateShakhsVMToShakhs(personelVM.Shakhs);

        //    return personel;
        //}

        [Route("GetNewPersonelObject")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetNewPersonelObject()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);



            PersonelVM personelVM = new PersonelVM()
            {
                Darsd = 0,
                AzTarikh = DateTime.Now,
                
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = personelVM });
        }


        [Route("GetPersonelById")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetPersonelById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


            var personel = await Rule.FindAsync(id);

            UserInfo user = new UserInfo();
            UserRole userRole = new UserRole();

            if (personel != null)
            {
                user = await this.Rule.UnitOfWork.RepositoryAsync<UserInfo>().Queryable().Where(x => x.ID == personel.UserID).FirstOrDefaultAsync();
                userRole = await this.Rule.UnitOfWork.RepositoryAsync<UserRole>().Queryable().Where(x => x.UserId == personel.UserID).FirstOrDefaultAsync();
            }

            PersonelVM personelVM = new PersonelVM()
            {
               AzTarikh = personel.AzTarikh,
               DarHalKhedmat = personel.DarHalKhedmat,
               Darsd = personel.Darsd,
               DisplayAzTarikh = PersianDateUtils.ToPersianDateTime(personel.AzTarikh),
               Email = user.Email,
               OrganizationId = organId,
               UserID = user.ID,
               FirstName = user.FirstName,
               LastName = user.LastName,
               MobileNo = user.MobileNo,
               NationalCode = user.NationalCode,
               NoeMozd = personel.NoeMozd,
               Password = user.Password,
               RoleID = userRole.RoleId,
               Salary = personel.Salary,
               TaTarikh = personel.TaTarikh,
               Tel = user.Tel,
               UserName = user.UserName,
               PlainPassword = user.PlainPassword,
               Gender = user.Gender,
               ID = personel.ID,
               
            };



            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = personelVM });
        }
    }
}
