using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface ICostItem
    {
        IList<CostItem> GetAll();
        //IList<CostItem> GetAllByCostId(int costId);
        //Task<IList<CostItem>> GetAllByCostIdAsync(int costId);
        Task<IList<CostItem>> GetAllAsync();
        CostItem GetById(int id);
        //Task<CostItem> GetByIdAsync(int id);
        bool Insert(CostItem costItem);
        bool Update(CostItem costItem);
        bool Delete(CostItem costItem);
        bool Delete(int id);
        //Task<List<CostItem>> GetByCostIdAsync(int costId);
    }
}