using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.Common;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using OMF.EntityFramework.Ef6;

namespace Zhivar.Utilities
{
    public class TranslateHelper
    {
        public static Document TranslateEntityVMToEntityDocument(DocumentVM documentVM)
        {
            Document document = new Document();

            if (documentVM == null)
            {
                document.Transactions = new List<Transaction>();
                return document;
            }
                

            document.Credit = documentVM.Credit;
            document.DateTime = documentVM.DateTime;
            document.Debit = documentVM.Credit;
            document.Description = documentVM.Description;
            document.DisplayDate = documentVM.DisplayDate;
            document.ID = documentVM.ID;
            document.IsFirsDocument = documentVM.IsFirsDocument;
            document.Number = documentVM.Number;
            document.Number2 = documentVM.Number2;
            document.Status = documentVM.Status;
            document.StatusString = documentVM.StatusString;
            document.IsManual = documentVM.IsManual;
            document.FinanYearId = documentVM.FinanYearId;
            document.IsFirsDocument = documentVM.IsFirsDocument;
            document.OrganId = documentVM.OrganId;
            document.Type = documentVM.Type;

            if (documentVM.Transactions != null)
            {
                document.Transactions = new List<Transaction>();
                Transaction transaction = new Transaction();
                foreach (var transactionVM in documentVM.Transactions)
                {
                    transaction = new Transaction();

                    //if (transactionVM.AccDocument != null)
                    //{
                    //    transaction.AccDocument = new Document()
                    //    {
                    //        Credit = documentVM.Credit,
                    //        DateTime = documentVM.DateTime,
                    //        Debit = documentVM.Credit,
                    //        Description = documentVM.Description,
                    //        DisplayDate = documentVM.DisplayDate,
                    //        ID = documentVM.ID,
                    //        IsFirsDocument = documentVM.IsFirsDocument,
                    //        Number = documentVM.Number,
                    //        Number2 = documentVM.Number2,
                    //        Status = documentVM.Status,
                    //        StatusString = documentVM.StatusString,
                    //        IsManual = documentVM.IsManual,
                    //        FinanYearId = documentVM.FinanYearId,
                    //        OrganId = documentVM.OrganId
                    //    };
                    //}
                    if (transactionVM.Account != null)
                    {
                        transaction.Account = new Account()
                        {
                            Coding = transactionVM.Account.Coding,
                            ComplteCoding = transactionVM.Account.ComplteCoding,
                            ID = transactionVM.Account.ID,
                            Level = transactionVM.Account.Level,
                            Name = transactionVM.Account.Name,
                            OrganId = transactionVM.Account.OrganId,
                            ParentId = transactionVM.Account.ParentId
                        };

                        transaction.AccountId = transactionVM.AccountId;
                    transaction.Amount = transactionVM.Amount;
                    transaction.Cheque = transactionVM.Cheque;
                    transaction.ChequeId = transactionVM.ChequeId;
                    transaction.ContactId = transactionVM.ContactId;
                    transaction.CostId = transactionVM.CostId;
                    transaction.Credit = transactionVM.Credit;
                    transaction.Date = transactionVM.Date;
                    transaction.Debit = transactionVM.Debit;
                    transaction.Description = transactionVM.Description;
                    transaction.DisplayDate = transactionVM.DisplayDate;
                    transaction.DocumentId = transactionVM.DocumentId;
                    transaction.ID = transactionVM.ID;
                    transaction.InvoiceId = transactionVM.InvoiceId;
                    transaction.IsCredit = transactionVM.IsCredit;
                    transaction.IsDebit = transactionVM.IsDebit;
                    transaction.PaymentMethod = transactionVM.PaymentMethod;
                    transaction.PaymentMethodString = transactionVM.PaymentMethodString;
                    transaction.Reference = transactionVM.Reference;
                    transaction.RefTrans = transactionVM.RefTrans;
                    transaction.Remaining = transactionVM.Remaining;
                    transaction.RemainingType = transactionVM.RemainingType;
                    transaction.RowNumber = transactionVM.RowNumber;
                    transaction.Stock = transactionVM.Stock;
                    transaction.TransactionTypeString = transactionVM.TransactionTypeString;
                    transaction.Type = transactionVM.Type;
                    transaction.UnitPrice = transactionVM.UnitPrice;

                        }

                    document.Transactions.Add(transaction);
                }
            }

            if (documentVM.FinanYear != null)
            {
                document.FinanYear = new FinanYear()
                {
                    Closed = documentVM.FinanYear.Closed,
                    DisplayEndDate = documentVM.FinanYear.DisplayEndDate,
                    DisplayStartDate = documentVM.FinanYear.DisplayStartDate,
                    FirstYear = documentVM.FinanYear.FirstYear,
                    ID = documentVM.FinanYear.ID,
                    Name = documentVM.FinanYear.Name,
                    OrganId = documentVM.FinanYear.OrganId,
                };
            }

            return document;

        }

