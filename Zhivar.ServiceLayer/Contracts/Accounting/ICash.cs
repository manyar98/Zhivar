using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface ICash
    {
        IList<Cash> GetAll();
        Task<IList<Cash>> GetAllByOrganIdAsync(int organId);
        Task<IList<Cash>> GetAllAsync();
        Cash GetById(int id);
        Task<Cash> GetByIdAsync(int id);
        bool Insert(Cash cash);
        bool Update(Cash cash);
        bool Delete(Cash cash);
        bool Delete(int id);
        Task<Cash> GetByAccountId(int fromDetailAccountId);
    }
}