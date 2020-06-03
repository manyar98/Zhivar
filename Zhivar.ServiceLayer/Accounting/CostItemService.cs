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
    public class CostItemService : ICostItem
    {
        IUnitOfWork _uow;
        readonly IDbSet<CostItem> _costItems;

        public CostItemService(IUnitOfWork uow)
        {
            _uow = uow;
            _costItems = _uow.Set<CostItem>();
         
        }
        public bool Delete(int id)
        {
            try
            {
                var costItem = GetById(id);
                Delete(costItem);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(CostItem costItem)
        {
            try
            {
                _costItems.Attach(costItem);
                _costItems.Remove(costItem);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<CostItem> GetAll()
        {
            return _costItems.ToList();
        }
        //public IList<CostItem> GetAllByCostId(int costId)
        //{
        //    return _costItems.AsQueryable().Where(x => x.Cost.ID == costId).ToList();
        //}
        //public async Task<IList<CostItem>> GetAllByCostIdAsync(int costId)
        //{
        //    return await _costItems.AsQueryable().Where(x => x.Cost.ID == costId).ToListAsync();
        //}
        public async Task<IList<CostItem>> GetAllAsync()
        {
            return await _costItems.ToListAsync();
        }
        public CostItem GetById(int id)
        {
            return _costItems.Find(id);
        }

        public bool Insert(CostItem costItem)
        {
            try
            {
                _costItems.Add(costItem);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(CostItem costItem)
        {
            try
            {
                var local = _uow.Set<CostItem>()
                     .Local
                     .FirstOrDefault(f => f.ID == costItem.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _costItems.Attach(costItem);

                _uow.Entry(costItem).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CostItem> GetByIdAsync(int id)
        {
            return await _costItems.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<CostItem>> GetByCostIdAsync(int costId)
        {

            var costItemQuery = _costItems.AsQueryable().Where(x => x.CostId == costId);


            //var joinQuery = from costItem in costItemQuery
            //                join item in itemsQuery
            //                on costItem.ItemId equals item.ID
            //                select costItem;

            return await costItemQuery.ToListAsync();
        }
    }
}
