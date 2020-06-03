using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using System.Data.Entity;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses.Account;
using Zhivar.ViewModel.Common;

namespace Zhivar.ServiceLayer.Common
{
    public class PersonService : IPerson
    {
        IUnitOfWork _uow;
        readonly IDbSet<Person> _people;
       // readonly IDbSet<ApplicationUser> _user;
        public PersonService(IUnitOfWork uow)
        {
            _uow = uow;
            _people = _uow.Set<Person>();
         //   _user = _uow.Set<ApplicationUser>();
        }
        public bool Delete(int id)
        {
            try
            {
                var person = GetById(id);
                Delete(person);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Person person)
        {
            try
            {
                _people.Attach(person);
                _people.Remove(person);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Person> GetAll()
        {
            return _people.Where(x => x.IsOrgan == true).ToList();
        }
        public async Task<IList<Person>> GetAllAsync()
        {
            return await _people.Where(x => x.IsOrgan == true).ToListAsync();
        }
        public Person GetById(int id)
        {
            return _people.Find(id);
        }

        //public Person GetPersonByUserId(int userId)

        //{
        //    var user = _user.Find(userId);
        //    var person = _people.AsQueryable().Where(x => x.ID == user.TagInt1).SingleOrDefault();

        //    return person;
        //}

        public async Task<List<PersonVM>> GetAllByOrganIdAsync(int organId)
        {
            var personQuery =  _people.AsQueryable().Where(x => x.OrganId == organId);

            var resualt = from person in personQuery
                          select new PersonVM
                          {
                              ID = person.ID,
                              Nam = person.Nam,
                              NamKhanvadegi = person.NamKhanvadegi
                          };
            return await resualt.ToListAsync();
        }
        
        public bool Insert(Person person)
        {
            try
            {
                _people.Add(person);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Person person)
        {
            try
            {
                _people.Attach(person);
                
                _uow.Entry(person).State = EntityState.Modified;
                if (person.IsOrgan == false)
                {
                    _uow.Entry(person).Property(p => p.Vaziat).IsModified = false;
                }
                _uow.Entry(person).Property(p => p.IsOrgan).IsModified = false;

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            return await _people.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }
    }
}
