using AutoMapper;
using OMF.Business;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.Business.Accounting;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.Common;

namespace Zhivar.Web.Controllers.Common
{
    [RoutePrefix("api/Contact")]
    public class ContactController : NewApiControllerBaseAsync<Contact, ContactVM>
    {
        public ContactRule Rule => this.BusinessRule as ContactRule;

        protected override IBusinessRuleBaseAsync<Contact> CreateBusinessRule()
        {
            return new ContactRule();
        }

        //[Route("GetAllByOrganId")]
        //[HttpPost]
        //public  async Task<HttpResponseMessage> GetAllByOrganId()
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    var list = await contactRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
        //    // var list2 = list.Select(x => new { ID = x.ID, Title = x.Name, Code = x.Code }).ToList();

        //    var accounts = await accountRule.GetAllByOrganIdAsync(organId);


        //    List<ContactVM> contactVMs = new List<ContactVM>();
        //    var contactVM = new ContactVM();

        //    foreach (var item in list)
        //    {


        //        contactVM = new ContactVM();

        //        contactVM.Code = item.Code;
        //        contactVM.DetailAccount = new DetailAccount()
        //        {
        //            Id = Convert.ToInt32(item.Code),
        //            Code = item.Code,
        //            Node = new Node()
        //            {
        //                FamilyTree = "اشخاص",
        //                Id = Convert.ToInt32(item.Code),
        //                Name = "اشخاص",
        //            }
        //        };

