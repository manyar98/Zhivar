using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ServiceLayer.Contracts.Common
{
    public interface IContact
    {
        IList<Contact> GetAll();
        Task<IList<Contact>> GetAllByOrganIdAsync(int organId);
        Task<IList<Contact>> GetAllAsync();
        Contact GetById(int id);
        Task<Contact> GetByIdAsync(int id);
        bool Insert(Contact contact);
        bool Update(Contact contact);
        bool Delete(Contact contact);
        bool Delete(int id);
        Task UpdateContact(ZhivarEnums.NoeFactor invoiceType, int contactId);
    }
}
