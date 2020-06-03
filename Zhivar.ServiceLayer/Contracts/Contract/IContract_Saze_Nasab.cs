using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.ServiceLayer.Contracts.Contract
{

    public interface IContract_Saze_Nasab
    {
        IList<Contract_Saze_Nasab> GetAll();
        //Task<IList<Contract_Saze_Nasab>> GetAllByOrganIdAsync(int organId);
        Task<IList<Contract_Saze_Nasab>> GetAllAsync();
        Contract_Saze_Nasab GetById(int id);
        Task<Contract_Saze_Nasab> GetByIdAsync(int id);
        bool Insert(Contract_Saze_Nasab contract_Saze_Nasab);
        bool Update(Contract_Saze_Nasab contract_Saze_Nasab);
        bool Delete(Contract_Saze_Nasab contract_Saze_Nasab);
        bool Delete(int id);
    }
}
