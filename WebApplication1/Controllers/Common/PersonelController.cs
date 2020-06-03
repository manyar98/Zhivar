//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using AutoMapper;
//using Zhivar.DataLayer.Context;
//using Zhivar.DomainClasses;
//using Zhivar.ServiceLayer.BaseInfo;
//using Zhivar.ServiceLayer.Contracts.BaseInfo;
//using System.Net;
//using Zhivar.DomainClasses.BaseInfo;
//using Zhivar.ViewModel.BaseInfo;
//using System.Net.Http;
//using Newtonsoft.Json;
//using Kendo.DynamicLinq;
//using Microsoft.AspNet.Identity;
//using Zhivar.ServiceLayer.Contracts.Common;
//using Zhivar.ServiceLayer.Contracts.Accunting;
//using Zhivar.ViewModel.Accunting;
//using Zhivar.DomainClasses.Accunting;
//using Zhivar.Utilities;
//using Zhivar.DomainClasses.Common;
//using Zhivar.ViewModel.Common;
//using Zhivar.ServiceLayer.Contracts.Account;
//using Zhivar.DomainClasses.Account;

//namespace Zhivar.Web.Controllers.Common
//{

//    // [Authorize(Roles = "Admin")]

//    // [RouteArea("BaseInfo", AreaPrefix = "BaseInfo-GorohePersonel")]
//    public partial class PersonelController : Controller
//    {
//        private readonly IMappingEngine Mapper;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IPersonel _personelService;
//        private readonly IPerson personRule;
//        private readonly IBank bankRule;
//        private readonly IApplicationRoleManager _roleService;
//        private readonly IApplicationUserManager _userService;
//        public PersonelController(IUnitOfWork unitOfWork, IPersonel personelService, IPerson personService, IBank bankService, IApplicationRoleManager roleService, IApplicationUserManager userService, IMappingEngine mappingEngine)
//        {
//            _unitOfWork = unitOfWork;
//            _personelService = personelService;
//            personRule = personService;
//            bankRule = bankService;
//            _roleService = roleService;
//            _userService = userService;
//            Mapper = mappingEngine;
//        }
//        [Route("Index")]
//        public virtual ActionResult Index()
//        {
//            return View();
//        }
//        [Route("GetAll")]
//        public virtual async Task<ActionResult> GetAll()
//        {

//            var request = JsonConvert.DeserializeObject<DataSourceRequest>(
//                this.Request.Url.ParseQueryString().GetKey(0)
//             );

//            var list = await _personelService.GetAllAsync();
//            return Json(list.AsQueryable()
//                       .ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter),
//                       JsonRequestBehavior.AllowGet);
//        }

//        [Route("GetAllByPersonId")]
//        public virtual async Task<ActionResult> GetAllByPersonId()
//        {
//            var userId = SecurityManager.CurrentUserContext.UserId;
//            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            var request = JsonConvert.DeserializeObject<DataSourceRequest>(
//                this.Request.Url.ParseQueryString().GetKey(0)
//             );

//            var list = await _personelService.GetAllByPersonIdAsync(Convert.ToInt32(person.ID));
//            var list2 = list.Select(x => new { ID = x.ID, Title = x.PersonVM.Nam + " " + x.PersonVM.NamKhanvadegi,RoleName = x.RoleName }).ToList();


//            return Json(list2.AsQueryable()
//                       .ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter),
//                       JsonRequestBehavior.AllowGet);
//        }

//        [Route("Add")]
//        public virtual async Task<ActionResult> AddPersonel()
//        {
//            var personelVM = new PersonelVM();

//            personelVM.PersonVM = new PersonVM();
//            personelVM.PersonVM.NoeShakhs = Enums.NoeShakhs.Haghighi;

//            var userId = SecurityManager.CurrentUserContext.UserId;
//            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            ViewData["Roles"] = new SelectList(await _roleService.GetAllCustomRolesAsync(Enums.TypeRole.Public), "Id", "PersianName");


