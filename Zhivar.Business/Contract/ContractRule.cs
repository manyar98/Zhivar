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
using Zhivar.ViewModel.Contract;
using Zhivar.Business.Accounting;
using Zhivar.DomainClasses.Contract;
using Zhivar.Business.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using System.Globalization;
using Zhivar.DataLayer.Validation;
using static Zhivar.DomainClasses.ZhivarEnums;
using static OMF.Common.Enums;

namespace Zhivar.Business.Contract
{
    public partial class ContractRule : BusinessRuleBase<DomainClasses.Contract.Contract>
    {
        public ContractRule()
            : base()
        {

        }

        public ContractRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ContractRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        //public async Task<List<ContractVM>> GetAllByOrganIdAsync(int organId)
        //{
        //    var invoiceQuery = this.Queryable().Where(x => x.OrganId == organId);
        //    var invoiceItemQuery = this.unitOfWork.RepositoryAsync<InvoiceItem>().Queryable();
        //    var itemQuery = this.unitOfWork.RepositoryAsync<Item>().Queryable();

        //    var joinQuery = from invoice in invoiceQuery
        //                    join invoiceItem2 in (from invoiceItem in invoiceItemQuery
        //                                          join item in itemQuery
        //                                          on invoiceItem.ItemId equals item.ID
        //                                          select new InvoiceItemVM
        //                                          {
        //                                              CalcTax = invoiceItem.CalcTax,
        //                                              Description = invoiceItem.Description,
        //                                              Discount = invoiceItem.Discount,
        //                                              InvoiceId = invoiceItem.InvoiceId,
        //                                              ID = invoiceItem.ID,
        //                                              Inv = invoiceItem.Inv,
        //                                              //Invoice = invoiceItem.Invoice,
        //                                              Item = new ViewModel.BaseInfo.ItemVM()
        //                                              {
        //                                                  Barcode = item.Barcode,
        //                                                  BuyPrice = item.BuyPrice,
        //                                                  Code = item.Code,
        //                                                  ID = item.ID,
        //                                                  IsGoods = item.IsGoods,
        //                                                  IsService = item.IsService,
        //                                                  ItemGroupId = item.ItemGroupId,
        //                                                  ItemType = item.ItemType,
        //                                                  MoneyStock = item.MoneyStock,
        //                                                  Name = item.Name,
        //                                                  OrganId = item.OrganIdItem,
        //                                                  PurchasesTitle = item.PurchasesTitle,
        //                                                  SalesTitle = item.SalesTitle,
        //                                                  SellPrice = item.SellPrice,
        //                                                  Stock = item.Stock,
        //                                                  Unit = item.Unit
        //                                              },
        //                                              ItemId = item.ID,
        //                                              ItemInput = invoiceItem.ItemInput,
        //                                              Quantity = invoiceItem.Quantity,
        //                                              RowNumber = invoiceItem.RowNumber,
        //                                              Sum = invoiceItem.SumInvoiceItem,
        //                                              Tax = invoiceItem.Tax,
        //                                              TotalAmount = invoiceItem.TotalAmount,
        //                                              Unit = invoiceItem.UnitInvoiceItem,
        //                                              UnitPrice = invoiceItem.UnitPrice
        //                                          })
        //                                          on invoice.ID equals invoiceItem2.InvoiceId into invoiceItemGroup
        //                    select new InvoiceVM
        //                    {
        //                        Sum = invoice.Sum,
        //                        Contact = new ViewModel.Common.ContactVM()
        //                        {
        //                            // OrganId = invoice.Contact.OrganId,
        //                            // Address = invoice.Contact.Address,
        //                            // Balance = invoice.Contact.Balance,
        //                            // City = invoice.Contact.City,
        //                            // Code = invoice.Contact.Address,
        //                            // ContactType = invoice.Contact.ContactType,
        //                            // Credits = invoice.Contact.Credits,
        //                            // EconomicCode = invoice.Contact.EconomicCode,
        //                            //Email = invoice.Contact.Email,
        //                            // Fax = invoice.Contact.Fax,
        //                            // FirstName = invoice.Contact.FirstName,
        //                            // LastName = invoice.Contact.LastName,
        //                            // IsCustomer= invoice.Contact.IsCustomer,
        //                            // ID = invoice.Contact.ID,
        //                            // IsEmployee = invoice.Contact.IsEmployee,

