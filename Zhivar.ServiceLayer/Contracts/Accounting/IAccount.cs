using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IAccount
    {
        IList<DomainClasses.Accounting.Account> GetAll();
        Task<List<DomainClasses.Accounting.Account>> GetAllByOrganIdAsync(int organId);
        Task<IList<DomainClasses.Accounting.Account>> GetAllAsync();
        DomainClasses.Accounting.Account GetById(int id);
        Task<DomainClasses.Accounting.Account> GetByIdAsync(int id);
        bool Insert(DomainClasses.Accounting.Account account);
        bool Update(DomainClasses.Accounting.Account account);
        bool Delete(DomainClasses.Accounting.Account account);
        bool Delete(int id);
        Task DeleteAccountByComplteCodingAsync(string completeCode);
    }

}