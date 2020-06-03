using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface ISahamdaran
    {
        IList<Sahamdaran> GetAll();
        Task<IList<SahamdaranVM>> GetAllByOrganIdAsync(int organId);
        Task<IList<Sahamdaran>> GetAllAsync();
        Sahamdaran GetById(int id);
        Task<Sahamdaran> GetByIdAsync(int id);
        bool Insert(Sahamdaran sahamdaran);
        bool Update(Sahamdaran sahamdaran);
        bool Delete(Sahamdaran sahamdaran);
        bool Delete(int id);
    }
}