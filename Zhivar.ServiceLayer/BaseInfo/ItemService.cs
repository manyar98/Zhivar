using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using System.Data.Entity;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;

namespace Zhivar.ServiceLayer.BaseInfo
{
    public class ItemService : IItem
    {
        IUnitOfWork _uow;
        readonly IDbSet<Item> _items;
        public ItemService(IUnitOfWork uow)
        {
            _uow = uow;
            _items = _uow.Set<Item>();
        }
        public bool Delete(int id)
        {
            try
            {
                var item = GetById(id);
                Delete(item);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Item item)
        {
            try
            {
                _items.Attach(item);
                _items.Remove(item);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Item> GetAll()
        {
            return _items.ToList();
        }
    
        public async Task<IList<Item>> GetAllAsync()
        {
            return await _items.ToListAsync();
        }
        public Item GetById(int id)
        {
            return _items.Find(id);
        }

        public bool Insert(Item item)
        {
            try
            {
                _items.Add(item);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Item item)
        {
            try
            {
                var local = _uow.Set<Item>()
                     .Local
                     .FirstOrDefault(f => f.ID == item.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _items.Attach(item);

                _uow.Entry(item).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _items.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
