using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface INoeSaze
    {
        IList<NoeSaze> GetAll();
        Task<IList<NoeSaze>> GetAllByOrganIdAsync(int organId);
        Task<IList<NoeSaze>> GetAllAsync();
        NoeSaze GetById(int id);
        Task<NoeSaze> GetByIdAsync(int id);
        bool Insert(NoeSaze noeSaze);
        bool Update(NoeSaze noeSaze);
        bool Delete(NoeSaze noeSaze);
        bool Delete(int id);
    }
}