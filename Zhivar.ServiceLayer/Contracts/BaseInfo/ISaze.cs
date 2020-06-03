using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface ISaze
    {
        IList<Saze> GetAll();
        Task<IList<Saze>> GetAllAsync();
        IList<SazeVM> GetAllByPersonId(int personId);
        Task<IList<SazeVM>> GetAllByPersonIdAsync(int personId);
        IList<Saze> GetAllByGorohId(int gorohId);
        Task<IList<Saze>> GetAllByGorohIdAsync(int gorohId);
        Saze GetById(int id);
        Task<Saze> GetByIdAsync(int id);
        bool Insert(Saze saze);
        bool Update(Saze saze);
        bool Delete(Saze saze);
        bool Delete(int id);
        Task<SazeVM> GetSazeForEdit(int id);
    }
}
