using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Account;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Common;
using AutoMapper;
using Zhivar.DomainClasses;

namespace Zhivar.ServiceLayer.Common
{
    public class PersonelService : IPersonel
    {
        IUnitOfWork _uow;
        readonly IDbSet<Personel> _personels;
        readonly IDbSet<Person> _person;
    
        public PersonelService(IUnitOfWork uow)
        {
            _uow = uow;
            _personels = _uow.Set<Personel>();
            _person = _uow.Set<Person>();
       

        }
        //public bool Delete(int id)
        //{
        //    try
        //    {
        //        var personel = GetById(id);
        //        Delete(personel);
        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public bool Delete(Personel personel)
        //{
        //    try
        //    {
        //        _personels.Attach(personel);

        //        //_uow.Entry(personel.Bank).State = EntityState.Deleted;

        //        _personels.Remove(personel);

        //        return true;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public IList<Personel> GetAll()
        //{
        //    return _personels.ToList();
        //}
        //public IList<PersonelVM> GetAllByPersonId(int personId)
        //{
        //    var personelQuery = _personels.AsQueryable().Where(x => x.OrganID == personId).Include("Person");
        //    var userQuery = _user.AsQueryable();
        //    var userRoleQuery = _userRole.AsQueryable();
        //    var roleQuery = _role.AsQueryable();

        //    var joinQuery = from personel in personelQuery
        //                    join user in userQuery
        //                    on personel.UserID equals user.Id
        //                    join userRole in userRoleQuery
        //                    on user.Id equals userRole.UserId
        //                    join role in roleQuery
        //                    on userRole.RoleId equals role.Id
        //                    select new PersonelVM
        //                    {
        //                        ID = personel.ID,
        //                        RoleName = role.PersianName,
        //                        PersonVM = new PersonVM()
        //                        {
        //                            ID = personel.Person.ID,
        //                            Nam = personel.Person.Nam,
        //                            NamKhanvadegi = personel.Person.NamKhanvadegi,
        //                            Address = personel.Person.Address,
        //                            CodeEghtesadi = personel.Person.CodeEghtesadi,
        //                            CodeMeli = personel.Person.CodeMeli,
        //                            CodePosti = personel.Person.CodePosti,
        //                            Jenseat = personel.Person.Jenseat,
        //                            ModateEtebar = personel.Person.ModateEtebar,
        //                            NamPedar = personel.Person.NamPedar,
        //                            NoeShakhs = personel.Person.NoeShakhs,
        //                            SahebEmtiaz = personel.Person.SahebEmtiaz,
        //                            ShomareSabt = personel.Person.ShomareSabt,
        //                            TarikhSoudor = personel.Person.TarikhSoudor,
        //                            Tel = personel.Person.Tel,
        //                            TypeHoghoghi = personel.Person.TypeHoghoghi,
        //                        },
        //                        UserID = personel.UserID,
        //                        RoleID = role.Id,
        //                        UserName = user.UserName
        //                    };

        //    return joinQuery.ToList();
        //}
        //public async Task<IList<PersonelVM>> GetAllByPersonIdAsync(int personId)
        //{
        //    var personelQuery = _personels.AsQueryable().Where(x => x.OrganID == personId).Include("Person");
        //    var userQuery = _user.AsQueryable();
        //    var userRoleQuery = _userRole.AsQueryable();
        //    var roleQuery = _role.AsQueryable();

        //    var joinQuery = from personel in personelQuery
        //                    join user in userQuery
        //                    on personel.UserID equals user.Id
        //                    join userRole in userRoleQuery
        //                    on user.Id equals userRole.UserId
        //                    join role in roleQuery
        //                    on userRole.RoleId equals role.Id
        //                    select new PersonelVM
        //                    {
        //                        ID = personel.ID,
        //                        RoleName = role.PersianName,
        //                        PersonVM = new PersonVM()
        //                        {
                 
        //                            Nam = personel.Person.Nam,
        //                            NamKhanvadegi = personel.Person.NamKhanvadegi
        //                        }
        //                    };

        //    return await joinQuery.ToListAsync();
        //}
        //public async Task<IList<Personel>> GetAllAsync()
        //{
        //    return await _personels.ToListAsync();
        //}
        //public Personel GetById(int id)
        //{
        //    return _personels.Find(id);
        //}

        //public PersonelVM GetForUpdate(int id)
        //{
        //    var personelQuery = _personels.AsQueryable().Where(x => x.ID == id).Include("Person");
        //    var userQuery = _user.AsQueryable();
        //    var userRoleQuery = _userRole.AsQueryable();
        //    var roleQuery = _role.AsQueryable();

