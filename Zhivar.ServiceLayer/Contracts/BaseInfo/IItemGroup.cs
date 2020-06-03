using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface IItemGroup
    {
        IList<ItemGroup> GetAll();
        Task<IList<ItemGroup>> GetAllByOrganIdAsync(int organId);
        Task<IList<ItemGroup>> GetAllAsync();
        ItemGroup GetById(int id);
        Task<ItemGroup> GetByIdAsync(int id);
        bool Insert(ItemGroup itemGroup);
        bool Update(ItemGroup itemGroup);
        bool Delete(ItemGroup itemGroup);
        bool Delete(int id);
    }
}