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
using Zhivar.ViewModel.Contract;
using Zhivar.Business.Contract;
using Zhivar.Web.Controllers.Accounting;
using Zhivar.DomainClasses.Contract;
using OMF.Security.Model;
using Zhivar.ViewModel.Security;
using Zhivar.Business.Security;
using System.Globalization;

namespace Zhivar.Web.Controllers.Contract
{
    [RoutePrefix("api/Contract")]
    public class ContractController : NewApiControllerBaseAsync<DomainClasses.Contract.Contract, ContractVM>
    {
        public ContractRule Rule => this.BusinessRule as ContractRule;

        protected override IBusinessRuleBaseAsync<DomainClasses.Contract.Contract> CreateBusinessRule()
        {
            return new ContractRule();
        }


        public async Task<HttpResponseMessage> loadContractData([FromBody] loadContractDataBusi contractData)
        {
            try
            {


                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var resualt = new ContractData();

                var userInfosQuery = this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Queryable().Where(x => x.OrganizationId == organId);
                var userRoleQuery = this.BusinessRule.UnitOfWork.Repository<UserRole>().Queryable();
                var roleQuery = this.BusinessRule.UnitOfWork.Repository<Role>().Queryable();

                var joinQuery = from userInfo in userInfosQuery
                                join userRole in userRoleQuery
                                        on userInfo.ID equals userRole.UserId
                                join role in roleQuery
                                on userRole.RoleId equals role.ID
                                select new UsersForRule
                                {
                                    ID = userInfo.ID,
                                    Name = userInfo.FirstName + " " + userInfo.LastName,
                                    RoleId = userRole.RoleId,
                                    RoleName = role.Name,


                                };
                var joinList = await joinQuery.ToListAsync2();

                resualt.nasabs = joinList.Where(x => x.RoleId == 10).ToList();
                resualt.chapkhanes = joinList.Where(x => x.RoleId == 9).ToList();
                resualt.tarahs = joinList.Where(x => x.RoleId == 8).ToList();
                resualt.bazaryabs = joinList.Where(x => x.RoleId == 7).ToList();

                CashRule cashRule = new CashRule();
                var cashes = await cashRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.cashes = Mapper.Map<List<CashVM>>(cashes);

                BankRule bankRule = new BankRule();
                var banks = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.banks = Mapper.Map<List<BankVM>>(banks);

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



                }



                resualt.contacts = contacts;
                resualt.contractSettings = new ContractSettings()
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


                NoeEjareRule noeEjareRule = new NoeEjareRule();
                var noeEjares = await noeEjareRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.noeEjares = noeEjares;


                NoeChapRule noeChapRule = new NoeChapRule();
                var noeChaps = await noeChapRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.noeChaps = noeChaps;


                GoroheSazeRule goroheSazeRule = new GoroheSazeRule();
                var itemGroups = await goroheSazeRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                var items = new List<SazeVM>();
                var item = new SazeVM();

                foreach (var itemGroup in itemGroups)
                {
                    foreach (var KalaKhadmat in itemGroup.Items)
                    {
                        item = new SazeVM()
                        {
                            DetailAccount = new DetailAccount()
                            {
                                Code = KalaKhadmat.Code,
                                Id = KalaKhadmat.ID,
                                Node = new Node()
                                {
                                    FamilyTree = itemGroup.Title,
                                    Name = itemGroup.Title,
                                    Id = itemGroup.ID
                                }
                            },
                            Code = KalaKhadmat.Code,
                            OrganId = KalaKhadmat.OrganId,
                            Address = KalaKhadmat.Address,
                            Arz = KalaKhadmat.Arz,
                            GoroheSazeID = KalaKhadmat.GoroheSazeID,
                            ID = KalaKhadmat.ID,
                            Title = KalaKhadmat.Title,
                            Tol = KalaKhadmat.Tol,
                            NoeSazeId = KalaKhadmat.NoeSazeId,
                            NoeEjare = KalaKhadmat.NoeEjare,
                            GoroheName = KalaKhadmat.GoroheName,
                            NoeEjareName = KalaKhadmat.NoeEjareName,
                            NoeSazeName = KalaKhadmat.NoeSazeName,
                            Latitude = KalaKhadmat.Latitude,
                            Longitude = KalaKhadmat.Longitude,
                            NoorDard = KalaKhadmat.NoorDard,
                            NoeEjareID = KalaKhadmat.NoeEjareID,

                        };

                        items.Add(item);
                    }
                }

                resualt.items = items;

                var Contract_Sazes = new List<Contract_SazeVM>();
                var Contract_PayRecevieVMs = new List<Contract_PayRecevieVM>();


                if (contractData.id == 0)
                {
                    var countSaze = 0;
                    if (contractData.lstSaze != null && contractData.lstSaze.Count > 0)
                    {
                        contractData.lstSaze = contractData.lstSaze.Where(x => x.sazeID != 0).ToList();

                        foreach (var lstSaze in contractData.lstSaze)
                        {
                            var saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(lstSaze.sazeID));

                            var startDate = DateTime.Now;
                            var displayStartDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);

                            bool minValue = false;
                            bool maxValue = false;

                            if (!string.IsNullOrEmpty(lstSaze.minDate) && !string.IsNullOrWhiteSpace(lstSaze.minDate))
                            {
                                startDate = PersianDateUtils.ToDateTime(lstSaze.minDate);
                                displayStartDate = lstSaze.minDate;
                                minValue = true;
                            }

                            var endDate = DateTime.Now;
                            var displayEndDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);

                            if (!string.IsNullOrEmpty(lstSaze.maxDate) && !string.IsNullOrWhiteSpace(lstSaze.maxDate))
                            {
                                endDate = PersianDateUtils.ToDateTime(lstSaze.maxDate);
                                displayStartDate = lstSaze.maxDate;
                                maxValue = true;
                            }

                            var quantity = 0;

                            if (minValue && maxValue)
                            {
                                var diff = endDate.Date - startDate.Date;
                                var days = diff.TotalDays;

                                quantity = Convert.ToInt32(days);

                            }

                            Contract_Sazes.Add(new Contract_SazeVM()
                            {
                                Description = "",
                                Discount = 0,
                                ID = 0,
                                ItemInput = "",
                                Quantity = quantity,
                                RowNumber = countSaze,
                                Sum = 0,
                                Tax = 0,
                                TotalAmount = 0,
                                UnitPrice = 0,
                                PriceChap = 0,
                                PriceBazareab = 0,
                                PriceNasab = 0,
                                PriceTarah = 0,
                                Saze = saze,
                                SazeId = saze.ID,
                                NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(saze.NoeEjareID)),
                                NoeEjareId = saze.NoeEjareID,
                                DisplayTarikhShorou = displayStartDate,
                                TarikhShorou = startDate
                            });

