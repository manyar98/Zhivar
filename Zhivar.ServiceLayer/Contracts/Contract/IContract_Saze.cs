using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.ServiceLayer.Contracts.Contract
{
    public interface IContract_Saze
    {
        IList<Contract_Saze> GetAll();
        //Task<IList<Contract_Saze>> GetAllByOrganIdAsync(int organId);
        Task<IList<Contract_Saze>> GetAllAsync();
        Contract_Saze GetById(int id);
        Task<Contract_Saze> GetByIdAsync(int id);
        bool Insert(Contract_Saze contract_Saze);
        bool Update(Contract_Saze contract_Saze);
        bool Delete(Contract_Saze contract_Saze);
        bool Delete(int id);
    }
}
