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

namespace Zhivar.ServiceLayer.Accunting
{
    public class CostService : ICost
    {
        IUnitOfWork _uow;
        IMappingEngine Mapper;
        readonly IDbSet<Cost> _costs;
        readonly IDbSet<FinanYear> _finanYears;

        public CostService(IUnitOfWork uow, IMappingEngine mappingEngine)
        {
            _uow = uow;
            Mapper = mappingEngine;
            _costs = _uow.Set<Cost>();
            _finanYears = _uow.Set<FinanYear>();
        }
        //public async Task<bool> Delete(int id)
        //{
        //    try
        //    {
        //        var cost = await GetByIdAsync(id);
        //        Delete(cost);
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        public bool Delete(Cost cost)
        {
            try
            {
                _costs.Attach(cost);
                _costs.Remove(cost);



                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

 
        //public async Task<IList<Cost>> GetAllByOrganIdAsync(int organId)
        //{
        //    //var finanYear = await _finanYears.Where(x => x.Closed == false && x.OrganId == organId).SingleOrDefaultAsync();
        //    return await _costs.AsQueryable().Where(x => x.OrganId == organId).Include(x => x.CostItems).Include(x => x.Contact).OrderByDescending(x => x.ID).ToListAsync();
        //}

        //public async Task<bool> Insert(Cost cost)
        //{
        //    try
        //    {
        //        //var finanYear = await _finanYears.Where(x => x.Closed == false).SingleOrDefaultAsync();
        //        //cost.FinanYear = finanYear;
        //        //cost.FinanYearId = finanYear.ID;

        //        if (cost.Contact != null)
        //            _uow.Entry(cost.Contact).State = EntityState.Unchanged;

        //        foreach (var costItem in cost.CostItems ?? new List<CostItem>())
        //        {
        //            if (costItem.Item != null)
        //            {
        //                var local = _uow.Set<DomainClasses.Accounting.Account>()
        //                    .Local.FirstOrDefault(f => f.ID == costItem.Item.ID);
        //                if (local != null)
        //                {
        //                    _uow.Entry(local).State = EntityState.Detached;
        //                }

        //                _uow.Entry(costItem.Item).State = EntityState.Unchanged;


        //                _uow.Entry(costItem.Item).State = EntityState.Unchanged;

        //            }
        //        }
        //        _costs.Add(cost);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //}

        //public async Task<bool> Update(Cost cost)
        //{
        //    try
        //    {
        //        var local = _uow.Set<Cost>()
        //             .Local
        //             .FirstOrDefault(f => f.ID == cost.ID);
        //        if (local != null)
        //        {
        //            _uow.Entry(local).State = EntityState.Detached;
        //        }



        //        if (cost.Contact != null)
        //            _uow.Entry(cost.Contact).State = EntityState.Unchanged;

        //        foreach (var costItem in cost.CostItems ?? new List<CostItem>())
        //        {
        //            if (costItem.Item != null)
        //            {
        //                var localCostItem = _uow.Set<CostItem>().Local.FirstOrDefault(f => f.ID == costItem.ID);
        //                if (localCostItem != null)
        //                {
        //                    _uow.Entry(localCostItem).State = EntityState.Detached;
        //                }

        //                costItem.ItemId = costItem.Item.ID;

        //                costItem.ItemId = costItem.Item.ID;

        //                var localAccount = _uow.Set<DomainClasses.Accounting.Account>()
        //                  .Local.FirstOrDefault(f => f.ID == costItem.Item.ID);
        //                if (localAccount != null)
        //                {
        //                    _uow.Entry(local).State = EntityState.Detached;
        //                }

        //                _uow.Entry(costItem.Item).State = EntityState.Unchanged;
        //            }

        //            costItem.CostId = cost.ID;
        //            if (costItem.ID > 0)
        //                _uow.Entry(costItem).State = EntityState.Modified;
        //            else
        //                _uow.Entry(costItem).State = EntityState.Added;
        //        }

        //        _costs.Attach(cost);

        //        _uow.Entry(cost).State = EntityState.Modified;
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public async Task<Cost> GetByIdAsync(int id)
        //{
        //    return await _costs.AsQueryable().Where(x => x.ID == id).Include(x => x.CostItems.Select(c => c.Item)).Include(x => x.Contact).FirstOrDefaultAsync();
           
        //}

        

    }
}