//            return View(personelVM);
//        }

//        [Route("Add")]
//        [HttpPost]
//        public virtual async Task<ActionResult> AddPersonel(PersonelVM personelVM)
//        {
//            var userId = SecurityManager.CurrentUserContext.UserId;
//            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            ValidatePerson(personelVM.PersonVM);

//            if (!ModelState.IsValid)
//            {

//                ViewData["Roles"] = new SelectList(await _roleService.GetAllCustomRolesAsync(Enums.TypeRole.Public), "Id", "PersianName");

//                return View(personelVM);
//            }


//            var personel = new Personel();


//            Mapper.Map(personelVM, personel);
//            personel.OrganID = person.ID;


//            var user = await _userService.FindByNameAsync(personelVM.UserName);

//            if (user == null)
//            {
//                user = new ApplicationUser
//                {
//                    UserName = personelVM.UserName,
//                    Email = personelVM.UserName + "@smbt.ir",
//                    EmailConfirmed = true,
//                    Tag1Int = person.ID
//                };

//                var result = await _userService.CreateAsync(user, personelVM.Password);
//                result = await _userService.SetLockoutEnabledAsync(user.Id, false);
//                if (!result.Succeeded)
//                {
//                    ViewData["Roles"] = new SelectList(await _roleService.GetAllCustomRolesAsync(Enums.TypeRole.Public), "Id", "PersianName");

//                    return View(personelVM);
//                }
//            }

//            var role = await _roleService.FindByIdAsync(personelVM.RoleID);

//            var rolesForUser = await _userService.GetRolesAsync(user.Id);

//            string[] array = new string[rolesForUser.Count];
//            rolesForUser.CopyTo(array, 0);

//            await _userService.RemoveFromRolesAsync(user.Id, array);

//            if (!rolesForUser.Contains(role.Name))
//            {
//                var result = await _userService.AddToRoleAsync(user.Id, role.Name);
//                if (!result.Succeeded)
//                {
//                    ViewData["Roles"] = new SelectList(await _roleService.GetAllCustomRolesAsync(Enums.TypeRole.Public), "Id", "PersianName");

//                    return View(personelVM);
//                }

//            }

//            personel.UserID = user.Id;

//            if (personelVM.ID.HasValue)
//            {

//                //personel.ModifiedBy = User.Identity.Name;
//                //personel.ModifiedOn = DateTime.Now;
//                // personel.PersonID = personel.Person.ID;
//                _personelService.Update(personel);

//                TempData["message"] = "کارمند مورد نظر با موفقیت ویرایش شد";
//            }
//            else
//            {

//                // personel.CreatedBy = User.Identity.Name;
//                // personel.CreatedOn = DateTime.Now;

//                _personelService.Insert(personel);

//                TempData["message"] = "کارمند مورد نظر با موفقیت ثبت شد";
//            }

//            await _unitOfWork.SaveAllChangesAsync();

//            return RedirectToAction(MVC.Personel.ActionNames.Index);
//        }
//        [Route("Edit/{id:int?}")]
//        public virtual async Task<ActionResult> EditPersonel(int id)
//        {
//            var userId = SecurityManager.CurrentUserContext.UserId;
//            var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            PersonelVM personelVM = await _personelService.GetForUpdateAsync(id);
//            //PersonelVM personelVM = new PersonelVM();


//            ViewData["Roles"] = new SelectList(await _roleService.GetAllCustomRolesAsync(Enums.TypeRole.Public), "Id", "PersianName");


//           // Mapper.Map(personel, personelVM);




//            return View(MVC.Personel.Views.AddPersonel, personelVM);
//        }
//        //  [Route("Post")]
//        [HttpPost]
//        public virtual async Task<ActionResult> Post(PersonelVM personelVM)
//        {
//            if (!ModelState.IsValid)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            var personel = new Personel();

//            Mapper.Map(personelVM, personel);

//            //  Mapper.Map(gorohePersonelVM, gorohePersonel);

