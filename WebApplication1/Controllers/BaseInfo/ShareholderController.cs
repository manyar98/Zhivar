using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Net;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;
using System.Net.Http;
using Newtonsoft.Json;
using Zhivar.Utilities;
using Zhivar.ServiceLayer.Contracts.Common;
using System.Web.Http;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using Zhivar.Business.BaseInfo;
using OMF.Business;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;

namespace Zhivar.Web.Controllers.BaseInfo
{
    [RoutePrefix("api/Shareholder")]
    public partial class ShareholderController : NewApiControllerBaseAsync<Shareholder, ShareholderVM>
    {
        public ShareholderRule Rule => this.BusinessRule as ShareholderRule;

        protected override IBusinessRuleBaseAsync<Shareholder> CreateBusinessRule()
        {
            return new ShareholderRule();
        }

        [Route("GetAllByOrganId")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ShareholderRule shareholderRule = new ShareholderRule();
            var list = await shareholderRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list });
        }

        [Route("GetNewShareholderObject")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetNewShareholderObject()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            //   var list = await _sandoghService.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            var Accounts = new List<AccountVM>();
            Accounts.Add(new AccountVM()
            {
                Balance = 0,
                BalanceType = 2,
                Code = "03",
                Coding = "1103",
                GroupCode = "1",
                ID = 5,
                Level = ZhivarEnums.AccountType.Moen,
                LevelString = "معین",
                Name = "بانک ها",
                ParentCoding = "11",
                SystemAccount = 8,
                SystemAccountName = "بانک",
                credit = 0,
                debit = 0,


            });
            Accounts.Add(new AccountVM()
            {
                Balance = 0,
                BalanceType = 2,
                Code = "02",
                Coding = "2102",
                GroupCode = "2",
                ID = 23,
                Level = ZhivarEnums.AccountType.Moen,
                LevelString = "معین",
                Name = "اسناد پرداختنی",
                ParentCoding = "21",
                SystemAccount = 12,
                SystemAccountName = "اسناد پرداختنی",
                credit = 0,
                debit = 0,
            });

            Accounts.Add(new AccountVM()
            {
                Balance = 0,
                BalanceType = 2,
                Code = "06",
                Coding = "1106",
                GroupCode = "1",
                ID = 8,
                Level = ZhivarEnums.AccountType.Moen,
                LevelString = "معین",
                Name = "چک های در جریان وصول",
                ParentCoding = "11",
                SystemAccount = 10,
                SystemAccountName = "اسناد در جریان وصول",
                credit = 0,
                debit = 0,
            });

            ShareholderVM shareholderVM = new ShareholderVM()
            {
               
                //AccountNumber = "",
                //Balance = 0,
                //Branch = "",
                Code = await CreateShareholderCode(organId),
                DetailAccount = new DetailAccount()
                {
                    Accounts = Accounts,
                    Balance = 0,
                    BalanceType = 0,
                    Code = "0002",
                    Id = 0,
                    Name = "",
                    Node = new Node
                    {
                        FamilyTree = "حسابهای بانکی",
                        Id = 4,
                        Name = "حسابهای بانکی",
                        Parent = null,
                        Parents = ",4,",
                        SystemAccount = 4
                    },
                    RelatedAccounts = ",5,23,8,",
                    credit = 0,
                    debit = 0,

                },
                //FullName = "",
                Id = 0,
                Name = ""

            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = shareholderVM });
        }


        [Route("GetShareholderById")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetShareholderById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ShareholderRule shareholderRule = new ShareholderRule();
            var shareholder = await shareholderRule.FindAsync(id);

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = shareholder });
        }
        [Route("Add")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Add(ShareholderVM shareholderVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.ValidationError, data = "" });
            }

            var shareholder = new Shareholder();
            Mapper.Map(shareholderVM, shareholder);

            shareholder.OrganId = organId;

            //ShareholderRule shareholderRule = new ShareholderRule();

            if (shareholderVM.ID.HasValue)
            {
                Rule.Update(shareholder);
            }
            else
            {
                Rule.Insert(shareholder);

                AccountRule accountRule = new AccountRule();
                var accounts = await accountRule.GetAllByOrganIdAsync(organId);

                var accountShareholder = accounts.Where(x => x.ComplteCoding == "1103").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountShareholder = new DomainClasses.Accounting.Account();
                tempAccountShareholder.Coding = shareholder.Contact.Code;
                tempAccountShareholder.ComplteCoding = "1103" + shareholder.Contact.Code;
                tempAccountShareholder.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountShareholder.Name = shareholder.Contact.Name;
                tempAccountShareholder.OrganId = organId;
                tempAccountShareholder.ParentId = accountShareholder.ID;

                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountShareholder);
                //accountRule.Insert(tempAccountShareholder);



                var accountAsnadPardakhtani = accounts.Where(x => x.ComplteCoding == "2102").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountAsnadPardakhtani = new DomainClasses.Accounting.Account();
                tempAccountAsnadPardakhtani.Coding = shareholder.Contact.Code;
                tempAccountAsnadPardakhtani.ComplteCoding = "2102" + shareholder.Contact.Code;
                tempAccountAsnadPardakhtani.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountAsnadPardakhtani.Name = shareholder.Contact.Name;
                tempAccountAsnadPardakhtani.OrganId = organId;
                tempAccountAsnadPardakhtani.ParentId = accountAsnadPardakhtani.ID;

                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountAsnadPardakhtani);
                //accountRule.Insert(tempAccountAsnadPardakhtani);



                var accountChequeDarJareanVosol = accounts.Where(x => x.ComplteCoding == "1106").SingleOrDefault();

                DomainClasses.Accounting.Account tempAccountChequeDarJareanVosol = new DomainClasses.Accounting.Account();
                tempAccountChequeDarJareanVosol.Coding = shareholder.Contact.Code;
                tempAccountChequeDarJareanVosol.ComplteCoding = "1106" + shareholder.Contact.Code;
                tempAccountChequeDarJareanVosol.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountChequeDarJareanVosol.Name = shareholder.Contact.Name;
                tempAccountChequeDarJareanVosol.OrganId = organId;
                tempAccountChequeDarJareanVosol.ParentId = accountChequeDarJareanVosol.ID;

                //accountRule.Insert(tempAccountChequeDarJareanVosol);
                this.BusinessRule.UnitOfWork.Repository<DomainClasses.Accounting.Account>().Insert(tempAccountChequeDarJareanVosol);
            }

            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = shareholder });

        }

        [Route("SaveShareholders")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> SaveShareholders([FromBody]List<ShareholderVM> shareholderVMs)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.ValidationError, data = "" });
            }

           var shareholder = new Shareholder();

            List<int> listUpdate = new List<int>();

            //ShareholderRule shareholderRule = new ShareholderRule();
            List<Shareholder> listShareHolder = await Rule.GetShareholderByPersonIdAsync(organId);

            foreach (var shareholderVM in shareholderVMs)
            {
                shareholder = new Shareholder() {
                    ContactId = Convert.ToInt32( shareholderVM.ID),
                    IsActive = true,
                    OrganId = organId,
                    SharePercent = shareholderVM.SharePercent,
                   
                };
                
                Shareholder existingShareHolder = await Rule.GetShareholderByContractIdAsync(organId,(int)shareholderVM.ID);

                if (existingShareHolder != null)
                {
                    listUpdate.Add(existingShareHolder.ID);

                    shareholder.ID = existingShareHolder.ID;
                    Rule.Update(shareholder);
                }
                else
                {
                    Rule.Insert(shareholder);

                    

                    await CreateShareHolderAccounts(organId, shareholder.ContactId);


                }
            }

    
            var rejectList = listShareHolder.Where(i => listUpdate.Contains(i.ID));
            var filteredList = listShareHolder.Except(rejectList);

            foreach (var shareHolder in filteredList)
            {
                //Shareholder existingShareHolder = await shareholderRule.GetShareholderByContractIdAsync(organId, (int)shareHolder.ID);

                Rule.Delete(shareHolder);
            }

            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = shareholder });

        }

        private async Task CreateShareHolderAccounts(int organId , int contactId)
        {
            ContactRule contactRule = new ContactRule();
            var contact = await contactRule.FindAsync(contactId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var accountShareholder = accounts.Where(x => x.ComplteCoding == "3101").SingleOrDefault();
            DomainClasses.Accounting.Account tempAccountShareholder = new DomainClasses.Accounting.Account();

            tempAccountShareholder = accounts.Where(x => x.ComplteCoding == "3101" + contact.Code).SingleOrDefault();

            if (tempAccountShareholder == null)
            {
                tempAccountShareholder = new DomainClasses.Accounting.Account();

                tempAccountShareholder.Coding = contact.Code;
                tempAccountShareholder.ComplteCoding = "3101" + contact.Code;
                tempAccountShareholder.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountShareholder.Name = contact.Name;
                tempAccountShareholder.OrganId = organId;
                tempAccountShareholder.ParentId = accountShareholder.ID;

                accountRule.Insert(tempAccountShareholder);
            }

            accountShareholder = accounts.Where(x => x.ComplteCoding == "3102").SingleOrDefault();
            tempAccountShareholder = new DomainClasses.Accounting.Account();

            tempAccountShareholder = accounts.Where(x => x.ComplteCoding == "3102" + contact.Code).SingleOrDefault();

            if (tempAccountShareholder == null)
            {
                tempAccountShareholder = new DomainClasses.Accounting.Account();

                tempAccountShareholder.Coding = contact.Code;
                tempAccountShareholder.ComplteCoding = "3102" + contact.Code;
                tempAccountShareholder.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountShareholder.Name = contact.Name;
                tempAccountShareholder.OrganId = organId;
                tempAccountShareholder.ParentId = accountShareholder.ID;

                accountRule.Insert(tempAccountShareholder);
            }

            accountShareholder = accounts.Where(x => x.ComplteCoding == "3103").SingleOrDefault();
            tempAccountShareholder = new DomainClasses.Accounting.Account();

            tempAccountShareholder = accounts.Where(x => x.ComplteCoding == "3103" + contact.Code).SingleOrDefault();

            if (tempAccountShareholder == null)
            {
                tempAccountShareholder = new DomainClasses.Accounting.Account();

                tempAccountShareholder.Coding = contact.Code;
                tempAccountShareholder.ComplteCoding = "3103" + contact.Code;
                tempAccountShareholder.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountShareholder.Name = contact.Name;
                tempAccountShareholder.OrganId = organId;
                tempAccountShareholder.ParentId = accountShareholder.ID;

                accountRule.Insert(tempAccountShareholder);
            }

            accountShareholder = accounts.Where(x => x.ComplteCoding == "3104").SingleOrDefault();
            tempAccountShareholder = new DomainClasses.Accounting.Account();

            tempAccountShareholder = accounts.Where(x => x.ComplteCoding == "3104" + contact.Code).SingleOrDefault();

            if (tempAccountShareholder == null)
            {
                tempAccountShareholder = new DomainClasses.Accounting.Account();

                tempAccountShareholder.Coding = contact.Code;
                tempAccountShareholder.ComplteCoding = "3104" + contact.Code;
                tempAccountShareholder.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountShareholder.Name = contact.Name;
                tempAccountShareholder.OrganId = organId;
                tempAccountShareholder.ParentId = accountShareholder.ID;

                accountRule.Insert(tempAccountShareholder);
            }

            accountShareholder = accounts.Where(x => x.ComplteCoding == "3105").SingleOrDefault();
            tempAccountShareholder = new DomainClasses.Accounting.Account();

            tempAccountShareholder = accounts.Where(x => x.ComplteCoding == "3105" + contact.Code).SingleOrDefault();

            if (tempAccountShareholder == null)
            {
                tempAccountShareholder = new DomainClasses.Accounting.Account();

                tempAccountShareholder.Coding = contact.Code;
                tempAccountShareholder.ComplteCoding = "3105" + contact.Code;
                tempAccountShareholder.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountShareholder.Name = contact.Name;
                tempAccountShareholder.OrganId = organId;
                tempAccountShareholder.ParentId = accountShareholder.ID;

                accountRule.Insert(tempAccountShareholder);
            }
        }


        //[Route("Post")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Post(ShareholderVM shareholderVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });


            var shareholder = new Shareholder();

            Mapper.Map(shareholderVM, shareholder);

            shareholder.OrganId = organId;
            //ShareholderRule shareholderRule = new ShareholderRule();
            Rule.Insert(shareholder);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = shareholder });
        }

        [HttpPut]
        //[Route("Update/{id =int}")]
        public virtual async Task<HttpResponseMessage> Update(int id, Shareholder shareholder)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            //ShareholderRule shareholderRule = new ShareholderRule();

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });



            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });

            shareholder.OrganId = organId;

            Rule.Update(shareholder);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = shareholder });

        }

        [Route("Delete")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Delete([FromBody]int id)
        {
            //ShareholderRule shareholderRule = new ShareholderRule();
            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });

        }

        private async Task<string> CreateShareholderCode(int organId)
        {
            var count = 0;
            ShareholderRule shareholderRule = new ShareholderRule();
            var shareholderlst = await shareholderRule.GetAllByOrganIdAsync(organId);

            count = shareholderlst.Count();
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
    }
}