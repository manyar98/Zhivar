using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface INoeChap
    {
        IList<NoeChap> GetAll();
        Task<IList<NoeChap>> GetAllByOrganIdAsync(int organId);
        Task<IList<NoeChap>> GetAllAsync();
        NoeChap GetById(int id);
        Task<NoeChap> GetByIdAsync(int id);
        bool Insert(NoeChap noeChap);
        bool Update(NoeChap noeChap);
        bool Delete(NoeChap noeChap);
        bool Delete(int id);
    }
}