//            _personelService.Insert(personel);

//            await _unitOfWork.SaveAllChangesAsync();

//            // گرید آی دی جدید را به این صورت دریافت می‌کند
//            return Json(new DataSourceResult { Data = new[] { personel } });





//        }
//        [HttpPut]
//        // [Route("Update/{id:int}")]
//        public virtual async Task<ActionResult> Update(int id, Personel personel)
//        {
//            var item = await _personelService.GetByIdAsync(id);

//            if (item == null)
//                return new HttpNotFoundResult();


//            if (!ModelState.IsValid || id != item.ID)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            //  var gorohePersonel = new GorohePersonel();
//            //  Mapper.Map(gorohePersonelVM, gorohePersonel);

//            _personelService.Update(personel);

//            await _unitOfWork.SaveAllChangesAsync();

//            //Return HttpStatusCode.OK
//            return new HttpStatusCodeResult(HttpStatusCode.OK);
//        }


//        [HttpDelete]
//        public virtual async Task<ActionResult> DeletePersonel(int id)
//        {

//            var item = _personelService.GetById(id);

//            if (item == null)
//                return new HttpNotFoundResult();

//            _personelService.Delete(id);

//            await _unitOfWork.SaveAllChangesAsync();

//            return Json(item);

//        }

//        private void ValidatePerson(PersonVM userViewModel)
//        {


//            switch (userViewModel.NoeShakhs)
//            {
//                case Enums.NoeShakhs.Haghighi:
//                    {

//                        if (string.IsNullOrEmpty(userViewModel.NamKhanvadegi))
//                        {
//                            ModelState.AddModelError("", "لطفا نام خانوادگی را وارد کنید.");
//                        }
//                        if (string.IsNullOrEmpty(userViewModel.CodeMeli))
//                        {
//                            ModelState.AddModelError("", "لطفا کد ملی را وارد کنید.");
//                        }
//                    }
//                    break;
//                case Enums.NoeShakhs.Hoghoghi:
//                    {
//                        if (userViewModel.TypeHoghoghi == null)
//                        {
//                            ModelState.AddModelError("", "لطفا نوع شخص حقوقی را انتخاب کنید.");
//                        }

//                        switch (userViewModel.TypeHoghoghi)
//                        {
//                            case Enums.TypeHoghoghi.Sherkat:
//                                {
//                                    if (string.IsNullOrEmpty(userViewModel.CodeMeli))
//                                    {
//                                        ModelState.AddModelError("", "لطفا شناسه ملی را وارد کنید.");
//                                    }
//                                    if (string.IsNullOrEmpty(userViewModel.CodeEghtesadi))
//                                    {
//                                        ModelState.AddModelError("", "لطفا کد اقتصادی را وارد کنید.");
//                                    }
//                                    if (string.IsNullOrEmpty(userViewModel.ShomareSabt))
//                                    {
//                                        ModelState.AddModelError("", "لطفا شماره ثبت را وارد کنید.");
//                                    }
//                                }
//                                break;
//                            case Enums.TypeHoghoghi.Edare:
//                                break;
//                            case Enums.TypeHoghoghi.Kanon:
//                                {
//                                    if (userViewModel.TarikhSoudor == null)
//                                    {
//                                        ModelState.AddModelError("", "لطفا تاریخ صدور پروانه را وارد کنید.");
//                                    }
//                                    if (string.IsNullOrEmpty(userViewModel.ModateEtebar))
//                                    {
//                                        ModelState.AddModelError("", "لطفا مدت اعتبار پروانه را وارد کنید.");
//                                    }
//                                    if (string.IsNullOrEmpty(userViewModel.SahebEmtiaz))
//                                    {
//                                        ModelState.AddModelError("", "لطفا نام و نام خانوادگی صاحب امتیاز را وارد کنید.");
//                                    }

//                                }
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                    break;
//                default:
//                    break;
//            }

//        }
//    }
//}