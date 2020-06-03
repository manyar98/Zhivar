using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;

namespace Zhivar.ServiceLayer.Accunting
{
    public class TarfeHesabService : ITarfeHesab
    {
        IUnitOfWork _uow;
        readonly IDbSet<TarfeHesab> _tarfeHesabs;
        public TarfeHesabService(IUnitOfWork uow)
        {
            _uow = uow;
            _tarfeHesabs = _uow.Set<TarfeHesab>();

        }
        public bool Delete(int id)
        {
            try
            {
                var tarfeHesab = GetForUpdate(id);
                Delete(tarfeHesab);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(TarfeHesab tarfeHesab)
        {
            try
            {
                _tarfeHesabs.Attach(tarfeHesab);
                
                _uow.Entry(tarfeHesab.Bank).State = EntityState.Deleted;
             
                _tarfeHesabs.Remove(tarfeHesab);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<TarfeHesab> GetAll()
        {
            return _tarfeHesabs.ToList();
        }
        public IList<TarfeHesab> GetAllByPersonId(int personId)
        {
            return _tarfeHesabs.AsQueryable().Where(x => x.OrganID == personId).ToList();
        }
        public async Task<IList<TarfeHesab>> GetAllByPersonIdAsync(int personId)
        {
            return await _tarfeHesabs.AsQueryable().Include("Person").Where(x => x.OrganID == personId).ToListAsync();

        }
        public async Task<IList<TarfeHesab>> GetAllAsync()
        {
            return await _tarfeHesabs.ToListAsync();
        }
        public TarfeHesab GetById(int id)
        {
            return _tarfeHesabs.Find(id);
        }

        public TarfeHesab GetForUpdate(int id)
        {
            return _tarfeHesabs.Include("Person").Include("Bank").SingleOrDefault(x => x.ID == id);
        }
        public async Task<TarfeHesab> GetForUpdateAsync(int id)
        {
            return await _tarfeHesabs.Include("Person").Include("Bank").SingleOrDefaultAsync(x => x.ID == id);
        }
        
        public bool Insert(TarfeHesab tarfeHesab)
        {
            try
            {
                if (string.IsNullOrEmpty(tarfeHesab.Bank.Name))
                {
                    tarfeHesab.Bank = null;
                }

                _tarfeHesabs.Add(tarfeHesab);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(TarfeHesab tarfeHesab)
        {
            try
            {
                var local = _uow.Set<TarfeHesab>()
                         .Local
                         .FirstOrDefault(f => f.ID == tarfeHesab.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                var local2 = _uow.Set<Person>()
                       .Local
                       .FirstOrDefault(f => f.ID == tarfeHesab.Person.ID);
                if (local2 != null)
                {
                    _uow.Entry(local2).State = EntityState.Detached;
                }
                var local3 = _uow.Set<Bank>()
                   .Local
                   .FirstOrDefault(f => f.ID == tarfeHesab.Bank.ID);
                if (local3 != null)
                {
                    _uow.Entry(local3).State = EntityState.Detached;
                }


                _tarfeHesabs.Attach(tarfeHesab);

                if (tarfeHesab.Person.ID > 0)
                {
                    _uow.Entry(tarfeHesab.Person).State = EntityState.Modified;
                }
                else if (tarfeHesab.Person != null)
                {
                    _uow.Entry(tarfeHesab.Person).State = EntityState.Added;
                }

                if (tarfeHesab.Bank != null && tarfeHesab.Bank.ID > 0)
                {
                    _uow.Entry(tarfeHesab.Bank).State = EntityState.Modified;
                }
                else if (tarfeHesab.Bank != null)
                {
                    _uow.Entry(tarfeHesab.Bank).State = EntityState.Added;
                }

                _uow.Entry(tarfeHesab).State = EntityState.Modified;

               
                //_uow.Entry(tarfeHesab).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(tarfeHesab).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<TarfeHesab> GetByIdAsync(int id)
        {
            return await _tarfeHesabs.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }


    }
}
