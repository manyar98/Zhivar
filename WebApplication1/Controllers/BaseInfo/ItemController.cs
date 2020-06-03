using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Globalization;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ViewModel.Accunting;
using Newtonsoft.Json;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DataLayer.Validation;
using OMF.Enterprise.MVC;
using Zhivar.Business.BaseInfo;
using OMF.Business;
using OMF.Common.Security;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;

namespace Zhivar.Web.Controllers.Accunting
{

    public partial class ItemController : NewApiControllerBaseAsync<Item, ItemVM>
    {
        public ItemRule Rule => this.BusinessRule as ItemRule;

        protected override IBusinessRuleBaseAsync<Item> CreateBusinessRule()
        {
            return new ItemRule();
        }

   
        [Route("GetAll")]
        public async Task<HttpResponseMessage> GetAll()
        {
            var list = await Rule.GetAllAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list });
        }

        //[Route("GetAllByOrganId")]
        //[HttpPost]
        //public async Task<HttpResponseMessage> GetAllByOrganId()
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    var list = await itemGroupRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

        //    var list2 = new List<ItemVM>();
        //    foreach (var item in list)
        //    {
        //        if (item.Items != null)
        //            list2.AddRange(item.Items.Select(x => new ItemVM { ID = x.ID, Name = x.Name }).ToList());

        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = list2 });

        //}
        [Route("GetAllByOrganId")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetAllByOrganId([FromBody] string type)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                //var list = await itemGroupRule.GetAllByOrganIdAsync(Convert.ToInt32(organId))
                AccountRule accountRule = new AccountRule();

                var accounts = await accountRule.GetAllByOrganIdAsync(organId);

                ItemGroupRule itemGroupRule = new ItemGroupRule();
                var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                var items = new List<ItemVM>();
                var item = new ItemVM();

                foreach (var itemGroup in itemGroups)
                {
                    if (itemGroup.Items != null)
                    {
                        if (type == "product")
                        {
                            itemGroup.Items = itemGroup.Items.Where(x => x.IsGoods == true).ToList();
                        }
                        else if (type == "service")
                        {
                            itemGroup.Items = itemGroup.Items.Where(x => x.IsService == true).ToList();
                        }

                    }
                    foreach (var KalaKhadmat in itemGroup.Items ?? new List<ItemVM>())
                    {
                        var itemAccount = new DomainClasses.Accounting.Account();

                        if (KalaKhadmat.IsGoods == true)
                        {
                            itemAccount = accounts.Where(x => x.ComplteCoding == "1108" + KalaKhadmat.Code).SingleOrDefault();
                            if (itemAccount == null)
                            {
                                Item itemCommon = new Item();
                                Mapper.Map(KalaKhadmat, itemCommon);
                                await Rule.CreateGoodAccountsAsync(itemCommon, organId);
                                accounts = await accountRule.GetAllByOrganIdAsync(organId);

                                itemAccount = accounts.Where(x => x.ComplteCoding == "1108" + KalaKhadmat.Code).SingleOrDefault();
                            }

                        }

                        else
                        {
                            itemAccount = accounts.Where(x => x.ComplteCoding == "7101" + KalaKhadmat.Code).SingleOrDefault();
                            if (itemAccount == null)
                            {
                                Item itemCommon = new Item();
                                Mapper.Map(KalaKhadmat, itemCommon);
                                await Rule.CreateServiceAccountAsync(itemCommon, organId);
                                accounts = await accountRule.GetAllByOrganIdAsync(organId);

                                itemAccount = accounts.Where(x => x.ComplteCoding == "7101" + KalaKhadmat.Code).SingleOrDefault();
                            }
                        }
                           

                        item = new ItemVM()
                        {
                            Code = KalaKhadmat.Code,
                            GroupName = itemGroup.Name,
                            Barcode = "",
                            BuyPrice = KalaKhadmat.BuyPrice,
                            DetailAccount = new DetailAccount()
                            {
                                Code = itemAccount.Coding,
                                Id = itemAccount.ID,
                                Name = itemAccount.Name,

                                Node = new Node()
                                {
                                    FamilyTree = itemGroup.Name,
                                    Name = itemGroup.Name,
                                    Id = itemGroup.ID
                                }
                            },
                            ID = KalaKhadmat.ID,
                            Name = KalaKhadmat.Name,
                            UnitID = KalaKhadmat.UnitID,
                            SalesTitle = KalaKhadmat.SalesTitle,
                            PurchasesTitle = KalaKhadmat.PurchasesTitle,
                            SellPrice = KalaKhadmat.SellPrice,
                            ItemType = KalaKhadmat.ItemType,
                            Stock = KalaKhadmat.Stock

                        };

                        items.Add(item);
                    }

                    //var list2 = new List<ItemVM>();

                    //foreach (var item in list)
                    //{
                    //    if (item.Items != null)
                    //    {
                    //        if (type == "product")
                    //        {
                    //            list2.AddRange(item.Items.Where(x => x.IsGoods == true).Select(x => new ItemVM { ID = x.ID, Name = x.Name,BuyPrice = x.BuyPrice, Code = x.Code,GroupName = item.Name,IsGoods = x.IsGoods, IsService = x.IsService,ItemGroupId = x.ItemGroupId , PurchasesTitle = x.PurchasesTitle, SellPrice = x.SellPrice, ItemType = x.ItemType, MoneyStock = x.MoneyStock , OrganId = x.OrganId , SalesTitle = x.SalesTitle , Stock = x.Stock, Unit = x.Unit }).ToList());
                    //        }
                    //        else if (type == "service")
                    //        {
                    //            list2.AddRange(item.Items.Where(x => x.IsService == true).Select(x => new ItemVM { ID = x.ID, Name = x.Name, BuyPrice = x.BuyPrice, Code = x.Code, GroupName = item.Name, IsGoods = x.IsGoods, IsService = x.IsService, ItemGroupId = x.ItemGroupId, PurchasesTitle = x.PurchasesTitle, SellPrice = x.SellPrice, ItemType = x.ItemType, MoneyStock = x.MoneyStock, OrganId = x.OrganId, SalesTitle = x.SalesTitle, Stock = x.Stock, Unit = x.Unit }).ToList());
                    //        }
                    //        else
                    //        {
                    //            list2.AddRange(item.Items.Select(x => new ItemVM { ID = x.ID, Name = x.Name, BuyPrice = x.BuyPrice, Code = x.Code, GroupName = item.Name, IsGoods = x.IsGoods, IsService = x.IsService, ItemGroupId = x.ItemGroupId, PurchasesTitle = x.PurchasesTitle, SellPrice = x.SellPrice, ItemType = x.ItemType, MoneyStock = x.MoneyStock, OrganId = x.OrganId, SalesTitle = x.SalesTitle, Stock = x.Stock, Unit = x.Unit }).ToList());
                    //        }
                    //    }


                    //}
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = items });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "خطای به وجود آمده است." });
            }
         
        }
        [Route("GetById")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetById([FromBody]int id)
        {

            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var item = await Rule.FindAsync(id);

            ItemUnitRule itemUnitRule = new ItemUnitRule();
            var units = await itemUnitRule.GetAllByOrganIdAsync(organId);

            var itemVM = new ItemVM()
            {
                Barcode = item.Barcode,
                BuyPrice = item.BuyPrice,
                Code = item.Code,
                ID = item.ID,
                IsGoods = item.IsGoods,
                IsService = item.IsService,
                ItemGroupId = item.ItemGroupId,
                ItemType = item.ItemType,
                MoneyStock = item.MoneyStock,
                Name = item.Name,
                OrganId = item.OrganIdItem,
                PurchasesTitle = item.PurchasesTitle,
                SalesTitle = item.SalesTitle,
                SellPrice = item.SellPrice,
                Stock = item.Stock,
                UnitID = item.UnitID,
                DetailAccount = new DetailAccount()
                {

                    Accounts = new List<AccountVM>() {
                            new AccountVM() {
                                Balance = 0, BalanceType = 2, Code = "08", Coding = "1108", GroupCode = "1", ID= 10,
                                Level=ZhivarEnums.AccountType.Moen, LevelString= "معین", Name= "موجودی کالا", ParentCoding= "11",
                                SystemAccount = 11, SystemAccountName= "موجودی کالا", credit= 0, debit=0
                            },
                            new AccountVM(){
                                Balance = 0, BalanceType= 2, Code= "01", Coding= "6101", GroupCode= "6", ID= 43,
                                Level=ZhivarEnums.AccountType.Moen, LevelString= "معین", Name= "فروش کالا", ParentCoding= "61",
                                SystemAccount= 14, SystemAccountName= "فروش", credit= 0, debit=0
                            },
                            new AccountVM(){
                                Balance= 0, BalanceType= 2, Code= "01", Coding= "5101", GroupCode= "5", ID= 39,
                                Level=ZhivarEnums.AccountType.Moen, LevelString= "معین", Name= "خرید کالا", ParentCoding= "51",
                                SystemAccount= 16, SystemAccountName= "خرید", credit= 0, debit=0
                            }
                        },
                    Code = item.Code,
                    Id = 0,

                    Node = new Node()
                    {
                        FamilyTree = "کالاها و خدمات",
                        Id = item.ItemGroupId,
                        Name = "کالاها و خدمات"
                    },
                    //, Parent= null, Parents: ",2,", SystemAccount= 2 },
                    //         //           RelatedAccounts: ",10,43,39,",
                    //          //          credit: 0,
                    //          //          debit:

                    //             //   },
                    //          //      Id: 0, IsGoods: true, IsService: false, ItemType: 0, MinStock: 0, Name: "", PurchasesTitle: "",
                    //           //     SalesTitle: "", SellPrice: 0, Stock: 0, Unit: "", WeightedAveragePrice:

                    //          //  },
                    

                },
                itemUnits = units
                
            };

            if (itemVM.UnitID > 0)
                itemVM.Unit = units.Where(x => x.ID == itemVM.UnitID).SingleOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = itemVM });

        }

        [Route("AddItem")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddItem([FromBody] ItemVM itemVM)
        {
            try
            {

                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });
                }

                Item item = new Item();
                item = Utilities.TranslateHelper.TranslateItemVMToItem(itemVM);
                //Mapper.Map(itemVM,item);
                item.OrganIdItem = organId;
                item.ItemGroupId = itemVM.DetailAccount.Node.Id;
                item.Code = itemVM.DetailAccount.Code;

                if (item.ItemType == ZhivarEnums.NoeItem.Item)
                {
                    item.IsGoods = true;
                    item.IsService = false;
                }
                else if (item.ItemType == ZhivarEnums.NoeItem.Service)
                {
                    item.IsGoods = false;
                    item.IsService = true;
                }

                if (itemVM.Unit != null && itemVM.Unit.ID > 0)
                {
                    item.UnitID = itemVM.Unit.ID;
                }
                else
                    item.UnitID = null;

                //ItemValidate validator = new ItemValidate();
                //FluentValidation.Results.ValidationResult results = validator.Validate(item);

                //string failurs = "";

                //if (!results.IsValid)
                //{
                //    foreach (var error in results.Errors)
                //    {
                //        failurs += "<br/>" + error.ErrorMessage;

                //    }
                //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = Enums.ResultCode.ValidationError, data = failurs });
                //}

                if (itemVM.ID > 0)
                {
                    Rule.Update(item);
                }
                else
                {
                    Rule.Insert(item);

                    if (item.IsGoods != null && item.IsGoods == true)
                        await Rule.CreateGoodAccountsAsync(item, organId);
                    else
                        await Rule.CreateServiceAccountAsync(item, organId);


                }

                await this.BusinessRule.UnitOfWork.SaveChangesAsync();

                itemVM.Code = await CreateCodeItem(organId);
                itemVM.BuyPrice = 0;
                itemVM.Name = string.Empty;
                itemVM.PurchasesTitle = string.Empty;
                itemVM.SalesTitle = string.Empty;
                itemVM.SellPrice = 0;
                itemVM.Stock = 0;
                itemVM.UnitID = 0;
                itemVM.ItemType = item.ItemType;
                itemVM.IsService = item.IsService;
                itemVM.IsGoods = item.IsGoods;

                if (itemVM.Next)
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = itemVM });
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = item });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //private async Task createServiceAccount(Item item, int personId)
        //{
        //    AccountRule accountRule = new AccountRule();

        //    var accounts = await accountRule.GetAllByOrganIdAsync(personId);

        //    var accountDaramdKhadamat = accounts.Where(x => x.ComplteCoding == "7101").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountDaramdKhadamat = new DomainClasses.Accounting.Account();
        //    tempAccountDaramdKhadamat.Coding = item.Code;
        //    tempAccountDaramdKhadamat.ComplteCoding = "7101" + item.Code;
        //    tempAccountDaramdKhadamat.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountDaramdKhadamat.Name = item.Name;
        //    tempAccountDaramdKhadamat.OrganId = personId;
        //    tempAccountDaramdKhadamat.ParentId = accountDaramdKhadamat.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountDaramdKhadamat);

        //    var accountSirHazeneh = accounts.Where(x => x.ComplteCoding == "8204").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountSirHazeneh = new DomainClasses.Accounting.Account();
        //    tempAccountSirHazeneh.Coding = item.Code;
        //    tempAccountSirHazeneh.ComplteCoding = "8204" + item.Code;
        //    tempAccountSirHazeneh.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountSirHazeneh.Name = item.Name;
        //    tempAccountSirHazeneh.OrganId = personId;
        //    tempAccountSirHazeneh.ParentId = accountSirHazeneh.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountSirHazeneh);
        //}

        //private async Task deleteGoodAccounts(Item item)
        //{
        //    AccountRule accountRule = new AccountRule();

        //    await accountRule.DeleteAccountByComplteCodingAsync("1108" + item.Code);
        //    await accountRule.DeleteAccountByComplteCodingAsync("5101" + item.Code);
        //    await accountRule.DeleteAccountByComplteCodingAsync("5102" + item.Code);
        //    await accountRule.DeleteAccountByComplteCodingAsync("6101" + item.Code);
        //    await accountRule.DeleteAccountByComplteCodingAsync("6102" + item.Code);
        //}

        //private async Task deleteServiceAccount(Item item)
        //{
        //    AccountRule accountRule = new AccountRule();

        //    await accountRule.DeleteAccountByComplteCodingAsync("7101" + item.Code);
        //    await accountRule.DeleteAccountByComplteCodingAsync("8204" + item.Code);
        //}

        //private async Task createGoodAccounts(Item item, int personId)
        //{
        //    AccountRule accountRule = new AccountRule();

        //    var accounts = await accountRule.GetAllByOrganIdAsync(personId);

        //    var accountMojodiKala = accounts.Where(x => x.ComplteCoding == "1108").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountMojodiKala = new DomainClasses.Accounting.Account();
        //    tempAccountMojodiKala.Coding = item.Code;
        //    tempAccountMojodiKala.ComplteCoding = "1108" + item.Code;
        //    tempAccountMojodiKala.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountMojodiKala.Name = item.Name;
        //    tempAccountMojodiKala.OrganId = personId;
        //    tempAccountMojodiKala.ParentId = accountMojodiKala.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountMojodiKala);

        //    var accountKharidKala = accounts.Where(x => x.ComplteCoding == "5101").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountKharidKala = new DomainClasses.Accounting.Account();
        //    tempAccountKharidKala.Coding = item.Code;
        //    tempAccountKharidKala.ComplteCoding = "5101" + item.Code;
        //    tempAccountKharidKala.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountKharidKala.Name = item.Name;
        //    tempAccountKharidKala.OrganId = personId;
        //    tempAccountKharidKala.ParentId = accountKharidKala.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountKharidKala);

        //    var accountBargashtKharidKala = accounts.Where(x => x.ComplteCoding == "5102").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountBargashtKharidKala = new DomainClasses.Accounting.Account();
        //    tempAccountBargashtKharidKala.Coding = item.Code;
        //    tempAccountBargashtKharidKala.ComplteCoding = "5102" + item.Code;
        //    tempAccountBargashtKharidKala.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountBargashtKharidKala.Name = item.Name;
        //    tempAccountBargashtKharidKala.OrganId = personId;
        //    tempAccountBargashtKharidKala.ParentId = accountBargashtKharidKala.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountBargashtKharidKala);


        //    var accountForoshKala = accounts.Where(x => x.ComplteCoding == "6101").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountAccountForoshKala = new DomainClasses.Accounting.Account();
        //    tempAccountAccountForoshKala.Coding = item.Code;
        //    tempAccountAccountForoshKala.ComplteCoding = "6101" + item.Code;
        //    tempAccountAccountForoshKala.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountAccountForoshKala.Name = item.Name;
        //    tempAccountAccountForoshKala.OrganId = personId;
        //    tempAccountAccountForoshKala.ParentId = accountForoshKala.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountAccountForoshKala);

        //    var accountBargashtForoshKala = accounts.Where(x => x.ComplteCoding == "6102").SingleOrDefault();

        //    DomainClasses.Accounting.Account tempAccountBargashtForoshKala = new DomainClasses.Accounting.Account();
        //    tempAccountBargashtForoshKala.Coding = item.Code;
        //    tempAccountBargashtForoshKala.ComplteCoding = "6102" + item.Code;
        //    tempAccountBargashtForoshKala.Level = ZhivarEnums.AccountType.Tafzeli;
        //    tempAccountBargashtForoshKala.Name = item.Name;
        //    tempAccountBargashtForoshKala.OrganId = personId;
        //    tempAccountBargashtForoshKala.ParentId = accountBargashtForoshKala.ID;

        //    this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountBargashtForoshKala);

        //}
        ////  [Route("Post")]
        //[HttpPost]
        //public async Task<HttpResponseMessage> Post(ItemVM itemVM)
        //{
        //    if (!ModelState.IsValid)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var item = new Item();

        //    Mapper.Map(itemVM, item);

        //    //  Mapper.Map(ItemVM, Item);

        //    _itemService.Insert(item);

        //    await _unitOfWork.SaveAllChangesAsync();

        //    // گرید آی دی جدید را به این صورت دریافت می‌کند
        //    return Json(new DataSourceResult { Data = new[] { item } });





        //}

        //[HttpPut]
        //// [Route("Update/{id=int}")]
        //public async Task<HttpResponseMessage> Update(int id, Item item)
        //{
        //    var item = await _itemService.GetByIdAsync(id);

        //    if (item == null)
        //        return new HttpNotFoundResult();


        //    if (!ModelState.IsValid || id != item.ID)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    //  var Item = new Item();
        //    //  Mapper.Map(ItemVM, Item);

        //    _itemService.Update(item);

        //    await _unitOfWork.SaveAllChangesAsync();

        //    //Return HttpStatusCode.OK
        //    return new HttpStatusCodeResult(HttpStatusCode.OK);
        //}

        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete([FromBody]List<Item> items)
        {
            try
            {
                foreach (var item in items)
                {
                    var itemFind = await Rule.FindAsync(item.ID);

                    if (itemFind != null)
                    {
                        await Rule.DeleteAsync(item.ID);

                        if (itemFind.IsGoods == true)
                            await Rule.DeleteGoodAccountsAsync(itemFind);
                        else if (itemFind.IsService == true)
                            await Rule.DeleteServiceAccountAsync(itemFind);


                        await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = items });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "خطای به وجود آمده است." });
            }
           

            


            
        }

        [HttpPost]
        [Route("GetNewObject")]
        public async Task<HttpResponseMessage> GetNewObject()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                ItemUnitRule itemUnitRule = new ItemUnitRule();
                var units = await itemUnitRule.GetAllByOrganIdAsync(organId);

                NewObjectKala newObjectKala = new NewObjectKala()
                {
                    item = new ItemVM()
                    {

                        Barcode = "",
                        BuyPrice = 0,
                        DetailAccount = new DetailAccount()
                        {

                            Accounts = new List<AccountVM>() {
                            new AccountVM() {
                                Balance = 0, BalanceType = 2, Code = "08", Coding = "1108", GroupCode = "1", ID= 10,
                                Level=ZhivarEnums.AccountType.Moen, LevelString= "معین", Name= "موجودی کالا", ParentCoding= "11",
                                SystemAccount = 11, SystemAccountName= "موجودی کالا", credit= 0, debit=0
                            },
                            new AccountVM(){
                                Balance = 0, BalanceType= 2, Code= "01", Coding= "6101", GroupCode= "6", ID= 43,
                                Level=ZhivarEnums.AccountType.Moen, LevelString= "معین", Name= "فروش کالا", ParentCoding= "61",
                                SystemAccount= 14, SystemAccountName= "فروش", credit= 0, debit=0
                            },
                            new AccountVM(){
                                Balance= 0, BalanceType= 2, Code= "01", Coding= "5101", GroupCode= "5", ID= 39,
                                Level=ZhivarEnums.AccountType.Moen, LevelString= "معین", Name= "خرید کالا", ParentCoding= "51",
                                SystemAccount= 16, SystemAccountName= "خرید", credit= 0, debit=0
                            }
                        },
                            Code = await CreateCodeItem(organId),
                            Id = 0,

                            Node = new Node()
                            {
                                FamilyTree = "کالاها و خدمات",
                                Id = 2,
                                Name = "کالاها و خدمات"
                            }
                            //, Parent= null, Parents: ",2,", SystemAccount= 2 },
                            //         //           RelatedAccounts: ",10,43,39,",
                            //          //          credit: 0,
                            //          //          debit:

                            //             //   },
                            //          //      Id: 0, IsGoods: true, IsService: false, ItemType: 0, MinStock: 0, Name: "", PurchasesTitle: "",
                            //           //     SalesTitle: "", SellPrice: 0, Stock: 0, Unit: "", WeightedAveragePrice:

                            //          //  },
                            //          ///  itemUnits: [{ 0: "عدد" }, { 1: "بسته" }, { 2: "کارتن" }, { 3: "دستگاه" },

                        },
                        ID = 0,
                        IsGoods = true,
                        IsService = false,
                        ItemType = 0, //MinStock= 0,
                        Name = "",
                        PurchasesTitle = "",
                        SalesTitle = "",
                        SellPrice = 0,
                        Stock = 0,
                        UnitID = 0, //WeightedAveragePrice=0
                        Code = await CreateCodeItem(organId),
                    },
                    showItemUnit = false,
                    itemUnits = units
                };
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = newObjectKala });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "خطای به وجود آمده است" });
            }
       
        }

        [HttpPost]
        [Route("GetItemSalesByInvoice")]
        public async Task<HttpResponseMessage> GetItemSalesByInvoice([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var items = new List<ItemSalesByInvoice>() {
                new ItemSalesByInvoice() {
                    Contact= "مونا ابراهیمی",
ContactId= 1,
Date= "1397/10/17",
Discount= 0,
Id= 1,
Number= "1",
Quantity= 1,
Reference= "",
Sum= 70000,
Tax= 6300,
TotalAmount= 76300,
UnitPrice= 70000,
                }
            };


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = items });
        }


        [HttpPost]
        [Route("GetInventoryItems")]
        public async Task<HttpResponseMessage> GetInventoryItems([FromBody] string untilDate)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var groupItems = await itemGroupRule.GetAllByOrganIdAsync(organId);

            var inventoryItems = new List<InventoryItem>();
            var inventoryItem = new InventoryItem();

            foreach (var groupItem in groupItems?? new List<ItemGroupVM>())
            {
                foreach (var item in groupItem.Items.Where(x => x.ItemType == ZhivarEnums.NoeItem.Item ).ToList() ?? new List<ItemVM>())
                {
                    inventoryItem = new InventoryItem();

                    inventoryItem.BuyPrice = item.BuyPrice;
                    inventoryItem.Code = item.Code;
                    inventoryItem.Id = item.ID;
                    inventoryItem.MoneyStock = item.MoneyStock;
                    inventoryItem.Name = item.Name;
                    inventoryItem.NodeName = item.Name;
                    inventoryItem.SellPrice = item.SellPrice;
                    inventoryItem.Stock = item.Stock;
                    inventoryItem.Unit = item.UnitID;
                   

                    inventoryItems.Add(inventoryItem);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = inventoryItems });
        }

        [HttpPost]
        [Route("GetItems")]
        public async Task<HttpResponseMessage> GetItems()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            var items = new List<ItemVM>();
            var item = new ItemVM();

            AccountRule accountRule = new AccountRule();

            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            foreach (var itemGroup in itemGroups)
            {
                foreach (var Item in itemGroup.Items)
                {
                    var account = accounts.Where(x => x.ComplteCoding == "1108" + Item.Code).SingleOrDefault();
                    item = new ItemVM()
                    {
                        Barcode = "",
                        BuyPrice = Item.BuyPrice,
                        DetailAccount = new DetailAccount()
                        {
                            Code = account.ComplteCoding,
                            Id = account.ID,
                            Node = new Node()
                            {
                                FamilyTree = itemGroup.Name,
                                Name = account.Name,
                                Id = itemGroup.ID
                            }
                        },
                        ID = Item.ID,
                        Name = Item.Name,
                        UnitID = Item.UnitID,
                        SalesTitle = Item.SalesTitle,
                        PurchasesTitle = Item.PurchasesTitle,
                        SellPrice = Item.SellPrice,
                        ItemType = Item.ItemType,
                        Stock = Item.Stock

                    };

                    items.Add(item);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = items });
        }


        [HttpPost]
        [Route("GetItemCard")]
        public async Task<HttpResponseMessage> GetItemCard([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


            var ItemCard = new ItemCard()
            {
                chart = new List<Chart>() {
                    new Chart() {In= 0,Month="فروردین 1397",Out=0,Purchase=0,Sale= 0 },
                    new Chart() {In=0,Month="اردیبهشت 1397",Out=0,Purchase=0,Sale=0},
                    new Chart() {In=0,Month="خرداد 1397",Out=0,Purchase=0,Sale= 0},
                    new Chart() {In=0,Month="تیر 1397",Out=0,Purchase=0,Sale= 0},
                    new Chart() {In=0,Month="مرداد 1397",Out=0,Purchase=0,Sale= 0},
                    new Chart() {In=0,Month="شهریور 1397",Out=0,Purchase=0,Sale=0,},
                    new Chart() {In=0,Month="مهر 1397",Out=0,Purchase=0,Sale= 0 },
                    new Chart() {In=0,Month="آبان 1397",Out=0,Purchase=0,Sale= 0 },
                    new Chart() {In=0,Month="آذر 1397",Out=0,Purchase=0,Sale= 0 },
                    new Chart() {In=0,Month="دی 1397",Out=1,Purchase=0,Sale= 70000},
                    new Chart() {In=0,Month="بهمن 1397",Out=0,Purchase=0,Sale= 0 },
                    new Chart() {In=0,Month="اسفند 1397",Out=1,Purchase=0,Sale= 70000},
                },
                invoiceItems = new List<InvoiceItemVM>()
                {
                    new InvoiceItemVM() {
                        //Code = "000001",ContactId=1,DateTime="1397/10/17",
                        Discount=0,
                        //DocId=2,InvoiceId=1,Name="مونا ابراهیمی",Number="1",
                        Quantity =1,
                        //Reference ="",
                        Tax =6300,TotalAmount =76300,
                        //Type =0,
                        Unit =0,
                        UnitPrice = 70000,
                    }
                },
                item = new ItemVM()
                {
                    Barcode= "", BuyPrice=50000,
                    //Code ="000001",
                    DetailAccount = new DetailAccount
                    { Accounts= null,Balance=0,BalanceType=0,Code="000001",Id=2,Name="کالا 1",
                    Node= new Node { Name= "کالاها و خدمات", Parents= ",2,", Parent= null, SystemAccount= 2, FamilyTree= "کالاها و خدمات"},
                        RelatedAccounts =",10,43,39,40,44,",credit=0,debit= 0},
                    ID =1,
                    IsGoods =true,
                    IsService = false,
                    ItemType =0,
                    //MinStock =0,
                    Name ="کالا 1",
                    PurchasesTitle ="کالا 1",
                    SalesTitle ="کالا 1",
                    SellPrice =70000,
                    Stock =3,
                    UnitID =0,
                    //WeightedAveragePrice =50000
                },
                list = new List<List>() {
                    new List() {Amount= 200000,
DateTime=
            "1397/01/01",
DocId=
            1,
In=
            4,
InvoiceId=
            0,
Number=
            1,
Out=
            0,
Remain=
            4,
Text= "موجودی کالا" },
                    new List() {
            Amount=
            70000,
DateTime=
            "1397/10/17",
DocId=
            2,
In=
            0,
InvoiceId=
            1,
Number=
            2,
Out=
            1,
Remain=
            3,
Text= "فروش کالا" },
                    new List() {
            Amount=
            0,
DateTime=
            "1397/12/29",
DocId=
            0,
In=
            4,
InvoiceId=
            0,
Number=0,
Out=
            1,
Remain=
            3,
Text= "مجموع" }
                }
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = ItemCard });
        }

        [HttpPost]
        [Route("GetItemsAndServiceSales")]
        public  async Task<HttpResponseMessage> GetItemsAndServiceSales()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(organId);

            InvoiceRule invoiceRule = new InvoiceRule();
            var invoices = await invoiceRule.GetAllByOrganIdAsync(organId);

            var invoiceSells = invoices.Where(x => x.InvoiceType == ZhivarEnums.NoeFactor.Sell).ToList();
            var invoiceReturnSells = invoices.Where(x => x.InvoiceType == ZhivarEnums.NoeFactor.ReturnSell).ToList();

            var itemsAndServices = new List<ItemsAndService>();
            var itemsAndService = new ItemsAndService();

            var invoiceItemsSells = new List<InvoiceItemVM>();
            var invoiceItemsReturnSells = new List<InvoiceItemVM>();

            var tempSells = new List<InvoiceItemVM>();
            var tempReturnSells = new List<InvoiceItemVM>();

            foreach (var invoice in invoiceSells)
            {
                foreach (var InvoiceItem in invoice.InvoiceItems ?? new List<InvoiceItemVM>())
                {
                    invoiceItemsSells.Add(InvoiceItem);

                }
            }

            foreach (var invoice in invoiceReturnSells)
            {
                foreach (var InvoiceItem in invoice.InvoiceItems ?? new List<InvoiceItemVM>())
                {
                    invoiceItemsReturnSells.Add(InvoiceItem);

                }
            }


            foreach (var itemGroup in itemGroups)
            {
                foreach (var item in itemGroup.Items?? new List<ItemVM>())
                {
                    tempSells = invoiceItemsSells.Where(x => x.ItemId == item.ID).ToList();
                    tempReturnSells = invoiceItemsReturnSells.Where(x => x.ItemId == item.ID).ToList();

                    var isAnySells = tempSells.Any();
                    var isAnyReturnSells = tempReturnSells.Any();

                    if (isAnySells || isAnyReturnSells)
                    {
                        itemsAndService = new ItemsAndService();

                        if(isAnySells)
                            itemsAndService.Amount = tempSells.Sum(x => x.Sum);

                        if (isAnyReturnSells)
                            itemsAndService.AmountReturn = tempReturnSells.Sum(x => x.Sum);

                        itemsAndService.Code = item.Code;
                        itemsAndService.Id = item.ID;

                        if (item.ItemType == ZhivarEnums.NoeItem.Item)
                        {
                            itemsAndService.IsGoods = true;
                        }
                        else
                        {
                            itemsAndService.IsGoods = false;
                        }
                            

                        itemsAndService.Name = item.Name;
                        itemsAndService.NodeName = itemGroup.Name;

                        if (isAnySells)
                            itemsAndService.Stock = tempSells.Sum(x => x.Quantity);

                        if (isAnyReturnSells)
                            itemsAndService.StockReturn = tempReturnSells.Sum(x => x.Quantity);

                        itemsAndService.Avg = (itemsAndService.Amount + itemsAndService.AmountReturn) / (itemsAndService.Stock + itemsAndService.StockReturn);

                        itemsAndServices.Add(itemsAndService);
                    }

                }
            }

           
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = itemsAndServices });
        }

        [HttpPost]
        [Route("GetItemsAndServicePurchases")]
        public  async Task<HttpResponseMessage> GetItemsAndServicePurchases()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(organId);

            InvoiceRule invoiceRule = new InvoiceRule();
            var invoices = await invoiceRule.GetAllByOrganIdAsync(organId);

            var invoiceBuys = invoices.Where(x => x.InvoiceType == ZhivarEnums.NoeFactor.Buy).ToList();
            var invoiceReturnBuys = invoices.Where(x => x.InvoiceType == ZhivarEnums.NoeFactor.ReturnBuy).ToList();

            var itemsAndServices = new List<ItemsAndService>();
            var itemsAndService = new ItemsAndService();

            var invoiceItemsBuys = new List<InvoiceItemVM>();
            var invoiceItemsReturnBuys = new List<InvoiceItemVM>();

            var tempBuys = new List<InvoiceItemVM>();
            var tempReturnBuys = new List<InvoiceItemVM>();

            foreach (var invoice in invoiceBuys)
            {
                foreach (var InvoiceItem in invoice.InvoiceItems ?? new List<InvoiceItemVM>())
                {
                    invoiceItemsBuys.Add(InvoiceItem);

                }
            }

            foreach (var invoice in invoiceReturnBuys)
            {
                foreach (var InvoiceItem in invoice.InvoiceItems ?? new List<InvoiceItemVM>())
                {
                    invoiceItemsReturnBuys.Add(InvoiceItem);

                }
            }


            foreach (var itemGroup in itemGroups)
            {
                foreach (var item in itemGroup.Items ?? new List<ItemVM>())
                {
                    tempBuys = invoiceItemsBuys.Where(x => x.ItemId == item.ID).ToList();
                    tempReturnBuys = invoiceItemsReturnBuys.Where(x => x.ItemId == item.ID).ToList();

                    var isAnyBuys = tempBuys.Any();
                    var isAnyReturnBuys = tempReturnBuys.Any();

                    if (isAnyBuys || isAnyReturnBuys)
                    {
                        itemsAndService = new ItemsAndService();

                        if (isAnyBuys)
                            itemsAndService.Amount = tempBuys.Sum(x => x.Sum);

                        if (isAnyReturnBuys)
                            itemsAndService.AmountReturn = tempReturnBuys.Sum(x => x.Sum);

                        itemsAndService.Code = item.Code;
                        itemsAndService.Id = item.ID;

                        if (item.ItemType == ZhivarEnums.NoeItem.Item)
                        {
                            itemsAndService.IsGoods = true;
                        }
                        else
                        {
                            itemsAndService.IsGoods = false;
                        }


                        itemsAndService.Name = item.Name;
                        itemsAndService.NodeName = itemGroup.Name;

                        if (isAnyBuys)
                            itemsAndService.Stock = tempBuys.Sum(x => x.Quantity);

                        if (isAnyReturnBuys)
                            itemsAndService.StockReturn = tempReturnBuys.Sum(x => x.Quantity);

                        itemsAndService.Avg = (itemsAndService.Amount + itemsAndService.AmountReturn) / (itemsAndService.Stock + itemsAndService.StockReturn);

                        itemsAndServices.Add(itemsAndService);
                    }

                }
            }


            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = itemsAndServices });
        }
        private async Task<string> CreateCodeItem(int organId)
        {
            var count = 0;
            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var itemGrouplst = await itemGroupRule.GetAllByOrganIdForCreateCodeAsync(organId);

            List<ItemVM> items = new List<ItemVM>();

            foreach (var itemGroup in itemGrouplst)
            {
                items.AddRange(itemGroup.Items);
            }

            var lastItem = items.OrderByDescending(x => x.ID).FirstOrDefault();


                if (lastItem != null)
                    count = Convert.ToInt32(lastItem.Code);


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


    //public class ItemsAndService
    //{
    //    public decimal Amount { get; set; }
    //    public decimal AmountReturn { get; set; }
    //    public decimal Avg { get; set; }
    //    public string Code { get; set; }
    //    public int Id { get; set; }
    //    public bool IsGoods { get; set; }
    //    public string Name { get; set; }
    //    public string NodeName { get; set; }
    //    public int Stock { get; set; }
    //    public int StockReturn { get; set; }
    //}
    }