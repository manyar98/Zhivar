using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.DataLayer.Context;
using Zhivar.DataLayer.Validation;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.Utilities;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using FluentValidation;
using Zhivar.ViewModel.Accounting;
using Zhivar.DomainClasses.BaseInfo;
using OMF.Common.Security;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.BaseInfo;
using OMF.EntityFramework.Ef6;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Workflow.Model;
using OMF.Workflow;
using static OMF.Workflow.Enums;
using static OMF.Common.Enums;
using OMF.Common.Extensions;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/Cost")]
    public class CostController : NewApiControllerBaseAsync<Cost, CostVM>
    {
        public CostRule Rule => this.BusinessRule as CostRule;

        protected override IBusinessRuleBaseAsync<Cost> CreateBusinessRule()
        {
            return new CostRule();
        }
        [HttpPost]
        [Route("loadCostData")]
        public async Task<HttpResponseMessage> loadCostData([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var resualt = new CostData();

            List<ContactVM> contacts = new List<ContactVM>();

            ContactRule contactRule = new ContactRule();
            var contactsSource = await contactRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
            contactsSource = contactsSource.ToList();

            contacts = Mapper.Map<IList<Contact>, List<ContactVM>>(contactsSource);

            foreach (var contact in contacts)
            {
                contact.DetailAccount = new DetailAccount()
                {
                    Code = contact.Code,
                    Id = (int)contact.ID,
                    Node = new Node()
                    {
                        FamilyTree = "اشخاص",
                        Id = (int)contact.ID,
                        Name = "اشخاص"
                    }
                };

                //var account1104 = await CalcAccountByCodeTafziliAsync(organId, "1104" + contact.Code);
                //var account1105 = await CalcAccountByCodeTafziliAsync(organId, "1105" + contact.Code);
                //var account2101 = await CalcAccountByCodeTafziliAsync(organId, "2101" + contact.Code);

                //contact.Balance = account1104.sumTotal + account1105.sumTotal + account2101.sumTotal;
                //contact.Credits = account1104.sumCredit + account1105.sumCredit + account2101.sumCredit;
                //contact.Liability = account1104.sumDebit + account1105.sumDebit + account2101.sumDebit;

            }



            resualt.contacts = contacts;
            resualt.costSettings = new InvoiceSettings()
            {
                allowApproveWithoutStock = false,
                autoAddTax = true,
                bottomMargin = "20",
                businessLogo = "",
                font = "Iransans",
                fontSize = "Medium",
                footerNote = "",
                footerNoteDraft = "",
                hideZeroItems = false,
                onlineInvoiceEnabled = false,
                pageSize = "A4portrait",
                payReceiptTitle = "رسید پرداخت وجه / چک",
                purchaseInvoiceTitle = "فاکتور خرید",
                receiveReceiptTitle = "رسید دریافت وجه / چک",
                rowPerPage = "18",
                saleDraftInvoiceTitle = "پیش فاکتور",
                saleInvoiceTitle = "صورتحساب فروش کالا و خدمات",
                showAmountInWords = false,
                showCustomerBalance = false,
                showItemUnit = false,
                showSignaturePlace = true,
                showTransactions = true,
                showVendorInfo = true,
                topMargin = "10",
                updateBuyPrice = false,
                updateSellPrice = false
            };

            AccountRule accountRule = new AccountRule();
            List<DomainClasses.Accounting.Account> accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var parentAccount = accounts.Where(x => x.ComplteCoding == "8205").SingleOrDefault();

            accounts = accounts.Where(x => x.ParentId == parentAccount.ID).ToList();

            resualt.items = accounts;

            var CostItems = new List<CostItemVM>();

            if (id == 0)
            {
                CostItems.Add(new CostItemVM()
                {
                    Description = "",
                    ID = 0,
                    RowNumber = 0,
                    Sum = 0,
                    Rest = 0
                });

                CostItems.Add(new CostItemVM()
                {
                    Description = "",
                    ID = 0,
                    RowNumber = 1,
                    Sum = 0,
                    Rest = 0
                });

                CostItems.Add(new CostItemVM()
                {
                    Description = "",
                    ID = 0,
                    RowNumber = 2,
                    Sum = 0,
                    Rest = 0
                });

                CostItems.Add(new CostItemVM()
                {
                    Description = "",
                    ID = 0,
                    RowNumber = 3,
                    Sum = 0,
                    Rest = 0
                });

                resualt.cost = new CostVM()
                {
                    Contact = null,
                    ContactTitle = "",
                    DateTime = DateTime.Now,
                    DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                    ID = 0,
                    CostItems = CostItems,
                    Number = await createNumberCost(organId),
                    Paid = 0,
                    Payable = 0,
                    Rest = 0,
                    Status = 0,
                    Sum = 0,
                    Explain = ""
                };
            }
            else
            {
                CostRule costRule = new CostRule();
                var cost = await costRule.FindAsync(id);

                foreach (var costItem in cost.CostItems ?? new List<CostItem>())
                {
                    CostItems.Add(new CostItemVM()
                    {
                        Description = costItem.Description,
                        ID = costItem.ID,
                        RowNumber = costItem.RowNumber,
                        Sum = costItem.Sum,
                        Rest = costItem.Rest,
                        Item = accounts.Where(x => x.ID == costItem.ItemId).SingleOrDefault(),
                        ItemId = costItem.ItemId
                    });

                }

                var contact = new Contact();

                if (cost.ContactId > 0)
                {
                    contact = await this.Rule.UnitOfWork.RepositoryAsync<Contact>().Queryable().Where(x => x.ID == cost.ContactId).SingleOrDefaultAsync2();
                }
            

                resualt.cost = new CostVM()
                {
                     Contact = Mapper.Map<Contact, ContactVM>(contact),
                     ContactTitle = contact.Name,
                    DateTime = cost.DateTime,
                    DisplayDate = cost.DisplayDate,
                    ID = cost.ID,
                    CostItems = CostItems,
                    Number = cost.Number,
                    Paid = cost.Paid,
                    Payable = cost.Payable,
                    Rest = cost.Rest,
                    Status = cost.Status,
                    Sum = cost.Sum,
                    Explain = cost.Explain
                };

                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }


        private async Task<string> createNumberCost(int organId)
        {
            var count = 0;
            CostRule costRule = new CostRule();
            var costQuery = await costRule.GetAllByOrganIdAsync(organId);

            count = costQuery.Count();
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

        [HttpPost]
        [Route("GetCosts")]
        public  async Task<HttpResponseMessage> GetCosts()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            CostRule costRule = new CostRule();
            var costQuery = await costRule.GetAllByOrganIdAsync(organId);

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = costQuery.ToList() });
        }


        public async Task<HttpResponseMessage> SaveCost([FromBody] CostVM costVM)
        {
            try
            {
                bool ShouldSend = false;

                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                FinanYearRule finanYearRule = new FinanYearRule();
                var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
                var finanYear = finanYears.Where(x => x.Closed == false && x.OrganId == organId);

                costVM.CostItems = costVM.CostItems.Where(x => x.Item != null && x.Sum > 0).ToList();
                foreach (var costItem in costVM.CostItems)
                {
                    if (costItem.Item != null)
                        costItem.ItemId = costItem.Item.ID;
                }
                Cost cost = new Cost();
                Mapper.Map(costVM, cost);


                cost.OrganId = organId;

                if (costVM.Contact != null)
                {

                    cost.ContactId = costVM.Contact.ID;//.DetailAccount.Node.Id;
                }
                cost.CostItems = cost.CostItems.Where(x => x.ItemId != 0 && x.Sum > 0).ToList();

                CostValidate validator = new CostValidate();
                FluentValidation.Results.ValidationResult results = validator.Validate(cost);

                string failurs = "";

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs += "<br/>" + error.ErrorMessage;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }

                if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager"))
                {
                    cost.Status = ZhivarEnums.CostStatus.WaitingToPay;
                }
                else if (costVM.Status == ZhivarEnums.CostStatus.WaitingToPay)
                {
                    ShouldSend = true;
                }
                else
                {
                    cost.Status = ZhivarEnums.CostStatus.Temporary;
                }

                //if (cost.Status == ZhivarEnums.CostStatus.WaitingToPay)
                //{
                //    var document = await Rule.RegisterDocument(costVM, organId);
                //    DocumentRule documentRule = new DocumentRule();
                //    await documentRule.InsertAsync(document, organId);
                //}

                if (cost.ID > 0)
                {
                    foreach (var costItem in cost.CostItems)
                    {
                        costItem.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                        costItem.CostId = cost.ID;
                    }

                    cost.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                }
                else
                {
                    foreach (var costItem in cost.CostItems)
                    {
                        costItem.ObjectState = OMF.Common.Enums.ObjectState.Added;
                    }

                    cost.ObjectState = OMF.Common.Enums.ObjectState.Added;
                }

                Rule.InsertOrUpdateGraph(cost);
        


                if (costVM.ID <= 0)
                {
                    if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager"))
                    {
                        var document = await Rule.RegisterDocument(costVM, organId);
                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(document, organId);
                        await documentRule.SaveChangesAsync();
                        cost.DocumentId = document.ID;
                    }
                   
                }

                await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                if (ShouldSend)
                {

                    await StartWorkFlow(new WorkFlowBusiClass()
                    {
                        Code = "Cost",
                        ID = cost.ID,
                        OrganizationID = 22,
                        InstanceTitle = cost.Explain
                    }, true);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = cost });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<HttpResponseMessage> StartWorkFlow(WorkFlowBusiClass entity, bool shoulSend = false)
        {


            SecurityManager.ThrowIfUserContextNull();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    #region Validate


                    var cost = BusinessContext.GetBusinessRule<Cost>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                           .Queryable().Where(dr => dr.ID == entity.ID).SingleOrDefault();


                    if (cost.Status != ZhivarEnums.CostStatus.Temporary && !shoulSend)
                    {
                        throw new OMFValidationException("این رسید هزینه قبلا ارسال شده است.");
                    }

                    var workFlowQuery = uow.Repository<WorkflowInfo>()
                                                                     .Queryable()
                                                                     .Where(wf => wf.Code == entity.Code)
                                                                     .SingleOrDefault();
                    var workFlowID = workFlowQuery.ID;

                    var instanceQuery = uow.RepositoryAsync<WorkflowInstance>()
                                                                    .Queryable()
                                                                    .Where(ins => ins.RelatedRecordId == entity.ID
                                                                               && ins.WorkflowInfoId == workFlowID);
                    #endregion

                  

                    dynamic result1;
                 
                    result1 = await WorkflowManager.StartWorkflowAsync(new WorkflowStartInfo()
                    {
                        Code = entity.Code,
                        ExchangeData = entity.ExchangeData,
                        InstanceTitle = entity.InstanceTitle,
                        RelatedRecordId = entity.ID,
                        StarterOrganizationId = entity.OrganizationID,
                        StarterUserId = SecurityManager.CurrentUserContext.UserId,
                        StartType = OMF.Workflow.Enums.StartType.Request

                    });
                    if (result1.Code == 1)
                    {
                        var workflowInstance = uow.Repository<WorkflowInstance>().Queryable()
                                                  .Where(ins => ins.RelatedRecordId == entity.ID &&
                                                                ins.Status == WfStateStatus.Open
                                                            ).SingleOrDefault();
                        WFExchangeData ex = (WFExchangeData)workflowInstance.InitialExchangeData;

                        ex[WfConstants.StarterOrganizationIdKey] = Convert.ToString(entity.OrganizationID);
                        workflowInstance.InitialExchangeData = (string)ex;
                        workflowInstance.ObjectState = ObjectState.Modified;
                        uow.RepositoryAsync<WorkflowInstance>().Update(workflowInstance);
                        await uow.SaveChangesAsync();

                    }




                    if (result1.Code == 1)
                    {
                        //InvoiceRule invoiceRule = new InvoiceRule();
                        //await invoiceRule.UpdateVaziatInvoice(entity.ID, NoeInsertFactor.waitingForConfirmation);

                        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { records = "" } });
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { resultCode = (int)ResultCode.Exception, data = new { records = "" } });
                }
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }
        [HttpPost]
        [Route("LoadCostTransObj")]
        public async Task<HttpResponseMessage> LoadCostTransObj([FromBody] int id)
        {
            try
            {


                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var cost = await Rule.FindAsync(id);
                CostTransObj costTransObj = new CostTransObj();

                costTransObj.cost = Mapper.Map<CostVM>(cost);
                costTransObj.costItems = costTransObj.cost.CostItems;
                TransactionRule transactionRule = new TransactionRule();
                costTransObj.payments = await transactionRule.GetAllByCostIdAsync(id);

                for (int i = 0; i < costTransObj.costItems.Count; i++)
                {
                    costTransObj.costItems[i].Item = await this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().FindAsync(costTransObj.costItems[i].ItemId);
                }

                ContactRule contactRule = new ContactRule();
                costTransObj.cost.Contact = Mapper.Map<ContactVM>(await contactRule.FindAsync(cost.ContactId));
                CashRule cashRule = new CashRule();
                var cashes = await cashRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                costTransObj.cashes = Mapper.Map<List<CashVM>>(cashes);

                BankRule bankRule = new BankRule();
                var banks = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                costTransObj.banks = Mapper.Map<List<BankVM>>(banks);


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = costTransObj });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Route("GetNewCostAccountObject")]
        [HttpPost]
        public  async Task<HttpResponseMessage> GetNewCostAccountObject()
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
                        Code = await CreateCostAccountCode(organId),
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
                    Code = await CreateCostAccountCode(organId),
                },
                showItemUnit = false,


                itemUnits = units 
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = newObjectKala });
        }

        [Route("GetCostAccountById")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetCostAccountById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var costAccount = await accountRule.FindAsync(id);

            CostAccountVM costAccountVM = new CostAccountVM()
            {
                //Balance = costAccount.Balance,
                Code = costAccount.Coding,
                DetailAccount = new DetailAccount()
                {
                    Accounts = null,
                    Code = costAccount.Coding,
                    Id = costAccount.ID,
                },
                ID = costAccount.ID,
                Name = costAccount.Name,
                OrganId = costAccount.OrganId,
            };



            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = costAccountVM });
        }

        [Route("AddCostAccount")]
        [HttpPost]
        public  async Task<HttpResponseMessage> AddCostAccount(CostAccountVM costAccountVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var accountParent = accounts.Where(x => x.ComplteCoding == "8205").SingleOrDefault();
            var account = new DomainClasses.Accounting.Account();


            account.OrganId = organId;
            account.Coding = await CreateCostAccountCode(organId);
            account.ComplteCoding = "8205" + account.Coding;
            account.Level = ZhivarEnums.AccountType.Tafzeli;
            account.Name = costAccountVM.Name;
            account.ParentId = accountParent.ID;




            if (costAccountVM.ID.HasValue && costAccountVM.ID > 0)
            {
                account.ID = (int)costAccountVM.ID;
                accountRule.Update(account);
            }
            else
            {
                accountRule.Insert(account);
            }

            await accountRule.SaveChangesAsync();

            costAccountVM.DetailAccount.Node = new Node() {
                Id = account.ID,
                Name = account.Name,
                
            };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = costAccountVM });// RedirectToAction(MVC.cash.ActionNames.Index);
        }
   
        private async Task<AccountTafzili> CalcAccountByCodeTafziliAsync(int organId, string code)
        {
            var organId2 = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            decimal amount = 0;
            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var operatinIncomeAccount = accounts.Where(x => x.ComplteCoding == code).SingleOrDefault();




            var selected = transactions.Where(a => a.AccountId == operatinIncomeAccount.ID).ToList();

            AccountTafzili accountTafzili = new AccountTafzili();

            if (selected.Any())
            {
                accountTafzili.sumCredit = selected.Sum(x => x.Credit);
                accountTafzili.sumDebit = selected.Sum(x => x.Debit);
                accountTafzili.sumTotal = accountTafzili.sumDebit - accountTafzili.sumCredit;
            }

            return accountTafzili;
        }

        private async Task<string> CreateCostAccountCode(int organId)
        {
            var count = 0;
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var accountParent = accounts.Where(x => x.ComplteCoding == "8205").SingleOrDefault();
            var lastAccount = accounts.Where(x => x.ParentId == accountParent.ID).OrderByDescending(x => x.ID).FirstOrDefault();


            if (lastAccount != null)
                count = Convert.ToInt32(lastAccount.Coding);


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

        [Route("DeleteCosts")]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteCosts([FromBody] string strIds)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                string failurs = "";

                string[] values = strIds.Split(',');
                for (int i = 0; i < values.Length - 1; i++)
                {

                    var id = Convert.ToInt32(values[i].Trim());

                    PayRecevieRule payRecevieRule = new PayRecevieRule();
                    var payRecevieQuery = await payRecevieRule.GetByCostIdAsync(id);

                    if (payRecevieQuery.Any())
                    {
                        failurs += "<br/>" + "برای این صورت هزینه دریافت/ پرداخت انجام شده است برای حذف باید دریافت و پرداخت حذف گردد.";
                    }
                    else
                    {
                        CostRule costRule = new CostRule();
                        await costRule.DeleteAsync(id);
                        await costRule.SaveChangesAsync();
                    }
                }



                if (!string.IsNullOrEmpty(failurs))
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
            }

            catch (Exception ex)
            {

                throw;
            }
        }
    }

}