        //        contactVM.Address = item.Address;
        //        contactVM.City = item.City;
        //        contactVM.Credits = item.Credits;
        //        contactVM.EconomicCode = item.EconomicCode;
        //        contactVM.Email = item.Email;
        //        contactVM.Fax = item.Fax;
        //        contactVM.ID = item.ID;
        //        contactVM.IsCustomer = item.IsCustomer;
        //        contactVM.IsEmployee = item.IsEmployee;
        //        contactVM.IsShareHolder = item.IsShareHolder;
        //        contactVM.IsVendor = item.IsVendor;
        //        contactVM.Liability = item.Liability;
        //        contactVM.Mobile = item.Mobile;
        //        contactVM.Name = item.Name;
        //        contactVM.NationalCode = item.NationalCode;
        //        contactVM.Note = item.Note;
        //        contactVM.OrganId = item.OrganId;
        //        contactVM.Phone = item.Phone;
        //        contactVM.PostalCode = item.PostalCode;
        //        contactVM.Rating = item.Rating;
        //        contactVM.RegistrationDate = item.RegistrationDate;
        //        contactVM.RegistrationNumber = item.RegistrationNumber;
        //        contactVM.SharePercent = Convert.ToDecimal(item.SharePercent);
        //        contactVM.State = item.State;
        //        contactVM.Website = item.Website;
        //        contactVM.FirstName = item.FirstName;
        //        contactVM.LastName = item.LastName;
        //        contactVMs.Add(contactVM);
        //    }



        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = contactVMs });

        //}


        [Route("GetAllByOrganId")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetAllByOrganId([FromBody] string type)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                // var list2 = list.Select(x => new { ID = x.ID, Title = x.Name, Code = x.Code }).ToList();

                AccountRule accountRule = new AccountRule();
                var accounts = await accountRule.GetAllByOrganIdAsync(organId);


                List<ContactVM> contactVMs = new List<ContactVM>();
                var contactVM = new ContactVM();

                var detailAccount = new DomainClasses.Accounting.Account();
                foreach (var item in list)
                {
                    switch (type)
                    {
                        case "debtors":
                            {
                                detailAccount = accounts.Where(x => x.ComplteCoding == "1104" + item.Code).SingleOrDefault();

                              

                                break;
                            }
                        case "payables":
                        case "inProgress":
                            {
                                detailAccount = accounts.Where(x => x.ComplteCoding == "2102" + item.Code).SingleOrDefault();

                                if (detailAccount == null)
                                {
                                    DomainClasses.Accounting.Account account = new DomainClasses.Accounting.Account()
                                    {
                                        Coding = item.Code,
                                        Level = ZhivarEnums.AccountType.Moen,
                                        ObjectState = OMF.Common.Enums.ObjectState.Added,
                                        Name = item.Name,
                                        ComplteCoding ="2102" + item.Code,
                                        OrganId = organId,
                                        ParentId = 1525
                            
                                    };

                                    accountRule.Insert(account);
                                    await accountRule.SaveChangesAsync();

                                    accounts = await accountRule.GetAllByOrganIdAsync(organId);

                                    detailAccount = accounts.Where(x => x.ComplteCoding == "2102" + item.Code).SingleOrDefault();
                                }
                               

                                break;
                            }
                        case "receivables":
                            {
                                detailAccount = accounts.Where(x => x.ComplteCoding == "1105" + item.Code).SingleOrDefault();
                                break;
                            }
                        case "creditors":
                            {
                                detailAccount = accounts.Where(x => x.ComplteCoding == "2101" + item.Code).SingleOrDefault();
                                break;
                            }


                        default:
                            {
                                detailAccount = accounts.Where(x => x.ComplteCoding == "1104" + item.Code).SingleOrDefault();
                                break;
                            }
                            // break;
                    }

                    contactVM = new ContactVM();

                    contactVM.Code = item.Code;
                    if (detailAccount != null)
                    {
                        contactVM.DetailAccount = new DetailAccount()
                        {
                            Id = detailAccount.ID,
                            Code = detailAccount.Coding,
                            Name = detailAccount.Name,

                            Node = new Node()
                            {
                                FamilyTree = "اشخاص",
                                Id = Convert.ToInt32(item.Code),
                                Name = "اشخاص",
                            }
                        };
                    }

                    contactVM.Address = item.Address;
                    contactVM.City = item.City;
                    contactVM.Credits = item.Credits;
                    contactVM.EconomicCode = item.EconomicCode;
                    contactVM.Email = item.Email;
                    contactVM.Fax = item.Fax;
                    contactVM.ID = item.ID;
                    contactVM.IsCustomer = item.IsCustomer;
                    contactVM.IsEmployee = item.IsEmployee;
                    contactVM.IsShareHolder = item.IsShareHolder;
                    contactVM.IsVendor = item.IsVendor;
                    contactVM.Liability = item.Liability;
                    contactVM.Mobile = item.Mobile;
                    contactVM.Name = item.Name;
                    contactVM.NationalCode = item.NationalCode;
                    contactVM.Note = item.Note;
                    contactVM.OrganId = item.OrganId;
                    contactVM.Phone = item.Phone;
                    contactVM.PostalCode = item.PostalCode;
                    contactVM.Rating = item.Rating;
                    contactVM.RegistrationDate = item.RegistrationDate;
                    contactVM.RegistrationNumber = item.RegistrationNumber;
                    contactVM.SharePercent = Convert.ToDecimal(item.SharePercent);
                    contactVM.State = item.State;
                    contactVM.Website = item.Website;
                    contactVM.FirstName = item.FirstName;
                    contactVM.LastName = item.LastName;
                    contactVMs.Add(contactVM);
                }



                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contactVMs });

            }
            catch (Exception ex)
            {

                throw;
            }
         
        }


        [Route("AddContact")]
        [HttpPost]
        public  async Task<HttpResponseMessage> AddContact([FromBody] ContactVM contactVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

          if(string.IsNullOrEmpty( contactVM.Name))
                throw new OMFValidationException("نام شخص را وارد نمایید.");

            var contact = new Contact();

            if (contactVM.TasvirBlobBase64 != null)
            {
                if (!string.IsNullOrWhiteSpace(contactVM.TasvirBlobBase64) && !string.IsNullOrEmpty(contactVM.TasvirBlobBase64))
                {
                    contactVM.TasvirBlobBase64 = contactVM.TasvirBlobBase64.Replace("data:image/jpeg;base64,", "");
                    contactVM.Blob = Convert.FromBase64String(contactVM.TasvirBlobBase64);
                }
            }

            contact = Utilities.TranslateHelper.TranslateContactVMToContact(contactVM);



     

            contact.RegistrationDate = DateTime.Now;

            contact.OrganId = organId;

            //ContactRule contactRule = new ContactRule();
            if (contact.ID > 0)
            {
                Rule.Update(contact);
            }
            else
            {
                Rule.Insert(contact);

                var organColor = Rule.UnitOfWork.RepositoryAsync<Organ_Color>().Queryable().Where(x => x.OrganID == organId).SingleOrDefault();
                if(organColor == null)
                {
                    Rule.UnitOfWork.RepositoryAsync<Organ_Color>().Insert(new Organ_Color() {
                        LastColorIDUsed = contactVM.ColorID,
                        OrganID = organId,
                        
                    });
                  //  await Rule.UnitOfWork.SaveChangesAsync();
                }
                else
                {
                    organColor.LastColorIDUsed = contactVM.ColorID;
                    Rule.UnitOfWork.RepositoryAsync<Organ_Color>().Update(organColor);
                 //   await Rule.UnitOfWork.SaveChangesAsync();
                }

                AccountRule accountRule = new AccountRule();
                var accounts = await accountRule.GetAllByOrganIdAsync(organId);

                var accountDreaftani = accounts.Where(x => x.ComplteCoding == "1104").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountDreaftani = new DomainClasses.Accounting.Account();
                tempAccountDreaftani.Coding = contact.Code;
                tempAccountDreaftani.ComplteCoding = "1104" + contact.Code;
                tempAccountDreaftani.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountDreaftani.Name = contact.Name;
                tempAccountDreaftani.OrganId = organId;
                tempAccountDreaftani.ParentId = accountDreaftani.ID;

                this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountDreaftani);

                var accountPardakhtani = accounts.Where(x => x.ComplteCoding == "2101").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountPardakhtani = new DomainClasses.Accounting.Account();
                tempAccountPardakhtani.Coding = contact.Code;
                tempAccountPardakhtani.ComplteCoding = "2101" + contact.Code;
                tempAccountPardakhtani.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountPardakhtani.Name = contact.Name;
                tempAccountPardakhtani.OrganId = organId;
                tempAccountPardakhtani.ParentId = accountPardakhtani.ID;

                this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountPardakhtani);


                var accountAsnadDareaftani = accounts.Where(x => x.ComplteCoding == "1105").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountAsnadDareaftani = new DomainClasses.Accounting.Account();
                tempAccountAsnadDareaftani.Coding = contact.Code;
                tempAccountAsnadDareaftani.ComplteCoding = "1105" + contact.Code;
                tempAccountAsnadDareaftani.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountAsnadDareaftani.Name = contact.Name;
                tempAccountAsnadDareaftani.OrganId = organId;
                tempAccountAsnadDareaftani.ParentId = accountAsnadDareaftani.ID;

                this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountAsnadDareaftani);
              
            }
            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            contactVM.ID = contact.ID;
            contactVM.RegistrationDate = DateTime.Now;


            AccountRule accountRule2 = new AccountRule();
            var accounts2 = await accountRule2.GetAllByOrganIdAsync(organId);

            var detailAccount = accounts2.Where(x => x.ComplteCoding == "1104" + contact.Code).SingleOrDefault();

            switch (contactVM.TypeAccount)
            {
                case "payables":
                case "inProgress":
                    {
                        detailAccount = accounts2.Where(x => x.ComplteCoding == "2102" + contact.Code).SingleOrDefault();

                        if (detailAccount == null)
                        {
                            DomainClasses.Accounting.Account account = new DomainClasses.Accounting.Account()
                            {
                                Coding = contact.Code,
                                Level = ZhivarEnums.AccountType.Moen,
                                ObjectState = OMF.Common.Enums.ObjectState.Added,
                                Name = contact.Name,
                                ComplteCoding = "2102" + contact.Code,
                                OrganId = organId,
                                ParentId = 1525

                            };

                            accountRule2.Insert(account);
                            await accountRule2.SaveChangesAsync();

                            accounts2 = await accountRule2.GetAllByOrganIdAsync(organId);

                            detailAccount = accounts2.Where(x => x.ComplteCoding == "2102" + contact.Code).SingleOrDefault();
                            break;
                        }
                        break;
                    }
                case "receivables":
                    {
                        detailAccount = accounts2.Where(x => x.ComplteCoding == "1105" + contact.Code).SingleOrDefault();
                        break;
                    }
                case "creditors":
                    {
                        detailAccount = accounts2.Where(x => x.ComplteCoding == "2101" + contact.Code).SingleOrDefault();
                        break;
                    }
            }
               

           

                contactVM.DetailAccount = new DetailAccount()
                {
                    Id = detailAccount.ID,
                    Code = detailAccount.Coding,
                    Name = detailAccount.Name,

                    Node = new Node()
                    {
                        FamilyTree = "اشخاص",
                        Id = Convert.ToInt32(contact.Code),
                        Name = "اشخاص",
                    }
                };
            

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contactVM });
        }

        [Route("GetNewContactObject")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetNewContactObject()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                string code = await CreateCodeContact(organId);

                var contactVM = new ContactVM();

                contactVM.Code = code;
                contactVM.DetailAccount = new DetailAccount()
                {
                    Id = Convert.ToInt32(code),
                    Code = code,
                    Node = new Node()
                    {
                        FamilyTree = "اشخاص",
                        Id = Convert.ToInt32(code),
                        Name = "اشخاص",
                    },


                };
                contactVM.RegistrationDate = DateTime.Now;

                var organ_ColorQuery = this.BusinessRule.UnitOfWork.RepositoryAsync<Organ_Color>().Queryable().Where(x => x.OrganID == organId);

                var baseColorQuery = this.BusinessRule.UnitOfWork.RepositoryAsync<BaseColor>().Queryable();
                var baseColor = baseColorQuery.FirstOrDefault();

                if (organ_ColorQuery.Any())
                {
                    var organ_Color = organ_ColorQuery.OrderByDescending(x => x.ID).FirstOrDefault();
                    baseColor = baseColorQuery.Where(x => x.ID == (organ_Color.LastColorIDUsed + 1)).SingleOrDefault();

                    if (baseColor == null)
                        baseColor = baseColorQuery.FirstOrDefault();

                }



                //  Random rnd = new Random();
                //    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

                contactVM.Styles = new Dictionary<string, string>();

                contactVM.Styles.Add("background-color", baseColor.HEX);
                contactVM.Color = baseColor.HEX;


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contactVM });
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }


        [Route("GetContactById")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetContactById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var item = await Rule.FindAsync(id);

            var contactVM = new ContactVM();

            contactVM.Code = item.Code;
            contactVM.DetailAccount = new DetailAccount()
            {
                Id = Convert.ToInt32(item.Code),
                Code = item.Code,
                Node = new Node()
                {
                    FamilyTree = "اشخاص",
                    Id = Convert.ToInt32(item.Code),
                    Name = "اشخاص",
                }
            };

            contactVM.Address = item.Address;
            contactVM.City = item.City;
            contactVM.Credits = item.Credits;
            contactVM.EconomicCode = item.EconomicCode;
            contactVM.Email = item.Email;
            contactVM.Fax = item.Fax;
            contactVM.ID = item.ID;
            contactVM.IsCustomer = item.IsCustomer;
            contactVM.IsEmployee = item.IsEmployee;
            contactVM.IsShareHolder = item.IsShareHolder;
            contactVM.IsVendor = item.IsVendor;
            contactVM.Liability = item.Liability;
            contactVM.Mobile = item.Mobile;
            contactVM.Name = item.Name;
            contactVM.NationalCode = item.NationalCode;
            contactVM.Note = item.Note;
            contactVM.OrganId = item.OrganId;
            contactVM.Phone = item.Phone;
            contactVM.PostalCode = item.PostalCode;
            contactVM.Rating = item.Rating;
            contactVM.RegistrationDate = item.RegistrationDate;
            contactVM.RegistrationNumber = item.RegistrationNumber;
            contactVM.SharePercent = Convert.ToDecimal(item.SharePercent);
            contactVM.State = item.State;
            contactVM.Website = item.Website;
            contactVM.ContactType = item.ContactType;
            contactVM.FirstName = item.FirstName;
            contactVM.LastName = item.LastName;
            contactVM.Color = item.Color;


            if (item.Blob != null)
            {
                contactVM.TasvirBlobBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(item.Blob));
                contactVM.Blob = item.Blob;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contactVM });

        }
        private async Task<string> CreateCodeContact(int organId)
        {
            var count = 0;
            var contacts = await Rule.GetAllByOrganIdAsync(organId);

            var lastContacts = contacts.OrderByDescending(x => x.ID).FirstOrDefault();

            if (lastContacts != null)
                    count = Convert.ToInt32(lastContacts.Code);

            count++;

            string code = "";

                if (count < 10)
                {
                    code = "00000" + count;
                }
                else if (count < 100)
                {
                    code = "0000" + count;
                }
                else if (count < 1000)
                {
                    code = "000" + count;
                }
                else if (count < 10000)
                {
                    code = "00" + count;
                }
                else if (count < 100000)
                {
                    code = "0" + count;
                }
                else
                {
                    code = count.ToString();
                }

                return code;
            }
            
         
        //  [Route("Post")]
        [HttpPost]
        public  async Task<HttpResponseMessage> Post(ContactVM contactVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });

            var contact = new Contact();

            Mapper.Map(contactVM, contact);

            contact.OrganId = organId;
            Rule.Insert(contact);

            await Rule.SaveChangesAsync();

            // گرید آی دی جدید را به این صورت دریافت می‌کند
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contact });




        }
        [HttpPut]
        // [Route("Update/{id:int}")]
        public  async Task<HttpResponseMessage> Update(int id, Contact contact)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            //ContactRule contactRule = new ContactRule();
            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });



            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            contact.OrganId = organId;

            Rule.Update(contact);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contact });
        }


        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete([FromBody]List<Contact> contacts)
        {
            ContactRule contactRule = new ContactRule();

            foreach (var contact in contacts)
            {
                var item = contactRule.Find(contact.ID);

                if (item != null)
                {
                    contactRule.Delete(item);

                    await this.BusinessRule.SaveChangesAsync();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = contacts });
        }
    }
}