        //    var joinQuery = from personel in personelQuery
        //                    join user in userQuery
        //                    on personel.UserID equals user.Id
        //                    join userRole in userRoleQuery
        //                    on user.Id equals userRole.UserId
        //                    join role in roleQuery
        //                    on userRole.RoleId equals role.Id
        //                    select new PersonelVM
        //                    {
        //                        ID = personel.ID,
        //                        RoleName = role.PersianName,
        //                        PersonVM = new PersonVM()
        //                        {
        //                            ID = personel.Person.ID,
        //                            Nam = personel.Person.Nam,
        //                            NamKhanvadegi = personel.Person.NamKhanvadegi,
        //                            Address = personel.Person.Address,
        //                            CodeEghtesadi = personel.Person.CodeEghtesadi,
        //                            CodeMeli = personel.Person.CodeMeli,
        //                            CodePosti = personel.Person.CodePosti,
        //                            Jenseat = personel.Person.Jenseat,
        //                            ModateEtebar = personel.Person.ModateEtebar,
        //                            NamPedar = personel.Person.NamPedar,
        //                            NoeShakhs = personel.Person.NoeShakhs,
        //                            SahebEmtiaz = personel.Person.SahebEmtiaz,
        //                            ShomareSabt = personel.Person.ShomareSabt,
        //                            TarikhSoudor = personel.Person.TarikhSoudor,
        //                            Tel = personel.Person.Tel,
        //                            TypeHoghoghi = personel.Person.TypeHoghoghi,
        //                        },
        //                        UserID = personel.UserID,
        //                        RoleID = role.Id,
        //                        UserName = user.UserName
        //                    };

        //    return joinQuery.SingleOrDefault();

        //}
        //public async Task<PersonelVM> GetForUpdateAsync(int id)
        //{
        //    var personelQuery = _personels.AsQueryable().Where(x => x.ID == id).Include("Person");
        //    var userQuery = _user.AsQueryable();
        //    var userRoleQuery = _userRole.AsQueryable();
        //    var roleQuery = _role.AsQueryable();

        //    var joinQuery = from personel in personelQuery
        //                    join user in userQuery
        //                    on personel.UserID equals user.Id
        //                    join userRole in userRoleQuery
        //                    on user.Id equals userRole.UserId
        //                    join role in roleQuery
        //                    on userRole.RoleId equals role.Id
        //                    select new PersonelVM
        //                    {
        //                        ID = personel.ID,
        //                        RoleName = role.PersianName,
        //                        PersonVM = new PersonVM()
        //                        {
        //                            ID = personel.Person.ID,
        //                            Nam = personel.Person.Nam,
        //                            NamKhanvadegi = personel.Person.NamKhanvadegi,
        //                            Address = personel.Person.Address,
        //                            CodeEghtesadi = personel.Person.CodeEghtesadi,
        //                            CodeMeli = personel.Person.CodeMeli,
        //                            CodePosti = personel.Person.CodePosti,
        //                            Jenseat = personel.Person.Jenseat,
        //                            ModateEtebar = personel.Person.ModateEtebar,
        //                            NamPedar = personel.Person.NamPedar,
        //                            NoeShakhs = personel.Person.NoeShakhs,
        //                            SahebEmtiaz = personel.Person.SahebEmtiaz,
        //                            ShomareSabt = personel.Person.ShomareSabt,
        //                            TarikhSoudor = personel.Person.TarikhSoudor,
        //                            Tel = personel.Person.Tel,
        //                            TypeHoghoghi = personel.Person.TypeHoghoghi,
        //                        },
        //                        UserID = personel.UserID,
        //                        RoleID = role.Id,
        //                        UserName = user.UserName
        //                    };

        //    return await joinQuery.SingleOrDefaultAsync();


        //}

        public bool Insert(Personel personel)
        {
            try
            {
                //if (string.IsNullOrEmpty(personel.Bank.Nam))
                //{
                //    personel.Bank = null;
                //}

                _personels.Add(personel);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Personel personel)
        {
            try
            {
                var local = _uow.Set<Personel>()
                         .Local
                         .FirstOrDefault(f => f.ID == personel.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                var local2 = _uow.Set<Person>()
                       .Local
                       .FirstOrDefault(f => f.ID == personel.Person.ID);
                if (local2 != null)
                {
                    _uow.Entry(local2).State = EntityState.Detached;
                }
              


                _personels.Attach(personel);

                if (personel.Person.ID > 0)
                {
                    _uow.Entry(personel.Person).State = EntityState.Modified;
                }
                else if (personel.Person != null)
                {
                    _uow.Entry(personel.Person).State = EntityState.Added;
                }

         
                _uow.Entry(personel).State = EntityState.Modified;


                //_uow.Entry(personel).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(personel).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Personel> GetByIdAsync(int id)
        {
            return await _personels.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        //public async Task<IList<PersonelVM>> GetByOrganIdAsync(int organId,string noeSemat)
        //{
        //    var personelQuery = _personels.AsQueryable().Where(x => x.OrganID == organId).Include("Person");
        //    var userQuery = _user.AsQueryable();
        //    var userRoleQuery = _userRole.AsQueryable();
        //    var roleQuery = _role.AsQueryable().Where(x => x.Name == noeSemat);

        //    var joinQuery = from personel in personelQuery
        //                    join user in userQuery
        //                    on personel.UserID equals user.Id
        //                    join userRole in userRoleQuery
        //                    on user.Id equals userRole.UserId
        //                    join role in roleQuery
        //                    on userRole.RoleId equals role.Id
        //                    select new PersonelVM
        //                    {
        //                        ID = personel.ID,
        //                        RoleName = role.PersianName,
        //                        Title = personel.Person.Nam+" "+ personel.Person.NamKhanvadegi,
        //                        //PersonVM = new PersonVM()
        //                        //{

        //                        //    Nam = personel.Person.Nam,
        //                        //    NamKhanvadegi = personel.Person.NamKhanvadegi
        //                        //}
        //                    };

        //    return await joinQuery.ToListAsync();
        //}
    }
}
