using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface IGoroheSaze
    {
        IList<GoroheSaze> GetAll();
        IList<GoroheSaze> GetAllByPersonId(int personId);
        Task<IList<GoroheSaze>> GetAllByPersonIdAsync(int personId);
        Task<IList<GoroheSaze>> GetAllAsync();
        GoroheSaze GetById(int id);
        Task<GoroheSaze> GetByIdAsync(int id);
        bool Insert(GoroheSaze goroheSaze);
        bool Update(GoroheSaze goroheSaze);
        bool Delete(GoroheSaze goroheSaze);
        bool Delete(int id);
    }
}
