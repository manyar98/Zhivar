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
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.Business.BaseInfo;
using OMF.EntityFramework.Ef6;
using static Zhivar.DomainClasses.ZhivarEnums;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Workflow;
using OMF.Workflow.Model;
using static OMF.Workflow.Enums;
using static OMF.Common.Enums;
using OMF.Common.Extensions;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/Invoice")]
    public class InvoiceController : NewApiControllerBaseAsync<Invoice, InvoiceVM>
    {
        public InvoiceRule Rule => this.BusinessRule as InvoiceRule;

        protected override IBusinessRuleBaseAsync<Invoice> CreateBusinessRule()
        {
            return new InvoiceRule();
        }

     
        public async Task<HttpResponseMessage> loadInvoiceData([FromBody] int id)
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var resualt = new InvoiceData();

            List<ContactVM> contacts = new List<ContactVM>();
            ContactRule contactRule = new ContactRule();

            var contactsSource = await contactRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
            contactsSource = contactsSource.ToList();

            contacts = TranslateHelper.TranslateEntityToEntityVMListContact(contactsSource);

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
            resualt.invoiceSettings = new InvoiceSettings()
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

            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            var items = new List<ItemVM>();
            var item = new ItemVM();

            foreach (var itemGroup in itemGroups)
            {
                foreach (var KalaKhadmat in itemGroup.Items)
                {
                    item = new ItemVM()
                    {
                        Barcode = "",
                        BuyPrice = KalaKhadmat.BuyPrice,
                        DetailAccount = new DetailAccount()
                        {
                            Code = KalaKhadmat.Code,
                            Id = KalaKhadmat.ID,
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
                        Stock = KalaKhadmat.Stock,
                        Code = KalaKhadmat.Code,
                        IsGoods = KalaKhadmat.IsGoods,
                        IsService = KalaKhadmat.IsService,
                        MoneyStock = KalaKhadmat.MoneyStock,
                        OrganId = KalaKhadmat.OrganId,
                        ItemGroupId = KalaKhadmat.ItemGroupId

                    };

                    items.Add(item);
                }
            }

            resualt.items = items;

            var InvoiceItems = new List<InvoiceItemVM>();


            if (id == 0)
            {
                InvoiceItems.Add(new InvoiceItemVM()
                {
                    Description = "",
                    Discount = 0,
                    ID = 0,
                    Inv = null,
                    Item = null,
                    ItemInput = "",
                    Quantity = 0,
                    RowNumber = 0,
                    Sum = 0,
                    Tax = 0,
                    TotalAmount = 0,
                    Unit = 0,
                    UnitPrice = 0
                });

                InvoiceItems.Add(new InvoiceItemVM()
                {
                    Description = "",
                    Discount = 0,
                    ID = 0,
                    Inv = null,
                    Item = null,
                    ItemInput = "",
                    Quantity = 0,
                    RowNumber = 1,
                    Sum = 0,
                    Tax = 0,
                    TotalAmount = 0,
                    Unit = 0,
                    UnitPrice = 0
                });

                InvoiceItems.Add(new InvoiceItemVM()
                {
                    Description = "",
                    Discount = 0,
                    ID = 0,
                    Inv = null,
                    Item = null,
                    ItemInput = "",
                    Quantity = 0,
                    RowNumber = 2,
                    Sum = 0,
                    Tax = 0,
                    TotalAmount = 0,
                    Unit = 0,
                    UnitPrice = 0
                });

                InvoiceItems.Add(new InvoiceItemVM()
                {
                    Description = "",
                    Discount = 0,
                    ID = 0,
                    Inv = null,
                    Item = null,
                    ItemInput = "",
                    Quantity = 0,
                    RowNumber = 3,
                    Sum = 0,
                    Tax = 0,
                    TotalAmount = 0,
                    Unit = 0,
                    UnitPrice = 0
                });

                resualt.invoice = new InvoiceVM()
                {
                    Contact = null,
                    ContactTitle = "",
                    DateTime = DateTime.Now,
                    DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                    DisplayDueDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                    invoiceDueDate = PersianDateUtils.ToPersianDate(DateTime.Now),
       
                    DueDate = DateTime.Now,
                    ID = 0,
                    InvoiceItems = InvoiceItems,
                    InvoiceStatusString = "موقت",
                    InvoiceType = 0,
                    InvoiceTypeString = "فروش",
                    IsDraft = true,
                    IsPurchase = false,
                    IsPurchaseReturn = false,
                    IsSale = true,
                    IsSaleReturn = false,
                    IsWaste = false,
                    Note = "",
                    Number = await createNumberInvoice(organId),
                    Paid = 0,
                    Payable = 0,
                    Profit = 0,
                    Reference = "",
                    Rest = 0,
                    Returned = false,
                    Sent = false,
                    Status = 0,
                    Sum = 0,
                    Tag = ""
                };
            }
            else
            {

                var invoice = await Rule.FindAsync(id);

                foreach (var invoiceItem in invoice.InvoiceItems?? new List<InvoiceItem>())
                {
                    InvoiceItems.Add(new InvoiceItemVM()
                    {
                        Description = invoiceItem.Description,
                        Discount = invoiceItem.Discount,
                        ID = invoiceItem.ID,
                        Inv = invoiceItem.Inv,
                        Item =  Mapper.Map<Item,ItemVM>(this.BusinessRule.UnitOfWork.Repository<Item>().Find(invoiceItem.ItemId)),
                        ItemId = invoiceItem.ItemId,
                        ItemInput = invoiceItem.ItemInput,
                        Quantity = invoiceItem.Quantity,
                        RowNumber = invoiceItem.RowNumber,
                        Sum = invoiceItem.SumInvoiceItem,
                        Tax = invoiceItem.Tax,
                        TotalAmount = invoiceItem.TotalAmount,
                        Unit = invoiceItem.UnitInvoiceItem,
                        UnitPrice = invoiceItem.UnitPrice
                    });

                }

              

                resualt.invoice = new InvoiceVM()
                {
                    Contact =  Mapper.Map<Contact, ContactVM>(this.BusinessRule.UnitOfWork.Repository<Contact>().Find(invoice.ContactId)),
                    ContactTitle = invoice.ContactTitle,
                    DateTime = invoice.DateTime,
                    DisplayDate = invoice.DisplayDate,
                    DisplayDueDate = invoice.DisplayDueDate,
                    invoiceDueDate = invoice.DisplayDueDate,

                    DueDate = invoice.DueDate,
                    ID = invoice.ID,
                    InvoiceItems = InvoiceItems,
                    InvoiceStatusString = invoice.InvoiceStatusString,
                    InvoiceType = invoice.InvoiceType,
                    InvoiceTypeString = invoice.InvoiceTypeString,
                    IsDraft = invoice.IsDraft,
                    IsPurchase = invoice.IsPurchase,
                    IsPurchaseReturn = invoice.IsPurchaseReturn,
                    IsSale = invoice.IsSale,
                    IsSaleReturn = invoice.IsSaleReturn,
                    IsWaste = false,
                    Note = invoice.Note,
                    Number = invoice.Number,
                    Paid = invoice.Paid,
                    Payable = invoice.Payable,
                    Profit = invoice.Profit,
                    Reference = invoice.Refrence,
                    Rest = invoice.Rest,
                    Returned = invoice.Returned,
                    Sent = invoice.Sent,
                    Status = invoice.Status,
                    Sum = invoice.Sum,
                    Tag = invoice.Tag
                };

                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
        }

        //[Route("SaveInvoice")]
        //[HttpPost]
        //public  async Task<HttpResponseMessage> SaveInvoice([FromBody] Zhivar.ViewModel.Accunting.Invoice invoice)
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    if (!ModelState.IsValid)
        //    {

        //        //await Prepare();
        //        //var noe = (Enums.NoeInvoice)(Session[ZhivarConstants.NoeInvoice]);
        //        //if (noe == Enums.NoeInvoice.Forosh)
        //        //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceForosh);
        //        //else if (noe == Enums.NoeInvoice.Kharid)
        //        //    return RedirectToAction(MVC.Invoice.ActionNames.AddInvoiceKharid);
        //        //else if (noe == Enums.NoeInvoice.BargashtForosh)
        //        //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceBargashtForosh);
        //        //else if (noe == Enums.NoeInvoice.BargashtKharid)
        //        //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceBargashtKharid);
        //        //else
        //        //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceForosh);
        //    }

        //    var invoice = new Invoice();

        //    //hourOffShopModel.ShopId = shop.Id;
        //    //    Mapper.Map(invoiceVM, invoice);
        //    //     invoice.TarikhSarresid = PersianDateUtils.ToDateTime(invoiceVM.TarikhSarresid);
        //    //     invoice.TarikhInvoice = PersianDateUtils.ToDateTime(invoiceVM.TarikhInvoice);
        //    invoice.OrganId = organId;
        //    //    invoice.Noe = (Enums.NoeInvoice)(Session[ZhivarConstants.NoeInvoice]);


        //    //if (invoiceVM.ID.HasValue)
        //    //{
        //    //    invoiceRule.Update(invoice);

        //    //}
        //    //else
        //    //{
        //    //    invoiceRule.Insert(invoice);
        //    //}

        //    await _unitOfWork.SaveAllChangesAsync();

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)Enums.ResultCode.Successful, data = new { doc = 3, id = 3, number = 3, smsSent = true } });
        //    //if (invoice.Noe == Enums.NoeInvoice.Forosh)
        //    //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceForosh);
        //    //else if (invoice.Noe == Enums.NoeInvoice.Kharid)
        //    //    return RedirectToAction(MVC.Invoice.ActionNames.AddInvoiceKharid);
        //    //else if (invoice.Noe == Enums.NoeInvoice.BargashtForosh)
        //    //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceBargashtForosh);
        //    //else if (invoice.Noe == Enums.NoeInvoice.BargashtKharid)
        //    //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceBargashtKharid);
        //    //else
        //    //    return RedirectToAction(MVC.Invoice.ActionNames.ListInvoiceForosh);
        //}
        private async Task<string> createNumberInvoice(int organId)
        {
            var count = 0;
            var invoiceQuery = await Rule.GetAllByOrganIdAsync(organId);

            count = invoiceQuery.Count();
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
        [Route("GetInvoices")]
        public async Task<HttpResponseMessage> GetInvoices([FromBody] ZhivarEnums.NoeFactor invoiceType)
        {

            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var invoiceQuery = await Rule.GetAllByOrganIdAsync(organId);
                invoiceQuery = invoiceQuery.Where(x => x.InvoiceType == invoiceType).OrderByDescending(x => x.ID).ToList();



                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = invoiceQuery });

            }
            catch (Exception ex)
            {

                throw;
            }
          
        }

        [HttpPost]
        [Route("SaveInvoice")]
        public  async Task<HttpResponseMessage> SaveInvoice([FromBody] InvoiceVM invoiceVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                FinanYearRule finanYearRule = new FinanYearRule();
                var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
                var finanYear = finanYears.Where(x => x.Closed == false && x.OrganId == organId);

                invoiceVM.InvoiceItems = invoiceVM.InvoiceItems.Where(x => x.Item != null).ToList();

                foreach (var invoiceItem in invoiceVM.InvoiceItems)
                {
                    if (invoiceItem.Item != null)
                        invoiceItem.ItemId = invoiceItem.Item.ID;
                }

                if (invoiceVM.Contact != null)
                    invoiceVM.ContactId = invoiceVM.Contact.ID;

                Invoice invoice = new Invoice();
                Mapper.Map(invoiceVM, invoice);
                invoice.OrganId = organId;

                //if (invoice.Contact != null)
                //{

                //    invoice.ContactId = invoice.Contact.ID;
                //}

             //   invoice.InvoiceItems = invoice.InvoiceItems.Where(x => x.ItemId != 0).ToList();

            

                InvoiceValidate validator = new InvoiceValidate();
                FluentValidation.Results.ValidationResult results = validator.Validate(invoice);

                string failurs = "";

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs += "<br/>" + error.ErrorMessage;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }



                if (invoice.ID > 0)
                {
                    foreach (var invoiceItem in invoice.InvoiceItems)
                    {
                        invoiceItem.InvoiceId = invoice.ID;

                        if(invoiceItem.ID > 0)
                            invoiceItem.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                        else
                            invoiceItem.ObjectState = OMF.Common.Enums.ObjectState.Added;

                        //if (invoiceItem.Item != null)
                        //    invoiceItem.Item = null;//.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;
                    }

                    invoice.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                }

                else
                {
                    foreach (var invoiceItem in invoice.InvoiceItems)
                    {
                        invoiceItem.ObjectState = OMF.Common.Enums.ObjectState.Added;

                        //if (invoiceItem.Item != null)
                        //    invoiceItem.Item = null;//.ObjectState = OMF.Common.Enums.ObjectState.Unchanged;
                    }

                    invoice.ObjectState = OMF.Common.Enums.ObjectState.Added;
                }

               this.BusinessRule.UnitOfWork.RepositoryAsync<Invoice>().InsertOrUpdateGraph(invoice);
                ContactRule contactRule = new ContactRule();
                await contactRule.UpdateContact(invoice.InvoiceType, invoice.ContactId);
                //await this.BusinessRule.UnitOfWork.SaveChangesAsync();

                if (invoice.Status == ZhivarEnums.NoeInsertFactor.WaitingToReceive)
                {
                    var document = await Rule.RegisterDocumentAsync(invoiceVM, organId);

                    DocumentRule documentRule = new DocumentRule();
                    await documentRule.InsertAsync(document, organId);
                    await documentRule.SaveChangesAsync();

                    invoice.DocumentID = document.ID;
                    this.BusinessRule.UnitOfWork.RepositoryAsync<Invoice>().Update(invoice);
                    //await this.BusinessRule.UnitOfWork.SaveChangesAsync();


                }
                await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                // await RegisterDocument(invoice, organId);

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = invoice });
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
        [HttpPost]
        public async Task<HttpResponseMessage> StartWorkFlow(WorkFlowBusiClass entity)
        {


            SecurityManager.ThrowIfUserContextNull();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    #region Validate
    

                    var invoice = BusinessContext.GetBusinessRule<Invoice>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                           .Queryable().Where(dr => dr.ID == entity.ID).SingleOrDefault();


                    if (invoice.Status != NoeInsertFactor.Temporary)
                    {
                        throw new OMFValidationException("این فاکتور قبلا ارسال شده است.");
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

                    //if (vaziatDarkhast == Enums.VaziatDarkhast.SabteDarkhast && !await instanceQuery.Any())
                    //{

                    //   var darkhastRule = new DarkhastRule();

                    //   var validateStartWorkflow = new ValidateStartWorkflow();

                    //     var IsPaymentRequire = await darkhastRule.HasPaymentRequire(entity.NoeDarkhastID);

                    //validateStartWorkflow.Validate(new ValidateData
                    //{
                    //    darkhastID = entity.ID,
                    //    noeDarkhastID = entity.NoeDarkhastID,
                    //    IsPaymentRequire = IsPaymentRequire,
                    //    organID = entity.OrganizationID
                    //});

                    //  var validateExchangeData = new ValidateExchangeData();
                    //entity.ExchangeData = validateExchangeData.CreateExchange(new ValidateExData
                    //{
                    //    RelatedRecordID = entity.ID,
                    //    noeDarkhastID = entity.NoeDarkhastID,
                    //    ExchangeData = entity.ExchangeData,
                    //});

                    dynamic result1;
                    //if (entity.NoeDarkhastID == Enums.NoeDarkhastEnum.ENTEGHAL_SARMAIEH_ZABDARI)
                    //{
                    //    var daroukhanehID = Convert.ToInt32(entity.ExchangeData["daroukhanehShakhsIDDovom"]);
                    //    var organizationID = uow.Repository<Sherkat>().Queryable()
                    //                                  .Where(dr => dr.ID == daroukhanehID)
                    //                                  .SingleOrDefault().OrganID;
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
                        InvoiceRule invoiceRule = new InvoiceRule();
                        await invoiceRule.UpdateVaziatInvoice(entity.ID, NoeInsertFactor.waitingForConfirmation);

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

        public async Task<HttpResponseMessage> LoadInvoiceTransObj([FromBody] int id)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var invoice = await Rule.FindAsync(id);
                InvoiceTransObj invoiceTransObj = new InvoiceTransObj();

                invoiceTransObj.invoice = Mapper.Map<InvoiceVM>(invoice);
                invoiceTransObj.invoiceItems = invoiceTransObj.invoice.InvoiceItems;
                //invoiceTransObj.payments =  await transactionRule.GetAllByInvoiceIdAsync(id,true,false);
                PayRecevieRule payRecevieRule = new PayRecevieRule();
                invoiceTransObj.tempPayments = await payRecevieRule.GetTempPaymentsByInvoiceIdAsync(id);

                for (int i = 0; i < invoiceTransObj.invoiceItems.Count; i++)
                {
                    ItemRule itemRule = new ItemRule();
                    invoiceTransObj.invoiceItems[i].Item = Mapper.Map<ItemVM>(await itemRule.FindAsync(invoiceTransObj.invoiceItems[i].ItemId));
                }

                ContactRule contactRule = new ContactRule();
                invoiceTransObj.invoice.Contact = Mapper.Map<ContactVM>(await contactRule.FindAsync(invoice.ContactId));
                CashRule cashRule = new CashRule();
                var cashes = await cashRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                invoiceTransObj.cashes = Mapper.Map<List<CashVM>>(cashes);

                BankRule bankRule = new BankRule();
                var banks = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                invoiceTransObj.banks = Mapper.Map<List<BankVM>>(banks);


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = invoiceTransObj });
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
      

  
        private async Task<AccountTafzili> CalcAccountByCodeTafziliAsync(int organId, string code)
        {
           // var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

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

        [HttpPost]
        [Route("DeleteInvoices")]
        public  async Task<HttpResponseMessage> DeleteInvoices([FromBody] string strIds)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            string failurs = "";

            string[] values = strIds.Split(',');
            for (int i = 0; i < values.Length -1 ; i++)
            {
                
                var id = Convert.ToInt32(values[i].Trim());

                DocumentRule documentRule = new DocumentRule();
                PayRecevieRule payRecevieRule = new PayRecevieRule();
                var payRecevieQuery = await payRecevieRule.GetByInvoiceIdAsync(id);

                if (payRecevieQuery.Any())
                {
                    failurs += "<br/>" + "برای این فاکنور دریافت/ پرداخت انجام شده است برای حذف باید دریافت و پرداخت حذف گردد.";
                }
                else
                {
                    await Rule.DeleteAsync(id);
                    await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                }
            }



            if (!string.IsNullOrEmpty(failurs))
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
        }
    }

    //public class AccountTafzili
    //{
    //    public decimal sumCredit { get; set; }
    //    public decimal sumDebit { get; set; }
    //    public decimal sumTotal { get; set; }
    //}
    public class WorkFlowBusiClass
    {
        public int ID { get; set; }
        public int? OrganizationID { get; set; }
        public string Code { get; set; }
        public string InstanceTitle { get; set; }
        //public Enums.NoeDarkhastEnum NoeDarkhastID { get; set; }
        public OMF.Workflow.WFExchangeData ExchangeData { get; set; }
    }
}
