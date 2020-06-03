using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface IVahedTol
    {
        IList<VahedTol> GetAll();
        Task<IList<VahedTol>> GetAllByOrganIdAsync(int organId);
        Task<IList<VahedTol>> GetAllAsync();
        VahedTol GetById(int id);
        Task<VahedTol> GetByIdAsync(int id);
        bool Insert(VahedTol vahedTol);
        bool Update(VahedTol vahedTol);
        bool Delete(VahedTol vahedTol);
        bool Delete(int id);
    }
}