        public static List<Document> TranslateEntityVMToEntityListDocument(List<DocumentVM> documentVMs)
        {
            List<Document> documents = new List<Document>();

            if (documentVMs == null || documentVMs.Count == 0)
                return documents;


            foreach (var documentVM in documentVMs)
            {
                Document document = new Document();

                document.Credit = documentVM.Credit;
                document.DateTime = documentVM.DateTime;
                document.Debit = documentVM.Credit;
                document.Description = documentVM.Description;
                document.DisplayDate = documentVM.DisplayDate;
                document.ID = documentVM.ID;
                document.IsFirsDocument = documentVM.IsFirsDocument;
                document.Number = documentVM.Number;
                document.Number2 = documentVM.Number2;
                document.Status = documentVM.Status;
                document.StatusString = documentVM.StatusString;
                document.IsManual = documentVM.IsManual;
                document.FinanYearId = documentVM.FinanYearId;
                document.IsFirsDocument = documentVM.IsFirsDocument;
                document.OrganId = documentVM.OrganId;
                document.Type = documentVM.Type;

                if (documentVM.Transactions != null)
                {
                    document.Transactions = new List<Transaction>();
                    Transaction transaction = new Transaction();
                    foreach (var transactionVM in documentVM.Transactions)
                    {
                        transaction = new Transaction();

                        //if (transactionVM.AccDocument != null)
                        //{
                        //    transaction.AccDocument = new Document()
                        //    {
                        //        Credit = documentVM.Credit,
                        //        DateTime = documentVM.DateTime,
                        //        Debit = documentVM.Credit,
                        //        Description = documentVM.Description,
                        //        DisplayDate = documentVM.DisplayDate,
                        //        ID = documentVM.ID,
                        //        IsFirsDocument = documentVM.IsFirsDocument,
                        //        Number = documentVM.Number,
                        //        Number2 = documentVM.Number2,
                        //        Status = documentVM.Status,
                        //        StatusString = documentVM.StatusString,
                        //        IsManual = documentVM.IsManual,
                        //        FinanYearId = documentVM.FinanYearId,
                        //        OrganId = documentVM.OrganId
                        //    };
                        //}

                            if (transactionVM.Account != null)
                            {
                                transaction.Account = new Account()
                                {
                                    Coding = transactionVM.Account.Coding,
                                    ComplteCoding = transactionVM.Account.ComplteCoding,
                                    ID = transactionVM.Account.ID,
                                    Level = transactionVM.Account.Level,
                                    Name = transactionVM.Account.Name,
                                    OrganId = transactionVM.Account.OrganId,
                                    ParentId = transactionVM.Account.ParentId
                                };

                                transaction.AccountId = transactionVM.AccountId;
                                transaction.Amount = transactionVM.Amount;
                                transaction.Cheque = transactionVM.Cheque;
                                transaction.ChequeId = transactionVM.ChequeId;
                                transaction.ContactId = transactionVM.ContactId;
                                transaction.CostId = transactionVM.CostId;
                                transaction.Credit = transactionVM.Credit;
                                transaction.Date = transactionVM.Date;
                                transaction.Debit = transactionVM.Debit;
                                transaction.Description = transactionVM.Description;
                                transaction.DisplayDate = transactionVM.DisplayDate;
                                transaction.DocumentId = transactionVM.DocumentId;
                                transaction.ID = transactionVM.ID;
                                transaction.InvoiceId = transactionVM.InvoiceId;
                                transaction.IsCredit = transactionVM.IsCredit;
                                transaction.IsDebit = transactionVM.IsDebit;
                                transaction.PaymentMethod = transactionVM.PaymentMethod;
                                transaction.PaymentMethodString = transactionVM.PaymentMethodString;
                                transaction.Reference = transactionVM.Reference;
                                transaction.RefTrans = transactionVM.RefTrans;
                                transaction.Remaining = transactionVM.Remaining;
                                transaction.RemainingType = transactionVM.RemainingType;
                                transaction.RowNumber = transactionVM.RowNumber;
                                transaction.Stock = transactionVM.Stock;
                                transaction.TransactionTypeString = transactionVM.TransactionTypeString;
                                transaction.Type = transactionVM.Type;
                                transaction.UnitPrice = transactionVM.UnitPrice;

                            }

                            
                        
                        document.Transactions.Add(transaction);
                    }
                }

                if (documentVM.FinanYear != null)
                {
                    document.FinanYear = new FinanYear()
                    {
                        Closed = documentVM.FinanYear.Closed,
                        DisplayEndDate = documentVM.FinanYear.DisplayEndDate,
                        DisplayStartDate = documentVM.FinanYear.DisplayStartDate,
                        FirstYear = documentVM.FinanYear.FirstYear,
                        ID = documentVM.FinanYear.ID,
                        Name = documentVM.FinanYear.Name,
                        OrganId = documentVM.FinanYear.OrganId,
                    };
                }

             
                documents.Add(document);
            }

            return documents;
            }

