using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Contract;

namespace Zhivar.ServiceLayer.Contracts.Contract
{

    public interface IContarct_Saze_Bazareab
    {
        IList<Contract_Saze_Bazareab> GetAll();
        //Task<IList<Contarct_Saze_Bazareab>> GetAllByOrganIdAsync(int organId);
        Task<IList<Contract_Saze_Bazareab>> GetAllAsync();
        Contract_Saze_Bazareab GetById(int id);
        Task<Contract_Saze_Bazareab> GetByIdAsync(int id);
        bool Insert(Contract_Saze_Bazareab contarct_Saze_Bazareab);
        bool Update(Contract_Saze_Bazareab contarct_Saze_Bazareab);
        bool Delete(Contract_Saze_Bazareab contarct_Saze_Bazareab);
        bool Delete(int id);
    }
}
