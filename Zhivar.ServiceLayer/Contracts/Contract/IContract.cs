using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.ServiceLayer.Contracts.Contract
{
    public interface IContract
    {
        IList<DomainClasses.Contract.Contract> GetAll();
        Task<IList<DomainClasses.Contract.Contract>> GetAllByOrganIdAsync(int organId);
        Task<IList<DomainClasses.Contract.Contract>> GetAllAsync();
        DomainClasses.Contract.Contract GetAllSazeByContractId(int contractId);
        Task<DomainClasses.Contract.Contract> GetAllSazeByContractIdAsync(int contractId);
        DomainClasses.Contract.Contract GetById(int id);
        Task<DomainClasses.Contract.Contract> GetByIdAsync(int id);
        bool Insert(DomainClasses.Contract.Contract contract);
        bool Update(DomainClasses.Contract.Contract contract);
        bool Delete(DomainClasses.Contract.Contract contract);
        bool Delete(int id);
    }


}