        public static Contact TranslateContactVMToContact(ContactVM contactVM)
        {
            Contact contact = new Contact();

            if (contactVM == null )
                return contact;

                contact.Address = contactVM.Address;
                contact.Balance = contactVM.Balance;
                contact.City = contactVM.City;
                contact.Code = contactVM.Code;
                contact.ContactType = contactVM.ContactType;
                contact.Credits = contactVM.Credits;
                contact.EconomicCode = contactVM.EconomicCode;
                contact.Email = contactVM.Email;
                contact.Fax = contactVM.Fax;
                contact.FirstName = contactVM.FirstName;
                contact.ID = contactVM.ID;
                contact.IsCustomer = contactVM.IsCustomer;
                contact.IsEmployee = contactVM.IsEmployee;
                contact.IsShareHolder = contactVM.IsShareHolder;
                contact.IsVendor = contactVM.IsVendor;
                contact.LastName = contactVM.LastName;
                contact.Liability = contactVM.Liability;
                contact.Mobile = contactVM.Mobile;
                contact.Name = contactVM.Name;
                contact.NationalCode = contactVM.NationalCode;
                contact.Note = contactVM.Note;
                contact.OrganId = contactVM.OrganId;
                contact.Phone = contactVM.Phone;
                contact.PostalCode = contactVM.PostalCode;
                contact.Rating = contactVM.Rating;
                contact.RegistrationDate = contactVM.RegistrationDate;
                contact.RegistrationNumber = contactVM.RegistrationNumber;
                contact.SharePercent = contactVM.SharePercent;
                contact.State = contactVM.State;
                contact.Website = contactVM.Website;
                contact.Jensiat = contactVM.Jensiat;
                contact.Company = contactVM.Company;
                contact.FileName = contactVM.FileName;
                contact.FileSize = contactVM.FileSize;
                contact.MimeType = contactVM.MimeType;
                contact.Blob = contactVM.Blob;
                contact.Color = contactVM.Color;

            return contact;
            }