        //                            // IsShareHolder = invoice.Contact.IsShareHolder,
        //                            // IsVendor = invoice.Contact.IsVendor,
        //                            // Liability = invoice.Contact.Liability,
        //                            // Mobile = invoice.Contact.Mobile,
        //                            // Name = invoice.Contact.Name,
        //                            // Website = invoice.Contact.Name,
        //                            // NationalCode = invoice.Contact.Name,
        //                            // Note = invoice.Contact.Name,
        //                            // Phone = invoice.Contact.Name,
        //                            // PostalCode = invoice.Contact.Name,
        //                            // Rating = invoice.Contact.Rating,
        //                            // RegistrationDate = invoice.Contact.RegistrationDate,
        //                            // RegistrationNumber = invoice.Contact.RegistrationNumber,
        //                            // SharePercent = invoice.Contact.SharePercent,
        //                            // State = invoice.Contact.State,
        //                        },
        //                        ContactId = invoice.ContactId,
        //                        ContactTitle = invoice.ContactTitle,
        //                        DateTime = invoice.DateTime,
        //                        DisplayDate = invoice.DisplayDate,
        //                        DisplayDueDate = invoice.DisplayDueDate,
        //                        DueDate = invoice.DueDate,
        //                        ID = invoice.ID,
        //                        InvoiceItems = invoiceItemGroup.ToList(),
        //                        InvoiceStatusString = invoice.InvoiceStatusString,
        //                        InvoiceType = invoice.InvoiceType,
        //                        InvoiceTypeString = invoice.InvoiceTypeString,
        //                        IsDraft = invoice.IsDraft,
        //                        IsPurchase = invoice.IsPurchase,
        //                        IsPurchaseReturn = invoice.IsPurchaseReturn,
        //                        IsSale = invoice.IsSale,
        //                        IsSaleReturn = invoice.IsSaleReturn,
        //                        Note = invoice.Note,
        //                        Number = invoice.Number,
        //                        OrganId = invoice.OrganId,
        //                        Paid = invoice.Paid,
        //                        Payable = invoice.Payable,
        //                        Profit = invoice.Profit,
        //                        //Refrence = invoice.Refrence,
        //                        Rest = invoice.Rest,
        //                        Returned = invoice.Returned,
        //                        Sent = invoice.Sent,
        //                        Status = invoice.Status,
        //                        Tag = invoice.Tag

        //                    };

        //    return await joinQuery.ToListAsync2();
        //}

        protected override DomainClasses.Contract.Contract FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.Contract_Sazes == null)
            {
                this.LoadCollection<Contract_Saze>(entity, x => x.Contract_Sazes);

                foreach (var contract_Sazes in entity.Contract_Sazes)
                {
                    if (contract_Sazes.Contarct_Saze_Bazareabs == null)
                    {
                        contract_Sazes.Contarct_Saze_Bazareabs = this.unitOfWork.Repository<Contract_Saze_Bazareab>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToList();
                    }
                    if (contract_Sazes.Contract_Saze_Tarahs == null)
                    {
                        contract_Sazes.Contract_Saze_Tarahs = this.unitOfWork.Repository<Contract_Saze_Tarah>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToList();
                    }
                    if (contract_Sazes.Contract_Saze_Chapkhanes == null)
                    {
                        contract_Sazes.Contract_Saze_Chapkhanes = this.unitOfWork.Repository<Contract_Saze_Chapkhane>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToList();
                    }
                    if (contract_Sazes.Contract_Saze_Nasabs == null)
                    {
                        contract_Sazes.Contract_Saze_Nasabs = this.unitOfWork.Repository<Contract_Saze_Nasab>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToList();
                    }
                }
            }

