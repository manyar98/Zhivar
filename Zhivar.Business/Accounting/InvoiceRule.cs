using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using System.Threading;
using Zhivar.ViewModel.Accunting;
using AutoMapper;
using Zhivar.DomainClasses;
using Zhivar.Utilities;
using Zhivar.Business.BaseInfo;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.Business.Accounting
{
    public partial class InvoiceRule : BusinessRuleBase<Invoice>
    {
        public InvoiceRule()
            : base()
        {

        }

        public InvoiceRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public InvoiceRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        //public bool Delete(Invoice invoice)
        //{
        //    try
        //    {
        //        _invoices.Attach(invoice);
        //        _invoices.Remove(invoice);



        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public IList<Invoice> GetAll()
        //{
        //    return _invoices.ToList();
        //}
        //public IList<Invoice> GetAllByOrganId(int organId)
        //{
        //    return _invoices.AsQueryable().Where(x => x.OrganId == organId).ToList();
        //}
        public async Task<List<InvoiceVM>> GetAllByOrganIdAsync(int organId)
        {
            //var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
            var invoiceQuery =  this.Queryable().Where(x => x.OrganId == organId);
            var invoiceItemQuery = this.unitOfWork.RepositoryAsync<InvoiceItem>().Queryable();
            var itemQuery = this.unitOfWork.RepositoryAsync<Item>().Queryable();

            var joinQuery = from invoice in invoiceQuery
                            join invoiceItem2 in (from invoiceItem in invoiceItemQuery
                                                  join item in itemQuery
                                                  on invoiceItem.ItemId equals item.ID
                                                  select new InvoiceItemVM
                                                  {
                                                      CalcTax = invoiceItem.CalcTax,
                                                      Description = invoiceItem.Description,
                                                      Discount = invoiceItem.Discount,
                                                      InvoiceId = invoiceItem.InvoiceId,
                                                      ID = invoiceItem.ID,
                                                      Inv = invoiceItem.Inv,
                                                      //Invoice = invoiceItem.Invoice,
                                                      Item = new ViewModel.BaseInfo.ItemVM() {
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
                                                          UnitID = item.UnitID
                                                          
                                                      },
                                                      ItemId = item.ID,
                                                      ItemInput = invoiceItem.ItemInput,
                                                      Quantity = invoiceItem.Quantity,
                                                      RowNumber = invoiceItem.RowNumber,
                                                      Sum = invoiceItem.SumInvoiceItem,
                                                      Tax = invoiceItem.Tax,
                                                      TotalAmount = invoiceItem.TotalAmount,
                                                      Unit = invoiceItem.UnitInvoiceItem,
                                                      UnitPrice = invoiceItem.UnitPrice,
                                                      PriceBazareab = invoiceItem.PriceBazareab,
                                                      PriceChap = invoiceItem.PriceChap,
                                                      PriceNasab = invoiceItem.PriceNasab,
                                                      PriceTarah = invoiceItem.PriceTarah

                                                  }) 
                                                  on invoice.ID equals invoiceItem2.InvoiceId into invoiceItemGroup
                            select new InvoiceVM
                            {
                                Sum = invoice.Sum,
                                Contact = new ViewModel.Common.ContactVM()
                                {
                                   // OrganId = invoice.Contact.OrganId,
                                   // Address = invoice.Contact.Address,
                                   // Balance = invoice.Contact.Balance,
                                   // City = invoice.Contact.City,
                                   // Code = invoice.Contact.Address,
                                   // ContactType = invoice.Contact.ContactType,
                                   // Credits = invoice.Contact.Credits,
                                   // EconomicCode = invoice.Contact.EconomicCode,
                                   //Email = invoice.Contact.Email,
                                   // Fax = invoice.Contact.Fax,
                                   // FirstName = invoice.Contact.FirstName,
                                   // LastName = invoice.Contact.LastName,
                                   // IsCustomer= invoice.Contact.IsCustomer,
                                   // ID = invoice.Contact.ID,
                                   // IsEmployee = invoice.Contact.IsEmployee,
   
                                   // IsShareHolder = invoice.Contact.IsShareHolder,
                                   // IsVendor = invoice.Contact.IsVendor,
                                   // Liability = invoice.Contact.Liability,
                                   // Mobile = invoice.Contact.Mobile,
                                   // Name = invoice.Contact.Name,
                                   // Website = invoice.Contact.Name,
                                   // NationalCode = invoice.Contact.Name,
                                   // Note = invoice.Contact.Name,
                                   // Phone = invoice.Contact.Name,
                                   // PostalCode = invoice.Contact.Name,
                                   // Rating = invoice.Contact.Rating,
                                   // RegistrationDate = invoice.Contact.RegistrationDate,
                                   // RegistrationNumber = invoice.Contact.RegistrationNumber,
                                   // SharePercent = invoice.Contact.SharePercent,
                                   // State = invoice.Contact.State,
                                },
                                ContactId = invoice.ContactId,
                                ContactTitle = invoice.ContactTitle,
                                DateTime = invoice.DateTime,
                                DisplayDate = invoice.DisplayDate,
                                DisplayDueDate = invoice.DisplayDueDate,
                                DueDate = invoice.DueDate,
                                ID = invoice.ID,
                                InvoiceItems = invoiceItemGroup.ToList(),
                                InvoiceStatusString = invoice.InvoiceStatusString,
                                InvoiceType = invoice.InvoiceType,
                                InvoiceTypeString = invoice.InvoiceTypeString,
                                IsDraft = invoice.IsDraft,
                                IsPurchase = invoice.IsPurchase,
                                IsPurchaseReturn = invoice.IsPurchaseReturn,
                                IsSale = invoice.IsSale,
                                IsSaleReturn = invoice.IsSaleReturn,
                                Note = invoice.Note,
                                Number = invoice.Number,
                                OrganId = invoice.OrganId,
                                Paid = invoice.Paid,
                                Payable = invoice.Payable,
                                Profit = invoice.Profit,
                                //Refrence = invoice.Refrence,
                                Rest = invoice.Rest,
                                Returned = invoice.Returned,
                                Sent = invoice.Sent,
                                Status = invoice.Status,
                                Tag = invoice.Tag,
                                IsContract = invoice.IsContract

                            };

            return await joinQuery.ToListAsync2();
        }

        public List<InvoiceVM> GetAllByOrganId(int organId)
        {
            //var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
            var invoiceQuery = this.Queryable().Where(x => x.OrganId == organId);
            var invoiceItemQuery = this.unitOfWork.RepositoryAsync<InvoiceItem>().Queryable();
            var itemQuery = this.unitOfWork.RepositoryAsync<Item>().Queryable();

            var joinQuery = from invoice in invoiceQuery
                            join invoiceItem2 in (from invoiceItem in invoiceItemQuery
                                                  join item in itemQuery
                                                  on invoiceItem.ItemId equals item.ID
                                                  select new InvoiceItemVM
                                                  {
                                                      CalcTax = invoiceItem.CalcTax,
                                                      Description = invoiceItem.Description,
                                                      Discount = invoiceItem.Discount,
                                                      InvoiceId = invoiceItem.InvoiceId,
                                                      ID = invoiceItem.ID,
                                                      Inv = invoiceItem.Inv,
                                                      //Invoice = invoiceItem.Invoice,
                                                      Item = new ViewModel.BaseInfo.ItemVM()
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
                                                          UnitID = item.UnitID
                                                      },
                                                      ItemId = item.ID,
                                                      ItemInput = invoiceItem.ItemInput,
                                                      Quantity = invoiceItem.Quantity,
                                                      RowNumber = invoiceItem.RowNumber,
                                                      Sum = invoiceItem.SumInvoiceItem,
                                                      Tax = invoiceItem.Tax,
                                                      TotalAmount = invoiceItem.TotalAmount,
                                                      Unit = invoiceItem.UnitInvoiceItem,
                                                      UnitPrice = invoiceItem.UnitPrice
                                                  })
                                                  on invoice.ID equals invoiceItem2.InvoiceId into invoiceItemGroup
                            select new InvoiceVM
                            {
                                Sum = invoice.Sum,
                                Contact = new ViewModel.Common.ContactVM()
                                {
                                    // OrganId = invoice.Contact.OrganId,
                                    // Address = invoice.Contact.Address,
                                    // Balance = invoice.Contact.Balance,
                                    // City = invoice.Contact.City,
                                    // Code = invoice.Contact.Address,
                                    // ContactType = invoice.Contact.ContactType,
                                    // Credits = invoice.Contact.Credits,
                                    // EconomicCode = invoice.Contact.EconomicCode,
                                    //Email = invoice.Contact.Email,
                                    // Fax = invoice.Contact.Fax,
                                    // FirstName = invoice.Contact.FirstName,
                                    // LastName = invoice.Contact.LastName,
                                    // IsCustomer= invoice.Contact.IsCustomer,
                                    // ID = invoice.Contact.ID,
                                    // IsEmployee = invoice.Contact.IsEmployee,

                                    // IsShareHolder = invoice.Contact.IsShareHolder,
                                    // IsVendor = invoice.Contact.IsVendor,
                                    // Liability = invoice.Contact.Liability,
                                    // Mobile = invoice.Contact.Mobile,
                                    // Name = invoice.Contact.Name,
                                    // Website = invoice.Contact.Name,
                                    // NationalCode = invoice.Contact.Name,
                                    // Note = invoice.Contact.Name,
                                    // Phone = invoice.Contact.Name,
                                    // PostalCode = invoice.Contact.Name,
                                    // Rating = invoice.Contact.Rating,
                                    // RegistrationDate = invoice.Contact.RegistrationDate,
                                    // RegistrationNumber = invoice.Contact.RegistrationNumber,
                                    // SharePercent = invoice.Contact.SharePercent,
                                    // State = invoice.Contact.State,
                                },
                                ContactId = invoice.ContactId,
                                ContactTitle = invoice.ContactTitle,
                                DateTime = invoice.DateTime,
                                DisplayDate = invoice.DisplayDate,
                                DisplayDueDate = invoice.DisplayDueDate,
                                DueDate = invoice.DueDate,
                                ID = invoice.ID,
                                InvoiceItems = invoiceItemGroup.ToList(),
                                InvoiceStatusString = invoice.InvoiceStatusString,
                                InvoiceType = invoice.InvoiceType,
                                InvoiceTypeString = invoice.InvoiceTypeString,
                                IsDraft = invoice.IsDraft,
                                IsPurchase = invoice.IsPurchase,
                                IsPurchaseReturn = invoice.IsPurchaseReturn,
                                IsSale = invoice.IsSale,
                                IsSaleReturn = invoice.IsSaleReturn,
                                Note = invoice.Note,
                                Number = invoice.Number,
                                OrganId = invoice.OrganId,
                                Paid = invoice.Paid,
                                Payable = invoice.Payable,
                                Profit = invoice.Profit,
                                //Refrence = invoice.Refrence,
                                Rest = invoice.Rest,
                                Returned = invoice.Returned,
                                Sent = invoice.Sent,
                                Status = invoice.Status,
                                Tag = invoice.Tag

                            };

            return joinQuery.ToList();
        }
        //public async Task<IList<Invoice>> GetAllAsync()
        //{
        //    return await _invoices.ToListAsync();
        //}
        //public Invoice GetById(int id)
        //{
        //    return _invoices.Where(x => x.ID == id).Include(x => x.InvoiceItems).SingleOrDefault();
        //}


        //public async Task<bool> Insert(Invoice invoice)
        //{
        //    try
        //    {
        //        //var finanYear = await _finanYears.Where(x => x.Closed == false).SingleOrDefaultAsync();
        //        //invoice.FinanYear = finanYear;
        //        //invoice.FinanYearId = finanYear.ID;

        //        if (invoice.Contact != null)
        //            _uow.Entry(invoice.Contact).State = EntityState.Unchanged;

        //        foreach (var invoiceItem in invoice.InvoiceItems ?? new List<InvoiceItem>())
        //        {
        //            if (invoiceItem.Item != null)
        //            {
        //                _uow.Entry(invoiceItem.Item).State = EntityState.Unchanged;

        //                if (invoiceItem.Item.ItemGroup != null)
        //                {
        //                    _uow.Entry(invoiceItem.Item.ItemGroup).State = EntityState.Unchanged;
        //                }
        //            }
        //        }
        //        _invoices.Add(invoice);
        //        return true;
        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }
        //}

        //public async Task<bool> Update(Invoice invoice)
        //{
        //    try
        //    {
        //        var local = _uow.Set<Invoice>()
        //             .Local
        //             .FirstOrDefault(f => f.ID == invoice.ID);
        //        if (local != null)
        //        {
        //            _uow.Entry(local).State = EntityState.Detached;
        //        }



        //        if (invoice.Contact != null)
        //            _uow.Entry(invoice.Contact).State = EntityState.Unchanged;

        //        foreach (var invoiceItem in invoice.InvoiceItems ?? new List<InvoiceItem>())
        //        {
        //            if (invoiceItem.Item != null)
        //            {
        //                invoiceItem.ItemId = invoiceItem.Item.ID;
        //                _uow.Entry(invoiceItem.Item).State = EntityState.Unchanged;

        //                if (invoiceItem.Item.ItemGroup != null)
        //                {
        //                    _uow.Entry(invoiceItem.Item.ItemGroup).State = EntityState.Unchanged;
        //                }
        //            }

        //            invoiceItem.InvoiceId = invoice.ID;
        //            if (invoiceItem.ID > 0)
        //                _uow.Entry(invoiceItem).State = EntityState.Modified;
        //            else
        //                _uow.Entry(invoiceItem).State = EntityState.Added;
        //        }
        //        _invoices.Attach(invoice);

        //        _uow.Entry(invoice).State = EntityState.Modified;
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}



        protected override Invoice FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.InvoiceItems == null)
            {
                this.LoadCollection<InvoiceItem>(entity, x => x.InvoiceItems);

            }

            //if (entity.Contact == null)
            //{
            //    this.LoadReference<Contact>(entity, x => x.Contact);
            //}

            return entity;
        }

        protected async override Task<Invoice> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.InvoiceItems == null)
            {
                await this.LoadCollectionAsync<InvoiceItem>(entity, x => x.InvoiceItems);

                

            }

            //if (entity.Contact == null)
            //{
            //    await this.LoadReferenceAsync<Contact>(entity, x => x.Contact);
            //}

            return entity;
        }

        public async Task<IList<InvoiceVM>> GetAllByOrganIdAsync(int organId, DomainClasses.ZhivarEnums.NoeFactor noeInvoice)
        {
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();

            var invoices = await this.Queryable().Where(x => x.OrganId == organId && x.InvoiceType == noeInvoice).ToListAsync2();
            List<InvoiceVM> invoiceVMs = new List<InvoiceVM>();

            Mapper.Map(invoices, invoiceVMs);

            return invoiceVMs;

        }
        public async Task<List<InvoiceItemContactItem>> GetAllByContactIdAsync(int organId, int contactId)
        {
            var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();

            var invoicesQuery = this.Queryable().Where(x => x.OrganId == organId && x.ContactId == contactId);
            var invoiceItemsQuery = this.unitOfWork.RepositoryAsync<InvoiceItem>().Queryable();
            var itemsQuery = this.unitOfWork.RepositoryAsync<Item>().Queryable();
            var documentsQuery = this.unitOfWork.RepositoryAsync<Document>().Queryable().Where(x => x.OrganId == organId);
            var transactionsQuery = this.unitOfWork.RepositoryAsync<Transaction>().Queryable();
            var itemQuery = this.unitOfWork.RepositoryAsync<Item>().Queryable();

            var joinQuery = from invoices in invoicesQuery
                            join invoiceItems in invoiceItemsQuery
                            on invoices.ID equals invoiceItems.InvoiceId
                            join item in itemQuery
                            on invoiceItems.ItemId equals item.ID
                            //join transactions in transactionsQuery
                            //on invoices.ID equals transactions.InvoiceId
                            //join documents in documentsQuery
                            //on transactions.DocumentId equals documents.ID
                            select new InvoiceItemContactItem
                            {
                                // DocId = documents.ID,
                                Code = item.Code,
                                DateTime = invoices.DisplayDate,//.DateTime,
                                Discount = invoiceItems.Discount,
                                InvoiceId = invoices.ID,
                                ItemId = item.ID,
                                Name = item.Name,
                                Number = invoices.Number,
                                Quantity = invoiceItems.Quantity,
                                Reference = invoices.Refrence,
                                Tax = invoiceItems.Tax,
                                TotalAmount = invoiceItems.TotalAmount,
                                Type = invoices.InvoiceType,
                                Unit = invoiceItems.UnitInvoiceItem,
                                UnitPrice = invoiceItems.UnitPrice
                            };


            return await joinQuery.ToListAsync2();

        }

        public async Task UpdateVaziatInvoice(int id, ZhivarEnums.NoeInsertFactor vaziat)
        {
            var entity = BusinessContext.GetBusinessRule<Invoice>(this.OperationAccess, this.UnitOfWork).Find(id);

            entity.Status = vaziat;

            this.UpdateEntity(entity);
            await SaveChangesAsync();
            
        }
        public Invoice ConvertContractToInvoice(int contractId, ZhivarEnums.NoeFactor invoiceType)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    var contract = uow.Repository<DomainClasses.Contract.Contract>().Find(contractId);

                    Contact contact = new Contact();

                    if (contract != null)
                    {
                        contact = uow.Repository<Contact>().Find(contract.ContactId);
                    }

                    contract.Contract_Sazes = new List<Contract_Saze>();

                    contract.Contract_Sazes = uow.Repository<Contract_Saze>().Queryable().Where(x => x.ContractID == contractId).ToList();
                    contract.Contract_PayRecevies = uow.Repository<Contract_PayRecevies>().Queryable().Where(x => x.ContractId == contractId).ToList();

                    foreach (var contract_PayRecevie in contract.Contract_PayRecevies ?? new List<Contract_PayRecevies>())
                    {
                        contract_PayRecevie.Contract_DetailPayRecevies = uow.Repository<Contract_DetailPayRecevies>().Queryable().Where(x => x.Contract_PayRecevieId == contract_PayRecevie.ID).ToList();
                    }


                    var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                    FinanYearRule finanYearRule = new FinanYearRule();
                    var finanYears = finanYearRule.GetAllByOrganId(organId);
                    var finanYear = finanYears.Where(x => x.Closed == false && x.OrganId == organId);

                    Invoice invoice = new Invoice();

                    if (contract.ContractType == ZhivarEnums.ContractType.RentTo)
                    {
                        invoice.InvoiceStatusString = "فاکتور اجاره";
                        invoice.InvoiceTypeString = "اجاره";
                        invoice.InvoiceType = ZhivarEnums.NoeFactor.RentTo;
                    }
                    else if (contract.ContractType == ZhivarEnums.ContractType.RentFrom)
                    {
                        invoice.InvoiceStatusString = "فاکتور اجاره از صاحب رسانه";
                        invoice.InvoiceTypeString = "اجاره از صاحب رسانه";
                        invoice.InvoiceType = ZhivarEnums.NoeFactor.RentFrom;
                    }

                    invoice.ContactId = contract.ContactId;
                    invoice.ContactTitle = contact.Name;
                    invoice.DateTime = contract.DateTime;
                    invoice.DisplayDate = contract.DisplayDate;
                    invoice.DisplayDueDate = contract.DisplayDueDate;
                    invoice.DueDate = contract.DueDate;
                    invoice.IsDraft = contract.IsDraft;
                    invoice.IsPurchase = false;
                    invoice.IsPurchaseReturn = false;
                    invoice.IsSale = false;
                    invoice.IsSaleReturn = false;
                    invoice.Note = contract.Note;
                    invoice.Number = createNumberInvoice(organId);
                    invoice.ObjectState = Enums.ObjectState.Added;
                    invoice.OrganId = contract.OrganId;
                    invoice.Paid = contract.Paid;
                    invoice.Payable = contract.Payable;
                    invoice.Profit = contract.Profit;
                    invoice.Refrence = contract.Refrence;
                    invoice.Rest = contract.Rest;
                    invoice.Returned = false;
                    invoice.Sent = false;
                    invoice.Status = ZhivarEnums.NoeInsertFactor.WaitingToReceive;
                    invoice.Sum = contract.Sum;
                    invoice.Tag = contract.Tag;
                    invoice.IsContract = true;
                    uow.Repository<Invoice>().Insert(invoice);
                    //  this.UnitOfWork.RepositoryAsync<Invoice>().Insert(invoice);

                    //this.UnitOfWork.SaveChanges();
                    uow.SaveChanges();

                    invoice.InvoiceItems = new List<InvoiceItem>();
                    InvoiceItem invoiceItem = new InvoiceItem();
                    foreach (var contract_Sazes in contract.Contract_Sazes ?? new List<Contract_Saze>())
                    {
                        invoiceItem.CalcTax = contract_Sazes.CalcTax;
                        invoiceItem.Description = contract_Sazes.Description;
                        invoiceItem.Discount = contract_Sazes.Discount;
                        invoiceItem.InvoiceId = invoice.ID;
                        invoiceItem.Quantity = contract_Sazes.Quantity;
                        invoiceItem.RowNumber = contract_Sazes.RowNumber;
                        invoiceItem.SumInvoiceItem = contract_Sazes.Sum;
                        invoiceItem.Tax = contract_Sazes.Tax;
                        invoiceItem.TotalAmount = contract_Sazes.TotalAmount;
                        invoiceItem.UnitPrice = contract_Sazes.UnitPrice;
                        invoiceItem.UnitInvoiceItem = contract_Sazes.UnitItem;
                        invoiceItem.ItemId = MapSazeToItem(contract_Sazes.SazeId);
                        invoiceItem.PriceBazareab = contract_Sazes.PriceBazareab;
                        invoiceItem.PriceTarah = contract_Sazes.PriceTarah;
                        invoiceItem.PriceChap = contract_Sazes.PriceChap;
                        invoiceItem.PriceNasab = contract_Sazes.PriceNasab;

                        invoiceItem.ObjectState = Enums.ObjectState.Added;
                        //invoice.InvoiceItems.Add(invoiceItem);
                       // this.UnitOfWork.RepositoryAsync<InvoiceItem>().InsertOrUpdateGraph(invoiceItem);
                        uow.Repository<InvoiceItem>().InsertOrUpdateGraph(invoiceItem);

                        //this.UnitOfWork.SaveChanges();
                        uow.SaveChanges();
                    }




                    //this.UnitOfWork.RepositoryAsync<Invoice>().InsertOrUpdateGraph(invoice);

                    //this.UnitOfWork.SaveChangesAsync();

                    #region Pardakh
                    foreach (var Contract_PayRecevie in contract.Contract_PayRecevies ?? new List<Contract_PayRecevies>())
                    {
                        PayRecevie payRecevie = new PayRecevie();
                        payRecevie.Items = new List<DetailPayRecevie>();

                        payRecevie.ContactId = contract.ID;

                        payRecevie.InvoiceId = invoice.ID;

                        payRecevie.OrganId = organId;

                        payRecevie.Type = payRecevie.Type;
                        payRecevie.Date = DateTime.Now;
                        payRecevie.DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);// payRecevie.DisplayDate;
                        PayRecevieRule PayRecevieRule = new PayRecevieRule(this.unitOfWork);
                        var payRecevies = PayRecevieRule.GetAllByOrganId(organId);
                        var lastPayRecevies = payRecevies.OrderByDescending(x => x.ID).FirstOrDefault();

                        if (lastPayRecevies != null)
                            payRecevie.Number = lastPayRecevies.Number + 1;
                        else
                            payRecevie.Number = 1;

                        payRecevie.InvoiceId = invoice.ID;

                        if(invoiceType == ZhivarEnums.NoeFactor.RentFrom)
                            payRecevie.IsReceive = false;
                        else
                            payRecevie.IsReceive = true;

                        payRecevie.OrganId = organId;
                        payRecevie.Status = ZhivarEnums.Status.Temporary;
                        payRecevie.Type = ZhivarEnums.PayRecevieType.Sir;
                        payRecevie.ContactId = contract.ContactId;

                        payRecevie.ObjectState = OMF.Common.Enums.ObjectState.Added;

                        uow.Repository<PayRecevie>().Insert(payRecevie);
                        uow.SaveChanges();
                        // payRecevie.Date = DateTime.Now;
                        //payRecevie.DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);
                        decimal amount = 0;

                        foreach (var contract_DetailPayRecevie in Contract_PayRecevie.Contract_DetailPayRecevies ?? new List<Contract_DetailPayRecevies>())
                        {
                            DetailPayRecevie detailPayRecevie = new DetailPayRecevie();
                            amount += contract_DetailPayRecevie.Amount;

                            if (contract_DetailPayRecevie.Cash != null)
                            {
                                detailPayRecevie.CashId = contract_DetailPayRecevie.Cash.ID;
                            }
                            else if (contract_DetailPayRecevie.Bank != null)
                            {
                                detailPayRecevie.BankId = contract_DetailPayRecevie.Bank.ID;
                            }
                            else if (contract_DetailPayRecevie.Cheque != null && contract_DetailPayRecevie.Type == ZhivarEnums.DetailPayReceiveType.Cheque)
                            {
                            }

                            detailPayRecevie.ObjectState = Enums.ObjectState.Added;
                            detailPayRecevie.PayRecevieId = payRecevie.ID;
                            detailPayRecevie.Amount = contract_DetailPayRecevie.Amount;
                            detailPayRecevie.Type = contract_DetailPayRecevie.Type;

                            //payRecevie.Items.Add(detailPayRecevie);
                            uow.Repository<DetailPayRecevie>().Insert(detailPayRecevie);
                            uow.SaveChanges();
                        }
                        payRecevie.Amount = amount;
        

                
                        uow.Repository<PayRecevie>().Update(payRecevie);
                        //unitOfWork.RepositoryAsync<PayRecevie>().InsertOrUpdateGraph(payRecevie);

                        //unitOfWork.SaveChangesAsync();
                        uow.SaveChanges();
                    }



                    #endregion
                    contract.InvoiceId = invoice.ID;
                    uow.Repository<DomainClasses.Contract.Contract>().Update(contract);
                    uow.SaveChanges();
                    // this.UnitOfWork.RepositoryAsync<DomainClasses.Contract.Contract>().Update(contract);
                    //this.UnitOfWork.SaveChangesAsync();

                    return invoice;
                }
              
            }
            catch (Exception ex)
            {

                throw;
            }
         

        }
        public async Task<Invoice> ConvertContractToInvoiceAsync(int contractId)
        {
            try
            {
                var contract = BusinessContext.GetBusinessRule<DomainClasses.Contract.Contract>(this.OperationAccess, this.UnitOfWork).Find(contractId);
                contract.Contract_Sazes = new List<DomainClasses.Contract.Contract_Saze>();

                contract.Contract_Sazes = BusinessContext.GetBusinessRule<Contract_Saze>(this.OperationAccess, this.UnitOfWork).Queryable().Where(x => x.ContractID == contractId).ToList();
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                FinanYearRule finanYearRule = new FinanYearRule();
                var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
                var finanYear = finanYears.Where(x => x.Closed == false && x.OrganId == organId);

                Invoice invoice = new Invoice();

                if (contract.ContractType == ZhivarEnums.ContractType.RentTo)
                {
                    invoice.InvoiceStatusString = "فاکتور اجاره";
                    invoice.InvoiceTypeString = "اجاره";
                    invoice.InvoiceType = ZhivarEnums.NoeFactor.RentTo;
                }
                else if (contract.ContractType == ZhivarEnums.ContractType.RentFrom)
                {
                    invoice.InvoiceStatusString = "فاکتور اجاره از صاحب رسانه";
                    invoice.InvoiceTypeString = "اجاره از صاحب رسانه";
                    invoice.InvoiceType = ZhivarEnums.NoeFactor.RentFrom;
                }


                invoice.ContactId = contract.ContactId;
                invoice.ContactTitle = contract.ContractTitle;
                invoice.DateTime = contract.DateTime;
                invoice.DisplayDate = contract.DisplayDate;
                invoice.DisplayDueDate = contract.DisplayDueDate;
                invoice.DueDate = contract.DueDate;
                invoice.IsDraft = contract.IsDraft;
                invoice.IsPurchase = false;
                invoice.IsPurchaseReturn = false;
                invoice.IsSale = false;
                invoice.IsSaleReturn = false;
                invoice.Note = contract.Note;
                invoice.Number = createNumberInvoice(organId);
                invoice.ObjectState = Enums.ObjectState.Added;
                invoice.OrganId = contract.OrganId;
                invoice.Paid = contract.Paid;
                invoice.Payable = contract.Payable;
                invoice.Profit = contract.Profit;
                invoice.Refrence = contract.Refrence;
                invoice.Rest = contract.Rest;
                invoice.Returned = false;
                invoice.Sent = false;
                invoice.Status = ZhivarEnums.NoeInsertFactor.WaitingToReceive;
                invoice.Sum = contract.Sum;
                invoice.Tag = contract.Tag;
                invoice.IsContract = true;

                this.UnitOfWork.RepositoryAsync<Invoice>().Insert(invoice);

                this.UnitOfWork.SaveChanges();

                invoice.InvoiceItems = new List<InvoiceItem>();
                InvoiceItem invoiceItem = new InvoiceItem();
                foreach (var contract_Sazes in contract.Contract_Sazes ?? new List<DomainClasses.Contract.Contract_Saze>())
                {
                    invoiceItem.CalcTax = contract_Sazes.CalcTax;
                    invoiceItem.Description = contract_Sazes.Description;
                    invoiceItem.Discount = contract_Sazes.Discount;
                    invoiceItem.InvoiceId = invoice.ID;
                    invoiceItem.Quantity = contract_Sazes.Quantity;
                    invoiceItem.RowNumber = contract_Sazes.RowNumber;
                    invoiceItem.SumInvoiceItem = contract_Sazes.Sum;
                    invoiceItem.Tax = contract_Sazes.Tax;
                    invoiceItem.TotalAmount = contract_Sazes.TotalAmount;
                    invoiceItem.UnitPrice = contract_Sazes.UnitPrice;
                    invoiceItem.UnitInvoiceItem = contract_Sazes.UnitItem;
                    invoiceItem.ItemId = MapSazeToItem(contract_Sazes.SazeId);
                    invoiceItem.PriceBazareab = contract_Sazes.PriceBazareab;
                    invoiceItem.PriceTarah = contract_Sazes.PriceTarah;
                    invoiceItem.PriceChap = contract_Sazes.PriceChap;
                    invoiceItem.PriceNasab = contract_Sazes.PriceNasab;

                    invoiceItem.ObjectState = Enums.ObjectState.Added;
                    //invoice.InvoiceItems.Add(invoiceItem);
                    this.UnitOfWork.RepositoryAsync<InvoiceItem>().InsertOrUpdateGraph(invoiceItem);

                    this.UnitOfWork.SaveChanges();
                }




                this.UnitOfWork.RepositoryAsync<Invoice>().InsertOrUpdateGraph(invoice);

                await this.UnitOfWork.SaveChangesAsync();

                #region Pardakh
                foreach (var Contract_PayRecevie in contract.Contract_PayRecevies ?? new List<Contract_PayRecevies>())
                {
                    PayRecevie payRecevie = new PayRecevie();
                    payRecevie.Items = new List<DetailPayRecevie>();

                    payRecevie.ContactId = contract.ID;

                    payRecevie.InvoiceId = invoice.ID;

                    payRecevie.OrganId = organId;

                    payRecevie.Type = payRecevie.Type;
                    payRecevie.Date = PersianDateUtils.ToDateTime(payRecevie.DisplayDate);
                    payRecevie.DisplayDate = payRecevie.DisplayDate;
                    // payRecevie.Date = DateTime.Now;
                    //payRecevie.DisplayDate = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now);
                    decimal amount = 0;

                    foreach (var contract_DetailPayRecevie in Contract_PayRecevie.Contract_DetailPayRecevies ?? new List<Contract_DetailPayRecevies>())
                    {
                        DetailPayRecevie detailPayRecevie = new DetailPayRecevie();
                        amount += contract_DetailPayRecevie.Amount;

                        if (contract_DetailPayRecevie.Cash != null)
                        {
                            detailPayRecevie.CashId = contract_DetailPayRecevie.Cash.ID;
                        }
                        else if (contract_DetailPayRecevie.Bank != null)
                        {
                            detailPayRecevie.BankId = contract_DetailPayRecevie.Bank.ID;
                        }
                        else if (contract_DetailPayRecevie.Cheque != null && contract_DetailPayRecevie.Type == ZhivarEnums.DetailPayReceiveType.Cheque)
                        {
                        }

                        detailPayRecevie.ObjectState = Enums.ObjectState.Added;
                        detailPayRecevie.PayRecevieId = payRecevie.ID;
                        detailPayRecevie.Amount = contract_DetailPayRecevie.Amount;
                        detailPayRecevie.Type = 0;

                        payRecevie.Items.Add(detailPayRecevie);

                    }
                    payRecevie.Amount = amount;
                    PayRecevieRule PayRecevieRule = new PayRecevieRule(this.unitOfWork);

                    var payRecevies = PayRecevieRule.GetAllByOrganId(organId);
                    var lastPayRecevies = payRecevies.OrderByDescending(x => x.ID).FirstOrDefault();

                    if (lastPayRecevies != null)
                        payRecevie.Number = lastPayRecevies.Number + 1;
                    else
                        payRecevie.Number = 1;

                    payRecevie.InvoiceId = invoice.ID;
                    payRecevie.IsReceive = true;
                    payRecevie.OrganId = organId;
                    payRecevie.Status = ZhivarEnums.Status.Temporary;
                    payRecevie.Type = ZhivarEnums.PayRecevieType.Sir;

                    payRecevie.ObjectState = OMF.Common.Enums.ObjectState.Added;
                    unitOfWork.RepositoryAsync<PayRecevie>().InsertOrUpdateGraph(payRecevie);

                    await unitOfWork.SaveChangesAsync();
                }



                #endregion
                contract.InvoiceId = invoice.ID;
                this.UnitOfWork.RepositoryAsync<DomainClasses.Contract.Contract>().Update(contract);
                await this.UnitOfWork.SaveChangesAsync();

                return invoice;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        private async Task<int> MapSazeToItemAsync(int sazeId)
        {
            MapItemSazeRule mapItemSazeRule = new MapItemSazeRule();

            var resSubGroup = await mapItemSazeRule.GetItemIdBySazeIdAsync(sazeId);

            if (resSubGroup > 0)
                return resSubGroup;

            SazeRule sazeRule = new SazeRule();
            GoroheSazeRule goroheSazeRule = new GoroheSazeRule();

            var saze = await sazeRule.FindAsync(sazeId);
            var goroheSaze = await goroheSazeRule.FindAsync(saze.GoroheSazeID);

            var resGroup = await mapItemSazeRule.GetItemGroupIdBySazeGroupIdAsync(saze.GoroheSazeID);

            if (resGroup == 0)
            {
                ItemGroup itemGroup = new ItemGroup()
                {
                    Name = goroheSaze.Title,
                    OrganID = goroheSaze.OrganID,
                    IsGroupSaze = true,
                    ObjectState = Enums.ObjectState.Added
                };

                Item item = new Item()
                {
                    IsService = true,
                    IsGoods = false,
                    ItemGroupId = itemGroup.ID,
                    ItemType = ZhivarEnums.NoeItem.Service,
                    Name = saze.Title,
                    OrganIdItem = saze.OrganId,
                    SalesTitle = saze.Title,
                    IsSaze = true,
                    ObjectState = Enums.ObjectState.Added

                };


                this.UnitOfWork.RepositoryAsync<ItemGroup>().InsertOrUpdateGraph(itemGroup);
                await this.UnitOfWork.SaveChangesAsync();

                item.ItemGroupId = itemGroup.ID;

                this.UnitOfWork.RepositoryAsync<Item>().InsertOrUpdateGraph(item);
                await this.UnitOfWork.SaveChangesAsync();

                List<MapItemSaze> lst = new List<MapItemSaze>();
                lst.Add(new MapItemSaze()
                {
                    ItemID = itemGroup.ID,
                    SazeID = goroheSaze.ID,
                    Type = ZhivarEnums.MapItemSazeType.Group,
                    ObjectState = Enums.ObjectState.Added
                });

                lst.Add(new MapItemSaze()
                {
                    ItemID = item.ID,
                    SazeID = saze.ID,
                    Type = ZhivarEnums.MapItemSazeType.SubGroup,
                    ObjectState = Enums.ObjectState.Added
                });


                this.UnitOfWork.RepositoryAsync<MapItemSaze>().InsertRange(lst);
                await this.UnitOfWork.SaveChangesAsync();

                return item.ID;
            }
            else
            {
                Item item = new Item()
                {
                    IsService = true,
                    IsGoods = false,
                    ItemGroupId = resGroup,
                    ItemType = ZhivarEnums.NoeItem.Service,
                    Name = saze.Title,
                    OrganIdItem = saze.OrganId,
                    SalesTitle = saze.Title,
                    IsSaze = true,
                        ObjectState = Enums.ObjectState.Added
                };

                this.UnitOfWork.RepositoryAsync<Item>().InsertOrUpdateGraph(item);
                await this.UnitOfWork.SaveChangesAsync();

                List<MapItemSaze> lst = new List<MapItemSaze>();
    

                lst.Add(new MapItemSaze()
                {
                    ItemID = item.ID,
                    SazeID = saze.ID,
                    Type = ZhivarEnums.MapItemSazeType.SubGroup,
                    ObjectState = Enums.ObjectState.Added
                });


                this.UnitOfWork.RepositoryAsync<MapItemSaze>().InsertRange(lst);
                await this.UnitOfWork.SaveChangesAsync();

                return item.ID;
            }



        }
        private int MapSazeToItem(int sazeId)
        {
            MapItemSazeRule mapItemSazeRule = new MapItemSazeRule();

            var resSubGroup =  mapItemSazeRule.GetItemIdBySazeId(sazeId);

            if (resSubGroup > 0)
                return resSubGroup;

            SazeRule sazeRule = new SazeRule();
            GoroheSazeRule goroheSazeRule = new GoroheSazeRule();

            var saze = sazeRule.Find(sazeId);
            var goroheSaze = goroheSazeRule.Find(saze.GoroheSazeID);

            var resGroup = mapItemSazeRule.GetItemGroupIdBySazeGroupId(saze.GoroheSazeID);

            if (resGroup == 0)
            {
                ItemGroup itemGroup = new ItemGroup()
                {
                    Name = goroheSaze.Title,
                    OrganID = goroheSaze.OrganID,
                    IsGroupSaze = true,
                    ObjectState = Enums.ObjectState.Added
                };

                Item item = new Item()
                {
                    IsService = true,
                    IsGoods = false,
                    ItemGroupId = itemGroup.ID,
                    ItemType = ZhivarEnums.NoeItem.Service,
                    Name = saze.Title,
                    OrganIdItem = saze.OrganId,
                    SalesTitle = saze.Title,
                    IsSaze = true,
                    ObjectState = Enums.ObjectState.Added

                };


                this.UnitOfWork.Repository<ItemGroup>().Insert(itemGroup);
                this.UnitOfWork.SaveChanges();

                item.ItemGroupId = itemGroup.ID;

                this.UnitOfWork.Repository<Item>().Insert(item);
                this.UnitOfWork.SaveChanges();

                List<MapItemSaze> lst = new List<MapItemSaze>();
                lst.Add(new MapItemSaze()
                {
                    ItemID = itemGroup.ID,
                    SazeID = goroheSaze.ID,
                    Type = ZhivarEnums.MapItemSazeType.Group,
                    ObjectState = Enums.ObjectState.Added
                });

                lst.Add(new MapItemSaze()
                {
                    ItemID = item.ID,
                    SazeID = saze.ID,
                    Type = ZhivarEnums.MapItemSazeType.SubGroup,
                    ObjectState = Enums.ObjectState.Added
                });


                this.UnitOfWork.Repository<MapItemSaze>().InsertRange(lst);
                this.UnitOfWork.SaveChanges();

                return item.ID;
            }
            else
            {
                Item item = new Item()
                {
                    IsService = true,
                    IsGoods = false,
                    ItemGroupId = resGroup,
                    ItemType = ZhivarEnums.NoeItem.Service,
                    Name = saze.Title,
                    OrganIdItem = saze.OrganId,
                    SalesTitle = saze.Title,
                    IsSaze = true,
                    ObjectState = Enums.ObjectState.Added
                };

                this.UnitOfWork.Repository<Item>().Insert(item);
                this.UnitOfWork.SaveChanges();

                List<MapItemSaze> lst = new List<MapItemSaze>();


                lst.Add(new MapItemSaze()
                {
                    ItemID = item.ID,
                    SazeID = saze.ID,
                    Type = ZhivarEnums.MapItemSazeType.SubGroup,
                    ObjectState = Enums.ObjectState.Added
                });


                this.UnitOfWork.Repository<MapItemSaze>().InsertRange(lst);
                this.UnitOfWork.SaveChanges();

                return item.ID;
            }



        }
        public async Task<Document> RegisterDocumentAsync(InvoiceVM invoice, int personId)
        {
            var documentNumber = await createNumberDocumentAsync(personId);
            Document document = new Document();
            document.Credit = invoice.Sum;
            document.Debit = invoice.Sum;
            document.IsManual = false;
            document.Number = documentNumber;
            document.Number2 = documentNumber;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.StatusString = "تایید شده";
            document.DisplayDate = invoice.DisplayDate;
            document.DateTime = PersianDateUtils.ToDateTime(invoice.DisplayDate);

            switch (invoice.InvoiceType)
            {
                case ZhivarEnums.NoeFactor.Sell:
                    document.Type = ZhivarEnums.NoeDoc.Sell;
                    break;
                case ZhivarEnums.NoeFactor.Buy:
                    document.Type = ZhivarEnums.NoeDoc.Buy;
                    break;
                case ZhivarEnums.NoeFactor.ReturnSell:
                    document.Type = ZhivarEnums.NoeDoc.ReturnSell;
                    break;
                case ZhivarEnums.NoeFactor.ReturnBuy:
                    document.Type = ZhivarEnums.NoeDoc.ReturnBuy;
                    break;
                case ZhivarEnums.NoeFactor.RentTo:
                    document.Type = ZhivarEnums.NoeDoc.RentTo;
                    break;
                case ZhivarEnums.NoeFactor.RentFrom:
                    document.Type = ZhivarEnums.NoeDoc.RentFrom;
                    break;
                default:
                    break;
            }

            List<Transaction> transactions = new List<Transaction>();

            switch (invoice.InvoiceType)
            {
                case ZhivarEnums.NoeFactor.Sell:
                    {
                        document.Description = "فروش طی فاکتور شماره " + documentNumber;
                        transactions = await RegisterTransactionSellAsync(document, invoice, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.Buy:
                    {
                        document.Description = " خرید طی فاکتور با شماره ارجاع" + invoice.Refrence;
                        transactions = await RegisterTransactionBuyAsync(document, invoice, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.ReturnSell:
                    {
                        document.Description = " برگشت از فروش طی فاکتور شماره" + documentNumber;
                        transactions = await RegisterTransactionBarghashtForoshAsync(document, invoice, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.ReturnBuy:
                    {
                        document.Description = " برگشت از خرید طی فاکتور شماره" + documentNumber;
                        transactions = await RegisterTransactionReturnBuyAsync(document, invoice, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.RentTo:
                    {
                        document.Description = " اجاره دادن طی فاکتور شماره" + documentNumber;
                        transactions = await RegisterTransactionRentToAsync(document, invoice, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.RentFrom:
                    {
                        document.Description = " اجاره گرفتن از صاحب رسانه طی فاکتور شماره" + documentNumber;
                        transactions =await RegisterTransactionRentFromAsync(document, invoice, personId);
                        document.Transactions = transactions;
                        break;
                    }
            }

            if (document.Transactions != null && document.Transactions.Any())
            {
                var credit = document.Transactions.Sum(x => x.Credit);
                var debit = document.Transactions.Sum(x => x.Debit);

                document.Credit = credit;
                document.Debit = debit;
            }
            return document;
        }

        private async Task<List<Transaction>> RegisterTransactionSellAsync(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            string contactCode = "1104" + invoice.Contact.Code;
            var accountHesabDreaftani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabDreaftani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabDreaftani,
                Amount = invoice.Payable,
                Debit = invoice.Payable,
                Credit = 0,
                // Date = DateTime.Now,
                //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                IsDebit = true,
                IsCredit = false,
                Description = invoice.Number + " فروش طی فاکتور شماره ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "6101" + item.Item.Code).SingleOrDefault();
                else
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "7101" + item.Item.Code).SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = item.Sum,
                    Description = invoice.Number + " فروش طی فاکتور شماره ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "2106").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = 0,
                    Credit = tax,
                    Description = invoice.Number + " فروش طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "8305").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    IsDebit = true,
                    IsCredit = false,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = discount,
                    Credit = 0,
                    Description = invoice.Number + " فروش طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private async Task<List<Transaction>> RegisterTransactionBuyAsync(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            string contactCode = "2101" + invoice.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                ///AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = invoice.Payable,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = false,
                IsCredit = true,
                Debit = 0,
                Credit = invoice.Payable,
                Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "5101" + item.Item.Code).SingleOrDefault();
                else
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Item.Code).SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = item.Sum,
                    Credit = 0,
                    Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "1111").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = tax,
                    Credit = 0,
                    Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "7203").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = discount,
                    Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع  ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private async Task<List<Transaction>> RegisterTransactionBarghashtForoshAsync(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            string contactCode = "1104" + invoice.Contact.Code;
            var accountHesabDreaftani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabDreaftani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabDreaftani,
                Amount = invoice.Sum,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = false,
                IsCredit = true,
                Description = invoice.Tag + " برگشت از فروش - شماره ارجاع:",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "6102" + item.Item.Code).SingleOrDefault();


                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Description = invoice.Tag + " برگشت از فروش - شماره ارجاع:",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "2106").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Description = invoice.Tag + " برگشت از فروش - شماره ارجاع: ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "7203").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Description = invoice.Tag + " برگشت از فروش - شماره ارجاع:",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private async Task<List<Transaction>> RegisterTransactionReturnBuyAsync(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            string contactCode = "2101" + invoice.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = invoice.Sum,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = true,
                IsCredit = false,
                Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "5102" + item.Item.Code).SingleOrDefault();


                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "1111").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "8305").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }
        private async Task<List<Transaction>> RegisterTransactionRentToAsync(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            string contactCode = "1104" + invoice.Contact.Code;
            var accountHesabDreaftani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabDreaftani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabDreaftani,
                Amount = invoice.Payable,
                Debit = invoice.Payable,
                Credit = 0,
                // Date = DateTime.Now,
                //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                IsDebit = true,
                IsCredit = false,
                Description = invoice.Number + " اجاره دادن طی فاکتور شماره ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "6101" + item.Item.Code).SingleOrDefault();
                else
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "7101" + item.Item.Code).SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = item.Sum,
                    Description = invoice.Number + " اجاره دادن طی فاکتور شماره ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "2106").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = 0,
                    Credit = tax,
                    Description = invoice.Number + " اجاره دادن طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "8305").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    IsDebit = true,
                    IsCredit = false,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = discount,
                    Credit = 0,
                    Description = invoice.Number + " اجاره دادن طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }
        private async Task<List<Transaction>> RegisterTransactionRentFromAsync(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            string contactCode = "2101" + invoice.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                ///AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = invoice.Payable,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = false,
                IsCredit = true,
                Debit = 0,
                Credit = invoice.Payable,
                Description = invoice.Tag + " اجاره کردن از صاحب رسانه طی فاکتور شماره ارجاع ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                {
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "5101" + item.Item.Code).SingleOrDefault();

                    if (accoubtItem == null)
                    {
                        ItemRule itemRule = new ItemRule();
                        Item itemCommon = new Item();
                        Mapper.Map(item.Item, itemCommon);
                        itemRule.CreateGoodAccounts(itemCommon, organId);
                        accounts = await accountRule.GetAllByOrganIdAsync(organId);

                        accoubtItem = accounts.Where(x => x.ComplteCoding == "5101" + item.Item.Code).SingleOrDefault();
                    }
                }
                else
                {
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Item.Code).SingleOrDefault();

                    if (accoubtItem == null)
                    {
                        ItemRule itemRule = new ItemRule();
                        Item itemCommon = new Item();
                        Mapper.Map(item.Item, itemCommon);
                        itemRule.CreateGoodAccounts(itemCommon, organId);
                        accounts = await accountRule.GetAllByOrganIdAsync(organId);

                        accoubtItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Item.Code).SingleOrDefault();
                    }
                }

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = item.Sum,
                    Credit = 0,
                    Description = invoice.Tag + " جاره کردن از صاحب رسانه طی فاکتور شماره ارجاع ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "1111").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = tax,
                    Credit = 0,
                    Description = invoice.Tag + " جاره کردن از صاحب رسانه طی فاکتور شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "7203").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = discount,
                    Description = invoice.Tag + " جاره کردن از صاحب رسانه طی فاکتور شماره ارجاع  ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }


        public Document RegisterDocument(Invoice invoice, int personId)
        {
            
            var documentNumber =  createNumberDocument(personId);
            Document document = new Document();
            document.Credit = invoice.Sum;
            document.Debit = invoice.Sum;
            document.IsManual = false;
            document.Number = documentNumber;
            document.Number2 = documentNumber;
            document.Status = ZhivarEnums.DocumentStatus.TaeedShode;
            document.StatusString = "تایید شده";
            document.DisplayDate = invoice.DisplayDate;
            document.DateTime = PersianDateUtils.ToDateTime(invoice.DisplayDate);

            switch (invoice.InvoiceType)
            {
                case ZhivarEnums.NoeFactor.Sell:
                    document.Type = ZhivarEnums.NoeDoc.Sell;
                    break;
                case ZhivarEnums.NoeFactor.Buy:
                    document.Type = ZhivarEnums.NoeDoc.Buy;
                    break;
                case ZhivarEnums.NoeFactor.ReturnSell:
                    document.Type = ZhivarEnums.NoeDoc.ReturnSell;
                    break;
                case ZhivarEnums.NoeFactor.ReturnBuy:
                    document.Type = ZhivarEnums.NoeDoc.ReturnBuy;
                    break;
                case ZhivarEnums.NoeFactor.RentTo:
                    document.Type = ZhivarEnums.NoeDoc.RentTo;
                    break;
                case ZhivarEnums.NoeFactor.RentFrom:
                    document.Type = ZhivarEnums.NoeDoc.RentFrom;
                    break;
                default:
                    break;
            }

            List<Transaction> transactions = new List<Transaction>();

            var invoiceVM = Utilities.TranslateHelper.TranslateEntityToEntityVMInvoice(invoice);
            switch (invoice.InvoiceType)
            {
                case ZhivarEnums.NoeFactor.Sell:
                    {
                        document.Description = "فروش طی فاکتور شماره " + documentNumber;
                        transactions = RegisterTransactionSell(document, invoiceVM, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.Buy:
                    {
                        document.Description = " خرید طی فاکتور با شماره ارجاع" + invoice.Refrence;
                        transactions = RegisterTransactionBuy(document, invoiceVM, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.ReturnSell:
                    {
                        document.Description = " برگشت از فروش طی فاکتور شماره" + documentNumber;
                        transactions = RegisterTransactionBarghashtForosh(document, invoiceVM, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.ReturnBuy:
                    {
                        document.Description = " برگشت از خرید طی فاکتور شماره" + documentNumber;
                        transactions = RegisterTransactionReturnBuy(document, invoiceVM, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.RentTo:
                    {
                        document.Description = " اجاره دادن طی فاکتور شماره" + documentNumber;
                        transactions =  RegisterTransactionRentTo(document, invoiceVM, personId);
                        document.Transactions = transactions;
                        break;
                    }
                case ZhivarEnums.NoeFactor.RentFrom:
                    {
                        document.Description = " اجاره گرفتن از صاحب رسانه طی فاکتور شماره" + documentNumber;
                        transactions = RegisterTransactionRentFrom(document, invoiceVM, personId);
                        document.Transactions = transactions;
                        break;
                    }
            }

            if (document.Transactions != null && document.Transactions.Any())
            {
                var credit = document.Transactions.Sum(x => x.Credit);
                var debit = document.Transactions.Sum(x => x.Debit);

                document.Credit = credit;
                document.Debit = debit;
            }
            return document;
        }

        private List<Transaction> RegisterTransactionSell(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = accountRule.GetAllByOrganId(organId);
            string contactCode = "1104" + invoice.Contact.Code;
            var accountHesabDreaftani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabDreaftani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabDreaftani,
                Amount = invoice.Payable,
                Debit = invoice.Payable,
                Credit = 0,
                // Date = DateTime.Now,
                //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                IsDebit = true,
                IsCredit = false,
                Description = invoice.Number + " فروش طی فاکتور شماره ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "6101" + item.Item.Code).SingleOrDefault();
                else
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "7101" + item.Item.Code).SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = item.Sum,
                    Description = invoice.Number + " فروش طی فاکتور شماره ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "2106").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = 0,
                    Credit = tax,
                    Description = invoice.Number + " فروش طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "8305").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    IsDebit = true,
                    IsCredit = false,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = discount,
                    Credit = 0,
                    Description = invoice.Number + " فروش طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private List<Transaction> RegisterTransactionBuy(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = accountRule.GetAllByOrganId(organId);
            string contactCode = "2101" + invoice.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                ///AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = invoice.Payable,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = false,
                IsCredit = true,
                Debit = 0,
                Credit = invoice.Payable,
                Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "5101" + item.Item.Code).SingleOrDefault();
                else
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Item.Code).SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = item.Sum,
                    Credit = 0,
                    Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "1111").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = tax,
                    Credit = 0,
                    Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "7203").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = discount,
                    Description = invoice.Tag + " خرید طی فاکتور شماره ارجاع  ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private List<Transaction> RegisterTransactionBarghashtForosh(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = accountRule.GetAllByOrganId(organId);
            string contactCode = "1104" + invoice.Contact.Code;
            var accountHesabDreaftani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabDreaftani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabDreaftani,
                Amount = invoice.Sum,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = false,
                IsCredit = true,
                Description = invoice.Tag + " برگشت از فروش - شماره ارجاع:",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "6102" + item.Item.Code).SingleOrDefault();


                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Description = invoice.Tag + " برگشت از فروش - شماره ارجاع:",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "2106").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Description = invoice.Tag + " برگشت از فروش - شماره ارجاع: ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "7203").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Description = invoice.Tag + " برگشت از فروش - شماره ارجاع:",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private List<Transaction> RegisterTransactionReturnBuy(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = accountRule.GetAllByOrganId(organId);
            string contactCode = "2101" + invoice.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = invoice.Sum,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = true,
                IsCredit = false,
                Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "5102" + item.Item.Code).SingleOrDefault();


                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "1111").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "8305").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = invoice.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Description = invoice.Tag + " برگشت از خرید - شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }

        private List<Transaction> RegisterTransactionRentTo(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = accountRule.GetAllByOrganId(organId);
            string contactCode = "1104" + invoice.Contact.Code;
            var accountHesabDreaftani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabDreaftani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                //AccDocument = document,
              //  Account = accountHesabDreaftani,
                Amount = invoice.Payable,
                Debit = invoice.Payable,
                Credit = 0,
                // Date = DateTime.Now,
                //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                IsDebit = true,
                IsCredit = false,
                Description = invoice.Number + " اجاره طی فاکتور شماره ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                {
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "6101" + item.Item.Code).SingleOrDefault();

                    if (accoubtItem == null)
                    {
                        ItemRule itemRule = new ItemRule();
                        Item itemCommon = new Item();
                        Mapper.Map(item.Item, itemCommon);
                        itemRule.CreateGoodAccounts(itemCommon, organId);
                        accounts = accountRule.GetAllByOrganId(organId);

                        accoubtItem = accounts.Where(x => x.ComplteCoding == "6101" + item.Item.Code).SingleOrDefault();
                    }
                }
                    
                else
                {
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "7101" + item.Item.Code).SingleOrDefault();

                    if (accoubtItem == null)
                    {
                        ItemRule itemRule = new ItemRule();
                        Item itemCommon = new Item();
                        Mapper.Map(item.Item, itemCommon);
                        itemRule.CreateServiceAccount(itemCommon, organId);
                        accounts = accountRule.GetAllByOrganId(organId);

                        accoubtItem = accounts.Where(x => x.ComplteCoding == "7101" + item.Item.Code).SingleOrDefault();
                    }
                }
                    

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                   // Account = accoubtItem,
                    Amount = item.Sum,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = item.Sum,
                    Description = invoice.Number + " اجاره طی فاکتور شماره ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),
                    ObjectState = Enums.ObjectState.Added
                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "2106").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                  //  Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = 0,
                    Credit = tax,
                    Description = invoice.Number + " اجاره طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),
                    ObjectState = Enums.ObjectState.Added
                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "8305").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                  //  Account = accountDiscount,
                    Amount = discount,
                    IsDebit = true,
                    IsCredit = false,
                    //Date = DateTime.Now,
                    //DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now),
                    Debit = discount,
                    Credit = 0,
                    Description = invoice.Number + " اجاره طی فاکتور شماره ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),
                    ObjectState = Enums.ObjectState.Added
                });
            }
            return transactions;
        }

        private List<Transaction> RegisterTransactionRentFrom(Document document, InvoiceVM invoice, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = accountRule.GetAllByOrganId(organId);
            string contactCode = "2101" + invoice.Contact.Code;
            var accountHesabPardakhtani = accounts.Where(x => x.ComplteCoding == contactCode).SingleOrDefault();

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction()
            {
                AccountId = accountHesabPardakhtani.ID,
                ContactId = invoice.Contact.ID,
                DocumentId = document.ID,
                InvoiceId = invoice.ID,
                ///AccDocument = document,
                Account = accountHesabPardakhtani,
                Amount = invoice.Payable,
                //Contact = invoice.Contact,
                //Invoice = invoice,
                IsDebit = false,
                IsCredit = true,
                Debit = 0,
                Credit = invoice.Payable,
                Description = invoice.Tag + " اجاره گرفتن از صاحب رسانه طی فاکتور شماره ارجاع ",
                RowNumber = 1,
                DisplayDate = document.DisplayDate,
                Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

            });


            decimal tax = 0;
            decimal discount = 0;
            foreach (var item in invoice.InvoiceItems)
            {
                tax += item.Tax;
                discount += item.Discount;

                var accoubtItem = new DomainClasses.Accounting.Account();

                if (item.Item.ItemType == ZhivarEnums.NoeItem.Item)
                {
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "5101" + item.Item.Code).SingleOrDefault();

                    if (accoubtItem == null)
                    {
                        ItemRule itemRule = new ItemRule();
                        Item itemCommon = new Item();
                        Mapper.Map(item.Item, itemCommon);
                        itemRule.CreateGoodAccounts(itemCommon, organId);
                        accounts = accountRule.GetAllByOrganId(organId);

                        accoubtItem = accounts.Where(x => x.ComplteCoding == "5101" + item.Item.Code).SingleOrDefault();
                    }
                }
                else
                {
                    accoubtItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Item.Code).SingleOrDefault();

                    if (accoubtItem == null)
                    {
                        ItemRule itemRule = new ItemRule();
                        Item itemCommon = new Item();
                        Mapper.Map(item.Item, itemCommon);
                        itemRule.CreateServiceAccount(itemCommon, organId);
                        accounts = accountRule.GetAllByOrganId(organId);

                        accoubtItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Item.Code).SingleOrDefault();
                    }
                }


                transactions.Add(new Transaction()
                {
                    AccountId = accoubtItem.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtItem,
                    Amount = item.Sum,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = item.Sum,
                    Credit = 0,
                    Description = invoice.Tag + " اجاره گرفتن از صاحب رسانه طی فاکتور شماره ارجاع ",
                    //RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }


            if (tax > 0)
            {
                var accoubtTax = accounts.Where(x => x.ComplteCoding == "1111").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accoubtTax.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accoubtTax,
                    Amount = tax,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = true,
                    IsCredit = false,
                    Debit = tax,
                    Credit = 0,
                    Description = invoice.Tag + " اجاره گرفتن از صاحب رسانه طی فاکتور شماره ارجاع ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }

            if (discount > 0)
            {
                var accountDiscount = accounts.Where(x => x.ComplteCoding == "7203").SingleOrDefault();

                transactions.Add(new Transaction()
                {
                    AccountId = accountDiscount.ID,
                    ContactId = invoice.Contact.ID,
                    DocumentId = document.ID,
                    InvoiceId = invoice.ID,
                    //AccDocument = document,
                    Account = accountDiscount,
                    Amount = discount,
                    //Contact = invoice.Contact,
                    //Invoice = invoice,
                    IsDebit = false,
                    IsCredit = true,
                    Debit = 0,
                    Credit = discount,
                    Description = invoice.Tag + " اجاره گرفتن از صاحب رسانه طی فاکتور شماره ارجاع  ",
                    RowNumber = 1,
                    DisplayDate = document.DisplayDate,
                    Date = Utilities.PersianDateUtils.ToDateTime(document.DisplayDate),

                });
            }
            return transactions;
        }
        private async Task<int> createNumberDocumentAsync(int organId)
        {
            var count = 0;
            DocumentRule documentRule = new DocumentRule();
            var documentQuery = await documentRule.GetAllByOrganIdAsync(organId);

            count = documentQuery.Count();

            return count++;
        }
        private int createNumberDocument(int organId)
        {
            var count = 0;
            DocumentRule documentRule = new DocumentRule();

            var documentQuery = documentRule.GetAllByOrganId(organId);

            count = documentQuery.Count();

            return count++;
        }

        private async Task<string> createNumberInvoiceAsync(int organId)
        {
            var count = 0;

            InvoiceRule invoiceRule = new InvoiceRule();
            var invoiceQuery = await invoiceRule.GetAllByOrganIdAsync(organId);

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
        private string createNumberInvoice(int organId)
        {
            var count = 0;

            InvoiceRule invoiceRule = new InvoiceRule();
            var invoiceQuery = invoiceRule.GetAllByOrganId(organId);

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
    }
}