using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IBank
    {
        IList<Bank> GetAll();
        Task<IList<Bank>> GetAllAsync();
        IList<Bank> GetAllByOrganId(int personId);
        Task<IList<Bank>> GetAllByOrganIdAsync(int personId);
        Bank GetById(int id);
        Task<Bank> GetByIdAsync(int id);
        bool Insert(Bank bank);
        bool Update(Bank bank);
        bool Delete(Bank bank);
        bool Delete(int id);
        Task<Bank> GetByAccountId(int toDetailAccountId);
    }
}
