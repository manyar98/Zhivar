using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface IItem
    {
        IList<Item> GetAll();
        //Task<IList<Item>> GetAllByOrganIdAsync(int organId);
        Task<IList<Item>> GetAllAsync();
        Item GetById(int id);
        Task<Item> GetByIdAsync(int id);
        bool Insert(Item item);
        bool Update(Item item);
        bool Delete(Item item);
        bool Delete(int id);
    }
}