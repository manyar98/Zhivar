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
    public class ItemGroupService : IItemGroup
    {
        IUnitOfWork _uow;
        readonly IDbSet<ItemGroup> _itemGroups;
        public ItemGroupService(IUnitOfWork uow)
        {
            _uow = uow;
            _itemGroups = _uow.Set<ItemGroup>();
        }
        public bool Delete(int id)
        {
            try
            {
                var itemGroup = GetById(id);
                Delete(itemGroup);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(ItemGroup itemGroup)
        {
            try
            {
                _itemGroups.Attach(itemGroup);
                _itemGroups.Remove(itemGroup);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<ItemGroup> GetAll()
        {
            return _itemGroups.ToList();
        }
        public IList<ItemGroup> GetAllByOrganId(int organId)
        {
            return _itemGroups.AsQueryable().Where(x => x.OrganID == organId).Include(x => x.Items).ToList();
        }
        public async Task<IList<ItemGroup>> GetAllByOrganIdAsync(int organId)
        {
            return await _itemGroups.AsQueryable().Where(x => x.OrganID == organId).Include(x => x.Items).ToListAsync();
        }
        public async Task<IList<ItemGroup>> GetAllAsync()
        {
            return await _itemGroups.ToListAsync();
        }
        public ItemGroup GetById(int id)
        {
            return _itemGroups.Find(id);
        }

        public bool Insert(ItemGroup itemGroup)
        {
            try
            {
                _itemGroups.Add(itemGroup);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(ItemGroup itemGroup)
        {
            try
            {
                var local = _uow.Set<ItemGroup>()
                     .Local
                     .FirstOrDefault(f => f.ID == itemGroup.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _itemGroups.Attach(itemGroup);

                _uow.Entry(itemGroup).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ItemGroup> GetByIdAsync(int id)
        {
            return await _itemGroups.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}