using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;

namespace Zhivar.ServiceLayer.Contracts.Common
{
    public interface IBussiness
    {
        IList<Bussiness> GetAll();
        Task<IList<Bussiness>> GetAllByOrganIdAsync(int organId);
        Task<IList<Bussiness>> GetAllAsync();
        Bussiness GetById(int id);
        Task<Bussiness> GetByIdAsync(int id);
        bool Insert(Bussiness bussiness);
        bool Update(Bussiness bussiness);
        bool Delete(Bussiness bussiness);
        bool Delete(int id);
    }
}
