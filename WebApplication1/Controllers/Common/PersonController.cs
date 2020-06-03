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

//    // [RouteArea("BaseInfo", AreaPrefix = "BaseInfo-GorohePerson")]
//    public partial class PersonController : Controller
//    {

//        private readonly IBank bankRule;
//        private readonly IApplicationRoleManager _roleService;
//        private readonly IApplicationUserManager _userService;
//        public PersonController(IBank bankService, IApplicationRoleManager roleService, IApplicationUserManager userService)
//        {

//            bankRule = bankService;
//            _roleService = roleService;
//            _userService = userService;

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
//                   this.Request.Url.ParseQueryString().GetKey(0)
//                );

//            var list = await personRule.GetAllAsync();
//            var list2 = list.Select(x => new { ID = x.ID, Title = x.Nam + " " + x.NamKhanvadegi, Vaziat = x.Vaziat }).ToList();


//            return Json(list2.AsQueryable()
//                       .ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter),
//                       JsonRequestBehavior.AllowGet);
//        }

//        [Route("GetAllByPersonId")]
//        public virtual async Task<ActionResult> GetAllByPersonId()
//        {
//            //var userId = SecurityManager.CurrentUserContext.UserId;
//            //var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            var request = JsonConvert.DeserializeObject<DataSourceRequest>(
//                this.Request.Url.ParseQueryString().GetKey(0)
//             );

//            var list = await personRule.GetAllAsync();
//            var list2 = list.Select(x => new { ID = x.ID, Title = x.Nam + " " + x.NamKhanvadegi, Vaziat = x.Vaziat }).ToList();


//            return Json(list2.AsQueryable()
//                       .ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter),
//                       JsonRequestBehavior.AllowGet);
//        }

//        [Route("Add")]
//        public virtual async Task<ActionResult> AddPerson()
//        {
//            var personVM = new PersonVM();


//            var userId = SecurityManager.CurrentUserContext.UserId;
//            var organ = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            //  Mapper.Map(person, personVM);
//            ViewData["NoeShakhs"] = new SelectList(EnumHelper.GetKeyValues<Enums.NoeShakhs>(), "Key", "Value");
//            ViewData["TypeHoghoghi"] = new SelectList(EnumHelper.GetKeyValues<Enums.TypeHoghoghi>(), "Key", "Value");



//            return View(personVM);
//        }

//        [Route("Add")]
//        [HttpPost]
//        public virtual async Task<ActionResult> AddPerson(PersonVM personVM)
//        {
//            var userId = SecurityManager.CurrentUserContext.UserId;
//           // var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            ValidatePerson(personVM);

//            if (!ModelState.IsValid)
//            {

//                ViewData["NoeShakhs"] = new SelectList(EnumHelper.GetKeyValues<Enums.NoeShakhs>(), "Key", "Value");
//                ViewData["TypeHoghoghi"] = new SelectList(EnumHelper.GetKeyValues<Enums.TypeHoghoghi>(), "Key", "Value");
//                return View(personVM);
//            }


//            var person = new Person();


//            Mapper.Map(personVM, person);
    

           

//            if (personVM.ID.HasValue)
//            {

//                //person.ModifiedBy = User.Identity.Name;
//                //person.ModifiedOn = DateTime.Now;
//                // person.PersonID = person.Person.ID;
//                personRule.Update(person);

//                TempData["message"] = "شخص مورد نظر با موفقیت ویرایش شد";
//            }
//            else
//            {

//                // person.CreatedBy = User.Identity.Name;
//                // person.CreatedOn = DateTime.Now;

//                personRule.Insert(person);

//                TempData["message"] = "شخص مورد نظر با موفقیت ثبت شد";
//            }

//            await _unitOfWork.SaveAllChangesAsync();

//            return RedirectToAction(MVC.Person.ActionNames.Index);
//        }
//        [Route("Edit/{id:int?}")]
//        public virtual async Task<ActionResult> EditPerson(int id)
//        {
//           // var userId = SecurityManager.CurrentUserContext.UserId;
//           // var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

//            Person person = await personRule.GetByIdAsync(id);
//            PersonVM personVM = new PersonVM();
//            Mapper.Map(person, personVM);

//            ViewData["NoeShakhs"] = new SelectList(EnumHelper.GetKeyValues<Enums.NoeShakhs>(), "Key", "Value");
//            ViewData["TypeHoghoghi"] = new SelectList(EnumHelper.GetKeyValues<Enums.TypeHoghoghi>(), "Key", "Value");

//            // Mapper.Map(person, personVM);




//            return View(MVC.Person.Views.AddPerson, personVM);
//        }
//        //  [Route("Post")]
//        [HttpPost]
//        public virtual async Task<ActionResult> Post(PersonVM personVM)
//        {
//            if (!ModelState.IsValid)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            var person = new Person();

//            Mapper.Map(personVM, person);

//            //  Mapper.Map(gorohePersonVM, gorohePerson);

//            personRule.Insert(person);

//            await _unitOfWork.SaveAllChangesAsync();

//            // گرید آی دی جدید را به این صورت دریافت می‌کند
//            return Json(new DataSourceResult { Data = new[] { person } });





//        }
//        [HttpPut]
//        // [Route("Update/{id:int}")]
//        public virtual async Task<ActionResult> Update(int id, Person person)
//        {
//            var item = await personRule.GetByIdAsync(id);

//            if (item == null)
//                return new HttpNotFoundResult();


//            if (!ModelState.IsValid || id != item.ID)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            //  var gorohePerson = new GorohePerson();
//            //  Mapper.Map(gorohePersonVM, gorohePerson);

//            personRule.Update(person);

//            await _unitOfWork.SaveAllChangesAsync();

//            //Return HttpStatusCode.OK
//            return new HttpStatusCodeResult(HttpStatusCode.OK);
//        }


//        [HttpDelete]
//        public virtual async Task<ActionResult> DeletePerson(int id)
//        {

//            var item = personRule.GetById(id);

//            if (item == null)
//                return new HttpNotFoundResult();

//            personRule.Delete(id);

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