            if (entity.Contract_PayRecevies == null)
            {
                this.LoadCollection<Contract_PayRecevies>(entity, x => x.Contract_PayRecevies);

                foreach (var contract_PayRecevies in entity.Contract_PayRecevies)
                {
                    if (contract_PayRecevies.Contract_DetailPayRecevies == null)
                    {
                        contract_PayRecevies.Contract_DetailPayRecevies = this.unitOfWork.Repository<Contract_DetailPayRecevies>().Queryable().Where(x => x.Contract_PayRecevieId == contract_PayRecevies.ID).ToList();
                    }

                }
            }
            //if (entity.Contact == null)
            //{
            //    this.LoadReference<Contact>(entity, x => x.Contact);
            //}

            return entity;
        }

        protected async override Task<DomainClasses.Contract.Contract> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.Contract_Sazes == null)
            {
                await this.LoadCollectionAsync<Contract_Saze>(entity, x => x.Contract_Sazes);

                foreach (var contract_Sazes in entity.Contract_Sazes)
                {
                    if (contract_Sazes.Contarct_Saze_Bazareabs == null)
                    {
                        contract_Sazes.Contarct_Saze_Bazareabs = await this.unitOfWork.RepositoryAsync<Contract_Saze_Bazareab>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToListAsync();


                    }
                    if (contract_Sazes.Contract_Saze_Tarahs == null)
                    {
                        contract_Sazes.Contract_Saze_Tarahs = await this.unitOfWork.RepositoryAsync<Contract_Saze_Tarah>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToListAsync();
                    }
                    if (contract_Sazes.Contract_Saze_Chapkhanes == null)
                    {
                        contract_Sazes.Contract_Saze_Chapkhanes = await this.unitOfWork.RepositoryAsync<Contract_Saze_Chapkhane>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToListAsync();
                    }
                    if (contract_Sazes.Contract_Saze_Nasabs == null)
                    {
                        contract_Sazes.Contract_Saze_Nasabs = await this.unitOfWork.RepositoryAsync<Contract_Saze_Nasab>().Queryable().Where(x => x.ContarctSazeID == contract_Sazes.ID).ToListAsync();
                    }
                }
            }

            if (entity.Contract_PayRecevies == null)
            {
                await this.LoadCollectionAsync<Contract_PayRecevies>(entity, x => x.Contract_PayRecevies);

                foreach (var Contract_PayRecevie in entity.Contract_PayRecevies)
                {
                    if (Contract_PayRecevie.Contract_DetailPayRecevies == null)
                    {
                        Contract_PayRecevie.Contract_DetailPayRecevies = await this.unitOfWork.RepositoryAsync<Contract_DetailPayRecevies>().Queryable().Where(x => x.Contract_PayRecevieId == Contract_PayRecevie.ID).ToListAsync();


                    }

                }
            }
            //if (entity.Contact == null)
            //{
            //    await this.LoadReferenceAsync<Contact>(entity, x => x.Contact);
            //}

            return entity;
        }

        public async Task<IList<Contract_SazeVM>> GetAllSazeByContractIdAsync(int contractId)
        {
            try
            {
                var contract_Sazes = await this.unitOfWork.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.ContractID == contractId).ToListAsync2();


                List<Contract_SazeVM> contract_SazeVMs = new List<Contract_SazeVM>();

                Mapper.Map(contract_Sazes, contract_SazeVMs);

                SazeRule sazeRule = new SazeRule();
                NoeEjareRule noeEjareRule = new NoeEjareRule();

                foreach (var contract_SazeVM in contract_SazeVMs)
                {
                    contract_SazeVM.Saze = Mapper.Map<SazeVM>(await sazeRule.FindAsync(contract_SazeVM.SazeId));
                    contract_SazeVM.NoeEjare = Mapper.Map<NoeEjareVM>(await noeEjareRule.FindAsync(contract_SazeVM.NoeEjareId));
                }
                return contract_SazeVMs;
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async Task<IList<ContractVM>> GetAllByOrganIdAsync(int organId)
        {
            try
            {
                var finanYear = await this.unitOfWork.RepositoryAsync<FinanYear>().Queryable().Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync2();
                var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();

                var contractsQuery =  this.Queryable().Where(x => x.OrganId == organId);
                //var contractSazeQuery = this.unitOfWork.RepositoryAsync<Contract_Saze>().Queryable();

                var joinQuery = from contract in contractsQuery
                                join contact in contactQuery
                                on contract.ContactId equals contact.ID
                                select new ContractVM
                                {
                                    ContactTitle = contact.Name,
                                    DisplayDate = contract.DisplayDate,
                                    ID = contract.ID,
                                    ContactId = contract.ContactId,
                                    ContactCommon = contact,
                                    ContractTitle = contract.ContractTitle,
                                    ContractType = contract.ContractType,
                                    DateTime = contract.DateTime,
                                    DisplayDueDate = contract.DisplayDueDate,
                                    DueDate = contract.DueDate,
                                    InvoiceId = contract.InvoiceId,
                                    IsDraft = contract.IsDraft,
                                    Note = contract.Note,
                                    Number = contract.Number,
                                    OrganId = contract.OrganId,
                                    Paid = contract.Paid,
                                    Payable = contract.Payable,
                                    Profit = contract.Profit,
                                    Refrence = contract.Refrence,
                                    Rest = contract.Rest,
                                    Sent = contract.Sent,
                                    Status = contract.Status,
                                    Sum = contract.Sum,
                                    Tag = contract.Tag,
                                    
                                   

                                };


                //List <ContractVM> contractVMs = new List<ContractVM>();

                //Mapper.Map(await contractsQuery.ToListAsync2(), contractVMs);

                return await joinQuery.ToListAsync2();
            }
            catch (Exception ex)
            {

                throw;
            }


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
                                //Type = invoices.InvoiceType,
                                Unit = invoiceItems.UnitInvoiceItem,
                                UnitPrice = invoiceItems.UnitPrice
                            };


            return await joinQuery.ToListAsync2();

        }

        public async Task UpdateVaziatContract(int contractId, ZhivarEnums.Status vaziat)
        {
            var entity = BusinessContext.GetBusinessRule<DomainClasses.Contract.Contract>(this.OperationAccess, this.UnitOfWork).Find(contractId);

            entity.Status = vaziat;

            this.UpdateEntity(entity);
            await SaveChangesAsync();

        }

        public async Task UpdateVaziatContract_Saze(int contract_SazeID, ZhivarEnums.Status vaziat)
        {
            var contract_Saze = BusinessContext.GetBusinessRule<Contract_Saze>(this.OperationAccess, this.UnitOfWork).Find(contract_SazeID);

            contract_Saze.Status = vaziat;

            BusinessContext.GetBusinessRule<Contract_Saze>(this.OperationAccess, this.UnitOfWork).Update(contract_Saze);
            await BusinessContext.GetBusinessRule<Contract_Saze>(this.OperationAccess, this.UnitOfWork).SaveChangesAsync();


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

        protected override void DeleteEntity(DomainClasses.Contract.Contract entity)
        {
            entity.Contract_Sazes = this.UnitOfWork.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.ContractID == entity.ID).ToList();
            foreach (var contract_Saze in entity.Contract_Sazes)
            {
                contract_Saze.Contarct_Saze_Bazareabs = this.UnitOfWork.RepositoryAsync<Contract_Saze_Bazareab>().Queryable().Where(x => x.ContarctSazeID == contract_Saze.ID).ToList();
                contract_Saze.Contract_Saze_Tarahs = this.UnitOfWork.RepositoryAsync<Contract_Saze_Tarah>().Queryable().Where(x => x.ContarctSazeID == contract_Saze.ID).ToList();
                contract_Saze.Contract_Saze_Chapkhanes = this.UnitOfWork.RepositoryAsync<Contract_Saze_Chapkhane>().Queryable().Where(x => x.ContarctSazeID == contract_Saze.ID).ToList();
                contract_Saze.Contract_Saze_Nasabs = this.UnitOfWork.RepositoryAsync<Contract_Saze_Nasab>().Queryable().Where(x => x.ContarctSazeID == contract_Saze.ID).ToList();
                foreach (var contarct_Saze_Bazareab in contract_Saze.Contarct_Saze_Bazareabs ?? new List<Contract_Saze_Bazareab>())
                {
                    contarct_Saze_Bazareab.ObjectState = Enums.ObjectState.Deleted;
                }

                foreach (var Contract_Saze_Tarah in contract_Saze.Contract_Saze_Tarahs ?? new List<Contract_Saze_Tarah>())
                {
                    Contract_Saze_Tarah.ObjectState = Enums.ObjectState.Deleted;
                }

                foreach (var Contract_Saze_Chapkhane in contract_Saze.Contract_Saze_Chapkhanes ?? new List<Contract_Saze_Chapkhane>())
                {
                    Contract_Saze_Chapkhane.ObjectState = Enums.ObjectState.Deleted;
                }

                foreach (var Contract_Saze_Nasab in contract_Saze.Contract_Saze_Nasabs ?? new List<Contract_Saze_Nasab>())
                {
                    Contract_Saze_Nasab.ObjectState = Enums.ObjectState.Deleted;
                }
                contract_Saze.ObjectState = Enums.ObjectState.Deleted;
            }


            entity.Contract_PayRecevies = this.UnitOfWork.RepositoryAsync<Contract_PayRecevies>().Queryable().Where(x => x.ContractId == entity.ID).ToList();
            foreach (var contract_PayRecevie in entity.Contract_PayRecevies)
            {
                contract_PayRecevie.Contract_DetailPayRecevies = this.UnitOfWork.RepositoryAsync<Contract_DetailPayRecevies>().Queryable().Where(x => x.Contract_PayRecevieId == contract_PayRecevie.ID).ToList();

                foreach (var contract_DetailPayRecevie in contract_PayRecevie.Contract_DetailPayRecevies ?? new List<Contract_DetailPayRecevies>())
                {
                    contract_DetailPayRecevie.ObjectState = Enums.ObjectState.Deleted;
                }


                contract_PayRecevie.ObjectState = Enums.ObjectState.Deleted;
            }


            base.DeleteEntity(entity);
        }

        public async Task<List<ContractStopsVM>> GetAllStopContractByContractID(int contactId)
        {
            try
            {
                var contractStopsQuery = this.unitOfWork.RepositoryAsync<ContractStops>().Queryable().Where(x => x.ContractID == contactId);
                var contractStopDetailsQuery = this.unitOfWork.RepositoryAsync<ContractStopDetails>().Queryable();

                var joinQuery = from contractStop in contractStopsQuery
                                join contractStopDetails in contractStopDetailsQuery
                                on contractStop.ID equals contractStopDetails.ContractStopID into groups

                                select new ContractStopsVM
                                {
                                    ContractID = contractStop.ContractID,
                                    ContractStopDetailCommon = groups.ToList(),
                                    DateRegister = contractStop.DateRegister,
                                    Description = contractStop.Description,
                                    DisplayDateRegister = contractStop.DisplayDateRegister,
                                    ID = contractStop.ID,
                                    InvoiceID = contractStop.InvoiceID,
                                    OtherContractID = contractStop.OtherContractID,
                                    Type = contractStop.Type
                                };

                // List<Destination> listDest = mapper.Map<Source[], List<Destination>>(sources);
                 return await joinQuery.ToListAsync2();
            }
            catch (Exception ex)
            {

                throw;
            }
        

        }

        public async Task<List<string>> SaveContract(ContractVM contractVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                FinanYearRule finanYearRule = new FinanYearRule();
                var finanYears = await finanYearRule.GetAllByOrganIdAsync(organId);
                var finanYear = finanYears.Where(x => x.Closed == false && x.OrganId == organId);

                contractVM.Contract_Sazes = contractVM.Contract_Sazes.Where(x => x.Saze != null).ToList();

                foreach (var contract_Saze in contractVM.Contract_Sazes)
                {
                    if (contract_Saze.Saze != null)
                        contract_Saze.SazeId = contract_Saze.Saze.ID;

                    if (contract_Saze.NoeEjare != null)
                        contract_Saze.NoeEjareId = contract_Saze.NoeEjare.ID;

                    if (contract_Saze.DisplayTarikhShorou != null)
                    {
                        contract_Saze.TarikhShorou = PersianDateUtils.ToDateTime(contract_Saze.DisplayTarikhShorou);



                        if (contract_Saze.NoeEjareId == 1)
                            contract_Saze.TarikhPayan = contract_Saze.TarikhShorou.AddDays((double)contract_Saze.Quantity);
                        else if (contract_Saze.NoeEjareId == 2)
                        {
                            PersianCalendar pc = new PersianCalendar();
                            contract_Saze.TarikhPayan = pc.AddMonths(contract_Saze.TarikhShorou, (int)contract_Saze.Quantity);
                            contract_Saze.DisplayTarikhPayan = PersianDateUtils.ToPersianDate(contract_Saze.TarikhPayan);
                        }
                    }

                }

                if (contractVM.Contact != null)
                    contractVM.ContactId = contractVM.Contact.ID;

                DomainClasses.Contract.Contract contract = new DomainClasses.Contract.Contract();
                Mapper.Map(contractVM, contract);
                contract.OrganId = organId;
                contract.DateTime = PersianDateUtils.ToDateTime(contract.DisplayDate);


                ContractValidate validator = new ContractValidate();
                FluentValidation.Results.ValidationResult results = validator.Validate(contract);

                List<string> failurs = new List<string>(); ;

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs.Add( error.ErrorMessage);

                    }
                    return failurs;
                }

                //  contract = savePaymentForContract(contract, contractVM);
                if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                      contractVM.ContractType == ContractType.RentFrom)
                {
                    contract.Status = Status.ConfirmationContract;
                }
                else if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                    contractVM.ContractType == ContractType.PreContract)
                {
                    contract.Status = Status.ConfirmationPreContract;

                    contract.ContractType = ContractType.RentTo;

                    // contract.Contract_Sazes = uow.Repository<Contract_Saze>().Queryable().Where(x => x.ContractID == contract.ID).ToList();

                    foreach (var contract_Saze in contract.Contract_Sazes ?? new List<Contract_Saze>())
                    {
                        contract_Saze.Status = Status.ConfirmationPreContract;
                        //contract_Saze.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                    }


                }
                else if (contractVM.ContractType == ContractType.PreContract && contractVM.Status == Status.SendPreContract)
                {
                    //nothing
                }
                else
                {
                    contract.Status = Status.Temporary;
                }

                if (contract.ID > 0)
                {
                    foreach (var contractSaze in contract.Contract_Sazes)
                    {
                        contractSaze.TarikhShorou = PersianDateUtils.ToDateTime(contractSaze.DisplayTarikhShorou);
                        contractSaze.ContractID = contract.ID;

                        if (contractSaze.ID > 0)
                        {
                            contractSaze.ContractID = contract.ID;
                            contractSaze.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                        }

                        else
                        {
                            contractSaze.ContractID = contract.ID;
                            contractSaze.ObjectState = OMF.Common.Enums.ObjectState.Added;
                        }


                        foreach (var contarct_Saze_Bazareab in contractSaze.Contarct_Saze_Bazareabs)
                        {
                            contractSaze.HasBazareab = true;
                            if (contarct_Saze_Bazareab.ID > 0)
                            {
                                contarct_Saze_Bazareab.ContarctSazeID = contractSaze.ID;
                                contarct_Saze_Bazareab.ObjectState = ObjectState.Modified;
                            }
                            else
                            {
                                contarct_Saze_Bazareab.ContarctSazeID = contractSaze.ID;
                                contarct_Saze_Bazareab.ObjectState = ObjectState.Added;
                            }

                        }

                        foreach (var contract_Saze_Tarah in contractSaze.Contract_Saze_Tarahs)
                        {
                            contractSaze.HasTarah = true;

                            if (contract_Saze_Tarah.ID > 0)
                            {
                                contract_Saze_Tarah.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Tarah.ObjectState = ObjectState.Modified;
                            }

                            else
                            {
                                contract_Saze_Tarah.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Tarah.ObjectState = ObjectState.Added;
                            }

                        }

                        foreach (var contract_Saze_Chapkhane in contractSaze.Contract_Saze_Chapkhanes)
                        {
                            contractSaze.HasChap = true;

                            if (contract_Saze_Chapkhane.ID > 0)
                            {
                                contract_Saze_Chapkhane.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Chapkhane.ObjectState = ObjectState.Modified;
                            }

                            else
                            {
                                contract_Saze_Chapkhane.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Chapkhane.ObjectState = ObjectState.Added;
                            }

                        }

                        foreach (var contract_Saze_Nasab in contractSaze.Contract_Saze_Nasabs)
                        {
                            contractSaze.HasNasab = true;
                            if (contract_Saze_Nasab.ID > 0)
                            {
                                contract_Saze_Nasab.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Nasab.ObjectState = ObjectState.Modified;
                            }

                            else
                            {
                                contract_Saze_Nasab.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Nasab.ObjectState = ObjectState.Added;
                            }

                        }
                    }


                    foreach (var contract_PayRecevie in contract.Contract_PayRecevies)
                    {
                        if (contract_PayRecevie.ID > 0)
                        {
                            contract_PayRecevie.ContractId = contract.ID;
                            contract_PayRecevie.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                        }

                        else
                        {
                            contract_PayRecevie.ContractId = contract.ID;
                            contract_PayRecevie.IsReceive = true;
                            contract_PayRecevie.OrganId = organId;
                            contract_PayRecevie.Status = Status.Temporary;
                            contract_PayRecevie.Type = PayRecevieType.Sir;
                            contract_PayRecevie.Date = DateTime.Now;

                            contract_PayRecevie.ContactId = contract.ID;
                            contract_PayRecevie.ObjectState = OMF.Common.Enums.ObjectState.Added;
                        }


                        foreach (var contract_DetailPayRecevie in contract_PayRecevie.Contract_DetailPayRecevies)
                        {

                            if (contract_DetailPayRecevie.ID > 0)
                            {
                                contract_DetailPayRecevie.Contract_PayRecevieId = contract_PayRecevie.ID;
                                contract_DetailPayRecevie.ObjectState = ObjectState.Modified;
                            }
                            else
                            {
                                contract_DetailPayRecevie.Contract_PayRecevieId = contract_PayRecevie.ID;
                                contract_DetailPayRecevie.ObjectState = ObjectState.Added;
                            }

                        }

                    }
                    contract.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                }

                else
                {

                    foreach (var contractSaze in contract.Contract_Sazes)
                    {
                        contractSaze.ContractID = contract.ID;
                        contractSaze.TarikhShorou = PersianDateUtils.ToDateTime(contractSaze.DisplayTarikhShorou);

                        if (contractSaze.ID > 0)
                        {
                            contractSaze.ContractID = contract.ID;
                            contractSaze.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                        }

                        else
                        {
                            contractSaze.ContractID = contract.ID;
                            contractSaze.ObjectState = OMF.Common.Enums.ObjectState.Added;
                        }


                        foreach (var contarct_Saze_Bazareab in contractSaze.Contarct_Saze_Bazareabs)
                        {
                            contractSaze.HasBazareab = true;

                            if (contarct_Saze_Bazareab.NoeMozdBazryab == NoeMozd.Month)
                                contarct_Saze_Bazareab.Hazine = 0;

                            if (contarct_Saze_Bazareab.ID > 0)
                            {
                                contarct_Saze_Bazareab.ContarctSazeID = contractSaze.ID;
                                contarct_Saze_Bazareab.ObjectState = ObjectState.Modified;
                            }
                            else
                            {
                                contarct_Saze_Bazareab.ContarctSazeID = contractSaze.ID;
                                contarct_Saze_Bazareab.ObjectState = ObjectState.Added;
                            }
                        }

                        foreach (var contract_Saze_Tarah in contractSaze.Contract_Saze_Tarahs)
                        {
                            contractSaze.HasTarah = true;

                            if (contract_Saze_Tarah.NoeMozdTarah == NoeMozd.Month)
                                contract_Saze_Tarah.Hazine = 0;

                            if (contract_Saze_Tarah.ID > 0)
                            {
                                contract_Saze_Tarah.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Tarah.ObjectState = ObjectState.Modified;
                            }

                            else
                            {
                                contract_Saze_Tarah.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Tarah.ObjectState = ObjectState.Added;
                            }

                        }

                        foreach (var contract_Saze_Chapkhane in contractSaze.Contract_Saze_Chapkhanes)
                        {
                            contractSaze.HasChap = true;



                            if (contract_Saze_Chapkhane.ID > 0)
                            {
                                contract_Saze_Chapkhane.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Chapkhane.ObjectState = ObjectState.Modified;
                            }

                            else
                            {
                                contract_Saze_Chapkhane.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Chapkhane.ObjectState = ObjectState.Added;
                            }

                        }

                        foreach (var contract_Saze_Nasab in contractSaze.Contract_Saze_Nasabs)
                        {
                            contractSaze.HasNasab = true;

                            if (contract_Saze_Nasab.NoeMozdNasab == NoeMozd.Month)
                                contract_Saze_Nasab.Hazine = 0;

                            if (contract_Saze_Nasab.ID > 0)
                            {
                                contract_Saze_Nasab.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Nasab.ObjectState = ObjectState.Modified;
                            }

                            else
                            {
                                contract_Saze_Nasab.ContarctSazeID = contractSaze.ID;
                                contract_Saze_Nasab.ObjectState = ObjectState.Added;
                            }

                        }
                    }

                    if (contract.Contract_PayRecevies != null && contract.Contract_PayRecevies.Count() > 0 &&
                        contract.Contract_PayRecevies[0].Contract_DetailPayRecevies != null && contract.Contract_PayRecevies[0].Contract_DetailPayRecevies.Count() > 0)
                    {
                        foreach (var contract_PayRecevie in contract.Contract_PayRecevies)
                        {
                            contract_PayRecevie.ContractId = contract.ID;
                            contract_PayRecevie.Date = DateTime.Now;
                            contract_PayRecevie.IsReceive = true;


                            if (contract_PayRecevie.ID > 0)
                            {
                                contract_PayRecevie.ContractId = contract.ID;
                                contract_PayRecevie.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                            }

                            else
                            {
                                contract_PayRecevie.ContractId = contract.ID;
                                contract_PayRecevie.ObjectState = OMF.Common.Enums.ObjectState.Added;
                            }


                            foreach (var contract_DetailPayRecevie in contract_PayRecevie.Contract_DetailPayRecevies)
                            {

                                if (contract_DetailPayRecevie.ID > 0)
                                {
                                    contract_DetailPayRecevie.Contract_PayRecevieId = contract_PayRecevie.ID;
                                    contract_DetailPayRecevie.ObjectState = ObjectState.Modified;
                                }
                                else
                                {
                                    contract_DetailPayRecevie.Contract_PayRecevieId = contract_PayRecevie.ID;
                                    contract_DetailPayRecevie.ObjectState = ObjectState.Added;
                                }
                            }

                        }
                    }
                    else
                    {
                        contract.Contract_PayRecevies = null;
                    }
                    contract.ObjectState = OMF.Common.Enums.ObjectState.Added;
                }


                this.UnitOfWork.RepositoryAsync<DomainClasses.Contract.Contract>().InsertOrUpdateGraph(contract);



                //  ContactRule contactRule = new ContactRule();
                //   await contactRule.UpdateContact(invoice.InvoiceType, invoice.ContactId);


              
                await this.UnitOfWork.SaveChangesAsync();

                if (contractVM.ID <= 0)
                {
                    if ((SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                        contractVM.ContractType == ContractType.PreContract) || (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                        contractVM.Status == Status.ConfirmationContract))
                    {
                        InvoiceRule invoiceRule = new InvoiceRule();
                        var invoice = invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentTo);

                        var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(document, invoice.OrganId);
                        documentRule.SaveChanges();
                    }
                    else if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                        contractVM.ContractType == ContractType.RentTo)
                    {
                        InvoiceRule invoiceRule = new InvoiceRule();
                        invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentFrom);
                    }

                }




                return failurs;
            }
            catch (Exception ex)
            {
                var p = ex;
                throw;
            }

        }
    }
}