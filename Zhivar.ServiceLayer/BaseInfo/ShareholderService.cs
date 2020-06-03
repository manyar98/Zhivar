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
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.BaseInfo
{
    public class ShareholderService : IShareholder
    {
        IUnitOfWork _uow;
        readonly IDbSet<Shareholder> _shareholders;
        readonly IDbSet<Contact> _contacts;
        public ShareholderService(IUnitOfWork uow)
        {
            _uow = uow;
            _shareholders = _uow.Set<Shareholder>();
            _contacts = _uow.Set<Contact>();
        }
        public bool Delete(int id)
        {
            try
            {
                var shareholder = GetById(id);
                Delete(shareholder);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(Shareholder shareholder)
        {
            try
            {
                _shareholders.Attach(shareholder);
                _shareholders.Remove(shareholder);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<Shareholder> GetAll()
        {
            return _shareholders.ToList();
        }
        public async Task<IList<ShareholderVM>> GetAllByOrganIdAsync(int organId)
        {
            var shareholders = _shareholders.AsQueryable().Where(x => x.OrganId == organId);
            var contactQuery = _contacts.AsQueryable();

            var joinQuery = from shareholder in shareholders
                            join contact in contactQuery
                            on shareholder.ContactId equals contact.ID
                            select new ShareholderVM()
                            {
                                
                                Address = contact.Address,
                                Website = contact.Website,
                                ID = contact.ID,
                                City = contact.City,
                                Code = contact.Code,
                                ContactEmail = contact.Email,
                                Email = contact.Email,
                                ContactType = contact.ContactType,
                                Credits = contact.Credits,
                                EconomicCode = contact.EconomicCode,
                                Fax = contact.Fax,
                                FirstName = contact.FirstName,
                                IsCustomer = contact.IsCustomer,
                                IsEmployee = contact.IsEmployee,
                                IsShareHolder = contact.IsShareHolder,
                                IsVendor = contact.IsVendor,
                                LastName = contact.LastName,
                                Liability = contact.Liability,
                                Mobile = contact.Mobile,
                                Name = contact.Name,
                                NationalCode = contact.NationalCode,
                                Note = contact.Note,
                                Phone = contact.Phone,
                                PostalCode = contact.PostalCode,
                                Rating = contact.Rating,
                                RegistrationDate = contact.RegistrationDate,
                                RegistrationNumber = contact.RegistrationNumber,
                                SharePercent = shareholder.SharePercent,
                                State = contact.State,
                                
                                
                            };

            return await joinQuery.ToListAsync();

        }
        public async Task<IList<Shareholder>> GetAllAsync()
        {
            return await _shareholders.ToListAsync();
        }
        public Shareholder GetById(int id)
        {
            return _shareholders.Find(id);
        }

        public bool Insert(Shareholder shareholder)
        {
            try
            {
                _shareholders.Add(shareholder);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Shareholder shareholder)
        {
            try
            {
                var oldShareholder = _uow.Set<Shareholder>()
                .Local
                .FirstOrDefault(f => f.ID == shareholder.ID);

                if (oldShareholder != null)
                {
                    _uow.Entry(oldShareholder).State = EntityState.Detached;
                }

     
                _shareholders.Attach(shareholder);


                _uow.Entry(shareholder).State = EntityState.Modified;

          
                //_uow.Entry(shareholder).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(shareholder).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Shareholder> GetByIdAsync(int id)
        {
            return await _shareholders.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<Shareholder> GetShareholderByContractIdAsync(int organId,int contactId)
        {
           return await _shareholders.Where(x => x.OrganId == organId && x.ContactId == contactId && x.IsActive == true).SingleOrDefaultAsync();
        }

        public async Task<List<Shareholder>> GetShareholderByPersonIdAsync(int organId)
        {
            return await _shareholders.Where(x => x.OrganId == organId).ToListAsync();
        }
    }
}
