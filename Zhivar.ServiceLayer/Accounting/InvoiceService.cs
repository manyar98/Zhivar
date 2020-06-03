using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Data.Entity;
using Zhivar.DomainClasses;
using AutoMapper;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Accunting
{
    public class InvoiceService : IInvoice
    {
        IUnitOfWork _uow;
        IMappingEngine Mapper;
        readonly IDbSet<Invoice> _invoices;
        readonly IDbSet<FinanYear> _finanYears;
        readonly IDbSet<InvoiceItem> _invoiceItems;
        readonly IDbSet<Document> _documents;
        readonly IDbSet<Transaction> _transactions;
        readonly IDbSet<Item> _items;

        //public InvoiceService(IUnitOfWork uow, IMappingEngine mappingEngine)
        //{
        //    _uow = uow;
        //    Mapper = mappingEngine;
        //    _invoices = _uow.Set<Invoice>();
        //    _finanYears = _uow.Set<FinanYear>();
        //    _invoiceItems = _uow.Set<InvoiceItem>();
        //    _documents = _uow.Set<Document>();
        //    _transactions = _uow.Set<Transaction>();
        //    _items = _uow.Set<Item>();
        //}
        //public async Task<bool> Delete(int id)
        //{
        //    try
        //    {
        //        var invoice = await GetByIdAsync(id);
        //        Delete(invoice);
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        public bool Delete(Invoice invoice)
        {
            try
            {
                _invoices.Attach(invoice);
                _invoices.Remove(invoice);



                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        //public IList<Invoice> GetAll()
        //{
        //    return _invoices.ToList();
        //}
        //public IList<Invoice> GetAllByOrganId(int organId)
        //{
        //    return _invoices.AsQueryable().Where(x => x.OrganId == organId).ToList();
        //}
        //public async Task<IList<Invoice>> GetAllByOrganIdAsync(int organId)
        //{
        //    //var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
        //    return await _invoices.AsQueryable().Where(x => x.OrganId == organId ).Include(x => x.InvoiceItems).Include(x => x.InvoiceItems.Select(c => c.Item)).ToListAsync();
        //}
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
        //           _uow.Entry(invoice.Contact).State = EntityState.Unchanged;

        //        foreach (var invoiceItem in invoice.InvoiceItems ?? new List<InvoiceItem>())
        //        {
        //            if (invoiceItem.Item != null)
        //            {
        //                _uow.Entry(invoiceItem.Item).State = EntityState.Unchanged;

        //                //if (invoiceItem.Item.ItemGroup != null)
        //                //{
        //                //    _uow.Entry(invoiceItem.Item.ItemGroup).State = EntityState.Unchanged;
        //                //}
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

        public async Task<bool> Update(Invoice invoice)
        {
            try
            {
                var local = _uow.Set<Invoice>()
                     .Local
                     .FirstOrDefault(f => f.ID == invoice.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

         

                //if (invoice.Contact != null)
                //    _uow.Entry(invoice.Contact).State = EntityState.Unchanged;

                //foreach (var invoiceItem in invoice.InvoiceItems ?? new List<InvoiceItem>())
                //{
                //    if (invoiceItem.Item != null)
                //    {
                //        invoiceItem.ItemId = invoiceItem.Item.ID;
                //        _uow.Entry(invoiceItem.Item).State = EntityState.Unchanged;

                //        //if (invoiceItem.Item.ItemGroup != null)
                //        //{
                //        //    _uow.Entry(invoiceItem.Item.ItemGroup).State = EntityState.Unchanged;
                //        //}
                //    }

                //    invoiceItem.InvoiceId = invoice.ID;
                //    if(invoiceItem.ID > 0)
                //        _uow.Entry(invoiceItem).State = EntityState.Modified;
                //    else
                //        _uow.Entry(invoiceItem).State = EntityState.Added;
                //}
                _invoices.Attach(invoice);

                _uow.Entry(invoice).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public async Task<Invoice> GetByIdAsync(int id)
        //{
        //    return await _invoices.AsQueryable().Where(x => x.ID == id).Include(x => x.InvoiceItems.Select(c => c.Item)).Include(x => x.Contact).FirstOrDefaultAsync();
        //}

        public async Task<IList<InvoiceVM>> GetAllByOrganIdAsync(int organId, ZhivarEnums.NoeFactor noeInvoice)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();

            var invoices = await  _invoices.AsQueryable().Where(x => x.OrganId == organId && x.InvoiceType == noeInvoice ).ToListAsync();
            List<InvoiceVM> invoiceVMs = new List<InvoiceVM>();

            Mapper.Map(invoices, invoiceVMs);

            return invoiceVMs;

        }
        public async Task<List<InvoiceItemContactItem>> GetAllByContactIdAsync(int organId, int contactId)
        {
            var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();

            var invoicesQuery =  _invoices.AsQueryable().Where(x => x.OrganId == organId && x.ContactId == contactId);
            var invoiceItemsQuery = _invoiceItems.AsQueryable();
            var itemsQuery = _items.AsQueryable();
            var documentsQuery = _documents.AsQueryable().Where(x => x.OrganId == organId);
            var transactionsQuery = _transactions.AsQueryable();
            var itemQuery = _items.AsQueryable();

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


            return await joinQuery.ToListAsync();

        }
    }
}