        public static Item TranslateItemVMToItem(ItemVM itemVM)
        {
            Item item = new Item();

            if (itemVM == null)
                return item;

            item.Barcode = itemVM.Barcode;
            item.BuyPrice = itemVM.BuyPrice;
            item.Code = itemVM.Code;
            item.ID = itemVM.ID;
            item.IsGoods = itemVM.IsGoods;
            item.ItemGroupId = itemVM.ItemGroupId;
            item.ItemType = itemVM.ItemType;
            item.MoneyStock = itemVM.MoneyStock;
            item.Name = itemVM.Name;
            item.OrganIdItem = itemVM.OrganId;
            item.PurchasesTitle = itemVM.PurchasesTitle;
            item.SalesTitle = itemVM.SalesTitle;
            item.SellPrice = itemVM.SellPrice;
            item.Stock = itemVM.Stock;
            item.UnitID = itemVM.UnitID;


            return item;

        }

        public static List<ContactVM> TranslateEntityToEntityVMListContact(List<Contact> contacts)
        {
            List<ContactVM> contactVMs = new List<ContactVM>();

            if (contacts == null || contacts.Count == 0)
                return contactVMs;


            foreach (var contact in contacts)
            {
                ContactVM contactVM = new ContactVM();

                contactVM.Address = contact.Address;
                contactVM.Balance = contact.Balance;
                contactVM.City = contact.City;
                contactVM.Code = contact.Code;
                contactVM.ContactType = contact.ContactType;
                contactVM.Credits = contact.Credits;
                contactVM.EconomicCode = contact.EconomicCode;
                contactVM.Email = contact.Email;
                contactVM.Fax = contact.Fax;
                contactVM.FirstName = contact.FirstName;
                contactVM.ID = contact.ID;
                contactVM.IsCustomer = contact.IsCustomer;
                contactVM.IsEmployee = contact.IsEmployee;
                contactVM.IsShareHolder = contact.IsShareHolder;
                contactVM.IsVendor = contact.IsVendor;
                contactVM.LastName = contact.LastName;
                contactVM.Liability = contact.Liability;
                contactVM.Mobile = contact.Mobile;
                contactVM.Name = contact.Name;
                contactVM.NationalCode = contact.NationalCode;
                contactVM.Note = contact.Note;
                contactVM.OrganId = contact.OrganId;
                contactVM.Phone = contact.Phone;
                contactVM.PostalCode = contact.PostalCode;
                contactVM.Rating = contact.Rating;
                contactVM.RegistrationDate = contact.RegistrationDate;
                contactVM.RegistrationNumber = contact.RegistrationNumber;
                contactVM.SharePercent = contact.SharePercent;
                contactVM.State = contact.State;
                contactVM.Website = contact.Website;
                contactVM.Jensiat = contact.Jensiat;
                contactVM.Company = contact.Company;
                contactVMs.Add(contactVM);
            }

            return contactVMs;
        }

        public static InvoiceVM TranslateEntityToEntityVMInvoice(Invoice invoice)
        {
            InvoiceVM invoiceVM = new InvoiceVM();

            if (invoice == null)
                return invoiceVM;

            AutoMapper.Mapper.Map(invoice, invoiceVM);

            foreach (var invoiceItem in invoiceVM.InvoiceItems?? new List<InvoiceItemVM>())
            {
                using (var uow = new UnitOfWork())
                {
                    var item = uow.Repository<Item>().Find(invoiceItem.ItemId);
                    ItemVM itemVM = new ItemVM();
                    AutoMapper.Mapper.Map(item, itemVM);

                    invoiceItem.Item = itemVM;
                }
            }

            

            if (invoice.ContactId>0)
            {
                using (var uow = new UnitOfWork())
                {
                    var conatct = uow.Repository<Contact>().Find(invoice.ContactId);

                    ContactVM contactVM = new ContactVM();

                    AutoMapper.Mapper.Map(conatct, contactVM);

                    invoiceVM.Contact = contactVM;
                }
            }

            return invoiceVM;

        }
    }
    }
