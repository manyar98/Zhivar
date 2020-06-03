using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface ITarfeHesab
    {
        IList<TarfeHesab> GetAll();
        Task<IList<TarfeHesab>> GetAllAsync();
        IList<TarfeHesab> GetAllByPersonId(int personId);
        Task<IList<TarfeHesab>> GetAllByPersonIdAsync(int personId);
        TarfeHesab GetById(int id);
        TarfeHesab GetForUpdate(int id);
        Task<TarfeHesab> GetForUpdateAsync(int id);
        Task<TarfeHesab> GetByIdAsync(int id);
        bool Insert(TarfeHesab tarfeHesab);
        bool Update(TarfeHesab tarfeHesab);
        bool Delete(TarfeHesab tarfeHesab);
        bool Delete(int id);
    }
}
