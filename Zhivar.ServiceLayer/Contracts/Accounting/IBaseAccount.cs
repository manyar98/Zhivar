using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IBaseAccount
    {
        IList<BaseAccount> GetAll();
        //Task<IList<BaseAccount>> GetAllByOrganIdAsync(int organId);
        Task<IList<BaseAccount>> GetAllAsync();
        BaseAccount GetById(int id);
        Task<BaseAccount> GetByIdAsync(int id);
        bool Insert(BaseAccount baseAccount);
        bool Update(BaseAccount baseAccount);
        bool Delete(BaseAccount baseAccount);
        bool Delete(int id);
    }
}