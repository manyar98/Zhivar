using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface INoeEjare
    {
        IList<NoeEjare> GetAll();
        Task<IList<NoeEjare>> GetAllByOrganIdAsync(int organId);
        Task<IList<NoeEjare>> GetAllAsync();
        NoeEjare GetById(int id);
        Task<NoeEjare> GetByIdAsync(int id);
        bool Insert(NoeEjare noeEjare);
        bool Update(NoeEjare noeEjare);
        bool Delete(NoeEjare noeEjare);
        bool Delete(int id);
    }
}