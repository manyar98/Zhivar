using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Data.Entity;
using Zhivar.ViewModel.Accunting;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ServiceLayer.Accunting
{
    public class SahamdaranService : ISahamdaran
    {
        IUnitOfWork _uow;
        readonly IDbSet<Sahamdaran> _sahamdarans;
        readonly IDbSet<Person> _people;
        public SahamdaranService(IUnitOfWork uow)
        {
            _uow = uow;
            _people = _uow.Set<Person>();
            _sahamdarans = _uow.Set<Sahamdaran>();
        }
        public bool Delete(int id)
        {
            try
            {
                var sahamdaran = GetById(id);
                Delete(sahamdaran);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Sahamdaran sahamdaran)
        {
            try
            {
                _sahamdarans.Attach(sahamdaran);
                _sahamdarans.Remove(sahamdaran);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Sahamdaran> GetAll()
        {
            return _sahamdarans.ToList();
        }
        public IList<Sahamdaran> GetAllByOrganId(int organId)
        {
            return _sahamdarans.AsQueryable().Where(x => x.OrganId == organId).ToList();

        }
        public async Task<IList<SahamdaranVM>> GetAllByOrganIdAsync(int organId)
        {
            var sahamdaranQuery = _sahamdarans.AsQueryable().Where(x => x.OrganId == organId);
            var peopleQuery = _people.AsQueryable();

            var joinQuery = from sahamdar in sahamdaranQuery
                            join person in peopleQuery
                            on sahamdar.ShakhsId equals person.ID
                            select new SahamdaranVM
                            {
                                Address = person.Address,
                                CodeMeli = person.CodeMeli,
                                CodePosti = person.CodePosti,
                                FullName = person.Nam + " "+person.NamKhanvadegi,
                                ID = sahamdar.ID,
                                Jenseat = person.Jenseat,
                                Nam = person.Nam,
                                NamKhanvadegi = person.NamKhanvadegi,
                                NamPedar = person.NamPedar,
                                OrganId = sahamdar.OrganId,
                                Sahm = sahamdar.Sahm,
                                ShakhsId = person.ID,
                                Tel = person.Tel
                            };

            return await joinQuery.ToListAsync();
        }
        public async Task<IList<Sahamdaran>> GetAllAsync()
        {
            return await _sahamdarans.ToListAsync();
        }
        public Sahamdaran GetById(int id)
        {
            return _sahamdarans.Find(id);
        }

        public bool Insert(Sahamdaran sahamdaran)
        {
            try
            {
                _sahamdarans.Add(sahamdaran);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Sahamdaran sahamdaran)
        {
            try
            {
                var local = _uow.Set<Sahamdaran>()
                     .Local
                     .FirstOrDefault(f => f.ID == sahamdaran.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _sahamdarans.Attach(sahamdaran);

                _uow.Entry(sahamdaran).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Sahamdaran> GetByIdAsync(int id)
        {
            return await _sahamdarans.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
