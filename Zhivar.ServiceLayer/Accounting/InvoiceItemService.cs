using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Data.Entity;
using Zhivar.ViewModel.Accunting;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Accunting
{
    public class InvoiceItemService : IInvoiceItem
    {
        IUnitOfWork _uow;
        readonly IDbSet<InvoiceItem> _invoiceItems;
        readonly IDbSet<Item> _items;
        public InvoiceItemService(IUnitOfWork uow)
        {
            _uow = uow;
            _invoiceItems = _uow.Set<InvoiceItem>();
            _items = _uow.Set<Item>();
        }
        public bool Delete(int id)
        {
            try
            {
                var invoiceItem = GetById(id);
                Delete(invoiceItem);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(InvoiceItem invoiceItem)
        {
            try
            {
                _invoiceItems.Attach(invoiceItem);
                _invoiceItems.Remove(invoiceItem);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<InvoiceItem> GetAll()
        {
            return _invoiceItems.ToList();
        }
        public IList<InvoiceItem> GetAllByInvoiceId(int invoiceId)
        {
            return _invoiceItems.AsQueryable().Where(x => x.InvoiceId == invoiceId).ToList();
        }
        public async Task<IList<InvoiceItem>> GetAllByInvoiceIdAsync(int invoiceId)
        {
            return await _invoiceItems.AsQueryable().Where(x => x.InvoiceId == invoiceId).ToListAsync();
        }
        public async Task<IList<InvoiceItem>> GetAllAsync()
        {
            return await _invoiceItems.ToListAsync();
        }
        public InvoiceItem GetById(int id)
        {
            return _invoiceItems.Find(id);
        }

        public bool Insert(InvoiceItem invoiceItem)
        {
            try
            {
                _invoiceItems.Add(invoiceItem);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(InvoiceItem invoiceItem)
        {
            try
            {
                var local = _uow.Set<InvoiceItem>()
                     .Local
                     .FirstOrDefault(f => f.ID == invoiceItem.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _invoiceItems.Attach(invoiceItem);

                _uow.Entry(invoiceItem).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<InvoiceItem> GetByIdAsync(int id)
        {
            return await _invoiceItems.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<InvoiceItem>> GetByInvoiceIdAsync(int invoiceId)
        {
   
            var invoiceItemQuery = _invoiceItems.AsQueryable().Where(x => x.InvoiceId == invoiceId);
            var itemsQuery = _items.AsQueryable();

            var joinQuery = from invoiceItem in invoiceItemQuery
                            join item in itemsQuery
                            on invoiceItem.ItemId equals item.ID
                            select invoiceItem ;

            return await joinQuery.ToListAsync();
        }
    }
}
