using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.ServiceLayer.Contracts.Contract
{
    public interface IContract_Saze_Chapkhane
    {
        IList<Contract_Saze_Chapkhane> GetAll();
       // Task<IList<Contract_Saze_Chapkhane>> GetAllByOrganIdAsync(int organId);
        Task<IList<Contract_Saze_Chapkhane>> GetAllAsync();
        Contract_Saze_Chapkhane GetById(int id);
        Task<Contract_Saze_Chapkhane> GetByIdAsync(int id);
        bool Insert(Contract_Saze_Chapkhane contract_Saze_Chapkhane);
        bool Update(Contract_Saze_Chapkhane contract_Saze_Chapkhane);
        bool Delete(Contract_Saze_Chapkhane contract_Saze_Chapkhane);
        bool Delete(int id);
    }
}