                            countSaze++;
                        }
                    }

                    if (countSaze < 4)
                    {
                        for (int i = 0; i < 4-countSaze; i++)
                        {
                            Contract_Sazes.Add(new Contract_SazeVM()
                            {
                                Description = "",
                                Discount = 0,
                                ID = 0,
                                ItemInput = "",
                                Quantity = 0,
                                RowNumber = countSaze+i,
                                Sum = 0,
                                Tax = 0,
                                TotalAmount = 0,
                                UnitPrice = 0,
                                PriceChap = 0,
                                PriceBazareab = 0,
                                PriceNasab = 0,
                                PriceTarah = 0,
                                Saze = null,
                                SazeId = 0,
                                NoeEjare = null,
                                NoeEjareId = 0,
                                DisplayTarikhShorou = "",
                              //  TarikhShorou = null
                            });
                        }
                    
                    }
  
                    resualt.contract = new ContractVM()
                    {
                        ContactTitle = "",
                        DateTime = DateTime.Now,
                        DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                        DisplayDueDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                        DueDate = DateTime.Now,
                        ID = 0,
                        Contract_Sazes = Contract_Sazes,
                        IsDraft = true,
                        Note = "",
                        Number = await createNumberContract(organId),
                        Paid = 0,
                        Payable = 0,
                        Profit = 0,
                        Rest = 0,
                        Sent = false,
                        Status = 0,
                        Sum = 0,
                        Tag = "",
                        AutoTax = true

                    };

                    if (contractData.contractType != null)
                        resualt.contract.ContractType = (ZhivarEnums.ContractType)contractData.contractType;

                    resualt.contract.Contract_PayRecevies = new List<Contract_PayRecevieVM>();
                    resualt.contract.Contract_PayRecevies.Add(new Contract_PayRecevieVM()
                    {
                        Contract_DetailPayRecevies = new List<Contract_DetailPayReceviesVM>()
                    });
                }
                else
                {
                    var invoice = await Rule.FindAsync(contractData.id);

                    foreach (var Contract_PayRecevie in invoice.Contract_PayRecevies ?? new List<Contract_PayRecevies>())
                    {
                        Contract_PayRecevieVMs.Add(new Contract_PayRecevieVM()
                        {
                            AccountId = Contract_PayRecevie.AccountId,
                            Date = Contract_PayRecevie.Date,
                            DocumentId = Contract_PayRecevie.DocumentId,
                            //Type = Contract_PayRecevie.Type,
                            OrganId = Contract_PayRecevie.OrganId,
                            Status = Contract_PayRecevie.Status,
                            IsReceive = Contract_PayRecevie.IsReceive,
                            
                            Amount = Contract_PayRecevie.Amount,
                            Description = Contract_PayRecevie.Description,
                            DisplayDate = Contract_PayRecevie.DisplayDate,
                            ID = Contract_PayRecevie.ID,
                            IsRecevie = Contract_PayRecevie.IsReceive,
                            Number = Contract_PayRecevie.Number,
                            Contract_DetailPayRecevies = Mapper.Map<List<Contract_DetailPayRecevies>, List<Contract_DetailPayReceviesVM>>(Contract_PayRecevie.Contract_DetailPayRecevies),
                        });

                    }




                    foreach (var contract_Saze in invoice.Contract_Sazes ?? new List<Contract_Saze>())
                    {
                        Contract_Sazes.Add(new Contract_SazeVM()
                        {
                            Description = contract_Saze.Description,
                            Discount = contract_Saze.Discount,
                            ID = contract_Saze.ID,
                            Saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(contract_Saze.SazeId)),
                            SazeId = contract_Saze.SazeId,
                            ItemInput = contract_Saze.ItemInput,
                            Quantity = contract_Saze.Quantity,
                            RowNumber = contract_Saze.RowNumber,
                            Sum = contract_Saze.Sum,
                            Tax = contract_Saze.Tax,
                            TotalAmount = contract_Saze.TotalAmount,
                            NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(contract_Saze.NoeEjareId)),
                            NoeEjareId = contract_Saze.NoeEjareId,
                            CalcTax = contract_Saze.CalcTax,
                            ContractID = contract_Saze.ContractID,
                            DisplayTarikhShorou = contract_Saze.DisplayTarikhShorou,
                            DisplayTarikhPayan = contract_Saze.DisplayTarikhPayan,
                            HasBazareab = contract_Saze.HasBazareab,
                            HasChap = contract_Saze.HasChap,
                            HasNasab = contract_Saze.HasNasab,
                            HasTarah = contract_Saze.HasTarah,
                            PriceBazareab = contract_Saze.PriceBazareab,
                            PriceChap = contract_Saze.PriceChap,
                            PriceNasab = contract_Saze.PriceNasab,
                            PriceTarah = contract_Saze.PriceTarah,
                            TarikhShorou = contract_Saze.TarikhShorou,
                            TarikhPayan = contract_Saze.TarikhPayan,
                            UnitItem = contract_Saze.UnitItem,
                            UnitPrice = contract_Saze.UnitPrice,
                            Contarct_Saze_Bazareabs = Mapper.Map<List<Contract_Saze_Bazareab>, List<Contract_Saze_BazareabVM>>(contract_Saze.Contarct_Saze_Bazareabs),
                            Contract_Saze_Chapkhanes = Mapper.Map<List<Contract_Saze_Chapkhane>, List<Contract_Saze_ChapkhaneVM>>(contract_Saze.Contract_Saze_Chapkhanes),
                            Contract_Saze_Tarahs = Mapper.Map<List<Contract_Saze_Tarah>, List<Contract_Saze_TarahVM>>(contract_Saze.Contract_Saze_Tarahs),
                            Contract_Saze_Nasabs = Mapper.Map<List<Contract_Saze_Nasab>, List<Contract_Saze_NasabVM>>(contract_Saze.Contract_Saze_Nasabs),
                        });

                    }


                    if (Contract_Sazes != null)
                    {
                        foreach (var contract_Saze in Contract_Sazes)
                        {
                            if (contract_Saze.Contarct_Saze_Bazareabs != null)
                            {
                                foreach (var contarct_Saze_Bazareab in contract_Saze.Contarct_Saze_Bazareabs)
                                {
                                    contarct_Saze_Bazareab.User = Mapper.Map<UserInfo, UsersForRule>(this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Find(contarct_Saze_Bazareab.UserID));
                                    contarct_Saze_Bazareab.User.RoleId = 7;
                                    contarct_Saze_Bazareab.User.Name = contarct_Saze_Bazareab.User.FirstName + " " + contarct_Saze_Bazareab.User.LastName;
                                }
                            }

                            if (contract_Saze.Contract_Saze_Tarahs != null)
                            {
                                foreach (var contract_Saze_Tarahs in contract_Saze.Contract_Saze_Tarahs)
                                {
                                    contract_Saze_Tarahs.User = Mapper.Map<UserInfo, UsersForRule>(this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Find(contract_Saze_Tarahs.UserID));
                                    contract_Saze_Tarahs.User.RoleId = 8;
                                    contract_Saze_Tarahs.User.Name = contract_Saze_Tarahs.User.FirstName + " " + contract_Saze_Tarahs.User.LastName;
                                }
                            }

                            if (contract_Saze.Contract_Saze_Chapkhanes != null)
                            {
                                foreach (var contract_Saze_Chapkhane in contract_Saze.Contract_Saze_Chapkhanes)
                                {
                                    contract_Saze_Chapkhane.User = Mapper.Map<UserInfo, UsersForRule>(this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Find(contract_Saze_Chapkhane.UserID));
                                    contract_Saze_Chapkhane.User.RoleId = 9;
                                    contract_Saze_Chapkhane.User.Name = contract_Saze_Chapkhane.User.FirstName + " " + contract_Saze_Chapkhane.User.LastName;
                                }
                            }

                            if (contract_Saze.Contract_Saze_Nasabs != null)
                            {
                                foreach (var contract_Saze_Nasab in contract_Saze.Contract_Saze_Nasabs)
                                {
                                    contract_Saze_Nasab.User = Mapper.Map<UserInfo, UsersForRule>(this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Find(contract_Saze_Nasab.UserID));
                                    contract_Saze_Nasab.User.RoleId = 10;
                                    contract_Saze_Nasab.User.Name = contract_Saze_Nasab.User.FirstName + " " + contract_Saze_Nasab.User.LastName;


                                }
                            }
                        }
                    }


                    resualt.contract = new ContractVM()
                    {
                        Contact = Mapper.Map<Contact, ContactVM>(this.BusinessRule.UnitOfWork.Repository<Contact>().Find(invoice.ContactId)),
                       // ContactTitle = invoice.ContractTitle,
                        ContractTitle = invoice.ContractTitle,
                        DateTime = invoice.DateTime,
                        DisplayDate = invoice.DisplayDate,
                        DisplayDueDate = invoice.DisplayDueDate,
                        // invoiceDueDate = invoice.DisplayDueDate,

                        DueDate = invoice.DueDate,
                        ID = invoice.ID,

                        Contract_Sazes = Contract_Sazes,
                        Contract_PayRecevies = Contract_PayRecevieVMs,
                        //InvoiceStatusString = invoice.InvoiceStatusString,
                        // InvoiceType = invoice.InvoiceType,
                        //  InvoiceTypeString = invoice.InvoiceTypeString,
                        IsDraft = invoice.IsDraft,
                        //  IsPurchase = invoice.IsPurchase,
                        //   IsPurchaseReturn = invoice.IsPurchaseReturn,
                        //  IsSale = invoice.IsSale,
                        //   IsSaleReturn = invoice.IsSaleReturn,
                        //  IsWaste = false,
                        Note = invoice.Note,
                        Number = invoice.Number,
                        Paid = invoice.Paid,
                        Payable = invoice.Payable,
                        Profit = invoice.Profit,
                        //    Reference = invoice.Refrence,
                        Rest = invoice.Rest,
                        //     Returned = invoice.Returned,
                        Sent = invoice.Sent,
                        Status = invoice.Status,
                        Sum = invoice.Sum,
                        Tag = invoice.Tag,
                        ContractType = invoice.ContractType,
                        AutoTax = invoice.AutoTax
                    };

                    //if (Contract_PayRecevieVMs != null && Contract_PayRecevieVMs.Count > 0)
                    //{
                    //    resualt.contract.Items = Contract_PayRecevieVMs[0].Contract_DetailPayReceviesVMs;
                    //}
                    //else
                    //{
                    //    resualt.contract.Items = new List<Contract_DetailPayReceviesVM>();
                    //}
                    return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<HttpResponseMessage> loadStopContractData([FromBody] loadStopContractDataBusi contractData)
        {
            try
            {


                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var resualt = new ContractStopData();

                NoeEjareRule noeEjareRule = new NoeEjareRule();
                var noeEjares = await noeEjareRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.noeEjares = noeEjares;


                GoroheSazeRule goroheSazeRule = new GoroheSazeRule();
                var itemGroups = await goroheSazeRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                var items = new List<SazeVM>();
                var item = new SazeVM();

                foreach (var itemGroup in itemGroups)
                {
                    foreach (var KalaKhadmat in itemGroup.Items)
                    {
                        item = new SazeVM()
                        {
                            DetailAccount = new DetailAccount()
                            {
                                Code = KalaKhadmat.Code,
                                Id = KalaKhadmat.ID,
                                Node = new Node()
                                {
                                    FamilyTree = itemGroup.Title,
                                    Name = itemGroup.Title,
                                    Id = itemGroup.ID
                                }
                            },
                            Code = KalaKhadmat.Code,
                            OrganId = KalaKhadmat.OrganId,
                            Address = KalaKhadmat.Address,
                            Arz = KalaKhadmat.Arz,
                            GoroheSazeID = KalaKhadmat.GoroheSazeID,
                            ID = KalaKhadmat.ID,
                            Title = KalaKhadmat.Title,
                            Tol = KalaKhadmat.Tol,
                            NoeSazeId = KalaKhadmat.NoeSazeId,
                            NoeEjare = KalaKhadmat.NoeEjare,
                            GoroheName = KalaKhadmat.GoroheName,
                            NoeEjareName = KalaKhadmat.NoeEjareName,
                            NoeSazeName = KalaKhadmat.NoeSazeName,
                            Latitude = KalaKhadmat.Latitude,
                            Longitude = KalaKhadmat.Longitude,
                            NoorDard = KalaKhadmat.NoorDard,
                            NoeEjareID = KalaKhadmat.NoeEjareID,

                        };

                        items.Add(item);
                    }
                }

                resualt.items = items;

                var contractStopDetails = new List<ContractStopDetailsVM>();

                var contract = await Rule.FindAsync(contractData.contractID);

                if (contractData.isStopGroup == false)
                    contract.Contract_Sazes = contract.Contract_Sazes.Where(x => x.SazeId == contractData.sazeID).ToList();

                var count = 0;

                var ContractStop = await Rule.UnitOfWork.RepositoryAsync<ContractStops>().Queryable().Where(x => x.ContractID == contractData.contractID).SingleOrDefaultAsync2();

                if (ContractStop != null)
                {
                    //    ContractStop.ContractStopDetails = await Rule.UnitOfWork.RepositoryAsync<ContractStopDetails>().Queryable().Where(x => x.ContractStopID == ContractStop.ID).ToListAsync2();

                    //    foreach (var contractStopDetail in ContractStop.ContractStopDetails ?? new List<ContractStopDetails>())
                    //    {
                    //        contractStopDetails.Add(new ContractStopDetailsVM
                    //        {
                    //            ID = contractStopDetail.ID,
                    //            Saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(contractStopDetail.SazeID)),
                    //            Quantity = contractStopDetail.Quantity,
                    //            NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(contractStopDetail.NoeEjareID)),
                    //            DisplayStartDate = contractStopDetail.DisplayStartDate,
                    //            NoeEjareID = contractStopDetail.NoeEjareID,
                    //            SazeID = contractStopDetail.SazeID,
                    //            StartDate = contractStopDetail.StartDate,
                    //            RowNumber = count,
                    //            DisplayEndDate = contractStopDetail.DisplayEndDate,
                    //            EndDate = contractStopDetail.EndDate,
                    //            ContractStopID = contractStopDetail.ContractStopID

                    //        });

                    //        count++;
                    //    }



                    foreach (var contract_Saze in contract.Contract_Sazes ?? new List<Contract_Saze>())
                    {
                        contractStopDetails.Add(new ContractStopDetailsVM
                        {
                            ID = 0,
                            Saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(contract_Saze.SazeId)),
                            Quantity = 1,
                            NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(contract_Saze.NoeEjareId)),
                            DisplayStartDate = Utilities.PersianDateUtils.ToPersianDateTime(DateTime.Now),
                            NoeEjareID = contract_Saze.NoeEjareId,
                            SazeID = contract_Saze.SazeId,
                            StartDate = DateTime.Now,
                            RowNumber = count,


                        });

                        count++;

                    }


                    resualt.contractStop = new ContractStopsVM()
                    {
                        ID = ContractStop.ID,
                        ContractID = ContractStop.ContractID,
                        DateRegister = ContractStop.DateRegister,
                        DisplayDateRegister = ContractStop.DisplayDateRegister,
                        Type = ContractStop.Type,
                        ContractStopDetails = contractStopDetails,
                        Description = ContractStop.Description,
                        FileName = ContractStop.FileName,
                        FileSize = ContractStop.FileSize,
                        InvoiceID = ContractStop.InvoiceID,
                        MimeType = ContractStop.MimeType,
                        OtherContractID = ContractStop.OtherContractID,

                    };

                    if (ContractStop.Blob != null)
                    {
                        resualt.contractStop.TasvirBlobBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(ContractStop.Blob));
                        resualt.contractStop.Blob = ContractStop.Blob;
                    }

                }
                else
                {
                    count = 0;

                    foreach (var contract_Saze in contract.Contract_Sazes ?? new List<Contract_Saze>())
                    {
                        contractStopDetails.Add(new ContractStopDetailsVM
                        {
                            ID = 0,
                            Saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(contract_Saze.SazeId)),
                            Quantity = 1,
                            NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(contract_Saze.NoeEjareId)),
                            DisplayStartDate = Utilities.PersianDateUtils.ToPersianDateTime(DateTime.Now),
                            NoeEjareID = contract_Saze.NoeEjareId,
                            SazeID = contract_Saze.SazeId,
                            StartDate = DateTime.Now,
                            RowNumber = count,


                        });

                        count++;

                    }

                    resualt.contractStop = new ContractStopsVM()
                    {
                        ID = 0,
                        ContractID = contractData.contractID,
                        DateRegister = DateTime.Now,
                        DisplayDateRegister = Utilities.PersianDateUtils.ToPersianDate(DateTime.Now),
                        Type = ContractStopType.NoAction,
                        ContractStopDetails = contractStopDetails,
                        Description = string.Empty,

                    };

                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<HttpResponseMessage> loadRockData([FromBody] loadRockDataBusi rockData)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var resualt = new RockData();

                GoroheSazeRule goroheSazeRule = new GoroheSazeRule();
                var itemGroups = await goroheSazeRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            
                var items = new List<SazeVM>();
                var item = new SazeVM();

                foreach (var itemGroup in itemGroups)
                {
                    foreach (var KalaKhadmat in itemGroup.Items)
                    {
                        item = new SazeVM()
                        {
                            DetailAccount = new DetailAccount()
                            {
                                Code = KalaKhadmat.Code,
                                Id = KalaKhadmat.ID,
                                Node = new Node()
                                {
                                    FamilyTree = itemGroup.Title,
                                    Name = itemGroup.Title,
                                    Id = itemGroup.ID
                                }
                            },
                            Code = KalaKhadmat.Code,
                            OrganId = KalaKhadmat.OrganId,
                            Address = KalaKhadmat.Address,
                            Arz = KalaKhadmat.Arz,
                            GoroheSazeID = KalaKhadmat.GoroheSazeID,
                            ID = KalaKhadmat.ID,
                            Title = KalaKhadmat.Title,
                            Tol = KalaKhadmat.Tol,
                            NoeSazeId = KalaKhadmat.NoeSazeId,
                            NoeEjare = KalaKhadmat.NoeEjare,
                            GoroheName = KalaKhadmat.GoroheName,
                            NoeEjareName = KalaKhadmat.NoeEjareName,
                            NoeSazeName = KalaKhadmat.NoeSazeName,
                            Latitude = KalaKhadmat.Latitude,
                            Longitude = KalaKhadmat.Longitude,
                            NoorDard = KalaKhadmat.NoorDard,
                            NoeEjareID = KalaKhadmat.NoeEjareID
                        };

                        items.Add(item);
                    }
                }

                resualt.Sazes = items;

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
                }

                resualt.contacts = contacts;

                if (rockData.SazeID != null)
                {
                    int temp = (int)rockData.SazeID;
                    itemGroups = itemGroups.Where(l => l.Items.Any(m => m.ID == temp)).ToList();

                    foreach (var itemGroup in itemGroups)
                    {
                        itemGroup.Items = itemGroup.Items.Where(x => x.ID == temp).ToList();
                    }
                }
      
        
                resualt.listTemplateDate = new List<TemplateDate>();
                resualt.listTemplateDate = createTemplateDate(ref rockData);

                List<SazeOfContractInTime> list = await GetListSazeOfContractInTime(rockData);


                if (rockData.ContactID != null && rockData.ContactID > 0)
                {
                    int tempContactID = Convert.ToInt32(rockData.ContactID);

                    List<int> listSazeID = list.Where(x => x.ContactID == tempContactID).Select(x => x.SazeID).ToList();

                 
                    itemGroups = itemGroups.Where(l => l.Items.Any(m => listSazeID.Contains(m.ID))).ToList();

                    foreach (var itemGroup in itemGroups)
                    {
                        itemGroup.Items = itemGroup.Items.Where(x => listSazeID.Contains( x.ID)).ToList();
                    }
                }

                foreach (var itemGroup in itemGroups)
                {
                    foreach (var KalaKhadmat in itemGroup.Items)
                    {
                        KalaKhadmat.SazeOfContractInTimes = list.Where(x => x.SazeID == KalaKhadmat.ID).ToList();
                    }
                }

                resualt.listGoroheSaze = itemGroups.Where(x => x.Items.Any()).ToList();

                List<CalcTotal> totals = new List<CalcTotal>();

                foreach (var item2 in list)
                {
                    var total = totals.Find(x => x.SazeID == item2.SazeID);
                    if (total != null)
                    {
                        total.Occupy += item2.Occupy;
                    }
                    else
                    {
                        totals.Add(new CalcTotal() {
                            Occupy=item2.Occupy,
                            GroupSazeID = item2.GoroheSazeID,
                            SazeID = item2.SazeID
                        });
                    }
                }

                bool showCostFrom = false;
                if(SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == ZhivarConstants.ManageRoleCode))
                {
                    showCostFrom = true;
                }

                foreach (var goroheSaz in resualt.listGoroheSaze?? new List<GoroheSazeVM>())
                {
                    var count = goroheSaz.Items.Count();
                    goroheSaz.Occupy = Math.Round( totals.Where(x => x.GroupSazeID == goroheSaz.ID).Sum(x => x.Occupy)/count,2);
                    goroheSaz.CostRentTo = Math.Round(list.Where(x => x.GoroheSazeID == goroheSaz.ID).Sum(x => x.CostRentTo),0);

                    if (showCostFrom)
                        goroheSaz.CostRentFrom = Math.Round(list.Where(x => x.GoroheSazeID == goroheSaz.ID).Sum(x => x.CostRentFrom), 0);
                    else
                        goroheSaz.CostRentFrom = 0;

                   goroheSaz.ShowCostFrom = showCostFrom;
                   goroheSaz.CountOfRent = list.Where(x => x.GoroheSazeID == goroheSaz.ID).Count();
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<HttpResponseMessage> loadReservationData([FromBody] string reservationID)
        {
            try
            {


                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var resualt = new ContractData();

                var userInfosQuery = this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().Queryable().Where(x => x.OrganizationId == organId);
                var userRoleQuery = this.BusinessRule.UnitOfWork.Repository<UserRole>().Queryable();
                var roleQuery = this.BusinessRule.UnitOfWork.Repository<Role>().Queryable();

                var joinQuery = from userInfo in userInfosQuery
                                join userRole in userRoleQuery
                                        on userInfo.ID equals userRole.UserId
                                join role in roleQuery
                                on userRole.RoleId equals role.ID
                                select new UsersForRule
                                {
                                    ID = userInfo.ID,
                                    Name = userInfo.FirstName + " " + userInfo.LastName,
                                    RoleId = userRole.RoleId,
                                    RoleName = role.Name,


                                };
                var joinList = await joinQuery.ToListAsync2();

                resualt.nasabs = joinList.Where(x => x.RoleId == 10).ToList();
                resualt.chapkhanes = joinList.Where(x => x.RoleId == 9).ToList();
                resualt.tarahs = joinList.Where(x => x.RoleId == 8).ToList();
                resualt.bazaryabs = joinList.Where(x => x.RoleId == 7).ToList();

                CashRule cashRule = new CashRule();
                var cashes = await cashRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.cashes = Mapper.Map<List<CashVM>>(cashes);

                BankRule bankRule = new BankRule();
                var banks = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.banks = Mapper.Map<List<BankVM>>(banks);

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



                }



                resualt.contacts = contacts;
                resualt.contractSettings = new ContractSettings()
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


                NoeEjareRule noeEjareRule = new NoeEjareRule();
                var noeEjares = await noeEjareRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.noeEjares = noeEjares;


                NoeChapRule noeChapRule = new NoeChapRule();
                var noeChaps = await noeChapRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                resualt.noeChaps = noeChaps;


                GoroheSazeRule goroheSazeRule = new GoroheSazeRule();
                var itemGroups = await goroheSazeRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                var items = new List<SazeVM>();
                var item = new SazeVM();

                foreach (var itemGroup in itemGroups)
                {
                    foreach (var KalaKhadmat in itemGroup.Items)
                    {
                        item = new SazeVM()
                        {
                            DetailAccount = new DetailAccount()
                            {
                                Code = KalaKhadmat.Code,
                                Id = KalaKhadmat.ID,
                                Node = new Node()
                                {
                                    FamilyTree = itemGroup.Title,
                                    Name = itemGroup.Title,
                                    Id = itemGroup.ID
                                }
                            },
                            Code = KalaKhadmat.Code,
                            OrganId = KalaKhadmat.OrganId,
                            Address = KalaKhadmat.Address,
                            Arz = KalaKhadmat.Arz,
                            GoroheSazeID = KalaKhadmat.GoroheSazeID,
                            ID = KalaKhadmat.ID,
                            Title = KalaKhadmat.Title,
                            Tol = KalaKhadmat.Tol,
                            NoeSazeId = KalaKhadmat.NoeSazeId,
                            NoeEjare = KalaKhadmat.NoeEjare,
                            GoroheName = KalaKhadmat.GoroheName,
                            NoeEjareName = KalaKhadmat.NoeEjareName,
                            NoeSazeName = KalaKhadmat.NoeSazeName,
                            Latitude = KalaKhadmat.Latitude,
                            Longitude = KalaKhadmat.Longitude,
                            NoorDard = KalaKhadmat.NoorDard,
                            NoeEjareID = KalaKhadmat.NoeEjareID,

                        };

                        items.Add(item);
                    }
                }

                resualt.items = items;

                var Contract_Sazes = new List<Contract_SazeVM>();
                var Contract_PayRecevieVMs = new List<Contract_PayRecevieVM>();

                var intReservationID = Convert.ToInt32(reservationID);

                var reservation = await Rule.UnitOfWork.RepositoryAsync<Reservation>().FindAsync(intReservationID);
                var reservationDetails = await Rule.UnitOfWork.RepositoryAsync<Reservation_Detail>().Queryable().Where( x => x.ReservationID == intReservationID).ToListAsync2();

                var countRes = 0;
                foreach (var reservationDetail in reservationDetails ?? new List<Reservation_Detail>())
                    {
                        Contract_Sazes.Add(new Contract_SazeVM()
                        {
                            Discount = 0,
                            ID = 0,
                            Saze = Mapper.Map<Saze, SazeVM>(this.BusinessRule.UnitOfWork.Repository<Saze>().Find(reservationDetail.SazeID)),
                            SazeId = reservationDetail.SazeID,
                            ItemInput = string.Empty,
                            Quantity = reservationDetail.Quantity,
                            RowNumber = countRes,
                            Sum = 0,
                            Tax = 0,
                            TotalAmount = 0,
                            NoeEjare = Mapper.Map<NoeEjare, NoeEjareVM>(this.BusinessRule.UnitOfWork.Repository<NoeEjare>().Find(reservationDetail.NoeEjareID)),
                            NoeEjareId = reservationDetail.NoeEjareID,
                            CalcTax = true,
                            ContractID = 0,
                            DisplayTarikhShorou = reservationDetail.StartDisplayDate,
                            //HasBazareab = contract_Saze.HasBazareab,
                            //HasChap = contract_Saze.HasChap,
                            //HasNasab = contract_Saze.HasNasab,
                            //HasTarah = contract_Saze.HasTarah,
                            //PriceBazareab = contract_Saze.PriceBazareab,
                            //PriceChap = contract_Saze.PriceChap,
                            //PriceNasab = contract_Saze.PriceNasab,
                            //PriceTarah = contract_Saze.PriceTarah,
                            TarikhShorou = reservationDetail.StartDate,
                           // UnitItem = contract_Saze.UnitItem,
                           // UnitPrice = contract_Saze.UnitPrice,
                            //Contarct_Saze_Bazareabs = Mapper.Map<List<Contract_Saze_Bazareab>, List<Contract_Saze_BazareabVM>>(contract_Saze.Contarct_Saze_Bazareabs),
                            //Contract_Saze_Chapkhanes = Mapper.Map<List<Contract_Saze_Chapkhane>, List<Contract_Saze_ChapkhaneVM>>(contract_Saze.Contract_Saze_Chapkhanes),
                            //Contract_Saze_Tarahs = Mapper.Map<List<Contract_Saze_Tarah>, List<Contract_Saze_TarahVM>>(contract_Saze.Contract_Saze_Tarahs),
                            //Contract_Saze_Nasabs = Mapper.Map<List<Contract_Saze_Nasab>, List<Contract_Saze_NasabVM>>(contract_Saze.Contract_Saze_Nasabs),
                        });

                    countRes += 1;

                    }


                if (countRes < 3)
                {
                    for (int i = 0; i < 3 - countRes; i++)
                    {
                        Contract_Sazes.Add(new Contract_SazeVM()
                        {
                            Discount = 0,
                            ID = 0,
                            Saze = null,
                            SazeId = 0,
                            ItemInput = string.Empty,
                            Quantity = 0,
                            RowNumber = countRes + i,
                            Sum = 0,
                            Tax = 0,
                            TotalAmount = 0,
                            NoeEjare = null,
                            NoeEjareId = 0,
                            CalcTax = true,
                            ContractID = 0,
                            DisplayTarikhShorou = "",
   
                         
                        });
                    }

                }

                resualt.contract = new ContractVM()
                    {
                        Contact = Mapper.Map<Contact, ContactVM>(this.BusinessRule.UnitOfWork.Repository<Contact>().Find(reservation.ContactID)),
                        ContactTitle = "",
                        DateTime = DateTime.Now,
                        DisplayDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                        DisplayDueDate = PersianDateUtils.ToPersianDate(DateTime.Now),
                        DueDate = DateTime.Now,
                        ID = 0,
                        Contract_Sazes = Contract_Sazes,
                        IsDraft = true,
                        Note = "",
                        Number = await createNumberContract(organId),
                        Paid = 0,
                        Payable = 0,
                        Profit = 0,
                        Rest = 0,
                        Sent = false,
                        Status = 0,
                        Sum = 0,
                        Tag = "",
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, new { resualtCode = (int)ZhivarEnums.ResultCode.Successful, data = resualt });


            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private async  Task<List<SazeOfContractInTime>> GetListSazeOfContractInTime(loadRockDataBusi rockData)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);
                List<SazeOfContractInTime> lstSazeOfContractInTime = new List<SazeOfContractInTime>();
                int id = 0;
                var contracts = await Rule.GetAllByOrganIdAsync(organId);

                var contractStopsMain = await BusinessRule.UnitOfWork.RepositoryAsync<ContractStops>().Queryable().ToListAsync2();

                if (rockData.ContactID != null)
                {
                    contracts = contracts.Where(x => x.ContactId == rockData.ContactID).ToList();
                }

                foreach (var contract in contracts.Where(x => x.ContractType == ContractType.PreContract || x.ContractType == ContractType.RentTo).ToList())
                {
                    var contractStopsSub = await Rule.GetAllStopContractByContractID(contract.ID);

                    var contract_Sazes = await Rule.GetAllSazeByContractIdAsync(contract.ID);

                    if (contractStopsSub.Count > 0)
                        contract_Sazes = SplitContractSazeTime(contract_Sazes, contractStopsSub);


                    foreach (var contract_Saze in contract_Sazes)
                    {
                        var endDate = DateTime.Now.Date;

                        if (contract_Saze.NoeEjareId == 1)
                        {
                            endDate = contract_Saze.TarikhShorou.AddDays((double)contract_Saze.Quantity);
                            if (contract_Saze.TarikhPayan == DateTime.MinValue)
                            {
                                contract_Saze.TarikhPayan = endDate;
                            }
                        }

                        else if (contract_Saze.NoeEjareId == 2)
                        {
                            PersianCalendar pc = new PersianCalendar();
                            endDate = pc.AddMonths(contract_Saze.TarikhShorou, (int)contract_Saze.Quantity);

                            if (contract_Saze.TarikhPayan == DateTime.MinValue)
                            {
                                contract_Saze.TarikhPayan = endDate;
                            }
                        }


                        var mainDiff = rockData.EndDate.Date - rockData.StartDate.Date;
                        var subDiff = endDate.Date - contract_Saze.TarikhShorou.Date;
                        var percentDay = 100 / (mainDiff.TotalDays + 1);
                        double percentCostRentTo = 0;

                        if (subDiff.TotalDays > 0)
                             percentCostRentTo = ((mainDiff.TotalDays + 1) * 100) / subDiff.TotalDays;

                        if (contract_Saze.TarikhShorou.Date >= rockData.StartDate.Date && contract_Saze.TarikhShorou.Date <= rockData.EndDate ||
                            endDate.Date >= rockData.StartDate.Date && endDate.Date < rockData.EndDate.Date ||
                            (rockData.StartDate.Date >= contract_Saze.TarikhShorou.Date && rockData.StartDate.Date < endDate.Date &&
                            rockData.EndDate.Date >= contract_Saze.TarikhShorou.Date && rockData.EndDate.Date <= endDate.Date))

                        {
                            double amount = 0;

                            if (endDate.Date <= rockData.EndDate.Date && contract_Saze.TarikhShorou.Date >= rockData.StartDate.Date)
                            {
                                amount = subDiff.TotalDays * percentDay;
                            }
                            else if (endDate.Date > rockData.StartDate.Date && endDate.Date <= rockData.EndDate.Date)
                            {
                                subDiff = endDate.Date - rockData.StartDate.Date;
                                amount = subDiff.TotalDays * percentDay;
                            }
                            else if (contract_Saze.TarikhShorou.Date >= rockData.StartDate.Date && contract_Saze.TarikhShorou.Date <= rockData.EndDate.Date)
                            {
                                subDiff = rockData.EndDate.Date - contract_Saze.TarikhShorou.Date;
                                amount = (subDiff.TotalDays + 1) * percentDay;
                            }
                            else if (contract_Saze.TarikhShorou.Date <= rockData.StartDate.Date && contract_Saze.TarikhPayan.Date >= rockData.EndDate.Date)
                            {

                                amount = 100;
                            }
                            else
                                amount = 0;


                            var diffDistance = contract_Saze.TarikhShorou.Date - rockData.StartDate.Date;

                            double distance = 0;

                            if (diffDistance.TotalDays > 0)
                                distance = diffDistance.TotalDays * percentDay;

                            SazeOfContractInTime sazeOfContractInTime = new SazeOfContractInTime();
                            sazeOfContractInTime.ID = id;
                            sazeOfContractInTime.SazeID = contract_Saze.SazeId;
                            sazeOfContractInTime.GoroheSazeID = contract_Saze.Saze.GoroheSazeID;
                            sazeOfContractInTime.Type = contract.ContractType;
                            sazeOfContractInTime.Amount = amount;
                            sazeOfContractInTime.Distance = distance;

                            var amountStr = amount.ToString() + "%";
                            sazeOfContractInTime.Styles = new Dictionary<string, string>();
                            sazeOfContractInTime.Styles.Add("width", amountStr);
                            var distanceStr = distance.ToString() + "%";
                            sazeOfContractInTime.Styles.Add("right", distanceStr);
                            sazeOfContractInTime.Occupy = amount;
                            sazeOfContractInTime.CostRentTo = ((Convert.ToDecimal(amount) / 100) * contract_Saze.TotalAmount * (Convert.ToDecimal(percentCostRentTo) / 100));
                            sazeOfContractInTime.CostRentFrom = CalcRentFromBuySazID(organId, contract_Saze.SazeId, Convert.ToDecimal(amount), rockData);

                            if (contract.ContractType == ContractType.PreContract)
                                sazeOfContractInTime.CheckedOut = true;
                            else
                            {
                                sazeOfContractInTime.CheckedIn = true;
                                sazeOfContractInTime.Styles.Add("background-color", contract.ContactCommon.Color);
                            }






                            sazeOfContractInTime.ContractID = contract.ID;
                            sazeOfContractInTime.ContractAmount = contract.Sum.ToString();
                            sazeOfContractInTime.SazeAmount = contract_Saze.TotalAmount.ToString();
                            sazeOfContractInTime.UnitPrice = contract_Saze.UnitPrice.ToString();
                            sazeOfContractInTime.ContactID = contract.ContactCommon.ID;
                            sazeOfContractInTime.ContactTitle = contract.ContactCommon.Name;// + " " + contract.ContactCommon.LastName;
                            sazeOfContractInTime.ContractNumber = contract.Number;
                            sazeOfContractInTime.ContractTitle = contract.ContractTitle;
                            sazeOfContractInTime.Title = contract.ContractTitle;
                            sazeOfContractInTime.StartDisplayDate = contract_Saze.DisplayTarikhShorou;
                            if (contract_Saze.NoeEjareId == 1)
                                sazeOfContractInTime.EndDisplayDate = PersianDateUtils.ToPersianDate(endDate.AddDays(-1));
                            else
                                sazeOfContractInTime.EndDisplayDate = PersianDateUtils.ToPersianDate(endDate);
                            lstSazeOfContractInTime.Add(sazeOfContractInTime);
                            id++;


                        }
                    }

                }

                ReservationRule reservationRule = new ReservationRule();
                var reservations = await reservationRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));


                if (rockData.ContactID != null)
                {
                    reservations = reservations.Where(x => x.ContactID == rockData.ContactID).ToList();
                }

                foreach (var reservation in reservations)
                {
                    var reservation_Details = await this.BusinessRule.UnitOfWork.Repository<Reservation_Detail>().Queryable().Where(x => x.ReservationID == reservation.ID).ToListAsync2();
                    foreach (var reservation_Detail in reservation_Details)
                    {
                        var mainDiff = rockData.EndDate.Date - rockData.StartDate.Date;
                        var subDiff = reservation_Detail.EndDate.Date - reservation_Detail.StartDate.Date;
                        var percentDay = 100 / (mainDiff.TotalDays + 1);

                        if (reservation_Detail.StartDate.Date >= rockData.StartDate.Date && reservation_Detail.StartDate.Date <= rockData.EndDate.Date ||
                            reservation_Detail.EndDate.Date >= rockData.StartDate.Date && reservation_Detail.EndDate.Date <= rockData.EndDate.Date ||
                            (rockData.StartDate.Date >= reservation_Detail.StartDate.Date && rockData.StartDate.Date <= reservation_Detail.EndDate.Date &&
                             rockData.EndDate.Date >= reservation_Detail.StartDate.Date && rockData.EndDate.Date <= reservation_Detail.EndDate.Date))
                        {
                            double amount = 0;



                            if (reservation_Detail.EndDate.Date <= rockData.EndDate.Date && reservation_Detail.StartDate.Date >= rockData.StartDate.Date)
                            {
                                amount = subDiff.TotalDays * percentDay;
                            }
                            else if (reservation_Detail.EndDate.Date > rockData.StartDate.Date && reservation_Detail.EndDate.Date <= rockData.EndDate.Date)
                            {
                                subDiff = reservation_Detail.EndDate.Date - rockData.StartDate.Date;
                                amount = subDiff.TotalDays * percentDay;
                            }
                            else if (reservation_Detail.StartDate.Date >= rockData.StartDate.Date && reservation_Detail.StartDate.Date <= rockData.EndDate.Date)
                            {
                                subDiff = rockData.EndDate.Date - reservation_Detail.StartDate.Date;
                                amount = (subDiff.TotalDays + 1) * percentDay;
                            }
                            else if (reservation_Detail.StartDate.Date <= rockData.StartDate.Date && reservation_Detail.EndDate.Date >= rockData.EndDate.Date)
                            {

                                amount = 100;
                            }
                            //else if (reservation_Detail.EndDate.Date >= rockData.StartDate.Date && reservation_Detail.EndDate.Date <= rockData.EndDate.Date)
                            //{
                            //    subDiff = reservation_Detail.EndDate.Date - rockData.StartDate.Date;
                            //    amount = (subDiff.TotalDays + 1) * percentDay;
                            //}
                            else
                                amount = 0;





                            var diffDistance = reservation_Detail.StartDate.Date - rockData.StartDate.Date;

                            double distance = 0;

                            if (diffDistance.TotalDays > 0)
                                distance = diffDistance.TotalDays * percentDay;

                            SazeOfContractInTime sazeOfContractInTime = new SazeOfContractInTime();
                            sazeOfContractInTime.ID = id;
                            sazeOfContractInTime.SazeID = reservation_Detail.SazeID;

                            sazeOfContractInTime.Amount = amount;
                            sazeOfContractInTime.Distance = distance;
                            sazeOfContractInTime.Reservation = true;
                            sazeOfContractInTime.ReservationID = reservation.ID;
                            sazeOfContractInTime.ReservationDetailID = reservation_Detail.ID;

                            var validationDay = reservation.ValiditDuration / 3;

                            var diff = DateTime.Now - reservation.RegisterDate;

                            sazeOfContractInTime.Styles = new Dictionary<string, string>();

                            if (diff.TotalDays <= validationDay)
                                sazeOfContractInTime.Styles.Add("background-color", "#FF0008");
                            else if (diff.TotalDays <= validationDay * 2)
                                sazeOfContractInTime.Styles.Add("background-color", "#FF8700");
                            else if (diff.TotalDays <= validationDay * 3)
                                sazeOfContractInTime.Styles.Add("background-color", "#18C700");


                            var amountStr = amount.ToString() + "%";

                            sazeOfContractInTime.Styles.Add("width", amountStr);
                            var distanceStr = distance.ToString() + "%";
                            sazeOfContractInTime.Styles.Add("right", distanceStr);

                            sazeOfContractInTime.ContactID = reservation.ContactID;
                            sazeOfContractInTime.ContractTitle = reservation.ContactTitle;// reservation.ContactCommon.Name;
                            sazeOfContractInTime.Title = reservation.ContactTitle;// reservation.ContactCommon.Name;
                            sazeOfContractInTime.StartDisplayDate = reservation_Detail.StartDisplayDate;
                            sazeOfContractInTime.EndDisplayDate = reservation_Detail.EndDisplayDate;

                            lstSazeOfContractInTime.Add(sazeOfContractInTime);
                            id++;


                        }
                    }

                }



                return lstSazeOfContractInTime;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private decimal CalcRentFromBuySazID(int organId, int sazeId, decimal amount, loadRockDataBusi rockData)
        {
            try
            {
                using (var uow = new UnitOfWork())
            {
                    rockData.StartDate = rockData.StartDate.Date;
                    rockData.EndDate = rockData.EndDate.Date;
                var contractQuery = uow.Repository<Zhivar.DomainClasses.Contract.Contract>().Queryable().Where(x => x.OrganId == organId && x.ContractType == ContractType.RentFrom);
                var contractSazeQuery = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.TarikhPayan != null && x.SazeId == sazeId && x.TarikhShorou <= rockData.StartDate && x.TarikhPayan >= rockData.EndDate);

                var joinQuery = from contract in contractQuery
                                join contractSaze in contractSazeQuery
                                on contract.ID equals contractSaze.ContractID
                                select contractSaze;

                if (joinQuery.Any())
                {
                    var contractSaze = joinQuery.OrderByDescending(x => x.ID).FirstOrDefault();

                    //var endDate = DateTime.Now.Date;

                    //if (contractSaze.NoeEjareId == 1)
                    //    endDate = contractSaze.TarikhShorou.AddDays((double)contractSaze.Quantity);
                    //else if (contractSaze.NoeEjareId == 2)
                    //{
                    //    PersianCalendar pc = new PersianCalendar();
                    //    endDate = pc.AddMonths(contractSaze.TarikhShorou, (int)contractSaze.Quantity);
                    //}

 
                    //var mainDiff = rockData.EndDate.Date - rockData.StartDate.Date;
                    //var subDiff = endDate.Date - contractSaze.TarikhShorou.Date;

                    //var percentCostRentFrom = ((mainDiff.TotalDays + 1) * 100) / subDiff.TotalDays;
                        //براساس مقداری که اجاره داده شده مبلغ اجاره گرفته را محاسبه می کند
                        // return ((Convert.ToDecimal(amount) / 100) * contractSaze.TotalAmount * (Convert.ToDecimal(percentCostRentFrom) / 100));
                        //طبق گفته خودشان کلی می خواهند محاسبه شود
                        return contractSaze.TotalAmount;


                }
                else
                    return 0;
            }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private IList<Contract_SazeVM> SplitContractSazeTime(IList<Contract_SazeVM> contract_Sazes, List<ContractStopsVM> contractStops)
        {
            List<Contract_SazeVM> result = new List<Contract_SazeVM>();

            foreach (var contract_Saze in contract_Sazes ?? new List<Contract_SazeVM>())
            {
                var contractStopSubs = contractStops.Where(e => e.ContractStopDetailCommon.Any( x => x.SazeID == contract_Saze.SazeId)).ToList();

                if (contractStopSubs.Count == 0)
                    result.Add(contract_Saze);
                else
                {
                    DateTime startDate = contract_Saze.TarikhShorou.Date;
                    decimal quantity = contract_Saze.Quantity;
                    string displayStartDate = contract_Saze.DisplayTarikhShorou;

                    var endDate = DateTime.Now;

                    if (contract_Saze.NoeEjareId == 1)
                        endDate = contract_Saze.TarikhShorou.AddDays((double)contract_Saze.Quantity);
                    else if (contract_Saze.NoeEjareId == 2)
                    {
                        PersianCalendar pc = new PersianCalendar();
                        endDate = pc.AddMonths(contract_Saze.TarikhShorou.Date, (int)contract_Saze.Quantity);
                    }

  
                    foreach (var contractStopSub in contractStopSubs.OrderBy(e => e.ContractStopDetailCommon.OrderBy(x => x.StartDate)).ToList())
                    {
                        foreach (var contractStopDetail in contractStopSub.ContractStopDetailCommon)
                        {
                            quantity = Convert.ToDecimal((contractStopDetail.StartDate - startDate.Date).TotalDays);
                            if (quantity > 0)
                            {
                                NoeEjareVM noeEjare2 = new NoeEjareVM()
                                {
                                    ID = 1
                                };

                                result.Add(new Contract_SazeVM()
                                {
                                    CalcTax = contract_Saze.CalcTax,
                                    Contarct_Saze_Bazareabs = contract_Saze.Contarct_Saze_Bazareabs,
                                    ContractSazeImages = contract_Saze.ContractSazeImages,
                                    ContractID = contract_Saze.ContractID,
                                    Contract_Saze_Chapkhanes = contract_Saze.Contract_Saze_Chapkhanes,
                                    Contract_Saze_Nasabs = contract_Saze.Contract_Saze_Nasabs,
                                    Contract_Saze_Tarahs = contract_Saze.Contract_Saze_Tarahs,
                                    Description = contract_Saze.Description,
                                    Discount = contract_Saze.Discount,
                                    HasBazareab = contract_Saze.HasBazareab,
                                    HasChap = contract_Saze.HasChap,
                                    HasNasab = contract_Saze.HasNasab,
                                    HasTarah = contract_Saze.HasTarah,
                                    ID = contract_Saze.ID,
                                    ItemInput = contract_Saze.ItemInput,
                                    Mah = contract_Saze.Mah,
                                    NoeEjare = noeEjare2,
                                    NoeEjareId = noeEjare2.ID,
                                    PriceBazareab = contract_Saze.PriceBazareab,
                                    PriceChap = contract_Saze.PriceChap,
                                    PriceNasab = contract_Saze.PriceNasab,
                                    PriceTarah = contract_Saze.PriceTarah,
                                    RowNumber = contract_Saze.RowNumber,
                                    Roz = contract_Saze.Roz,
                                    Saal = contract_Saze.Saal,
                                    Saze = contract_Saze.Saze,
                                    SazeId = contract_Saze.SazeId,
                                    Status = contract_Saze.Status,
                                    Sum = contract_Saze.Sum,
                                    Tax = contract_Saze.Tax,
                                    TotalAmount = contract_Saze.TotalAmount,
                                    UnitItem = contract_Saze.UnitItem,
                                    UnitPrice = contract_Saze.UnitPrice,
                                    ///////////////////////////////////////////
                                    TarikhShorou = startDate,
                                    DisplayTarikhShorou = displayStartDate,
                                    Quantity = quantity,
                                });
                            }
                         

                            startDate = contractStopDetail.EndDate.Date;
                            displayStartDate = PersianDateUtils.ToPersianDateTime(contractStopDetail.EndDate.Date);
                        }
                    }

                    quantity = Convert.ToDecimal((endDate.Date - startDate.Date).TotalDays);
                    if (quantity > 0)
                    {
                        NoeEjareVM noeEjare = new NoeEjareVM()
                        {
                            ID = 1
                        };


                        result.Add(new Contract_SazeVM()
                        {
                            CalcTax = contract_Saze.CalcTax,
                            Contarct_Saze_Bazareabs = contract_Saze.Contarct_Saze_Bazareabs,
                            ContractSazeImages = contract_Saze.ContractSazeImages,
                            ContractID = contract_Saze.ContractID,
                            Contract_Saze_Chapkhanes = contract_Saze.Contract_Saze_Chapkhanes,
                            Contract_Saze_Nasabs = contract_Saze.Contract_Saze_Nasabs,
                            Contract_Saze_Tarahs = contract_Saze.Contract_Saze_Tarahs,
                            Description = contract_Saze.Description,
                            Discount = contract_Saze.Discount,
                            HasBazareab = contract_Saze.HasBazareab,
                            HasChap = contract_Saze.HasChap,
                            HasNasab = contract_Saze.HasNasab,
                            HasTarah = contract_Saze.HasTarah,
                            ID = contract_Saze.ID,
                            ItemInput = contract_Saze.ItemInput,
                            Mah = contract_Saze.Mah,
                            NoeEjare = noeEjare,
                            NoeEjareId = noeEjare.ID,
                            PriceBazareab = contract_Saze.PriceBazareab,
                            PriceChap = contract_Saze.PriceChap,
                            PriceNasab = contract_Saze.PriceNasab,
                            PriceTarah = contract_Saze.PriceTarah,
                            RowNumber = contract_Saze.RowNumber,
                            Roz = contract_Saze.Roz,
                            Saal = contract_Saze.Saal,
                            Saze = contract_Saze.Saze,
                            SazeId = contract_Saze.SazeId,
                            Status = contract_Saze.Status,
                            Sum = contract_Saze.Sum,
                            Tax = contract_Saze.Tax,
                            TotalAmount = contract_Saze.TotalAmount,
                            UnitItem = contract_Saze.UnitItem,
                            UnitPrice = contract_Saze.UnitPrice,
                            ///////////////////////////////////////////
                            TarikhShorou = startDate,
                            DisplayTarikhShorou = displayStartDate,
                            Quantity = quantity,
                        });
                    }
                }
            }


            return result;
        }

        private List<TemplateDate> createTemplateDate(ref loadRockDataBusi rockData)
        {
            var listTemplateDate = new List<TemplateDate>();
            int vaziat = 0;

            if (string.IsNullOrEmpty(rockData.StartDisplayDate) && string.IsNullOrEmpty(rockData.EndDisplayDate))
                vaziat = 0;
            else if (!string.IsNullOrEmpty(rockData.StartDisplayDate) && string.IsNullOrEmpty(rockData.EndDisplayDate))
            {
                vaziat = 1;
                rockData.StartDate = PersianDateUtils.ToDateTime(rockData.StartDisplayDate);
            }
                
            else if (string.IsNullOrEmpty(rockData.StartDisplayDate) && !string.IsNullOrEmpty(rockData.EndDisplayDate))
            {
                vaziat = 2;
                rockData.EndDate = PersianDateUtils.ToDateTime(rockData.EndDisplayDate);
            }
                
            else if (!string.IsNullOrEmpty(rockData.StartDisplayDate) && !string.IsNullOrEmpty(rockData.EndDisplayDate))
            {
                vaziat = 3;

                rockData.StartDate = PersianDateUtils.ToDateTime(rockData.StartDisplayDate);
                rockData.EndDate = PersianDateUtils.ToDateTime(rockData.EndDisplayDate);
            }
                

            //if (string.IsNullOrEmpty(rockData.StartDisplayDate))
            //    rockData.StartDate = DateTime.Now;
            //else
            //    rockData.StartDate = PersianDateUtils.ToDateTime(rockData.StartDisplayDate);

            //if (string.IsNullOrEmpty(rockData.EndDisplayDate))
            //    rockData.EndDate = DateTime.Now;
            //else
            //    rockData.EndDate = PersianDateUtils.ToDateTime(rockData.EndDisplayDate);

           

            if (vaziat == 0)
            {
                    switch (rockData.TempleteType)
                    {
                        case TempleteType.Week:
                            var dayOfWeek = DateTime.Now.DayOfWeek;
                            switch (dayOfWeek)
                            {
                                case DayOfWeek.Sunday:
                                    rockData.StartDate = DateTime.Now.AddDays(-1);
                                    rockData.EndDate = DateTime.Now.AddDays(5);
                                    break;
                                case DayOfWeek.Monday:
                                    rockData.StartDate = DateTime.Now.AddDays(-2);
                                    rockData.EndDate = DateTime.Now.AddDays(4);
                                    break;
                                case DayOfWeek.Tuesday:
                                    rockData.StartDate = DateTime.Now.AddDays(-3);
                                    rockData.EndDate = DateTime.Now.AddDays(3);
                                    break;
                                case DayOfWeek.Wednesday:
                                    rockData.StartDate = DateTime.Now.AddDays(-4);
                                    rockData.EndDate = DateTime.Now.AddDays(2);
                                    break;
                                case DayOfWeek.Thursday:
                                    rockData.StartDate = DateTime.Now.AddDays(-5);
                                    rockData.EndDate = DateTime.Now.AddDays(1);
                                    break;
                                case DayOfWeek.Friday:
                                    rockData.StartDate = DateTime.Now.AddDays(-6);
                                    rockData.EndDate = DateTime.Now.AddDays(0);
                                    break;
                                case DayOfWeek.Saturday:
                                    rockData.StartDate = DateTime.Now.AddDays(0);
                                    rockData.EndDate = DateTime.Now.AddDays(6);
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case TempleteType.Month:
                            var dateMonth = PersianDateUtils.StartEndMonth(DateTime.Now);
                            rockData.StartDate = dateMonth.StartDate;
                            rockData.EndDate = dateMonth.EndDate;
                            rockData.TempleteType = TempleteType.Week;
                            break;
                        case TempleteType.Year:
                            var dateYear = PersianDateUtils.StartEndYear(DateTime.Now);
                            rockData.StartDate = dateYear.StartDate;
                            rockData.EndDate = dateYear.EndDate;
                            rockData.TempleteType = TempleteType.Month;
                            break;
                        default:
                            break;
                    }
                }
            else if (vaziat == 1)
            {
                switch (rockData.TempleteType)
                {
                    case TempleteType.Week:
                        rockData.StartDate = rockData.StartDate;
                        rockData.EndDate = rockData.StartDate.AddDays(6);
                        break;
                    case TempleteType.Month:
                        rockData.StartDate = rockData.StartDate;
                        rockData.EndDate = rockData.StartDate.AddDays(30);
                        rockData.TempleteType = TempleteType.Week;
                        break;
                    case TempleteType.Year:
                        rockData.StartDate = rockData.StartDate;
                        rockData.EndDate = rockData.StartDate.AddDays(365);
                        rockData.TempleteType = TempleteType.Month;
                        break;
                    default:
                        break;
                }
            }
            else if (vaziat == 2)
            {
                switch (rockData.TempleteType)
                {
                    case TempleteType.Week:
                        rockData.StartDate = rockData.EndDate.AddDays(-6);
                        rockData.EndDate = rockData.EndDate;
                        break;
                    case TempleteType.Month:
                        rockData.StartDate = rockData.EndDate.AddDays(-30);
                        rockData.EndDate = rockData.EndDate;
                        rockData.TempleteType = TempleteType.Week;
                        break;
                    case TempleteType.Year:
                        rockData.StartDate = rockData.EndDate.AddDays(-365);
                        rockData.EndDate = rockData.EndDate;
                        rockData.TempleteType = TempleteType.Month;
                        break;
                    default:
                        break;
                }
            }
            else if(vaziat == 3)
                {
                    var diff2 = rockData.EndDate.Date - rockData.StartDate;

                    if (diff2.TotalDays <= 31)
                    {
                        rockData.TempleteType = TempleteType.Week;
                    }
                    else if (diff2.TotalDays > 31 && diff2.TotalDays < 366)
                    {
                        rockData.TempleteType = TempleteType.Month;
                    }
                    else
                    {
                        rockData.TempleteType = TempleteType.Year;
                    }
                }

            var diff = rockData.EndDate.Date - rockData.StartDate.Date;

            switch (rockData.TempleteType)
            {
                case TempleteType.Week:
                    ;

                    for (int i = 0; i <= diff.TotalDays; i++)
                    {
                        var temp = PersianDateUtils.ToTempPersianDate(rockData.StartDate.AddDays(i));

                        TemplateDate templateDate = new TemplateDate();

                        templateDate.ID = i;
                        templateDate.Title1 = temp.TitleDay;
                        templateDate.Title2 = temp.Day + " " + temp.TitleMonth;
                        templateDate.Date = PersianDateUtils.ToPersianDate(rockData.StartDate.AddDays(i));
                        
                        if (rockData.StartDate.AddDays(i).Date == DateTime.Now.Date)
                        {
                            templateDate.Styles = new Dictionary<string, string>();
                            templateDate.Styles.Add("background", "#fde9a0");
                        }
                            

                        listTemplateDate.Add(templateDate);
                    }
                    break;
                case TempleteType.Month:
                case TempleteType.Year:

                    var totalManth = convertToDivMonth(rockData.StartDate, rockData.EndDate, Convert.ToDecimal(diff.TotalDays + 1));
                  //  var totalManth = Math.Ceiling((diff.TotalDays + 1) / 30.5);


                    
                    for (int i = 0; i < totalManth; i++)
                    {
                        PersianCalendar pc = new PersianCalendar();
                        var tempDate = pc.AddMonths(rockData.StartDate, i);
                        

                        var temp = PersianDateUtils.ToTempPersianDate(tempDate);


                        TemplateDate templateDate = new TemplateDate();

                        templateDate.ID = i;
                        templateDate.Title1 = temp.TitleMonth;
                        templateDate.Title2 = " سال " + temp.Year;
                        templateDate.Date = PersianDateUtils.ToPersianDate(tempDate);

                        if (pc.GetMonth(tempDate.Date) == pc.GetMonth(DateTime.Now.Date))
                        {
                            templateDate.Styles = new Dictionary<string, string>();
                            templateDate.Styles.Add("background", "#fde9a0");
                        }


                        listTemplateDate.Add(templateDate);


                        //listTemplateDate.Add(new TemplateDate()
                        //{
                        //    Title1 = temp.TitleMonth,
                        //    Title2 = " سال " + temp.Year,
                        //    Date = PersianDateUtils.ToPersianDate(tempDate)
                        //});
                    }
                    break;
                //case TempleteType.Year:
                //    var totalYear = Math.Ceiling((diff.TotalDays) / 365);
                //    for (int i = 0; i < totalYear; i++)
                //    {
                //        var temp = PersianDateUtils.ToTempPersianDate(rockData.StartDate.AddYears(i));

                //        listTemplateDate.Add(new TemplateDate()
                //        {
                //            Title1 = temp.Year,
                //            Title2 = "",
                //            Date = PersianDateUtils.ToPersianDate(rockData.StartDate.AddYears(i))
                //        });
                //    }
                //    break;
                default:
                    break;
            }

           

            return listTemplateDate;
        }

        private decimal convertToDivMonth(DateTime startDate, DateTime endDate, decimal totalDays)
        {
            PersianCalendar pc = new PersianCalendar();

            var tempStartDate = PersianDateUtils.ToTempPersianDate(startDate);
            var tempEndDate = PersianDateUtils.ToTempPersianDate(endDate);

            if (tempStartDate.IntMonth < 7 && tempEndDate.IntMonth < 7)
            {
                return Math.Ceiling(totalDays / 31);
            }
            else if (tempStartDate.IntMonth >= 7 && tempEndDate.IntMonth >= 7)
            {
                return Math.Ceiling(totalDays / 30);
            }
            else if (tempStartDate.IntMonth < 7 && tempEndDate.IntMonth >= 7)
            {
                var dayOfYear = pc.GetDayOfYear(startDate);
                dayOfYear = 186 - dayOfYear;
                decimal month = Math.Ceiling(Convert.ToDecimal( dayOfYear) / 31);

                dayOfYear = pc.GetDayOfYear(endDate);
                dayOfYear = dayOfYear - 186;

                month += Math.Ceiling(Convert.ToDecimal(dayOfYear) / 30);
                return month;
            }

            else if (tempStartDate.IntMonth >= 7 && tempEndDate.IntMonth < 7)
            {
                var dayOfYear = pc.GetDayOfYear(startDate);
                dayOfYear = 365 - dayOfYear ;
                decimal month = Math.Ceiling(Convert.ToDecimal(dayOfYear) / 30);

                dayOfYear = pc.GetDayOfYear(endDate);
              //  dayOfYear = dayOfYear - 186;

                month += Math.Ceiling(Convert.ToDecimal(dayOfYear) / 31);
                return month;
            }

            return 0;
        }

        private async Task<string> createNumberContract(int organId)
        {
            var count = 0;
            var ContractQuery = await Rule.GetAllByOrganIdAsync(organId);

            count = ContractQuery.Count();
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
        [Route("GetContracts")]
        public async Task<HttpResponseMessage> GetContracts([FromBody] ContractType contractType)
        {

            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var ContractQuery = await Rule.GetAllByOrganIdAsync(organId);

                ContractQuery = ContractQuery.Where(x => x.ContractType == contractType).OrderByDescending(x => x.ID).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = ContractQuery });

            }
            catch (Exception ex)
            {

                throw;
            }

        }
  
        [HttpPost]
        [Route("GetInfoContractByID")]
        public async Task<HttpResponseMessage> GetInfoMediaByID([FromBody] int id)
        {

            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                PersianCalendar pc = new PersianCalendar();

                var day = pc.GetDayOfMonth(DateTime.Now);
                var month = pc.GetMonth(DateTime.Now);
                var year = pc.GetYear(DateTime.Now);
                var dayOfMonth = pc.GetDaysInMonth(year, month);


                var dayOfWeek = pc.GetDayOfWeek(Utilities.PersianDateUtils.ToDateTime(year + "/" + month + "/" + 1));
                int dayOfWeekJalali = 0;

                ContractInfo contractInfo = new ContractInfo();
                contractInfo.NameOfMonth = PersianDateUtils.NameOfMonth(month);
                contractInfo.NameOfYear = year.ToString();
                contractInfo.NumberOfMonth = month;
                contractInfo.NumberOfYear = year;


                SazeRule sazeRule = new SazeRule();
                var saze = await sazeRule.FindAsync(id);

                contractInfo.Name = saze.Title;
                contractInfo.Address = saze.Address;

                if (saze.Images != null && saze.Images.Count > 0)
                {
                    if (saze.Images.FirstOrDefault().Blob != null)
                        contractInfo.Image = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(saze.Images.FirstOrDefault().Blob));

                    contractInfo.Images = new List<Image>();

                    foreach (var image in saze.Images)
                    {
                        contractInfo.Images.Add(new Image() {
                            Base64= string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(image.Blob))
                    });
                    }
                }
                

                contractInfo.Week1 = new List<PcCalender>();
                contractInfo.Week2 = new List<PcCalender>();
                contractInfo.Week3 = new List<PcCalender>();
                contractInfo.Week4 = new List<PcCalender>();
                contractInfo.Week5 = new List<PcCalender>();

               

                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        dayOfWeekJalali = 1;
                        break;
                    case DayOfWeek.Monday:
                        dayOfWeekJalali = 2;
                        break;
                    case DayOfWeek.Tuesday:
                        dayOfWeekJalali = 3;
                        break;
                    case DayOfWeek.Wednesday:
                        dayOfWeekJalali = 4;
                        break;
                    case DayOfWeek.Thursday:
                        dayOfWeekJalali = 5;
                        break;
                    case DayOfWeek.Friday:
                        dayOfWeekJalali = 6;
                        break;
                    case DayOfWeek.Saturday:
                        dayOfWeekJalali = 0;
                        break;
                    default:
                        break;
                }

                bool startDate = false;
                int intDay = 0;
                for (int i = 0; i < 35; i++)
                {
                    if (i == dayOfWeekJalali)
                        startDate = true;

                    if (startDate && intDay < dayOfMonth)
                        intDay += 1;
                    else
                        intDay = 0;

                    switch (i)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            contractInfo.Week1.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                            contractInfo.Week2.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            contractInfo.Week3.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                        case 26:
                        case 27:
                            contractInfo.Week4.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 28:
                        case 29:
                        case 30:
                        case 31:
                        case 32:
                        case 33:
                        case 34:
                            contractInfo.Week5.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        default:
                            break;
                    }
                  
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contractInfo });

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [HttpPost]
        [Route("GetCalender")]
        public async Task<HttpResponseMessage> GetCalender([FromBody] GetCalenderBusi getCalenderBusi)
        {

            try
            {
                PersianCalendar pc = new PersianCalendar();


                var dayOfMonth = pc.GetDaysInMonth(getCalenderBusi.year, getCalenderBusi.month);

                var dayOfWeek = pc.GetDayOfWeek(PersianDateUtils.ToDateTime(getCalenderBusi.year + "/" + getCalenderBusi.month + "/" + 1));
                int dayOfWeekJalali = 0;

                ContractInfo contractInfo = new ContractInfo();
                contractInfo.NameOfMonth = PersianDateUtils.NameOfMonth(getCalenderBusi.month);
                contractInfo.NameOfYear = getCalenderBusi.year.ToString();

                contractInfo.NumberOfMonth = getCalenderBusi.month;
                contractInfo.NumberOfYear = getCalenderBusi.year;

                contractInfo.Week1 = new List<PcCalender>();
                contractInfo.Week2 = new List<PcCalender>();
                contractInfo.Week3 = new List<PcCalender>();
                contractInfo.Week4 = new List<PcCalender>();
                contractInfo.Week5 = new List<PcCalender>();



                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        dayOfWeekJalali = 1;
                        break;
                    case DayOfWeek.Monday:
                        dayOfWeekJalali = 2;
                        break;
                    case DayOfWeek.Tuesday:
                        dayOfWeekJalali = 3;
                        break;
                    case DayOfWeek.Wednesday:
                        dayOfWeekJalali = 4;
                        break;
                    case DayOfWeek.Thursday:
                        dayOfWeekJalali = 5;
                        break;
                    case DayOfWeek.Friday:
                        dayOfWeekJalali = 6;
                        break;
                    case DayOfWeek.Saturday:
                        dayOfWeekJalali = 0;
                        break;
                    default:
                        break;
                }

                bool startDate = false;
                bool endDate = false;
                int intDay = 0;
                int countDay = 0;
                for (int i = 0; i < 35; i++)
                {
                    if (i == dayOfWeekJalali)
                        startDate = true;

                    if (startDate && countDay < dayOfMonth )
                    {
                        intDay += 1;
                        countDay++;
                    }
                        
                    else
                    {
                        intDay = 0;

                    }
                        

                    switch (i)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            contractInfo.Week1.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                            contractInfo.Week2.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            contractInfo.Week3.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                        case 26:
                        case 27:
                            contractInfo.Week4.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        case 28:
                        case 29:
                        case 30:
                        case 31:
                        case 32:
                        case 33:
                        case 34:
                            contractInfo.Week5.Add(new PcCalender()
                            {
                                id = i,
                                day = intDay
                            });
                            break;
                        default:
                            break;
                    }


                   

                }

                for (int i = 0; i < dayOfMonth - countDay; i++)
                {
                    countDay = countDay + 1;

                    var element = contractInfo.Week1.Find(e => e.id == i);

                    element.day = countDay;

                }
                
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contractInfo });

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        //[HttpPost]
        //[Route("GetRentContracts")]
        //public async Task<HttpResponseMessage> GetRentContracts([FromBody] ZhivarEnums.Status status)
        //{

        //    try
        //    {
        //        var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

        //        var ContractQuery = await Rule.GetAllByOrganIdAsync(organId);

        //        ContractQuery = ContractQuery.Where(x => x.ContractType == ContractType.RentFrom).OrderByDescending(x => x.ID).ToList();


        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = ContractQuery });

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}

        [HttpPost]
        [Route("GetSazesOfContract")]
        public async Task<HttpResponseMessage> GetSazesOfContract([FromBody] int contractId)
        {

            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                ContractRule contractRule = new ContractRule();
                var contract = await contractRule.FindAsync(contractId);

                var title = string.Empty;
                var titleButton = string.Empty;
                var type = 0;

                if (contract.ContractType == ContractType.PreContract)
                {
                    title = "لیست رسانه های پیش قرارداد " + contract.ContractTitle;
                    titleButton = " لیست پیش قراردادها ";
                    type = 1;
                }
                else if (contract.ContractType == ContractType.RentTo)
                {
                    title = "لیست رسانه های قرارداد اجاره  " + contract.ContractTitle;
                    titleButton = "لیست قراردادهای اجاره  ";
                    type = 2;
                }
                else if (contract.ContractType == ContractType.RentFrom)
                {
                    title = "لیست رسانه های قرارداد اجاره از صاحب رسانه " + contract.ContractTitle;
                    titleButton = "لیست قراردادهای اجاره از صاحب رسانه  ";
                    type = 3;
                }

                var ContractQuery = await Rule.GetAllSazeByContractIdAsync(contractId);
                ContractQuery = ContractQuery.OrderByDescending(x => x.ID).ToList();



                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data =new { Type=type, TitleButton = titleButton, Title = title, SazesOfContract = ContractQuery }  });

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        [Route("LoadContractSazeTransObj")]
        public async Task<HttpResponseMessage> LoadContractSazeTransObj([FromBody] int id)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                Contract_SazeRule contract_SazeRule = new Contract_SazeRule();

                var contract_Saze = await contract_SazeRule.FindAsync(id);
                Contract_SazeTransObj contract_SazeTransObj = new Contract_SazeTransObj();
                SazeRule sazeRule = new SazeRule();
                NoeEjareRule noeEjareRule = new NoeEjareRule();

                contract_SazeTransObj.Contract_Saze = Mapper.Map<Contract_SazeVM>(contract_Saze);
                contract_SazeTransObj.Contract_Saze.Saze = Mapper.Map<SazeVM>(await sazeRule.FindAsync(contract_SazeTransObj.Contract_Saze.SazeId));
                contract_SazeTransObj.Contract_Saze.NoeEjare = Mapper.Map<NoeEjareVM>(await noeEjareRule.FindAsync(contract_SazeTransObj.Contract_Saze.NoeEjareId));
                contract_SazeTransObj.Contarct_Saze_Bazareabs = contract_SazeTransObj.Contract_Saze.Contarct_Saze_Bazareabs;
                contract_SazeTransObj.Contract_Saze_Tarahs = contract_SazeTransObj.Contract_Saze.Contract_Saze_Tarahs;
                contract_SazeTransObj.Contract_Saze_Chapkhanes = contract_SazeTransObj.Contract_Saze.Contract_Saze_Chapkhanes;
                contract_SazeTransObj.Contract_Saze_Nasabs = contract_SazeTransObj.Contract_Saze.Contract_Saze_Nasabs;

                contract_SazeTransObj.ContractSazeImages = contract_SazeTransObj.Contract_Saze.ContractSazeImages;

                

                foreach (var contarct_Saze_Bazareab in contract_SazeTransObj.Contarct_Saze_Bazareabs)
                {
                    contarct_Saze_Bazareab.User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contarct_Saze_Bazareab.UserID));
                }

                foreach (var contract_Saze_Tarahs in contract_SazeTransObj.Contract_Saze_Tarahs)
                {
                    contract_Saze_Tarahs.User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_Saze_Tarahs.UserID));
                }

                foreach (var contract_Saze_Chapkhanes in contract_SazeTransObj.Contract_Saze_Chapkhanes)
                {
                    contract_Saze_Chapkhanes.User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_Saze_Chapkhanes.UserID));

                    if (contract_Saze_Chapkhanes.NoeChapID != null)
                    {
                        NoeChapRule noeChapRule = new NoeChapRule(true);
                        var noeChap = await noeChapRule.FindAsync(contract_Saze_Chapkhanes.NoeChapID);
                        contract_SazeTransObj.Contract_Saze.NoeChap = noeChap.Title;
                    }
                    
                }

                foreach (var contract_Saze_Nasab in contract_SazeTransObj.Contract_Saze_Nasabs)
                {
                    contract_Saze_Nasab.User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_Saze_Nasab.UserID));
                }
                using (var uow = new UnitOfWork())
                {
                    var userInfosQuery = uow.Repository<UserInfo>().Queryable().Where(x => x.OrganizationId == organId);
                    var userRoleQuery = uow.Repository<UserRole>().Queryable();
                    var roleQuery = uow.Repository<Role>().Queryable();


                    var joinQuery = from userInfo in userInfosQuery
                                    join userRole in userRoleQuery
                                    on userInfo.ID equals userRole.UserId
                                    join role in roleQuery
                                    on userRole.RoleId equals role.ID
                                    select new UsersForRule
                                    {
                                        ID = userInfo.ID,
                                        Name = userInfo.FirstName + " " + userInfo.LastName,
                                        RoleId = userRole.RoleId,
                                        RoleName = role.Name,


                                    };

                    contract_SazeTransObj.nasabs = await joinQuery.Where(x => x.RoleId == 10).ToListAsync2();
                    contract_SazeTransObj.chapkhanes = await joinQuery.Where(x => x.RoleId == 9).ToListAsync2();
                    contract_SazeTransObj.tarahs = await joinQuery.Where(x => x.RoleId == 8).ToListAsync2();
                    contract_SazeTransObj.bazaryabs = await joinQuery.Where(x => x.RoleId == 7).ToListAsync2();
                }


                for (int i = 0; i < contract_SazeTransObj.Contarct_Saze_Bazareabs.Count; i++)
                {

                    contract_SazeTransObj.Contarct_Saze_Bazareabs[i].User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_SazeTransObj.Contarct_Saze_Bazareabs[i].UserID));
                }

                for (int i = 0; i < contract_SazeTransObj.Contract_Saze_Tarahs.Count; i++)
                {
                    contract_SazeTransObj.Contract_Saze_Tarahs[i].User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_SazeTransObj.Contract_Saze_Tarahs[i].UserID));
                }

                for (int i = 0; i < contract_SazeTransObj.Contract_Saze_Chapkhanes.Count; i++)
                {
                    contract_SazeTransObj.Contract_Saze_Chapkhanes[i].User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_SazeTransObj.Contract_Saze_Chapkhanes[i].UserID));
                }

                for (int i = 0; i < contract_SazeTransObj.Contract_Saze_Nasabs.Count; i++)
                {
                    contract_SazeTransObj.Contract_Saze_Nasabs[i].User = Mapper.Map<UsersForRule>(await this.BusinessRule.UnitOfWork.RepositoryAsync<UserInfo>().FindAsync(contract_SazeTransObj.Contract_Saze_Nasabs[i].UserID));
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contract_SazeTransObj });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        [Route("SaveContract")]
        public async Task<HttpResponseMessage> SaveContract([FromBody] ContractVM contractVM)
        {
            try
            {
                bool ShouldSend = false;

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
                        {
                            contract_Saze.TarikhPayan = contract_Saze.TarikhShorou.AddDays((double)contract_Saze.Quantity - 1);
                            contract_Saze.DisplayTarikhPayan = PersianDateUtils.ToPersianDate(contract_Saze.TarikhPayan);
                        }
                          
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
                //if (invoice.Contact != null)
                //{

                //    invoice.ContactId = invoice.Contact.ID;
                //}

                //   invoice.InvoiceItems = invoice.InvoiceItems.Where(x => x.ItemId != 0).ToList();

                

                ContractValidate validator = new ContractValidate();
                FluentValidation.Results.ValidationResult results = validator.Validate(contract);

                string failurs = "";

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs += "<br/>" + error.ErrorMessage;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }

              //  contract = savePaymentForContract(contract, contractVM);
              if(SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") && 
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
                else if(contractVM.ContractType == ContractType.PreContract && contractVM.Status == Status.SendPreContract)
                {
                    ShouldSend = true;
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


                this.BusinessRule.UnitOfWork.RepositoryAsync<DomainClasses.Contract.Contract>().InsertOrUpdateGraph(contract);



                //  ContactRule contactRule = new ContactRule();
                //   await contactRule.UpdateContact(invoice.InvoiceType, invoice.ContactId);


                // if (contract.Status == ZhivarEnums.Status.WaitingToReceive)
                //  {
                // var document = await Rule.RegisterDocumentAsync(invoiceVM, organId);

                //DocumentRule documentRule = new DocumentRule();
                //  await documentRule.Insert(document, organId);

                //}
                await this.BusinessRule.UnitOfWork.SaveChangesAsync();

                if (contractVM.ID <= 0)
                {
                    if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                        contractVM.ContractType == ContractType.PreContract )
                    {
                        InvoiceRule invoiceRule = new InvoiceRule();

                        var invoice = invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentTo);

                        var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);
                        
                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(document, invoice.OrganId);
                        documentRule.SaveChanges();

                        contract.Status = Status.ConfirmationPreContract;
                    }
                    else if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                        contractVM.ContractType == ContractType.RentTo )
                    {
                        InvoiceRule invoiceRule = new InvoiceRule();

                        var invoice = invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentTo);

                        var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(document, invoice.OrganId);
                        documentRule.SaveChanges();

                       
                    }
                    else if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                    contractVM.ContractType == ContractType.RentFrom)
                    {
                        InvoiceRule invoiceRule = new InvoiceRule();

                        var invoice = invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentFrom);

                        var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                        DocumentRule documentRule = new DocumentRule();
                        await documentRule.InsertAsync(document, invoice.OrganId);
                        documentRule.SaveChanges();


                    }
                    //if (SecurityManager.CurrentUserContext.Roles.Any(x => x.RoleCode == "Manager") &&
                    //   contractVM.ContractType == ContractType.PreContract)
                    //{
                    //    await StartWorkFlowContractPrimative(new WorkFlowBusiClass() {
                    //        Code = "ContractPrimitive",
                    //        ID = contract.ID,
                    //        OrganizationID = 22,
                    //        InstanceTitle = " پیش قرارداد "+ contract.ContactTitle,

                    //    });
                    //}
                }


                if (ShouldSend)
                {

                    await StartWorkFlowContractPrimative(new WorkFlowBusiClass() {
                        Code = "ContractPrimitive",
                        ID = contract.ID,
                        OrganizationID = 22,
                        InstanceTitle = contract.ContractTitle
                    },true);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contract });
            }
            catch (Exception ex)
            {
                var p = ex;
                throw ;
            }

        }

        [HttpPost]
        [Route("ValidateDate")]
        public async Task<HttpResponseMessage> ValidateDate([FromBody] ContractVM contractVM)
        {
            try
            {


                if (contractVM.ContractType == DomainClasses.ZhivarEnums.ContractType.RentFrom)
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });

                contractVM.Contract_Sazes = contractVM.Contract_Sazes.Where(x => x.Saze !=null).ToList();

                var countInvoiceItems = contractVM.Contract_Sazes.Count();
                string failures = string.Empty;

                if (countInvoiceItems > 0)
                {

                    foreach (var contract_Saze in contractVM.Contract_Sazes)
                    {
                        if (contract_Saze.DisplayTarikhShorou != null)
                        {
                            contract_Saze.TarikhShorou = PersianDateUtils.ToDateTime(contract_Saze.DisplayTarikhShorou);

                            if (contract_Saze.TarikhShorou.Date < DateTime.Now.Date)
                                failures += "تاریخ شروع سازه " + contract_Saze.Saze.Title + "بزرگتر یا مساوی تاریخ روز است.";
                        }

                    }
                    
                }

                if (string.IsNullOrEmpty(failures))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failures });
                }

                
            }
            catch (Exception ex)
            {
                var p = ex;
                throw;
            }

        }
        //private DomainClasses.Contract.Contract savePaymentForContract(DomainClasses.Contract.Contract contract, ContractVM contractVM)
        //{
        //    List<Contract_PayRecevies> contract_PayRecevies = new List<Contract_PayRecevies>();
        //    Contract_PayRecevies contract_PayRecevie = new Contract_PayRecevies();
        //    Contract_DetailPayRecevies contract_DetailPayRecevies = new Contract_DetailPayRecevies();

        //    if (contractVM.Items != null)
        //    {
        //        foreach (var item in contractVM.Items ?? new List<Contract_DetailPayReceviesVM>())
        //        {

        //            contract_DetailPayRecevies = new Contract_DetailPayRecevies();

        //            contract_DetailPayRecevies.Amount = item.Amount;
        //            contract_DetailPayRecevies.Type = item.Type;
        //            contract_PayRecevie.Contract_DetailPayRecevies.Add(contract_DetailPayRecevies);

        //            if (item.ID > 0)
        //            {
        //                contract_DetailPayRecevies.ID = item.ID;
        //                contract_DetailPayRecevies.ObjectState = ObjectState.Modified;
        //            }
        //            else
        //            {
        //                contract_DetailPayRecevies.ObjectState = ObjectState.Added;
        //            }

        //        }

        //        foreach (var item in contract_PayRecevie.Contract_DetailPayRecevies)
        //        {
        //            item.Contract_PayRecevieId = contract_PayRecevie.ID;
        //        }

        //        contract_PayRecevies.Add(contract_PayRecevie);




        //        if (contract.Contract_PayRecevies == null)
        //        {

        //            contract.Contract_PayRecevies = new List<Contract_PayRecevies>();
        //            contract.Contract_PayRecevies = contract_PayRecevies;
        //        }
        //        else
        //        {
        //            contract.Contract_PayRecevies = contract_PayRecevies;
        //        }

        //    }













        //    //if (contractVM.Items != null)
        //    //{
        //    //    if (contract.Contract_PayRecevies == null)
        //    //        contract.Contract_PayRecevies = new List<Contract_PayRecevies>();

        //    //    contract_PayRecevie.Contract_DetailPayRecevies = new List<Contract_DetailPayRecevies>();







        //    return contract;
        //}

        [HttpPost]
        public async Task<HttpResponseMessage> StartWorkFlowContractPrimative(WorkFlowBusiClass entity,bool shoulSend=false)
        {
            SecurityManager.ThrowIfUserContextNull();

            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    #region Validate

                    var contract = BusinessContext.GetBusinessRule<DomainClasses.Contract.Contract>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                       .Queryable().Where(dr => dr.ID == entity.ID).SingleOrDefault();

                    var contact = BusinessContext.GetBusinessRule<Contact>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                   .Queryable().Where(dr => dr.ID == contract.ContactId).SingleOrDefault();

                    if (contract.Status != Status.Temporary && !shoulSend)
                    {
                        throw new OMFValidationException("این قرارداد قبلا ارسال شده است.");
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
                        InstanceTitle = entity.InstanceTitle + " "+ contact.Name,
                        RelatedRecordId = entity.ID,
                        StarterOrganizationId = entity.OrganizationID,
                        StarterUserId = SecurityManager.CurrentUserContext.UserId,
                        StartType = Enums.StartType.Request

                    });
                    if (result1.Code == 1)
                    {
                        var workflowInstance = uow.Repository<WorkflowInstance>().Queryable()
                                                  .Where(ins => ins.RelatedRecordId == entity.ID &&
                                                                ins.Status == WfStateStatus.Open &&
                                                                ins.WorkflowInfoId == workFlowID
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
                        ContractRule contractRule = new ContractRule();

                        if (contract.Status == Status.Temporary)
                        {
                            await contractRule.UpdateVaziatContract(contract.ID, Status.SendPreContract);

                        }

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
        public async Task<HttpResponseMessage> StartWorkFlow(WorkFlowBusiClass entity)
        {
            SecurityManager.ThrowIfUserContextNull();

            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    #region Validate

                    var contract_Saze = BusinessContext.GetBusinessRule<Contract_Saze>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                       .Queryable().Where(dr => dr.ID == entity.ID).SingleOrDefault();

                    var contractTemp = BusinessContext.GetBusinessRule<DomainClasses.Contract.Contract>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                            .Queryable().Where(dr => dr.ID == contract_Saze.ContractID).SingleOrDefault();

                    var contact = BusinessContext.GetBusinessRule<Contact>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                               .Queryable().Where(dr => dr.ID == contractTemp.ContactId).SingleOrDefault();


                    if (contract_Saze.Status != Status.Temporary)
                    {
                        throw new OMFValidationException("این رسانه قبلا ارسال شده است.");
                    }

                    string code = GenerateCodeByType(contract_Saze);

                    var workFlowQuery = uow.Repository<WorkflowInfo>()
                                                                     .Queryable()
                                                                     .Where(wf => wf.Code == code)
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
                        Code = code,
                        ExchangeData = entity.ExchangeData,
                        InstanceTitle = entity.InstanceTitle + " "+ contractTemp.ContractTitle+" "+contact.Name,
                        RelatedRecordId = entity.ID,
                        StarterOrganizationId = entity.OrganizationID,
                        StarterUserId = SecurityManager.CurrentUserContext.UserId,
                        StartType = Enums.StartType.Request

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
                        ContractRule contractRule = new ContractRule();


                        var contract = BusinessContext.GetBusinessRule<DomainClasses.Contract.Contract>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                               .Queryable().Where(dr => dr.ID == contract_Saze.ContractID).SingleOrDefault();

                        if (contract.Status == Status.Temporary)
                        {
                            await contractRule.UpdateVaziatContract(contract.ID, Status.SendContract);

                            if (contract.InvoiceId == null)
                            {
                                InvoiceRule invoiceRule = new InvoiceRule();
                                var invoice = await invoiceRule.ConvertContractToInvoiceAsync(contract.ID);

                                var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                                DocumentRule documentRule = new DocumentRule();
                                await documentRule.InsertAsync(document, invoice.OrganId);
                                documentRule.SaveChanges();
                            }
                           

                        }

                        await contractRule.UpdateVaziatContract_Saze(contract_Saze.ID, Status.SendContract);

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
        public async Task<HttpResponseMessage> StartWorkFlowGroupOfSaze(WorkFlowBusiClass entity)
        {
            SecurityManager.ThrowIfUserContextNull();

            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    #region Validate

                    var contract = BusinessContext.GetBusinessRule<Zhivar.DomainClasses.Contract.Contract>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                       .Queryable().Where(dr => dr.ID == entity.ID).SingleOrDefault();

                    var contact = BusinessContext.GetBusinessRule<Contact>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                   .Queryable().Where(dr => dr.ID == contract.ContactId).SingleOrDefault();

                    if (contract.Status != Status.ConfirmationPreContract)
                    {
                        throw new OMFValidationException("این قرارداد قبلا ارسال شده است.");
                    }

                    var contract_Sazes = BusinessContext.GetBusinessRule<Contract_Saze>(this.BusinessRule.OperationAccess, this.BusinessRule.UnitOfWork)
                                                       .Queryable().Where(dr => dr.ContractID == contract.ID).ToList();

                    bool createdInvoice = false;

                    foreach (var contract_Saze in contract_Sazes)
                    {
                        if (contract_Saze.Status == Status.ConfirmationPreContract)
                        {
                            string code = GenerateCodeByType(contract_Saze);

                            var workFlowQuery = uow.Repository<WorkflowInfo>()
                                                                             .Queryable()
                                                                             .Where(wf => wf.Code == code)
                                                                             .SingleOrDefault();
                            var workFlowID = workFlowQuery.ID;

                            var instanceQuery = uow.RepositoryAsync<WorkflowInstance>()
                                                                            .Queryable()
                                                                            .Where(ins => ins.RelatedRecordId == contract_Saze.ID
                                                                                       && ins.WorkflowInfoId == workFlowID);
                            #endregion


                            dynamic result1;
                            result1 = await WorkflowManager.StartWorkflowAsync(new WorkflowStartInfo()
                            {
                                Code = code,
                                ExchangeData = entity.ExchangeData,
                                InstanceTitle = entity.InstanceTitle + " "+ contact.Name,
                                RelatedRecordId = contract_Saze.ID,
                                StarterOrganizationId = entity.OrganizationID,
                                StarterUserId = SecurityManager.CurrentUserContext.UserId,
                                StartType = Enums.StartType.Request

                            });
                            if (result1.Code == 1)
                            {
                                if (workFlowID != 10)
                                {
                                    var workflowInstance = uow.Repository<WorkflowInstance>().Queryable()
                                                              .Where(ins => ins.WorkflowInfoId == workFlowID &&
                                                                            ins.RelatedRecordId == contract_Saze.ID &&
                                                                            ins.Status == WfStateStatus.Open
                                                                        ).SingleOrDefault();
                                    WFExchangeData ex = (WFExchangeData)workflowInstance.InitialExchangeData;

                                    ex[WfConstants.StarterOrganizationIdKey] = Convert.ToString(entity.OrganizationID);
                                    workflowInstance.InitialExchangeData = (string)ex;
                                    workflowInstance.ObjectState = ObjectState.Modified;
                                    uow.RepositoryAsync<WorkflowInstance>().Update(workflowInstance);
                                    await uow.SaveChangesAsync();

                                    ContractRule contractRule2 = new ContractRule();
                                    await contractRule2.UpdateVaziatContract_Saze(contract_Saze.ID, Status.SendContract);
                                }
                                else
                                {
                                    ContractRule contractRule2 = new ContractRule();
                                    await contractRule2.UpdateVaziatContract_Saze(contract_Saze.ID, Status.ConfirmationContract);

                                    var contract_Sazes2 = uow.Repository<Contract_Saze>().Queryable()
                                                              .Where(ins => ins.ContractID == contract.ID).ToList();

                                    if (contract_Sazes2.All(x => x.Status == Status.ConfirmationContract))
                                    {
                                        contract.Status = Status.ConfirmationContract;
                                        contractRule2.Update(contract);
                                        contractRule2.SaveChanges();
                                    }
                                }

                           
                            }

                        }
                    }

       
                        ContractRule contractRule = new ContractRule();
                    
                        if (contract.Status == Status.ConfirmationPreContract && !createdInvoice)  
                        {
                            await contractRule.UpdateVaziatContract(contract.ID, Status.SendContract);

                        if (contract.InvoiceId == null)
                        {
                            InvoiceRule invoiceRule = new InvoiceRule();
                            var invoice = await invoiceRule.ConvertContractToInvoiceAsync(contract.ID);

                            var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                            DocumentRule documentRule = new DocumentRule();
                            await documentRule.InsertAsync(document, invoice.OrganId);
                            documentRule.SaveChanges();

                            createdInvoice = true;
                        }
                        else
                        {
                            createdInvoice = true;
                        }
                       
                        }

                      

                        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { records = "" } });
                    //}
                    //else
                    //    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { resultCode = (int)ResultCode.Exception, data = new { records = "" } });
                }
                  
            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
        }


        private string GenerateCodeByType(Contract_Saze contract_Saze)
        {
            string code = string.Empty;

            if (!contract_Saze.HasTarah && !contract_Saze.HasChap && !contract_Saze.HasNasab)
                code = "ContractNTNCHNN";
            else if (!contract_Saze.HasTarah && !contract_Saze.HasChap && contract_Saze.HasNasab)
                code = "ContractNTNCHN";
            else if (!contract_Saze.HasTarah && contract_Saze.HasChap && !contract_Saze.HasNasab)
                code = "ContractNTCHNN";
            else if (!contract_Saze.HasTarah && contract_Saze.HasChap && contract_Saze.HasNasab)
                code = "ContractNTCHN";
            else if (contract_Saze.HasTarah && !contract_Saze.HasChap && !contract_Saze.HasNasab)
                code = "ContractTNCHNN";
            else if (contract_Saze.HasTarah && !contract_Saze.HasChap && contract_Saze.HasNasab)
                code = "ContractTNCHN";
            else if (contract_Saze.HasTarah && contract_Saze.HasChap && !contract_Saze.HasNasab)
                code = "ContractTCHNN";
            else if (contract_Saze.HasTarah && contract_Saze.HasChap && contract_Saze.HasNasab)
                code = "ContractTCHN";


            return code;
        }

        public async Task<HttpResponseMessage> LoadContractTransObj([FromBody] int id)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var contract = await Rule.FindAsync(id);
                ContractTransObj contractTransObj = new ContractTransObj();

                contractTransObj.contract = Mapper.Map<ContractVM>(contract);
                contractTransObj.Contract_Sazes = contractTransObj.contract.Contract_Sazes;
                // invoiceTransObj.payments =  await transactionRule.GetAllByInvoiceIdAsync(id,true,false);

                for (int i = 0; i < contractTransObj.Contract_Sazes.Count; i++)
                {
                    ItemRule itemRule = new ItemRule();
                    contractTransObj.Contract_Sazes[i].Saze = Mapper.Map<SazeVM>(await itemRule.FindAsync(contractTransObj.Contract_Sazes[i].SazeId));
                }

                ContactRule contactRule = new ContactRule();
                contractTransObj.contract.Contact = Mapper.Map<ContactVM>(await contactRule.FindAsync(contract.ContactId));
                //CashRule cashRule = new CashRule();
                //var cashes = await cashRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                //invoiceTransObj.cashes = Mapper.Map<List<CashVM>>(cashes);

                //BankRule bankRule = new BankRule();
                //var banks = await bankRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                //invoiceTransObj.banks = Mapper.Map<List<BankVM>>(banks);


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contractTransObj });
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
        [Route("DeleteContracts")]
        public async Task<HttpResponseMessage> DeleteContracts([FromBody] string strIds)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                string failurs = "";

                string[] values = strIds.Split(',');
                for (int i = 0; i < values.Length - 1; i++)
                {

                    var id = Convert.ToInt32(values[i].Trim());

                    var contract = new DomainClasses.Contract.Contract();

                    using (var uow = new UnitOfWork())
                    {
                         contract = await uow.RepositoryAsync< DomainClasses.Contract.Contract>().Queryable().Where(x => x.ID == id).SingleOrDefaultAsync2();
                    }
                    

                    if (contract.InvoiceId != null && contract.InvoiceId > 0)
                    {
                        var invoice = await this.BusinessRule.UnitOfWork.RepositoryAsync<Invoice>().FindAsync(contract.InvoiceId);

                        if (invoice != null)
                        {
                            var invoiceItems = await this.BusinessRule.UnitOfWork.RepositoryAsync<InvoiceItem>().Queryable().Where(x => x.InvoiceId == invoice.ID).ToListAsync2();

                            foreach (var invoiceItem in invoiceItems)
                            {
                                invoiceItem.ObjectState = ObjectState.Deleted;

                                await this.BusinessRule.UnitOfWork.RepositoryAsync<InvoiceItem>().DeleteAsync(invoiceItem.ID);
                            }
                        }
                

                        invoice.ObjectState = ObjectState.Deleted;

                        await this.BusinessRule.UnitOfWork.RepositoryAsync<Invoice>().DeleteAsync(invoice.ID);

                        var transactions = await this.BusinessRule.UnitOfWork.RepositoryAsync<Transaction>().Queryable().Where(x => x.InvoiceId == invoice.ID).ToListAsync2();

                        if (transactions != null && transactions.Count > 0)
                        {
                            foreach (var transaction in transactions)
                            {
                                transaction.ObjectState = ObjectState.Deleted;

                                //  await this.BusinessRule.UnitOfWork.RepositoryAsync<Transaction>().DeleteAsync(transaction.ID);
                            }


                            var doc = await this.BusinessRule.UnitOfWork.RepositoryAsync<Document>().FindAsync(transactions.FirstOrDefault().DocumentId);
                            doc.Transactions = transactions;
                            await this.BusinessRule.UnitOfWork.RepositoryAsync<Document>().DeleteAsync(doc.ID);
                        }

           
                    }

                    await Rule.DeleteAsync(id);
                    await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                    //DocumentRule documentRule = new DocumentRule();
                    //PayRecevieRule payRecevieRule = new PayRecevieRule();
                    //var payRecevieQuery = await payRecevieRule.GetByInvoiceIdAsync(id);

                    //if (payRecevieQuery.Any())
                    //{
                    //    failurs += "<br/>" + "برای این فاکنور دریافت/ پرداخت انجام شده است برای حذف باید دریافت و پرداخت حذف گردد.";
                    //}
                    //else
                    //{

                    // }
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

        [Route("SaveContractSazeImages")]
        [HttpPost]
        public async Task<HttpResponseMessage> SaveContractSazeImages(List<ContractSazeImagesVM> contractSazeImagesVMs)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                //if (sazeVM.ID > 0)
                //{
                //    var deletedImages = this.BusinessRule.UnitOfWork.RepositoryAsync<SazeImage>().Queryable().Where(x => x.SazeId == sazeVM.ID).ToList();

                //    foreach (var deletedImage in deletedImages ?? new List<SazeImage>())
                //    {
                //        await this.BusinessRule.UnitOfWork.RepositoryAsync<SazeImage>().DeleteAsync(deletedImage.ID);
                //    }

                //}



                foreach (var image in contractSazeImagesVMs ?? new List<ContractSazeImagesVM>())
                {
                    if (image.TasvirBlobBase64 != null)
                    {
                        if (!string.IsNullOrWhiteSpace(image.TasvirBlobBase64) && !string.IsNullOrEmpty(image.TasvirBlobBase64))
                        {
                            image.TasvirBlobBase64 = image.TasvirBlobBase64.Replace("data:image/jpeg;base64,", "");
                            image.Blob = Convert.FromBase64String(image.TasvirBlobBase64);
                        }
                    }
                }

                List<ContractSazeImages> contractSazeImages =
    Mapper.Map<List<ContractSazeImagesVM>, List<ContractSazeImages>>(contractSazeImagesVMs);




                foreach (var image in contractSazeImages ?? new List<ContractSazeImages>())
                {

                    if (image.ID > 0)
                    {
                        image.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                    }
                    else
                    {
                        image.ObjectState = OMF.Common.Enums.ObjectState.Added;
                    }
                }

                this.BusinessRule.UnitOfWork.RepositoryAsync<ContractSazeImages>().InsertGraphRange(contractSazeImages);

                await this.BusinessRule.UnitOfWork.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contractSazeImages });

            }
            catch (Exception ex)
            {

                throw;
            }


        }


        [HttpPost]
        [Route("SaveContractStops")]
        public async Task<HttpResponseMessage> SaveContractStops([FromBody] ContractStopsVM contractStopsVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


                contractStopsVM.ContractStopDetails = contractStopsVM.ContractStopDetails.Where(x => x.Saze != null).ToList();

                foreach (var contractStopDetail in contractStopsVM.ContractStopDetails)
                {
                    if (contractStopDetail.Saze != null)
                        contractStopDetail.SazeID = contractStopDetail.Saze.ID;

                    if (contractStopDetail.NoeEjare != null)
                        contractStopDetail.NoeEjareID = contractStopDetail.NoeEjare.ID;

                    if (contractStopDetail.DisplayStartDate != null)
                        contractStopDetail.StartDate = PersianDateUtils.ToDateTime(contractStopDetail.DisplayStartDate);

                    if (contractStopDetail.NoeEjareID == 1)
                        contractStopDetail.EndDate = contractStopDetail.StartDate.AddDays((double)contractStopDetail.Quantity);
                    else if (contractStopDetail.NoeEjareID == 2)
                    {
                        PersianCalendar pc = new PersianCalendar();
                        contractStopDetail.EndDate = pc.AddMonths(contractStopDetail.StartDate, (int)contractStopDetail.Quantity);

                    }

                    contractStopDetail.DisplayEndDate = PersianDateUtils.ToPersianDateTime(contractStopDetail.EndDate);
                }

                if (contractStopsVM.TasvirBlobBase64 != null)
                {
                    if (!string.IsNullOrWhiteSpace(contractStopsVM.TasvirBlobBase64) && !string.IsNullOrEmpty(contractStopsVM.TasvirBlobBase64))
                    {
                        contractStopsVM.TasvirBlobBase64 = contractStopsVM.TasvirBlobBase64.Replace("data:image/jpeg;base64,", "");
                        contractStopsVM.Blob = Convert.FromBase64String(contractStopsVM.TasvirBlobBase64);
                    }
                }

                ContractStops contractStops = new ContractStops();
                Mapper.Map(contractStopsVM, contractStops);
                //contractStops.OrganID = organId;

          

                contractStops.DateRegister = DateTime.Now;
                contractStops.DisplayDateRegister = PersianDateUtils.ToPersianDateTime(DateTime.Now);



                ContractStopValidate validator = new ContractStopValidate();
                FluentValidation.Results.ValidationResult results = validator.Validate(contractStops);

                string failurs = "";

                if (!results.IsValid)
                {
                    foreach (var error in results.Errors)
                    {
                        failurs += "<br/>" + error.ErrorMessage;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }

                bool isValid = true;


                foreach (var contractStopDetail in contractStops.ContractStopDetails)
                {
                    using (var uow = new UnitOfWork())
                    {
                        var contractStopDetailOldList = uow.Repository<ContractStopDetails>().Queryable().Where(x => x.SazeID == contractStopDetail.SazeID).ToList();

                        foreach (var contractStopDetailOld in contractStopDetailOldList)
                        {
                            if (contractStopDetail.EndDate.Date >= contractStopDetailOld.StartDate.Date && contractStopDetail.EndDate.Date <= contractStopDetailOld.EndDate.Date)
                            {

                                var saze = uow.Repository<Saze>().Queryable().Where(x => x.ID == contractStopDetail.SazeID).SingleOrDefault();
                                failurs += "<br/>" + " سازه ی با عنوان " + saze.Title + " در بازه زمانی " + contractStopDetailOld.DisplayStartDate + " تا " + contractStopDetailOld.DisplayEndDate + " .قبلا متوقف شده است ";

                                isValid = false;
                            }
                            else if (contractStopDetail.StartDate.Date >= contractStopDetailOld.StartDate.Date && contractStopDetail.StartDate.Date <= contractStopDetailOld.EndDate.Date)
                            {

                                var saze = uow.Repository<Saze>().Queryable().Where(x => x.ID == contractStopDetail.SazeID).SingleOrDefault();
                                failurs += "<br/>" + " سازه ی با عنوان " + saze.Title + " در بازه زمانی " + contractStopDetailOld.DisplayStartDate + " تا " + contractStopDetailOld.DisplayEndDate + " .قبلا متوقف شده است ";

                                isValid = false;
                            }
                            else if (contractStopDetailOld.StartDate.Date >= contractStopDetail.StartDate.Date && contractStopDetailOld.StartDate.Date <= contractStopDetail.EndDate.Date)
                            {

                                var saze = uow.Repository<Saze>().Queryable().Where(x => x.ID == contractStopDetail.SazeID).SingleOrDefault();
                                failurs += "<br/>" + " سازه ی با عنوان " + saze.Title + " در بازه زمانی " + contractStopDetailOld.DisplayStartDate + " تا " + contractStopDetailOld.DisplayEndDate + " .قبلا متوقف شده است ";

                                isValid = false;
                            }
                        }
                    }
                }

                if (!isValid)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failurs });
                }

                if (contractStops.ID > 0)
                {
                    foreach (var contractStopDetail in contractStops.ContractStopDetails)
                    {
                        contractStopDetail.StartDate = PersianDateUtils.ToDateTime(contractStopDetail.DisplayStartDate);
                        contractStopDetail.ContractStopID = contractStops.ID;

                        if (contractStopDetail.ID > 0)
                        {
                            contractStopDetail.ContractStopID = contractStops.ID;
                            contractStopDetail.ObjectState = ObjectState.Modified;
                        }

                        else
                        {
                            contractStopDetail.ContractStopID = contractStops.ID;
                            contractStopDetail.ObjectState = ObjectState.Added;
                        }

                    }

                    contractStops.ObjectState = ObjectState.Modified;
                }

                else
                {

                    foreach (var contractStopDetail in contractStops.ContractStopDetails)
                    {
                        contractStopDetail.ContractStopID = contractStops.ID;
                        contractStopDetail.StartDate = PersianDateUtils.ToDateTime(contractStopDetail.DisplayStartDate);

                        if (contractStopDetail.ID > 0)
                        {
                            contractStopDetail.ContractStopID = contractStops.ID;
                            contractStopDetail.ObjectState = ObjectState.Modified;
                        }

                        else
                        {
                            contractStopDetail.ContractStopID = contractStops.ID;
                            contractStopDetail.ObjectState = ObjectState.Added;
                        }

                    }


                    contractStops.ObjectState = ObjectState.Added;
                }

                //Check Noe Stop
                List<string> failursContractStopType = new List<string>();

                switch (contractStopsVM.Type)
                {
                    case ContractStopType.NoAction:
                        break;
                    case ContractStopType.CostDeduction:
                        var res = await CalcCostDeducationForContractStopAsync(contractStopsVM);
                        if (res.Count> 0)
                            failursContractStopType.AddRange(res);
                        break;
                    case ContractStopType.AddTime:
                        res = await CalcCostAddTimeForContractStopAsync(contractStopsVM,1);
                        if (res.Count > 0)
                            failursContractStopType.AddRange(res);
                        break;
                    case ContractStopType.RatioAddTime:
                        res = await CalcCostAddTimeForContractStopAsync(contractStopsVM, contractStopsVM.Ratio);
                        if (res.Count > 0)
                            failursContractStopType.AddRange(res);
                        break;
                    default:
                        break;
                }

                if (failursContractStopType.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = failursContractStopType });
                }

                this.BusinessRule.UnitOfWork.RepositoryAsync<ContractStops>().InsertOrUpdateGraph(contractStops);

                await this.BusinessRule.UnitOfWork.SaveChangesAsync();


                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = contractStops });
            }
            catch (Exception ex)
            {
                var p = ex;
                throw;
            }

        }

        private async Task<List<string>> CalcCostAddTimeForContractStopAsync(ContractStopsVM contractStopsVM, int? ratio)
        {
            List<string> failuers = new List<string>();

            try
            {
                using (var uow = new UnitOfWork())
                {
                    var contract = await uow.Repository<DomainClasses.Contract.Contract>().Queryable().Where(x => x.ID == contractStopsVM.ContractID).SingleOrDefaultAsync2();
                    var contract_Sazes = await uow.Repository<Contract_Saze>().Queryable().Where(x => x.ContractID == contractStopsVM.ContractID).ToListAsync2();

                    ContractVM newContract = new ContractVM();

                    newContract.ContactId = contract.ContactId;
                    newContract.ContactTitle = "قرارداد جبران توقف سازه قرارداد شماره "+contract.Number;
                    newContract.ContractType = ContractType.RentTo;
                    newContract.DateTime = DateTime.Now;
                    newContract.DueDate = DateTime.Now;
                    newContract.DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);
                    newContract.DisplayDueDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);
                    newContract.IsDraft = false;
                    newContract.Number =await createNumberContract(contract.OrganId);
                    newContract.OrganId = contract.OrganId;
                    newContract.Status = Status.ConfirmationContract;
                    newContract.Contract_Sazes = new List<Contract_SazeVM>();
                    Contract_SazeVM contract_Saze = new Contract_SazeVM();
                    var rowNumber = 0;
                    foreach (var contractStop in contractStopsVM.ContractStopDetails ?? new List<ContractStopDetailsVM>())
                    {
                        var contract_SazeOld = contract_Sazes.Where(x => x.SazeId == contractStop.SazeID).SingleOrDefault();
                        contract_Saze = new Contract_SazeVM();

                        var diffContract = contract_SazeOld.TarikhPayan - contract_SazeOld.TarikhShorou;
                        var unitPriceContractSaze = contract_SazeOld.TotalAmount / Convert.ToDecimal(diffContract.TotalDays);

                        var diffStop = contractStop.EndDate - contractStop.StartDate;
                        var totalDays = Convert.ToDouble(diffStop.TotalDays * ratio);
                        var totalPrice = unitPriceContractSaze * Convert.ToDecimal( totalDays);

                        contract_Saze.CalcTax = contract_SazeOld.CalcTax;
                        contract_Saze.ContractID = newContract.ID;
                        contract_Saze.HasBazareab = false;
                        contract_Saze.HasChap = false;
                        contract_Saze.HasNasab = false;
                        contract_Saze.HasTarah = false;
                        contract_Saze.ItemInput = contract_SazeOld.ItemInput;
                        contract_Saze.Mah = contract_SazeOld.Mah;
                        contract_Saze.NoeEjareId = 1;
                        contract_Saze.PriceBazareab = 0;
                        contract_Saze.PriceChap = 0;
                        contract_Saze.PriceNasab = 0;
                        contract_Saze.PriceTarah = 0;
                        contract_Saze.Quantity = Convert.ToDecimal(totalDays);
                        contract_Saze.RowNumber = rowNumber;
                        contract_Saze.Roz = contract_SazeOld.Roz;
                        contract_Saze.Saal = contract_SazeOld.Saal;
                        contract_Saze.SazeId = contract_SazeOld.SazeId;
                        contract_Saze.Status = Status.ConfirmationContract;
                        contract_Saze.Sum = totalPrice;
                        contract_Saze.Discount = totalPrice;
                        contract_Saze.DisplayTarikhPayan = PersianDateUtils.ToPersianDateTime(contract_SazeOld.TarikhPayan.AddDays( totalDays));
                        contract_Saze.DisplayTarikhShorou = PersianDateUtils.ToPersianDateTime(contract_SazeOld.TarikhPayan.AddDays(1));
                        contract_Saze.TarikhPayan = contract_SazeOld.TarikhPayan.AddDays( totalDays);
                        contract_Saze.TarikhShorou = contract_SazeOld.TarikhPayan.AddDays(1);
                        contract_Saze.Tax = 0;
                        contract_Saze.TotalAmount = totalPrice;
                        contract_Saze.UnitItem = contract_SazeOld.UnitItem;
                        contract_Saze.UnitPrice = unitPriceContractSaze;
                        contract_Saze.Saze = new SazeVM();
                        Mapper.Map( await uow.RepositoryAsync<Saze>().Queryable().Where(x => x.ID == contract_SazeOld.SazeId).SingleOrDefaultAsync2(), contract_Saze.Saze);
                        rowNumber++;
                        newContract.Contract_Sazes.Add(contract_Saze);

                    }
                    newContract.Paid = 0;
                    newContract.Payable = newContract.Contract_Sazes.Sum(x => x.Sum);
                    newContract.Rest = 0;
                    newContract.Sum = newContract.Contract_Sazes.Sum(x => x.Sum);
                   
                    var res = await Rule.SaveContract(newContract);
                    if (res.Count > 0)
                        return res;
                    await Rule.SaveChangesAsync();
                }

                return failuers;

            }
            catch (Exception ex)
            {

                failuers.Add("خطای به وجود آمده است.");
                return failuers;
            }
        }

        private async Task<List<string>> CalcCostDeducationForContractStopAsync(ContractStopsVM contractStopsVM)
        {
            List<string> failuers = new List<string>();
            try
            {
                using (var uow = new UnitOfWork())
                {
                    var contract = await uow.Repository<DomainClasses.Contract.Contract>().Queryable().Where(x => x.ID == contractStopsVM.ContractID).SingleOrDefaultAsync2();

                    Contact contactTemp = new Contact();

                    if (contract != null)
                    {
                        contactTemp = await uow.Repository<Contact>().Queryable().Where(x => x.ID == contract.ContactId).SingleOrDefaultAsync2();
                    }

                    var contract_Sazes = await uow.Repository<Contract_Saze>().Queryable().Where(x => x.ContractID == contractStopsVM.ContractID).ToListAsync2();

                    AccountRule accountRule = new AccountRule();
                    var accounts = await accountRule.GetAllByOrganIdAsync(contract.OrganId);


                    Cost cost = new Cost();
                    cost.ContactId = contract.ContactId;
                    cost.DateTime = DateTime.Now;
                    cost.DisplayDate = PersianDateUtils.ToPersianDateTime(DateTime.Now);
                    cost.Explain = "هزینه توقف قرارداد شماره "+contract.Number+ " که در اجاره "+ contactTemp.Name;
                    cost.Status = CostStatus.WaitingToPay;
                    cost.CostItems = new List<CostItem>();
                    
                    var mapItemSazes = uow.Repository<MapItemSaze>().Queryable();
                    int rowNumber = 0;
                    foreach (var contractStop in contractStopsVM.ContractStopDetails ?? new List<ContractStopDetailsVM>())
                    {
                        var contract_Saze = contract_Sazes.Where(x => x.SazeId == contractStop.SazeID).SingleOrDefault();
                        var diffContract = contract_Saze.TarikhPayan - contract_Saze.TarikhShorou;
                        var unitPriceContractSaze = contract_Saze.TotalAmount / Convert.ToDecimal(diffContract.TotalDays);

                        var diffStop = contractStop.EndDate - contractStop.StartDate;
                        var totalPrice = unitPriceContractSaze * Convert.ToDecimal(diffStop.TotalDays);
                        var itemId = mapItemSazes.Where(x => x.SazeID == contractStop.SazeID).Select(x => x.ItemID).SingleOrDefault();
                        var item = uow.Repository<Item>().Queryable().Where(x => x.ID == itemId).SingleOrDefault();

                    

                        var accountItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Code).SingleOrDefault();

                        if (accountItem == null)
                        {
                            ItemRule itemRule = new ItemRule();
                            Item itemCommon = new Item();
                            Mapper.Map(item, itemCommon);
                            itemRule.CreateServiceAccount(itemCommon, contract.OrganId);
                            accounts = await accountRule.GetAllByOrganIdAsync(contract.OrganId);

                            accountItem = accounts.Where(x => x.ComplteCoding == "8204" + item.Code).SingleOrDefault();
                        }

                        cost.CostItems.Add(new CostItem() {
                            CostId = cost.ID,
                            ItemId = accountItem.ID,
                            RowNumber = rowNumber,
                            ObjectState = ObjectState.Added,
                            Rest = totalPrice,
                            Sum = totalPrice,
                            

                        });

                        rowNumber++;
                    }

                    cost.OrganId = contract.OrganId;
                    cost.Paid = 0;
                    cost.Sum = cost.CostItems.Sum(x => x.Sum);
                    cost.Rest = cost.CostItems.Sum(x => x.Sum);
                    cost.Payable = cost.CostItems.Sum(x => x.Sum);
                    cost.ObjectState = ObjectState.Added;

                    uow.Repository<Cost>().InsertOrUpdateGraph(cost);

                    CostRule costRule = new CostRule();
                    CostVM costVM = new CostVM();
                    Mapper.Map(cost, costVM);
                    var contact = await uow.Repository<Contact>().Queryable().Where(x => x.ID == cost.ContactId).SingleOrDefaultAsync2();
                    costVM.Contact = new ContactVM();
                    Mapper.Map(contact, costVM.Contact);

                    var document = await costRule.RegisterDocument(costVM, contract.OrganId);
                    DocumentRule documentRule = new DocumentRule(uow);
                    await documentRule.InsertAsync(document, contract.OrganId);

                    await uow.SaveChangesAsync();
                    return failuers;
                }
             
    
            }
            catch (Exception ex)
            {
                failuers.Add("خطای به وجود آمده است.");
                return failuers;
            }
        }

        [HttpPost]
        [Route("HasRent")]
        public async Task<HttpResponseMessage> HasRent([FromBody] HasRentVM hasRentVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                hasRentVM.StartDate = PersianDateUtils.ToDateTime(hasRentVM.DisplayTarikhShorou);

                if (hasRentVM.NoeEjare == 1)
                {
                    hasRentVM.EndDate = hasRentVM.StartDate.AddDays((double)hasRentVM.Quantity);
                    hasRentVM.EndDate = hasRentVM.EndDate.AddDays(-1);
                }
                else if (hasRentVM.NoeEjare == 2)
                {
                    PersianCalendar pc = new PersianCalendar();
                    hasRentVM.EndDate = pc.AddMonths(hasRentVM.StartDate, (int)hasRentVM.Quantity);
                    hasRentVM.EndDate = hasRentVM.EndDate.AddDays(-1);

                }

                using (var uow = new UnitOfWork())
                {

                    var contractsRentFromIds = await uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => (x.ContractType == ContractType.RentFrom) && x.OrganId == organId ).Select(x => x.ID).ToListAsync2();

                    var allSazesRentFrom = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => contractsRentFromIds.Contains(x.ContractID) && x.SazeId == hasRentVM.SazeID &&
                                                ((x.TarikhShorou >= hasRentVM.StartDate.Date && x.TarikhShorou <= hasRentVM.EndDate.Date) ||
                                                 (x.TarikhShorou <= hasRentVM.StartDate.Date && x.TarikhPayan >= hasRentVM.StartDate.Date) ||
                                                 (x.TarikhShorou <= hasRentVM.StartDate.Date && x.TarikhPayan >= hasRentVM.StartDate.Date) ||
                                                 (x.TarikhShorou >= hasRentVM.StartDate.Date && x.TarikhPayan <= hasRentVM.EndDate.Date)));

                    if (!await allSazesRentFrom.AnyAsync2())
                    {
                        string str = " این سازه از تاریخ " + hasRentVM.DisplayTarikhShorou + " تا تاریخ " + PersianDateUtils.ToPersianDate(hasRentVM.EndDate) + " در اجاره شرکت نمی باشد. ";
                      
                        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = str });
                    }

                    var contractsIds = await uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => (x.ContractType == ContractType.PreContract || x.ContractType == ContractType.RentTo && x.ID != hasRentVM.ContractID) && x.OrganId == organId).Select(x => x.ID).ToListAsync2();
                    var allSazes = uow.RepositoryAsync<Contract_Saze>().Queryable().Where(x => contractsIds.Contains(x.ContractID) && x.SazeId == hasRentVM.SazeID &&
                                                ((x.TarikhShorou >= hasRentVM.StartDate.Date && x.TarikhShorou <= hasRentVM.EndDate.Date) ||
                                                 (x.TarikhShorou <= hasRentVM.StartDate.Date && x.TarikhPayan >= hasRentVM.StartDate.Date) ||
                                                 (x.TarikhShorou <= hasRentVM.StartDate.Date && x.TarikhPayan >=  hasRentVM.StartDate.Date) ||
                                                 (x.TarikhShorou >= hasRentVM.StartDate.Date && x.TarikhPayan <= hasRentVM.EndDate.Date )));

                    var contractStopDetails = uow.RepositoryAsync<ContractStopDetails>().Queryable().Where(x => x.SazeID == hasRentVM.SazeID &&
                                           x.StartDate <= hasRentVM.StartDate.Date && x.EndDate > hasRentVM.EndDate.Date);

                    if(await contractStopDetails.AnyAsync2())
                        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });

                    if (await allSazes.AnyAsync2())
                    {
                        var p = await allSazes.FirstOrDefaultAsync2();
                        var contract = await uow.RepositoryAsync<DomainClasses.Contract.Contract>().Queryable().Where(x => x.ID == p.ContractID).SingleOrDefaultAsync2();

                        string noeEjareStr = "ماه";
                        if (p.NoeEjareId == 1)
                            noeEjareStr = "روز";
                        string str = " این سازه از تاریخ " + p.DisplayTarikhShorou + " به مدت " + ((int)p.Quantity).ToString() +" "+ noeEjareStr + " در اجاره قرارداد شماره " + Convert.ToInt32(contract.Number).ToString() + " با عنوان " + contract.ContractTitle +" می باشد. " ;
                        //string str = " می باشد. " + contract.ContactTitle + " با عنوان " + contract.Number + " در اجاره قرارداد شماره " + noeEjareStr + p.Quantity + " به مدت " + p.DisplayTarikhShorou + " این سازه از تاریخ ";
                        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = str });
                    }
                    else
                    {
                        var reservationsIds = uow.RepositoryAsync<Reservation>().Queryable().Where(x => x.OrganID == organId).Select(x => x.ID).ToList();

                        var reservation_Details = uow.RepositoryAsync<Reservation_Detail>().Queryable().Where(x => reservationsIds.Contains(x.ReservationID) && x.SazeID == hasRentVM.SazeID &&
                            ((x.StartDate >= hasRentVM.StartDate.Date && x.StartDate <= hasRentVM.EndDate.Date) ||
                             (x.StartDate <= hasRentVM.StartDate.Date && x.EndDate >= hasRentVM.StartDate.Date) ||
                             (x.StartDate <= hasRentVM.StartDate.Date && x.EndDate >= hasRentVM.StartDate.Date) ||
                             (x.StartDate >= hasRentVM.StartDate.Date && x.EndDate <= hasRentVM.EndDate.Date)));

                        if (await reservation_Details.AnyAsync2())
                        {
                            var reservation_Detail = await reservation_Details.FirstOrDefaultAsync2();
                            var reservation = await uow.RepositoryAsync<Reservation>().Queryable().Where(x => x.ID == reservation_Detail.ReservationID).SingleOrDefaultAsync2();

                            string noeEjareStr = "ماه";
                            if (reservation_Detail.NoeEjareID == 1)
                                noeEjareStr = "روز";
                            string str = " این سازه از تاریخ " + reservation_Detail.StartDisplayDate + " به مدت " + ((int)reservation_Detail.Quantity).ToString() + " " + noeEjareStr + " رزرو می باشد. ";// + Convert.ToInt32(reservation.).ToString() + " با عنوان " + reservation. + " می باشد. ";
                            //string str = " می باشد. " + contract.ContactTitle + " با عنوان " + contract.Number + " در اجاره قرارداد شماره " + noeEjareStr + p.Quantity + " به مدت " + p.DisplayTarikhShorou + " این سازه از تاریخ ";
                            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.ValidationError, data = str });
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = "" });
                    }

                }

                   
        


                  
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Exception, data = "خطای به وجود آمده است." });
            }
        }
        
    }


    public class WorkFlowBusiClass
    {
        public int ID { get; set; }
        public int? OrganizationID { get; set; }
        public string Code { get; set; }
        public string InstanceTitle { get; set; }
        //public Enums.NoeDarkhastEnum NoeDarkhastID { get; set; }
        public OMF.Workflow.WFExchangeData ExchangeData { get; set; }
    }

    public class loadContractDataBusi
    {
        public int id { get; set; }
        public ContractType? contractType { get; set; }
        public List<SazeInfo> lstSaze { get; set; }
    }
    public class loadStopContractDataBusi
    {
        public bool isStopGroup { get; set; }
        public int? sazeID { get; set; }
        public int contractID { get; set; }
    }


    public class loadRockDataBusi
    {
        public ZhivarEnums.TempleteType TempleteType { get; set; }
        public bool IsDefault { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string StartDisplayDate { get; set; }
        public string EndDisplayDate { get; set; }
        public int? SazeID { get; set; }
        public int? ContactID { get; set; }
    }


    public class ContractInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string NameOfYear { get; set; }
        public string NameOfMonth { get; set; }
        public int NumberOfYear { get; set; }
        public int NumberOfMonth { get; set; }
        public List<PcCalender> Week1 { get; set; }
        public List<PcCalender> Week2 { get; set; }
        public List<PcCalender> Week3 { get; set; }
        public List<PcCalender> Week4 { get; set; }
        public List<PcCalender> Week5 { get; set; }

        public string Image { get; set; }
        public List<Image> Images { get; set; }
    }

    public class Image
    {
        public string Base64 { get; set; }
    }
    public class PcCalender
    {
        public int id { get; set; }
        public int? day { get; set; }
    }

    public class GetCalenderBusi
    {
        public int month { get; set; }
        public int year { get; set; }
    }

    public class CalcTotal
    {
        public int SazeID { get; set; }
        public int GroupSazeID { get; set; }
        public double Occupy { get; set; }

    }
}
