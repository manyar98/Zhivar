using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using System.Data.Entity;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses;

namespace Zhivar.ServiceLayer.Common
{
    public class ContactService : IContact
    {
     
        IUnitOfWork _uow;
        readonly IDbSet<Contact> _contacts;
        public ContactService(IUnitOfWork uow)
        {
            _uow = uow;
            _contacts = _uow.Set<Contact>();
        }
        public bool Delete(int id)
        {
            try
            {
                var contact = GetById(id);
                Delete(contact);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Contact contact)
        {
            try
            {
                _contacts.Attach(contact);
                _contacts.Remove(contact);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IList<Contact> GetAll()
        {
            return _contacts.ToList();
        }
        public IList<Contact> GetAllByOrganId(int organId)
        {
            return _contacts.AsQueryable().Where(x => x.OrganId == organId).ToList();
        }
        public async Task<IList<Contact>> GetAllByOrganIdAsync(int organId)
        {
            return await _contacts.AsQueryable().Where(x => x.OrganId == organId).ToListAsync();
        }
        public async Task<IList<Contact>> GetAllAsync()
        {
            return await _contacts.ToListAsync();
        }
        public Contact GetById(int id)
        {
            return _contacts.Find(id);
        }

        public bool Insert(Contact contact)
        {
            try
            {
                _contacts.Add(contact);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Contact contact)
        {
            try
            {
                var local = _uow.Set<Contact>()
                     .Local
                     .FirstOrDefault(f => f.ID == contact.ID);
                if (local != null)
                {
                    _uow.Entry(local).State = EntityState.Detached;
                }

                _contacts.Attach(contact);

                _uow.Entry(contact).State = EntityState.Modified;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            return await _contacts.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task UpdateContact(ZhivarEnums.NoeFactor invoiceType, int contactId)
        {
            var contact =  await _contacts.AsQueryable().Where(x => x.ID == contactId).SingleOrDefaultAsync();
            if (contact != null)
            {
                switch (invoiceType)
                {
                    case ZhivarEnums.NoeFactor.Sell:
                        contact.IsCustomer = true;
                        break;
                    case ZhivarEnums.NoeFactor.Buy:
                        contact.IsVendor = true;
                        break;
                    case ZhivarEnums.NoeFactor.ReturnSell:
                        contact.IsVendor = true;
                        break;
                    case ZhivarEnums.NoeFactor.ReturnBuy:
                        contact.IsCustomer = true;
                        break;
                    default:
                        break;
                }

                Update(contact);
            }
        }
    }
}
